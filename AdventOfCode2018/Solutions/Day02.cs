using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day02
    {
        [Solution(2, 1)]
        public string Problem1(string input)
        {
            var lines = input.Split('\n')
                .Select(it => it.Trim())
                .Where(it => !string.IsNullOrEmpty(it))
                .Select(it => it.GroupBy(l => l))
                .Select(it => new
                {
                    Has2Group = it.Any(g => g.Count() == 2),
                    Has3Group = it.Any(g => g.Count() == 3)
                });

            return (lines.Where(it => it.Has2Group).Count() * lines.Where(it => it.Has3Group).Count()).ToString();
        }

        [Solution(2, 2)]
        public string Problem2(string input)
        {
            var lines = input.Split('\n')
                .Select(it => it.Trim())
                .Where(it => !string.IsNullOrEmpty(it))
                .ToArray();
            var targetLength = lines[0].Length - 1;

            for(var i = 0; i < lines.Length; i++)
            {
                var match = lines.Skip(i + 1)
                    .Select(it => it.Zip(lines[i], (a, b) => new { A = a, B = b }).Where(l => l.A == l.B))
                    .Where(it => it.Count() == targetLength)
                    .FirstOrDefault();

                if(match != null)
                    return string.Join(string.Empty, match.Select(it => it.A));
            }

            return null;
        }
    }
}
