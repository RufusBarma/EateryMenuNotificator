namespace ChelindbankEatery.DocParser;

public record DocumentInfoModel
{
	public DateTime DocumentUpdated { get; init; }
	public string Date { get; init; }
	public string[][] MenuTable { get; init; }
	public bool IsActual() => DocumentUpdated.Date == DateTime.Today.Date;
}
