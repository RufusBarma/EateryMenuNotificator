using ChelindbankEatery;
using Quartz;
using Quartz.Impl;

var menuJob = JobBuilder.Create<MenuJob>().WithIdentity("MenuUpdate").Build();
var trigger = TriggerBuilder.Create()
	.WithIdentity("MenuUpdate")
	.WithCronSchedule("0 30 9 * * ?")
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