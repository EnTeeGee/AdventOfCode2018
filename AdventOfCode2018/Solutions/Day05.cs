using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day05
    {
        [Solution(5, 1)]
        public int Problem1(string input)
        {
            var items = input.Trim().ToList();
            var index = 0;

            while(index + 1 < items.Count())
            {
                if(WillReact(items[index], items[index + 1]))
                {
                    items.RemoveAt(index);
                    items.RemoveAt(index);

                    if (index > 0)
                        index--;
                }
                else
                {
                    index++;
                }
            }

            return items.Count();
        }

        [Solution(5, 2)]
        public int Problem2(string input)
        {
            var min = int.MaxValue;

            for(var i = 'A'; i <= 'Z'; i++)
            {
                var filteredInput = new String(input.Where(it => it != i && it != i + Step).ToArray());
                var result = Problem1(filteredInput);

                if (result < min)
                    min = result;
            }

            return min;
        }

        private bool WillReact(char a, char b)
        {
            return a + Step == b || b + Step == a;
        }

        private const int Step = 32;
    }
}
