using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day07
    {
        [Solution(7, 1)]
        public string Problem1(string input)
        {
            var nodes = ToNodeList(input);

            var output = new List<char>();

            while (nodes.Any())
            {
                var next = nodes.Where(it => !it.Requirements.Any()).OrderBy(it => it.Letter).First();
                nodes.Remove(next);
                output.Add(next.Letter);
                foreach (var item in nodes)
                    item.Requirements.Remove(next.Letter);
            }

            return new string(output.ToArray());
        }

        private const int ExtraPerLetter = 60;
        private const int NumberOfWorkers = 5;

        [Solution(7, 2)]
        public int Problem2(string input)
        {
            var nodes = ToNodeList(input);

            var workers = Enumerable.Range(0, NumberOfWorkers).Select(it => new Worker(nodes)).ToArray();
            var totalTime = -1;

            while(nodes.Any() || workers.Any(it => it.IsWorking()))
            {
                foreach (var item in workers)
                    item.Decrement();
                foreach (var item in workers)
                    item.GetNextJob();
                ++totalTime;
            }

            return totalTime;
        }

        private List<Node> ToNodeList(string input)
        {
            var items = input
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(it => it.Split(' '))
                .Select(it => new { Step = it[7][0], Require = it[1][0] })
                .ToArray();

            var nodes = items.Select(it => it.Step)
                .Concat(items.Select(it => it.Require))
                .Distinct().Select(it => new Node(it))
                .ToList();

            foreach (var item in items)
            {
                var node = nodes.First(it => it.Letter == item.Step);
                node.Requirements.Add(item.Require);
            }

            return nodes;
        }

        private class Node
        {
            public char Letter { get; }

            public List<char> Requirements { get; }

            public Node(char letter)
            {
                this.Letter = letter;
                this.Requirements = new List<char>();
            }
        }

        private class Worker
        {
            private Node current;

            private int remaining;

            private List<Node> jobs;

            public Worker(List<Node> jobs)
            {
                this.jobs = jobs;
            }

            public void Decrement()
            {
                if (current == null)
                    return;

                remaining--;

                if (remaining != 0)
                    return;

                foreach (var item in jobs)
                    item.Requirements.Remove(current.Letter);
                current = null;
            }

            public void GetNextJob()
            {
                if (current != null)
                    return;

                var next = jobs.Where(it => !it.Requirements.Any()).OrderBy(it => it.Letter).FirstOrDefault();

                if (next == null)
                    return;

                jobs.Remove(next);
                current = next;
                remaining = TimeForLetter(current.Letter);
            }

            public bool IsWorking()
            {
                return current != null;
            }

            private int TimeForLetter(char letter)
            {
                return letter - 64 + ExtraPerLetter;
            }
        }
    }
}
