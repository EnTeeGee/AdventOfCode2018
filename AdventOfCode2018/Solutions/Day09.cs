using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    
    class Day09
    {
        [Solution(9, 1)]
        public long Problem1(string input)
        {
            var info = input.Split(' ');
            var players = int.Parse(info[0]);
            var steps = int.Parse(info[6]);

            var currentNode = new RingNode(0);
            currentNode.Next = currentNode;
            currentNode.Prev = currentNode;

            var scores = Enumerable.Range(0, players).ToDictionary(it => it, it => (long)0);

            for(var i = 1; i <= steps; i++)
            {
                if(i % 23 == 0)
                {
                    var toRemove = currentNode.Prev.Prev.Prev.Prev.Prev.Prev.Prev;
                    currentNode = toRemove.Next;
                    toRemove.Remove();
                    var currentPlayer = i % players;
                    scores[currentPlayer] = scores[currentPlayer] + i + toRemove.Value;
                }
                else
                {
                    var addedNode = new RingNode(i);
                    addedNode.InsertAfter(currentNode.Next);
                    currentNode = addedNode;
                }
            }

            return scores.Values.Max();
        }

        [Solution(9, 2)]
        public long Problem2(string input)
        {
            var info = input.Split(' ');
            var players = int.Parse(info[0]);
            var steps = int.Parse(info[6]);

            return Problem1($"{players} Players; last marble is worth {steps * 100} points");
        }

        class RingNode
        {
            public int Value { get; }

            public RingNode Next { get; set; }

            public RingNode Prev { get; set; }

            public RingNode(int value)
            {
                this.Value = value;
            }

            public void InsertAfter(RingNode item)
            {
                Next = item.Next;
                Next.Prev = this;
                Prev = item;
                Prev.Next = this;
            }

            public void Remove()
            {
                Prev.Next = Next;
                Next.Prev = Prev;
                Prev = null;
                Next = null;
            }
        }
    }
}
