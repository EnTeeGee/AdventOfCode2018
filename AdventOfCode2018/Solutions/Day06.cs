using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018.Solutions
{
    class Day06
    {
        [Solution(6, 1)]
        public int Problem1(string input)
        {
            var points = input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(it => new Point(it)).ToArray();

            var topLeft = new Point(points.Min(it => it.X), points.Min(it => it.Y));
            var bottomRight = new Point(points.Max(it => it.X), points.Max(it => it.Y));
            var dist = new Point(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

            var surrounding = Enumerable.Range(topLeft.X, dist.X).Select(it => new Point(it, topLeft.Y))
                .Concat(Enumerable.Range(topLeft.X, dist.X).Select(it => new Point(it, bottomRight.Y)))
                .Concat(Enumerable.Range(topLeft.Y, dist.Y).Select(it => new Point(topLeft.X, it)))
                .Concat(Enumerable.Range(topLeft.Y, dist.Y).Select(it => new Point(bottomRight.X, it)))
                .ToArray();

            var edgePoints = new List<Point>();

            foreach(var point in surrounding)
            {
                var nearest = points.OrderBy(it => point.Dist(it)).First();
                if (!edgePoints.Contains(nearest))
                    edgePoints.Add(nearest);
            }

            var result = Enumerable.Range(topLeft.X + 1, dist.X - 1)
                .SelectMany(it => Enumerable.Range(topLeft.Y + 1, dist.Y - 1).Select(y => new Point(it, y)))
                .Select(it => GetNearest(it, points))
                .Where(it =>  it != null && !edgePoints.Contains(it.Value))
                .GroupBy(it => it)
                .Select(it => it.Count())
                .Max();

            return result;
        }

        [Solution(6, 2)]
        public int Problem2(string input)
        {
            var maxDist = 10000;
            var points = input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(it => new Point(it)).ToArray();

            var topLeft = new Point(points.Min(it => it.X), points.Min(it => it.Y));
            var bottomRight = new Point(points.Max(it => it.X), points.Max(it => it.Y));
            var dist = new Point(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

            var allPoints = Enumerable.Range(topLeft.X + 1, dist.X - 1)
                .SelectMany(it => Enumerable.Range(topLeft.Y + 1, dist.Y - 1).Select(y => new { Point = new Point(it, y), Dist = 0 }));

            foreach(var item in points)
                allPoints = allPoints.Select(it => new { it.Point, Dist = it.Dist + it.Point.Dist(item) }).Where(it => it.Dist < maxDist);

            return allPoints.Count();
        }

        private Point? GetNearest(Point point, Point[] points)
        {
            var nearest = points.Select(it => new { it, dist = it.Dist(point) }).OrderBy(it => it.dist).Take(2).ToArray();
            if (nearest[0].dist.Equals(nearest[1].dist))
                return null;

            return nearest[0].it;
        }

        private struct Point
        {
            public int X { get; }

            public int Y { get; }

            public Point(string input)
            {
                var items = input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                X = int.Parse(items[0]);
                Y = int.Parse(items[1]);
            }

            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public int Dist(Point point)
            {
                return Math.Abs(X - point.X) + Math.Abs(Y - point.Y);
            }
        }
    }
}
