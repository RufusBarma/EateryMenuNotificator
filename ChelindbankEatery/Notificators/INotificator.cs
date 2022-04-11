namespace ChelindbankEatery.Notificators;

public interface INotificator
{
	public Task Send(string imgPath);
}