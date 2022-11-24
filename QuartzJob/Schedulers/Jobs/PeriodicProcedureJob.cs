using Quartz;

namespace QuartzJob.Schedulers.Jobs;

[DisallowConcurrentExecution]
public class PeriodicProcedureJob : IJob
{
    public int Sum(int n)
    {
        var sum = 0;
        for (var i = 1; i <= n; i++)
        {
            sum += i;
        }

        Console.WriteLine($"Sum is {sum} Time is {DateTime.Now}");
        return sum;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var rnd = new Random();
        var result = Sum(rnd.Next(1, 100));
        return Task.CompletedTask;
    }
}