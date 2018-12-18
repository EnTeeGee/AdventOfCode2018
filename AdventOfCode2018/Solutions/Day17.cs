using AdventOfCode2018.Common;
using System;
using System.Linq;
using System.Text;

namespace AdventOfCode2018.Solutions
{
    class Day17
    {
        [Solution(17, 1)]
        public int Problem1(string input)
        {
            var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(it => new PointRange(it)).ToArray();

            var minX = lines.SelectMany(it => it.Points).Min(it => it.X);
            var maxX = lines.SelectMany(it => it.Points).Max(it => it.X);
            var minY = lines.SelectMany(it => it.Points).Min(it => it.Y);
            var maxY = lines.SelectMany(it => it.Points).Max(it => it.Y);

            var map = new TileState[maxX - minX + 3, maxY - minY + 1];

            foreach(var point in lines.SelectMany(it => it.Points))
            {
                point.X -= (minX - 1);
                point.Y -= minY;
                map[point.X, point.Y] = TileState.Clay;
            }

            var startPoint = new Point(500 - minX + 1, 0);

            Drop(map, startPoint, maxY - minY);

            DumpMapToScreen(map);

            var totalWater = 0;
            for(var i = 0; i < map.GetLength(0); i++)
            {
                for(var j = 0; j < map.GetLength(1); j++)
                {
                    var tile = map[i, j];
                    if (tile == TileState.FlowingWater || tile == TileState.StillWater)
                        ++totalWater;
                }
            }

            return totalWater;
        }

        [Solution(17, 2)]
        public int Problem2(string input)
        {
            var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(it => new PointRange(it)).ToArray();

            var minX = lines.SelectMany(it => it.Points).Min(it => it.X);
            var maxX = lines.SelectMany(it => it.Points).Max(it => it.X);
            var minY = lines.SelectMany(it => it.Points).Min(it => it.Y);
            var maxY = lines.SelectMany(it => it.Points).Max(it => it.Y);

            var map = new TileState[maxX - minX + 3, maxY - minY + 1];

            foreach (var point in lines.SelectMany(it => it.Points))
            {
                point.X -= (minX - 1);
                point.Y -= minY;
                map[point.X, point.Y] = TileState.Clay;
            }

            var startPoint = new Point(500 - minX + 1, 0);

            Drop(map, startPoint, maxY - minY);

            DumpMapToScreen(map);

            var totalWater = 0;
            for (var i = 0; i < map.GetLength(0); i++)
            {
                for (var j = 0; j < map.GetLength(1); j++)
                {
                    var tile = map[i, j];
                    if (tile == TileState.StillWater)
                        ++totalWater;
                }
            }

            return totalWater;
        }

        private void Drop(TileState[,] map, Point startPoint, int maxY)
        {
            var currentPoint = startPoint;

            while (true)
            {
                map[currentPoint.X, currentPoint.Y] = TileState.FlowingWater;
                currentPoint = new Point(currentPoint.X, currentPoint.Y + 1);

                if (currentPoint.Y > maxY)
                    return;

                if (map[currentPoint.X, currentPoint.Y] == TileState.FlowingWater)
                    return;

                if(map[currentPoint.X, currentPoint.Y] == TileState.Clay || map[currentPoint.X, currentPoint.Y] == TileState.StillWater)
                {
                    //Console.WriteLine("About to spread");
                    //DumpMapToScreen(map);
                    Spread(map, new Point(currentPoint.X, currentPoint.Y - 1), maxY);
                    return;
                }
            }
        }

        private void Spread(TileState[,] map, Point startPoint, int maxY)
        {
            var isLeftFree = true;
            var currentPoint = startPoint;
            while (isLeftFree)
            {
                var nextPoint = new Point(currentPoint.X - 1, currentPoint.Y);
                if(map[nextPoint.X, nextPoint.Y] == TileState.Clay)
                {
                    isLeftFree = false;
                    break;
                }

                map[nextPoint.X, nextPoint.Y] = TileState.FlowingWater;

                if (map[nextPoint.X, nextPoint.Y + 1] == TileState.FlowingWater)
                    break;
                if(map[nextPoint.X, nextPoint.Y + 1] == TileState.Sand)
                {
                    //Console.WriteLine("About to drop left");
                    //DumpMapToScreen(map);
                    Drop(map, new Point(nextPoint.X, nextPoint.Y + 1), maxY);
                    break;
                }

                currentPoint = nextPoint;
            }

            var isRightFree = true;
            currentPoint = startPoint;
            while (isRightFree)
            {
                var nextPoint = new Point(currentPoint.X + 1, currentPoint.Y);
                if (map[nextPoint.X, nextPoint.Y] == TileState.Clay || map[nextPoint.X, nextPoint.Y] == TileState.StillWater)
                {
                    isRightFree = false;
                    break;
                }

                map[nextPoint.X, nextPoint.Y] = TileState.FlowingWater;

                if (map[nextPoint.X, nextPoint.Y + 1] == TileState.FlowingWater)
                    break;
                if (map[nextPoint.X, nextPoint.Y + 1] == TileState.Sand)
                {
                    //Console.WriteLine("About to drop right");
                    //DumpMapToScreen(map);
                    Drop(map, new Point(nextPoint.X, nextPoint.Y + 1), maxY);
                    break;
                }

                currentPoint = nextPoint;
            }

            if(!isLeftFree && !isRightFree)
            {
                currentPoint = startPoint;
                while(map[currentPoint.X, currentPoint.Y] != TileState.Clay)
                {
                    map[currentPoint.X, currentPoint.Y] = TileState.StillWater;
                    currentPoint = new Point(currentPoint.X - 1, currentPoint.Y);
                }

                currentPoint = startPoint;
                while (map[currentPoint.X, currentPoint.Y] != TileState.Clay)
                {
                    map[currentPoint.X, currentPoint.Y] = TileState.StillWater;
                    currentPoint = new Point(currentPoint.X + 1, currentPoint.Y);
                }

                map[startPoint.X, startPoint.Y - 1] = TileState.FlowingWater;
                //Console.WriteLine("About to spread up");
                //DumpMapToScreen(map);
                Spread(map, new Point(startPoint.X, startPoint.Y - 1), maxY);
            }

        }

        private void DumpMapToScreen(TileState[,] map)
        {
            for(var i = 0; i < map.GetLength(1); i++)
            {
                var sb = new StringBuilder();

                for(var j = 0; j < map.GetLength(0); j++)
                {
                    switch(map[j, i])
                    {
                        case TileState.Sand:
                            sb.Append(".");
                            break;
                        case TileState.Clay:
                            sb.Append("#");
                            break;
                        case TileState.FlowingWater:
                            sb.Append("|");
                            break;
                        case TileState.StillWater:
                            sb.Append("~");
                            break;
                    }
                }

                Console.WriteLine(sb.ToString());
            }
        }

        private class PointRange
        {
            public Point[] Points { get; }

            public PointRange(string input)
            {
                var items = input.Split(new[] { '=', ',', ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
                var single = int.Parse(items[1]);
                var rangeStart = int.Parse(items[3]);
                var rangeEnd = int.Parse(items[4]);
                var range = rangeEnd - rangeStart;

                if (items[0] == "x")
                    Points = Enumerable.Range(rangeStart, range + 1).Select(it => new Point(single, it)).ToArray();
                else
                    Points = Enumerable.Range(rangeStart, range + 1).Select(it => new Point(it, single)).ToArray();
            }

            public void MarkOnMap(bool[,] map)
            {

            }
        }

        private class Point
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        private enum TileState
        {
            Sand,
            Clay,
            FlowingWater,
            StillWater
        }
    }
}
