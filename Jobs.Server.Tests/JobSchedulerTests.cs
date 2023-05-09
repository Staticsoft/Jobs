using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Jobs.Abstractions;
using Staticsoft.Testing;
using Staticsoft.Timers.Abstractions;
using Staticsoft.Timers.Memory;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Staticsoft.Jobs.Server.Tests;

public class JobSchedulerTests : TestBase<JobScheduler>
{
    protected override IServiceCollection Services => base.Services
        .AddSingleton<JobScheduler>()
        .AddSingleton<JobRunner>()
        .AddSingleton<Time, UtcTime>()
        .AddSingleton<SimpleJob>()
        .ReuseSingleton<Job, SimpleJob>()
        .AddSingleton<ControlledNextMinute>()
        .ReuseSingleton<NextMinute, ControlledNextMinute>();

    SimpleJob Job
        => Get<SimpleJob>();

    ControlledNextMinute NextMinute
        => Get<ControlledNextMinute>();

    [Fact]
    public async Task DoesNotRunScheduledJobIfCancellationIsRequested()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        await SUT.StartAsync(source.Token);
        NextMinute.Tick();
        await Task.Delay(100);
        await SUT.StopAsync(new());
        Assert.Equal(0, Job.CompletedTimes);
    }

    [Fact]
    public async Task DoesNotRunScheduledJobIfScheduledJobIsStoppedBeforeTicking()
    {
        await SUT.StartAsync(new());
        await Task.Delay(100);
        await SUT.StopAsync(new());
        Assert.Equal(0, Job.CompletedTimes);
    }

    [Fact]
    public async Task RunsScheduledJob()
    {
        await SUT.StartAsync(new());
        NextMinute.Tick();
        await Task.Delay(100);
        await SUT.StopAsync(new());
        Assert.Equal(1, Job.CompletedTimes);
    }

    [Fact]
    public async Task RunsScheduledJobTwice()
    {
        await SUT.StartAsync(new());
        NextMinute.Tick();
        await Task.Delay(100);
        NextMinute.Tick();
        await Task.Delay(100);
        await SUT.StopAsync(new());
        Assert.Equal(2, Job.CompletedTimes);
    }
}

public class SimpleJob : Job
{
    public int CompletedTimes { get; private set; } = 0;

    public Schedule Schedule { get; } = new Schedule();

    public Task Run()
    {
        CompletedTimes++;
        return Task.CompletedTask;
    }
}
