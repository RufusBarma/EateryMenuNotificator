namespace ChelindbankEateryWeb.Domain.DocumentDownloader;

public interface IDocumentDownloader
{
	public Task<string> GetDocument();
}
