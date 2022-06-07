using ChelindbankEatery.DocParser;
using ChelindbankEateryWeb.Domain.DocumentDownloader;

namespace ChelindbankEateryWeb.Domain.Document;

public class DocumentHolder
{
	private readonly IDocumentDownloader _downloader;
	private readonly IDocumentParser _documentParser;
	private DocumentInfoModel _previous;

	public DocumentHolder(IDocumentDownloader downloader, IDocumentParser documentParser)
	{
		_downloader = downloader;
		_documentParser = documentParser;
		UpdateDocumentInfo().Wait();
	}

	public async Task<DocumentInfoModel> GetInfo()
	{
		return _previous;
	}

	public async Task UpdateDocumentInfo()
	{
		var documentPath = await _downloader.GetDocument();
		if (string.IsNullOrEmpty(documentPath))
		{
			if (_previous == null)
				documentPath = new DirectoryInfo("tmp").GetFiles().FirstOrDefault(file => file.Name.Contains("docx"))?.FullName;
			else
				return;
		}

		var documentInfo = _documentParser.GetInfo(documentPath);

		_previous = documentInfo;
	}
};
