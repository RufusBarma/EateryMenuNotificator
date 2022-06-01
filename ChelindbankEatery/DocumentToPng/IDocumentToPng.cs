using ChelindbankEatery.DocParser;

namespace ChelindbankEatery.DocumentToPng;

public interface IDocumentToPng
{
	public Task<Stream> GetPngAsync(DocumentInfo documentInfo);
}
