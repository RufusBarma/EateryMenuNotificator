using ChelindbankEatery.DocParser;

namespace ChelindbankEatery.DocumentToPng;

public interface IDocumentToPng
{
	public Stream GetPng(DocumentInfo documentInfo);
}
