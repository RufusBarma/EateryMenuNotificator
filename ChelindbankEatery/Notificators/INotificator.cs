namespace ChelindbankEatery.Notificators;

public interface INotificator
{
	public Task Send(Stream imgStream, string fileName);
}

public static class NotificatorExtensions
{
	public static async Task Send(this INotificator notificator, string imgPath)
	{
		await using var stream = File.OpenRead(imgPath);
		await notificator.Send(stream, imgPath);
	}
}