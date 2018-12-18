using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Common
{
    class Dumper
    {
        public static void DumpMap<T>(T[,] map, Func<T, string> converter)
        {
            for(var i = 0; i < map.GetLength(1); i++)
            {
                var sb = new StringBuilder();

                for(var j = 0; j < map.GetLength(0); j++)
                {
                    sb.Append(converter(map[j, i]));
                }

                Console.WriteLine(sb.ToString());
            }
        }
    }
}
