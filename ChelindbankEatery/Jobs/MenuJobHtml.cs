using ChelindbankEatery.DocParser;
using ChelindbankEatery.DocumentToPng;
using ChelindbankEatery.Notificators;
using GroupDocs.Conversion;
using Quartz;

namespace ChelindbankEatery;

public class MenuJobHtml: IJob
{
	private readonly INotificator _notificator;
	private readonly IDocumentToPng _documentToPng;
	private IDocumentParser _documentParser;

	public MenuJobHtml(IDocumentParser documentParser, INotificator notificator, IDocumentToPng documentToPng)
	{
		_notificator = notificator;
		_documentToPng = documentToPng;
		_documentParser = documentParser;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		var documentPath = await GetDocumentFromShared();
		if (string.IsNullOrEmpty(documentPath))
			return;

		var documentInfo = _documentParser.GetInfo(documentPath);
		var pngStream = await _documentToPng.GetPngAsync(documentInfo);
		await _notificator.Send(pngStream, documentPath);
	}

	public async Task<string> GetDocumentFromShared()
	{
		var fileSource = new DirectoryInfo("N:\\tmp\\tmp").GetFiles().FirstOrDefault(file => file.Name.Contains("меню", StringComparison.OrdinalIgnoreCase) && !file.Name.Contains("lock"));
		if (fileSource == null)
		{
			Console.WriteLine("Меню не найдено(");
			return null;
		}
		var tmpRootName = "tmp";
		var tmpRootDir = new DirectoryInfo(tmpRootName);
		if (tmpRootDir.Exists)
		{
			if (tmpRootDir.GetFiles().Any(file => file.Name == fileSource?.Name))
			{
				Console.WriteLine("Меню актуально");
				return null;
			}
			tmpRootDir.Delete(true);
		}
		tmpRootDir.Create();

		var tmpRoot = tmpRootDir.FullName;
		var file = fileSource?.CopyTo(Path.Combine(tmpRoot, fileSource.Name));
		if (file == null)
		{
			Console.WriteLine("Меню не найдено");
			return null;
		}

		using var converter = new Converter(file.FullName);
		var convertOptions =  converter.GetPossibleConversions()["docx"].ConvertOptions;
		var docxPath = Path.ChangeExtension(file.FullName, "docx");
		converter.Convert(docxPath, convertOptions);

		return docxPath;
	}
}
