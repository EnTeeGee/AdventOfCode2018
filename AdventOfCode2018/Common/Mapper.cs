using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Common
{
    class Mapper
    {
        public static string[] ToLines(string input)
        {
            return input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static T[,] ConvertToMap<T>(string input, Func<char, T> converter)
        {
            var lines = ToLines(input);
            var map = new T[lines[0].Length, lines.Length];

            for(var i = 0; i < lines.Length; i++)
            {
                for(var j = 0; j < lines[i].Length; j++)
                {
                    map[j, i] = converter(lines[i][j]);
                }
            }

            return map;
        }
    }
}
