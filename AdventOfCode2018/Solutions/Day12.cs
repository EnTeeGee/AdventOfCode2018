using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day12
    {
        [Solution(12, 1)]
        public int Problem1(string input)
        {
            return Run(input, 20);
        }

        // NOTE: This was not run to completion! ran until obvious pattern emerged and caculated by hand from there.
        [Solution(12, 2)]
        public int Problem2(string input)
        {
            return Run(input, 50000000000);
        }

        private int Run(string input, long generations)
        {
            var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            var pots = new LinkedList<Node>();

            var initialState = lines[0].Split(' ')[2];
            for (var i = 0; i < initialState.Length; i++)
                pots.AddLast(new Node(initialState[i] == '#', i));

            var results = lines.Skip(1).Select(it => new GrowthResult(it)).ToArray();
            var lastResult = pots.Where(it => it.IsGrown).Sum(it => it.Index);
            Console.WriteLine($"Value at 0: {lastResult}");

            for (var i = 0; i < generations; i++)
            {
                //Pad ends
                while (pots.First.Value.IsGrown || pots.First.Next.Value.IsGrown)
                    pots.AddFirst(new Node(false, pots.First.Value.Index - 1));

                while (pots.Last.Value.IsGrown || pots.Last.Previous.Value.IsGrown)
                    pots.AddLast(new Node(false, pots.Last.Value.Index + 1));

                var currentNode = pots.First;

                while (currentNode != null)
                {
                    var match = results.First(it => it.IsMatch(currentNode));
                    currentNode.Value.UpdateFrom(match);
                    currentNode = currentNode.Next;
                }

                foreach (var item in pots)
                    item.Swap();

                var newResult = pots.Where(it => it.IsGrown).Sum(it => it.Index);
                Console.WriteLine($"Value at {i + 1}: {newResult}\tChange: {newResult - lastResult}");
                lastResult = newResult;
            }

            return pots.Where(it => it.IsGrown).Sum(it => it.Index);
        }

        private class GrowthResult
        {
            bool Left2 { get; }
            bool Left1 { get; }
            bool Centre { get; }
            bool Right1 { get; }
            bool Right2 { get; }
            public bool Result { get; }

            public GrowthResult(string input)
            {
                Left2 = input[0] == '#';
                Left1 = input[1] == '#';
                Centre = input[2] == '#';
                Right1 = input[3] == '#';
                Right2 = input[4] == '#';
                Result = input[9] == '#';
            }

            public bool IsMatch(LinkedListNode<Node> input)
            {
                return input.Value.IsGrown == Centre
                    && (input.Previous?.Value.IsGrown ?? false) == Left1
                    && (input.Previous?.Previous?.Value.IsGrown ?? false) == Left2
                    && (input.Next?.Value.IsGrown ?? false) == Right1
                    && (input.Next?.Next?.Value.IsGrown ?? false) == Right2;
            }

            public Node Generate(Node source)
            {
                return new Node(Result, source.Index);
            }
        }

        private class Node
        {
            public bool IsGrown { get; set; }
            bool toSwap { get; set; }
            public int Index { get; }

            public Node(bool isGrown, int index)
            {
                this.IsGrown = isGrown;
                this.Index = index;
            }

            public void UpdateFrom(GrowthResult result)
            {
                this.toSwap = result.Result;
            }

            public void Swap()
            {
                IsGrown = toSwap;
            }
        }
    }
}
