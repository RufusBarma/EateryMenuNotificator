using GroupDocs.Conversion;

namespace ChelindbankEateryWeb.Domain.DocumentDownloader;

public class DocumentDownloader: IDocumentDownloader
{
	private readonly ILogger<DocumentDownloader> _logger;
	private readonly string _menuPath;

	public DocumentDownloader(ILogger<DocumentDownloader> logger, IConfiguration configuration)
	{
		_logger = logger;
		_menuPath = configuration["MenuRootPath"];
	}

	public Task<string> GetDocument() => GetDocumentFromShared();

	private async Task<string> GetDocumentFromShared()
	{
		var fileSource = new DirectoryInfo(_menuPath).GetFiles().FirstOrDefault(file => file.Name.Contains("меню", StringComparison.OrdinalIgnoreCase) && !file.Name.Contains("lock"));
		if (fileSource == null)
		{
			_logger.LogInformation("Меню не найдено");
			return null;
		}
		var tmpRootName = "tmp";
		var tmpRootDir = new DirectoryInfo(tmpRootName);
		if (tmpRootDir.Exists)
		{
			if (tmpRootDir.GetFiles().Any(file => file.Name == fileSource?.Name))
			{
				_logger.LogInformation("Меню актуально");
				return null;
			}
			tmpRootDir.Delete(true);
		}
		tmpRootDir.Create();

		var tmpRoot = tmpRootDir.FullName;
		var file = fileSource?.CopyTo(Path.Combine(tmpRoot, fileSource.Name));
		if (file == null)
		{
			_logger.LogInformation("Меню не найдено");
			return null;
		}

		using var converter = new Converter(file.FullName);
		var convertOptions =  converter.GetPossibleConversions()["docx"].ConvertOptions;
		var docxPath = Path.ChangeExtension(file.FullName, "docx");
		converter.Convert(docxPath, convertOptions);

		return docxPath;
	}
}
