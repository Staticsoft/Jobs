using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Jobs.Fakes;
using Staticsoft.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Staticsoft.Jobs.Abstractions.Tests;

public class JobRunnerTests : TestBase<JobRunner>
{
    protected override IServiceCollection Services => base.Services
        .AddSingleton<JobRunner>()
        .AddSingleton<EachMinuteJob>()
        .ReuseSingleton<Job, EachMinuteJob>()
        .AddSingleton<EachSecondMinuteJob>()
        .ReuseSingleton<Job, EachSecondMinuteJob>()
        .AddSingleton<EachThirdMinuteJob>()
        .ReuseSingleton<Job, EachThirdMinuteJob>()
        .AddSingleton<TimeFake>()
        .ReuseSingleton<Time, TimeFake>();

    TimeFake Time
        => Get<TimeFake>();

    [Fact]
    public async Task RunsEachMinuteJob()
    {
        Time.Minute = 1;
        await SUT.Run();
        var eachMinuteJob = Get<EachMinuteJob>();
        Assert.True(eachMinuteJob.Completed);
    }

    [Fact]
    public async Task DoesNotRunEachTwoMinutesJob()
    {
        Time.Minute = 1;
        await SUT.Run();
        var eachSecondMinuteJob = Get<EachSecondMinuteJob>();
        Assert.False(eachSecondMinuteJob.Completed);
    }

    [Fact]
    public async Task RunsEachTwoMinutesJob()
    {
        Time.Minute = 2;
        await SUT.Run();
        var eachSecondMinuteJob = Get<EachSecondMinuteJob>();
        Assert.True(eachSecondMinuteJob.Completed);
    }

    [Fact]
    public async Task RunsAllJobs()
    {
        Time.Minute = 6;
        await SUT.Run();
        var eachMinuteJob = Get<EachMinuteJob>();
        Assert.True(eachMinuteJob.Completed);
        var eachSecondMinuteJob = Get<EachSecondMinuteJob>();
        Assert.True(eachSecondMinuteJob.Completed);
        var eachThirdMinuteJob = Get<EachThirdMinuteJob>();
        Assert.True(eachThirdMinuteJob.Completed);
    }
}

public class EachMinuteJob : SimpleJob { }
public class EachSecondMinuteJob : SimpleJob
{
    public override Schedule Schedule { get; } = new() { Minutes = 2 };
}

public class EachThirdMinuteJob : SimpleJob
{
    public override Schedule Schedule { get; } = new() { Minutes = 3 };
}

public class SimpleJob : Job
{
    public virtual Schedule Schedule { get; } = new Schedule();
    public bool Completed { get; private set; } = false;

    public Task Run()
    {
        Completed = true;
        return Task.CompletedTask;
    }
}
