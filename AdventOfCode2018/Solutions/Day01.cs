using AdventOfCode2018.Common;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018.Solutions
{
    public class Day01
    {
        [Solution(1, 1)]
        public string Problem1(string input)
        {
            var result = input.Split('\n')
                .Select(it => it.Trim())
                .Where(it => !string.IsNullOrEmpty(it))
                .Select(it => int.Parse(it))
                .Aggregate(0, (acc, it) => acc += it);

            return result.ToString();
        }

        [Solution(1, 2)]
        public string Problem2(string input)
        {
            var items = input.Split('\n')
                .Select(it => it.Trim())
                .Where(it => !string.IsNullOrEmpty(it))
                .Select(it => int.Parse(it)).ToArray();

            // This is messy and could be done neater with an endless enumerable, but will work for now.
            var index = 0;
            var current = 0;
            var seenValues = new List<int>();
            while (true)
            {
                current += items[index];
                if (seenValues.Contains(current))
                    return current.ToString();

                seenValues.Add(current);
                if (++index == items.Length)
                    index = 0;
            }
        }
    }
}
