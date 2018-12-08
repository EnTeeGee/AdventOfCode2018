using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day08
    {
        [Solution(8, 1)]
        public int Problem1(string input)
        {
            var numbers = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(it => int.Parse(it)).ToArray();

            var node = new Node(numbers);

            return node.SumMetadata();
        }

        [Solution(8, 2)]
        public int Problem2(string input)
        {
            var numbers = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(it => int.Parse(it)).ToArray();

            var node = new Node(numbers);

            return node.Value;
        }

        private class Node
        {
            public int Length { get; }

            public List<Node> SubNodes { get; }

            public List<int> Metadata { get; }

            public int Value { get;  }

            public Node(IEnumerable<int> input)
            {
                this.SubNodes = new List<Node>();
                this.Metadata = new List<int>();

                var info = input.Take(2).ToArray();
                var subNodes = info[0];
                var metadata = info[1];

                var remaining = input.Skip(2);

                for(var i = 0; i < subNodes; i++)
                {
                    var newNode = new Node(remaining);
                    remaining = remaining.Skip(newNode.Length);
                    this.SubNodes.Add(newNode);
                }

                this.Metadata.AddRange(remaining.Take(metadata));

                this.Length = 2 + this.SubNodes.Sum(it => it.Length) + this.Metadata.Count;

                this.Value = SumValue();
            }

            public int SumMetadata()
            {
                return SubNodes.Sum(it => it.SumMetadata()) + Metadata.Sum();
            }

            private int SumValue()
            {
                if (!SubNodes.Any())
                    return Metadata.Sum();

                var output = 0;

                foreach(var item in Metadata)
                {
                    if (item < 1 || item > SubNodes.Count())
                        continue;

                    output += SubNodes[item - 1].SumValue();
                }

                return output;
            }
        }
    }
}
