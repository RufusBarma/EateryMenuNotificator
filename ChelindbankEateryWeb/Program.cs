using ChelindbankEatery.DocParser;
using ChelindbankEateryWeb.Domain.Document;
using ChelindbankEateryWeb.Domain.DocumentDownloader;
using ChelindbankEateryWeb.Domain.Jobs;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DocumentHolder>();
builder.Services
	.AddLogging(loggingBuilder => loggingBuilder.AddSimpleConsole())
	.AddTransient<IDocumentDownloader, DocumentDownloader>()
	.AddTransient<IDocumentParser, DocumentParser>()
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

		q.ScheduleJob<UpdateMenuJob>(trigger => trigger
			.WithIdentity("MenuUpdate")
			.StartNow()
			.WithCronSchedule("0 */2 * * * ?")
			.WithDescription("my awesome trigger configured for a job with single call")
		);
	})
	.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
