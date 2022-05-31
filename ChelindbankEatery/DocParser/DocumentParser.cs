using GroupDocs.Editor;
using GroupDocs.Editor.Options;
using HtmlAgilityPack;

namespace ChelindbankEatery.DocParser;

public class DocumentParser: IDocumentParser
{
	public DocumentInfo GetInfo(string documentPath)
	{
		var table = GetTable(documentPath);
		var date = GetDate(documentPath);

		return new DocumentInfo
		{
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
		// var doc = GetHtmlDocument(documentPath);
		// doc.DocumentNode.Descendants("p")
		return DateTime.Today.ToString();
	}

	public string[][] GetTable(string documentPath)
	{
		var doc = GetHtmlDocument(documentPath);
		var table = doc.DocumentNode
			.Descendants("tr")
			.Select(row => row.Descendants("td").Select(cell => cell.InnerText == "&#xa0;"? "": cell.InnerText).Take(3))
			.Select(row => row.ToArray())
			.ToArray();

		return table;
	}
}
