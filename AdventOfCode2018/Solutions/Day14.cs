using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day14
    {
        [Solution(14, 1)]
        public string Problem1(string input)
        {
            var scores = new LinkedList<int>();
            scores.AddLast(3);
            scores.AddLast(7);
            var elf1 = scores.First;
            var elf2 = scores.Last;
            var inputVal = int.Parse(input);
            var target = inputVal + 10;
            var current = 2;

            while(current < target)
            {
                var sum = elf1.Value + elf2.Value;
                var entries = sum.ToString();
                foreach(var item in entries)
                {
                    scores.AddLast(int.Parse(item.ToString()));
                }
                current += entries.Length;
                var elf1Score = elf1.Value + 1;
                var elf2Score = elf2.Value + 1;

                for (var i = 0; i < elf1Score; i++)
                    elf1 = elf1.Next ?? scores.First;
                for (var i = 0; i < elf2Score; i++)
                    elf2 = elf2.Next ?? scores.First;
            }

            var node = scores.Last;
            for (var i = current; i > inputVal + 1; i--)
                node = node.Previous;

            var output = new List<string>();
            for(var i = 0; i < 10; i++)
            {
                output.Add(node.Value.ToString());
                node = node.Next;
            }

            return string.Join(string.Empty, output);
        }

        [Solution(14, 2)]
        public int Problem2(string input)
        {
            var scores = new LinkedList<int>();
            scores.AddLast(3);
            scores.AddLast(7);
            var elf1 = scores.First;
            var elf2 = scores.Last;
            var inputVal = input.Select(it => int.Parse(it.ToString())).Reverse().ToArray();
            var current = -4;

            while (true)
            {
                var sum = elf1.Value + elf2.Value;
                var entries = sum.ToString();
                foreach (var item in entries)
                {
                    scores.AddLast(int.Parse(item.ToString()));
                    current += 1;
                    if (IsMatch(scores.Last, inputVal))
                        return current;
                }
                var elf1Score = elf1.Value + 1;
                var elf2Score = elf2.Value + 1;

                for (var i = 0; i < elf1Score; i++)
                    elf1 = elf1.Next ?? scores.First;
                for (var i = 0; i < elf2Score; i++)
                    elf2 = elf2.Next ?? scores.First;
            }
        }

        private bool IsMatch(LinkedListNode<int> last, int[] target)
        {
            var current = last;
            for(var i = 0; i < target.Length; i++)
            {
                if (current.Value != target[i])
                    return false;
                current = current.Previous;
            }

            return true;
        }
    }
}
