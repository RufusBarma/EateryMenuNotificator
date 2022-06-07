using System.Globalization;
using ChelindbankEatery.Notificators;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using Quartz;

namespace ChelindbankEatery;

public class MenuJobFromWeb: IJob
{
	private readonly INotificator _notificator;
	private readonly ILogger _logger;

	public MenuJobFromWeb(INotificator notificator, ILogger<MenuJobFromWeb> logger)
	{
		_notificator = notificator;
		_logger = logger;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		var latestDate = await new HttpClient().GetAsync("https://localhost:7006/Home/LatestUpdateDate");
		if (!latestDate.IsSuccessStatusCode)
		{
			_logger.LogWarning("Не могу подключиться к представлению");
			return;
		}
		var content = (await latestDate.Content.ReadAsStringAsync()).Replace("\"", "");
		var latestUpdateDate = DateTime.Parse(content);

		if (IsAlreadyNotified(latestUpdateDate))
		{
			_logger.LogInformation("Нет обновлений");
			return;
		}
		_logger.LogInformation("Есть обновление");

		using var browserFetcher = new BrowserFetcher(Product.Chrome);
		await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
		var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
		var page = await browser.NewPageAsync();
		await page.GoToAsync("https://localhost:7006/");
		var ms = new MemoryStream();
		if (!Directory.Exists("tmp"))
			Directory.CreateDirectory("tmp");
		await page.ScreenshotAsync("tmp/kek.png", new ScreenshotOptions{FullPage = true});
		ms.Write(await File.ReadAllBytesAsync("tmp/kek.png"));
		ms.Position = 0;
		await _notificator.Send(ms, DateTime.Today.ToString());
		_logger.LogInformation("Меню отправлено");
		CacheDocumentDate(latestUpdateDate);
	}

	public bool IsAlreadyNotified(DateTime documentDate)
	{
		if (!Directory.Exists("tmp"))
			Directory.CreateDirectory("tmp");
		if (File.Exists("tmp/date.txt"))
		{
			var text = File.ReadAllText("tmp/date.txt");
			var previous = DateTime.Parse(text);
			var different = previous - documentDate >= TimeSpan.FromSeconds(-1);

			return different;
		}

		return false;
	}

	public void CacheDocumentDate(DateTime documentDate)
	{
		if (!Directory.Exists("tmp"))
			Directory.CreateDirectory("tmp");
		if (File.Exists("tmp/date.txt"))
			File.Delete("tmp/date.txt");
		File.WriteAllText("tmp/date.txt", documentDate.ToString(CultureInfo.InvariantCulture));
	}
}
