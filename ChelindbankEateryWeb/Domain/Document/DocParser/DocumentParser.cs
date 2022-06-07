using GroupDocs.Editor;
using GroupDocs.Editor.Options;
using HtmlAgilityPack;

namespace ChelindbankEatery.DocParser;

public class DocumentParser: IDocumentParser
{
	public DocumentInfoModel GetInfo(string documentPath)
	{
		var table = GetTable(documentPath);
		var date = GetDate(documentPath);

		return new DocumentInfoModel
		{
			DocumentUpdated = new FileInfo(documentPath).LastWriteTime,
			Date = date,
			MenuTable = table,
		};
	}

	public HtmlDocument GetHtmlDocument(string documentPath)
	{
		using var editor = new Editor(documentPath);
		var editOptions = new WordProcessingEditOptions();
		var editableDocument = editor.Edit(editOptions);
		var body = editableDocument.GetBodyContent();;
		var doc = new HtmlDocument();
		doc.LoadHtml(body);

		return doc;
	}

	public string GetDate(string documentPath)
	{
		var doc = GetHtmlDocument(documentPath);
		var date = doc.DocumentNode.Descendants("p").Take(6).Last().InnerText;
		return date;
	}

	public string[][] GetTable(string documentPath)
	{
		var doc = GetHtmlDocument(documentPath);
		var table = doc.DocumentNode
			.Descendants("tr")
			.Select(row => row.Descendants("td").Select(cell => (cell.InnerText == "&#xa0;"? "": cell.InnerText).Trim()).Take(3))
			.Select(row => row.ToArray())
			.SkipLast(3)
			.ToArray();

		return table;
	}
}
