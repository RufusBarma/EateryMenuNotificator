using System.Text;
using ChelindbankEatery.DocParser;
using PuppeteerSharp;

namespace ChelindbankEatery.DocumentToPng;

public class DocumentToHtmlPng: IDocumentToPng
{
	public async Task<Stream> GetPngAsync(DocumentInfo documentInfo)
	{
		var html = new StringBuilder();
		html.Append(@"  
		<style>
			h3 {
				color: #01627F;
				margin-bottom:5px;
			}
			table {
				width:100%;
				margin-bottom:20px;
                margin-left:10px;
			}
		</style>");
		html.Append(@"<body>");
		var tableOpen = @"<table>";
		html.Append(tableOpen);
		foreach (var menuTable in documentInfo.MenuTable)
		{
			var row = menuTable.Where(cell => !string.IsNullOrEmpty(cell)).ToArray();
			if (row.Length == 1)
			{
				html.Append(@"</table>");
				html.Append($"<h3 style=\"font-weight: bold;\">{row.First()}</h3>");
				html.Append(tableOpen);
				continue;
			}
			html.Append("<tr>");
			html.Append(@"<th style=""width:80%; text-align:left"">" + menuTable[0] + "</th>");
			html.Append(@"<th style=""width:13%; text-align:left"">" + menuTable[1] + "</th>");
			html.Append(@"<th style=""width:7%; text-align:left"">" + menuTable[2] + "</th>");
			html.Append("</tr>");
		}
		html.Append(@"</table>");
		html.Append(@"</body>");

		var ms = await GetPngStream(html.ToString());
		return ms;
	}

	private async Task<Stream> GetPngStream(string input)
	{
		using var browserFetcher = new BrowserFetcher();
		await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
		var browser = await Puppeteer.LaunchAsync(new LaunchOptions
		{
			Headless = true
		});
		var page = await browser.NewPageAsync();
		await page.SetContentAsync(input);
		var ms = new MemoryStream();
		await page.ScreenshotAsync("kek.png", new ScreenshotOptions{FullPage = true});
		ms.Write(await File.ReadAllBytesAsync("kek.png"));
		ms.Position = 0;

		return ms;
	}
}
