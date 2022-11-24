using Microsoft.AspNetCore.Mvc;
using Quartz;
using QuartzJob.Schedulers;


namespace QuartzJob.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private const string NewCronExpression = "0 0/4 * ? * * *";
        private const string TriggerKeyName = "QuartzJob.Schedulers.Jobs.PeriodicProcedureJob.trigger";

        // POST api/<ValuesController1>
        [HttpPost]
        public async void CronExpression([FromServices] QuartzHostedService serviceQuartzHosted)
        {
            var serviceJobSchedule = HttpContext.RequestServices.GetService<JobSchedule>();
            if (serviceJobSchedule != null) serviceJobSchedule.CronExpression = NewCronExpression;
            var trigger = await serviceQuartzHosted.Scheduler.GetTrigger(new TriggerKey(TriggerKeyName, "DEFAULT")).ConfigureAwait(false);


            if (trigger != null)
                await serviceQuartzHosted.Scheduler.RescheduleJob(trigger.Key,
                    CreateTrigger(trigger, NewCronExpression));
        }

        private static ITrigger CreateTrigger(ITrigger oldTrigger, string cronExpression)
        {
            var builder = oldTrigger.GetTriggerBuilder();
            builder = builder.StartNow();
            if (!string.IsNullOrEmpty(cronExpression))
                builder = builder.WithIdentity(TriggerKeyName)
                    .WithCronSchedule(cronExpression)
                    .WithDescription(cronExpression);

            var newTrigger = builder.Build();
            if (newTrigger is ISimpleTrigger simpleTrigger)
            {
                if (oldTrigger is ISimpleTrigger trigger)
                    simpleTrigger.TimesTriggered = trigger.TimesTriggered;
            }

            return newTrigger;
        }

    }
}
