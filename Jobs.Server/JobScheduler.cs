using Microsoft.Extensions.Hosting;
using Staticsoft.Jobs.Abstractions;
using Staticsoft.Timers.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Staticsoft.Jobs.Server;

public class JobScheduler : IHostedService
{
    readonly JobRunner Runner;
    readonly NextMinute NextMinute;
    readonly Task Stopping;
    readonly SemaphoreSlim Stop = new SemaphoreSlim(0);

    Task RunTask;
    bool IsRunning = true;

    public JobScheduler(JobRunner runner, NextMinute nextMinute)
    {
        Runner = runner;
        NextMinute = nextMinute;
        Stopping = Stop.WaitAsync();
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        RunTask = cancellationToken.IsCancellationRequested ? Task.CompletedTask : Run();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken _)
    {
        IsRunning = false;
        Stop.Release();
        await RunTask;
    }

    async Task Run()
    {
        while (true)
        {
            await WaitNextMinute();
            if (!IsRunning) return;
            await Runner.Run();
        }
    }

    Task WaitNextMinute()
        => Task.WhenAny(Stopping, NextMinute.Wait());
}
