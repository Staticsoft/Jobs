using System;

namespace Staticsoft.Jobs.Abstractions
{
    public class UtcTime : Time
    {
        public int Minute
            => DateTime.UtcNow.Minute;

        public int Hour
            => DateTime.UtcNow.Hour;
    }
}
