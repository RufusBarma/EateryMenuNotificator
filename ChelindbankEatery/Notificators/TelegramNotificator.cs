﻿using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace ChelindbankEatery.Notificators;

public class TelegramNotificator : INotificator
{
	private TelegramBotClient _botClient;
	private string _channel;

	public TelegramNotificator()
	{
		_channel = Environment.GetEnvironmentVariable("telegram.bot.channel") ?? 
		           throw new Exception("Can't get channel name from environment");
		var token = Environment.GetEnvironmentVariable("telegram.bot.token");
		if (token == null)
			throw new Exception("Can't get token from environment");
		_botClient = new TelegramBotClient(token);
	}

	public async Task Send(string imgPath)
	{
		await using var stream = System.IO.File.OpenRead(imgPath);
		var inputOnlineFile = new InputOnlineFile(stream, Path.GetFileName(imgPath));
		await _botClient.SendPhotoAsync(new ChatId(_channel), inputOnlineFile);
	}
}