using ChelindbankEatery.DocParser;
using ChelindbankEatery.DocumentToPng;
using ChelindbankEatery.Notificators;
using GroupDocs.Editor;
using GroupDocs.Editor.Formats;
using GroupDocs.Editor.Options;
using HtmlAgilityPack;
using Quartz;

namespace ChelindbankEatery;

public class MenuJobLatex: IJob
{
	private readonly INotificator _notificator;
	private readonly IDocumentToPng _documentToPng;
	private IDocumentParser _documentParser;

	public MenuJobLatex(IDocumentParser documentParser, INotificator notificator, IDocumentToPng documentToPng)
	{
		_notificator = notificator;
		_documentToPng = documentToPng;
		_documentParser = documentParser;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		var documentPath = new DirectoryInfo("tmp").GetFiles().FirstOrDefault(file => file.Name.Contains("docx")).FullName; //remove late
		var documentInfo = _documentParser.GetInfo(documentPath);
		var pngStream = await _documentToPng.GetPngAsync(documentInfo);
		await _notificator.Send(pngStream, documentPath);
	}
}
