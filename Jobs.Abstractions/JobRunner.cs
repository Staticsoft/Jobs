using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Staticsoft.Jobs.Abstractions;

public class JobRunner
{
    readonly Time Time;
    readonly IEnumerable<Job> Jobs;

    public JobRunner(Time time, IEnumerable<Job> jobs)
        => (Time, Jobs) = (time, jobs);

    public Task Run()
        => Task.WhenAll(Jobs.Where(OnSchedule).Select(job => job.Run()));

    bool OnSchedule(Job job)
        => Time.Minute % job.Schedule.Minutes == 0
        && Time.Hour % job.Schedule.Hours == 0;
}
