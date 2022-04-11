﻿using System.Drawing;
using System.Drawing.Imaging;
using ChelindbankEatery.Notificators;
using Quartz;
using Telegram.Bot;

namespace ChelindbankEatery;

public class MenuJob: IJob
{
	private INotificator _notificator;
	public MenuJob()
	{
		_notificator = new TelegramNotificator();
	}

	public async Task Execute(IJobExecutionContext context)
	{
		var tmpRootName = "tmp";
		var tmpRootDir = new DirectoryInfo(tmpRootName);
		if (tmpRootDir.Exists)
			tmpRootDir.Delete(true);
		tmpRootDir.Create();

		var tmpRoot = tmpRootDir.FullName;
		var fileSource = new DirectoryInfo("N:\\tmp\\tmp").GetFiles().FirstOrDefault(file => file.Name.Contains("меню"));
		var file = fileSource?.CopyTo(Path.Combine(tmpRoot, fileSource.Name));
		if (file == null)
		{
			Console.WriteLine("Меню не найдено");
			return;
		}
		var converter =  new GroupDocs.Conversion.Converter(Path.Combine(tmpRoot,file.FullName));
		// set the convert options for PNG format
		var convertOptions = converter.GetPossibleConversions()["png"].ConvertOptions;
		// convert to PNG format
		var tmpPath = Path.ChangeExtension(Path.Combine(tmpRoot, file.Name), ".tmp");
		converter.Convert(tmpPath, convertOptions);

		var waterMarkHeight = 68;
		var image = Image.FromFile(tmpPath);
		var cropRect = new Rectangle(0, waterMarkHeight, image.Width, image.Height-waterMarkHeight);
		var resultPath = Path.ChangeExtension(file.Name, ".png");
		image.Crop(cropRect).Save(Path.Combine(tmpRoot, resultPath), ImageFormat.Png);
		Console.WriteLine("Notify");
		await _notificator.Send(Path.Combine(tmpRoot, resultPath));
		Console.WriteLine("Finally!");
	}
}