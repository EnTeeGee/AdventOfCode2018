using System;

namespace AdventOfCode2018.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SolutionAttribute : Attribute
    {
        public int Day { get; }
        public int Problem { get; }

        public SolutionAttribute(int day, int problem)
        {
            Day = day;
            Problem = problem;
        }
    }
}
