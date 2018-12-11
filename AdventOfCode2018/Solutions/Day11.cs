using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day11
    {
        [Solution(11, 1)]
        public string Problem1(string input)
        {
            var serial = int.Parse(input);
            var grid = new int[299,299];

            for(var i = 0; i < 299; i++)
            {
                for(var j = 0; j < 299; j++)
                {
                    if(i == 2 && j == 4)
                    {
                        System.Diagnostics.Debug.WriteLine("Debug");
                    }

                    var rackId = (i + 11);
                    var powerLevel = rackId * (j + 1);
                    powerLevel += serial;
                    powerLevel *= rackId;
                    powerLevel = (powerLevel / 100) % 10;
                    powerLevel -= 5;
                    grid[i, j] = powerLevel;
                }
            }

            var maxX = 0;
            var maxY = 0;
            var maxValue = int.MinValue;

            for(var i = 0; i < 297; i++)
            {
                for(var j = 0; j < 297; j++)
                {
                    var sum = grid[i, j] + grid[i, j + 1] + grid[i, j + 2]
                        + grid[i + 1, j] + grid[i + 1, j + 1] + grid[i + 1, j + 2]
                        + grid[i + 2, j] + grid[i + 2, j + 1] + grid[i + 2, j + 2];

                    if(sum > maxValue)
                    {
                        maxValue = sum;
                        maxX = i + 1;
                        maxY = j + 1;
                    }
                }
            }

            return $"{maxX},{maxY}";
        }

        [Solution(11, 2)]
        public string Problem2(string input)
        {
            var serial = int.Parse(input);
            var grid = new int[299, 299];

            for (var i = 0; i < 299; i++)
            {
                for (var j = 0; j < 299; j++)
                {
                    if (i == 2 && j == 4)
                    {
                        System.Diagnostics.Debug.WriteLine("Debug");
                    }

                    var rackId = (i + 11);
                    var powerLevel = rackId * (j + 1);
                    powerLevel += serial;
                    powerLevel *= rackId;
                    powerLevel = (powerLevel / 100) % 10;
                    powerLevel -= 5;
                    grid[i, j] = powerLevel;
                }
            }

            var maxX = 0;
            var maxY = 0;
            var maxSize = 1;
            var maxValue = int.MinValue;

            for(var s = 1; s < 301; s++)
            {
                var peak = 300 - s;

                for(var i = 0; i < peak; i++)
                {
                    for (var j = 0; j < peak; j++)
                    {
                        var sum = GetValue(i, j, s, grid);
                        if(sum > maxValue)
                        {
                            maxValue = sum;
                            maxX = i + 1;
                            maxY = j + 1;
                            maxSize = s;
                        }
                    }
                }
            }

            return $"{maxX},{maxY},{maxSize}";
        }

        private int GetValue(int x, int y, int size, int[,] grid)
        {
            var value = 0;

            for (var i = x; i < x + size; i++)
            {
                for (var j = y; j < y + size; j++)
                {
                    value += grid[i, j];
                }
            }

            return value;
        }
    }
}
