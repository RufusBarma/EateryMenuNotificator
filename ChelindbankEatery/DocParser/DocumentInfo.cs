namespace ChelindbankEatery.DocParser;

public record DocumentInfo
{
	public string Date { get; init; }
	public string[][] MenuTable { get; init; }
}
