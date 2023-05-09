using System.Threading.Tasks;

namespace Staticsoft.Jobs.Abstractions;

public interface Job
{
    Task Run();
    Schedule Schedule { get; }
}
