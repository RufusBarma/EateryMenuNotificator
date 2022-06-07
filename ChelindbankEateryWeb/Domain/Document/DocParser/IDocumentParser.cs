namespace ChelindbankEatery.DocParser;

public interface IDocumentParser
{
	public DocumentInfoModel GetInfo(string documentPath);
}
