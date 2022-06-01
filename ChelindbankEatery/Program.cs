using ChelindbankEatery;
using ChelindbankEatery.DocParser;
using ChelindbankEatery.DocumentToPng;
using ChelindbankEatery.Notificators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;

var configurationRoot = new ConfigurationBuilder()
	.AddEnvironmentVariables()
	.AddJsonFile("appsettings.json")
	.Build();

var serviceProvider = new ServiceCollection()
	.AddSingleton<IConfiguration>(configurationRoot)
	.AddTransient<MenuJob>()
	.AddTransient<IDocumentParser, DocumentParser>()
	.AddTransient<INotificator, TelegramNotificator>()
	.AddTransient<IDocumentToPng, DocumentToHtmlPng>()
	.AddTransient<MenuJobHtml>()
	.AddLogging(configure => configure.AddConsole())
	.AddQuartz(q =>
		{
			// handy when part of cluster or you want to otherwise identify multiple schedulers
			q.SchedulerId = "Scheduler-Core";
			// we take this from appsettings.json, just show it's possible
			q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

			q.UseMicrosoftDependencyInjectionJobFactory();

			// these are the defaults
			q.UseSimpleTypeLoader();
			q.UseInMemoryStore();
			q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });

			q.ScheduleJob<MenuJobHtml>(trigger => trigger
				.WithIdentity("MenuUpdate")
				.StartNow()
				.WithCronSchedule("0 */5 * * * ?")
				.WithDescription("my awesome trigger configured for a job with single call")
			);
		})
	.AddQuartzHostedService(q => q.WaitForJobsToComplete = true)
	.BuildServiceProvider();

var schedulerQuartz = await serviceProvider.GetRequiredService<ISchedulerFactory>().GetScheduler();
schedulerQuartz.Start();
await Task.Delay(Timeout.Infinite);

var menuJob = JobBuilder.Create<MenuJob>().WithIdentity("MenuUpdate").Build();
var trigger = TriggerBuilder.Create()
	.WithIdentity("MenuUpdate")
	.WithCronSchedule("0 */5 * * * ?")
	.ForJob(menuJob)
	.StartNow()
	.Build();
var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
await scheduler.Start();
await scheduler.ScheduleJob(menuJob, trigger);
var cts = new CancellationTokenSource();
var infinity = Task.Delay(Timeout.Infinite, cts.Token);
await scheduler.TriggerJob(menuJob.Key);
var sigintReceived = false;
AppDomain.CurrentDomain.ProcessExit += (_, _) =>
{
	if (!sigintReceived)
	{
		Console.WriteLine("Received SIGTERM");
		scheduler.Shutdown();
		cts.Cancel();
	}
	else
	{
		Console.WriteLine("Received SIGTERM, ignoring it because already processed SIGINT");
	}
};

Console.CancelKeyPress += (_, ea) =>
{
	// Tell .NET to not terminate the process
	ea.Cancel = true;

	Console.WriteLine("Received SIGINT (Ctrl+C)");
	scheduler.Shutdown();
	cts.Cancel();
	sigintReceived = true;
};

try
{
	await infinity;
}
catch
{
	// ignored
}