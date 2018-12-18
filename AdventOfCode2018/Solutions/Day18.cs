using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day18
    {
        private const int Billion = 1000000000;

        [Solution(18, 1)]
        public int Problem1(string input)
        {
            var map = Mapper.ConvertToMap(input, Convert);

            for(var loop = 0; loop < 10; loop++)
            {
                var newMap = new TileState[map.GetLength(0), map.GetLength(1)];

                for(var i = 0; i < map.GetLength(0); i++)
                {
                    for(var j = 0; j < map.GetLength(1); j++)
                    {
                        var surrounding = GetNeighbours(map, i, j);
                        var current = map[i, j];
                        newMap[i, j] = current;

                        if (current == TileState.Open && surrounding.Count(it => it == TileState.Trees) >= 3)
                            newMap[i, j] = TileState.Trees;

                        if (current == TileState.Trees && surrounding.Count(it => it == TileState.Lumber) >= 3)
                            newMap[i, j] = TileState.Lumber;

                        if (current == TileState.Lumber && (!surrounding.Any(it => it == TileState.Lumber) || !surrounding.Any(it => it == TileState.Trees)))
                            newMap[i, j] = TileState.Open;
                    }
                }

                map = newMap;
                Console.WriteLine("After" + (loop + 1) + " minutes");
                Dumper.DumpMap(map, ConvertBack);
            }

            var totalTrees = 0;
            var totalLumber = 0;

            for(var i = 0; i < map.GetLength(0); i++)
            {
                for(var j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == TileState.Trees)
                        totalTrees++;
                    else if (map[i, j] == TileState.Lumber)
                        totalLumber++;
                }
            }

            return totalTrees * totalLumber;
        }

        [Solution(18, 2)]
        public int Problem2(string input)
        {
            var map = Mapper.ConvertToMap(input, Convert);
            var scores = new List<int> { GetScore(map) };

            for (var loop = 0; loop < 1000000000; loop++)
            {
                var newMap = new TileState[map.GetLength(0), map.GetLength(1)];

                for (var i = 0; i < map.GetLength(0); i++)
                {
                    for (var j = 0; j < map.GetLength(1); j++)
                    {
                        var surrounding = GetNeighbours(map, i, j);
                        var current = map[i, j];
                        newMap[i, j] = current;

                        if (current == TileState.Open && surrounding.Count(it => it == TileState.Trees) >= 3)
                            newMap[i, j] = TileState.Trees;

                        if (current == TileState.Trees && surrounding.Count(it => it == TileState.Lumber) >= 3)
                            newMap[i, j] = TileState.Lumber;

                        if (current == TileState.Lumber && (!surrounding.Any(it => it == TileState.Lumber) || !surrounding.Any(it => it == TileState.Trees)))
                            newMap[i, j] = TileState.Open;
                    }
                }

                map = newMap;
                scores.Add(GetScore(map));
                var result = CheckForRepeat(scores);

                if (result != null)
                    return result.Value;
                //Console.WriteLine("After" + (loop + 1) + " minutes");
                //Dumper.DumpMap(map, ConvertBack);
                //Console.WriteLine($"score for {loop}:\t{GetScore(map)}");
            }

            var totalTrees = 0;
            var totalLumber = 0;

            for (var i = 0; i < map.GetLength(0); i++)
            {
                for (var j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == TileState.Trees)
                        totalTrees++;
                    else if (map[i, j] == TileState.Lumber)
                        totalLumber++;
                }
            }

            return totalTrees * totalLumber;
        }

        private TileState Convert(char input)
        {
            if (input == '|')
                return TileState.Trees;
            if (input == '#')
                return TileState.Lumber;

            return TileState.Open;
        }

        private string ConvertBack(TileState input)
        {
            if (input == TileState.Trees)
                return "|";
            if (input == TileState.Lumber)
                return "#";

            return ".";
        }

        private int GetScore(TileState[,] map)
        {
            var totalTrees = 0;
            var totalLumber = 0;

            for (var i = 0; i < map.GetLength(0); i++)
            {
                for (var j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == TileState.Trees)
                        totalTrees++;
                    else if (map[i, j] == TileState.Lumber)
                        totalLumber++;
                }
            }

            return totalTrees * totalLumber;
        }

        private int? CheckForRepeat(List<int> scores)
        {
            //Ignore if still quite short
            if(scores.Count < 10)
                return null;

            var flipped = scores.Reverse<int>().ToList();
            var currentSize = 2;
            while(currentSize * 2 <= scores.Count)
            {
                var areSame = flipped
                    .Take(currentSize)
                    .Zip(flipped.Skip(currentSize).Take(currentSize), (a, b) => new { a, b })
                    .All(it => it.a == it.b);

                if (areSame)
                {
                    Console.WriteLine($"Found match at size {currentSize} at length {scores.Count}");

                    var remaining = Billion - scores.Count();
                    var target = (remaining / currentSize) * currentSize;
                    target = Billion - target;
                    target -= currentSize;
                    return scores[target];
                }
                else
                    ++currentSize;
            }

            return null;
        }

        private TileState[] GetNeighbours(TileState[,] map, int x, int y)
        {
            var points = new List<(int x, int y)>
            {
                (x - 1, y),
                (x - 1, y - 1),
                (x, y - 1),
                (x + 1, y - 1),
                (x + 1, y),
                (x + 1, y + 1),
                (x, y + 1),
                (x - 1, y + 1),
            }.Where(it => it.x >= 0 && it.x < map.GetLength(0) && it.y >= 0 && it.y < map.GetLength(1))
            .Select(it => map[it.x, it.y]).ToArray();

            return points;
        }

        private enum TileState
        {
            Open,
            Trees,
            Lumber
        }
    }
}
