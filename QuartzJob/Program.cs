using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzJob.Schedulers;
using QuartzJob.Schedulers.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<QuartzHostedService>();
builder.Services.AddHostedService<QuartzHostedService>(provider => provider.GetService<QuartzHostedService>()!);
builder.Services.AddSingleton<IJobFactory, SingletonJobFactory>();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

var settingStartJob =  builder.Configuration["Quartz:PeriodicProcedureTime"];
builder.Services.AddSingleton<PeriodicProcedureJob>();
builder.Services.AddSingleton(new JobSchedule(
    jobType: typeof(PeriodicProcedureJob),
    cronExpression: settingStartJob)); // периодичность запуска PeriodicProcedureJob


builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
