using Staticsoft.Jobs.Abstractions;

namespace Staticsoft.Jobs.Fakes
{
    public class TimeFake : Time
    {
        public int Minute { get; set; } = 1;

        public int Hour { get; set; } = 1;
    }
}
