using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day15
    {
        [Solution(15, 1)]
        public int Problem1(string input)
        {
            var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            var map = new bool[lines[0].Length, lines.Length];
            var entities = new List<Entity>();

            for(var i = 0; i < lines.Length; i++)
            {
                for(var j = 0; j < lines[i].Length; j++)
                {
                    map[j, i] = lines[i][j] != '#';

                    if (lines[i][j] == 'E')
                        entities.Add(new Entity(new Point(j, i), EntityType.Elf));
                    else if (lines[i][j] == 'G')
                        entities.Add(new Entity(new Point(j, i), EntityType.Goblin));
                }
            }

            var totalRounds = 0;

            while(entities.Any(it => it.Type == EntityType.Elf) && entities.Any(it => it.Type == EntityType.Goblin))
            {
                var ordered = entities.OrderBy(it => it.Pos.Y).ThenBy(it => it.Pos.X).ToList();

                System.Diagnostics.Debug.WriteLine("Round " + (totalRounds + 1));

                foreach(var item in ordered)
                {
                    // ignore if already defeated
                    if (!entities.Contains(item))
                        continue;

                    //Attack
                    var targets = NextTo(item, entities);
                    if (targets.Any())
                    {
                        Attack(targets, entities);
                        continue;
                    }

                    //Move
                    //Find nearest
                    var potentialTargets = entities
                        .Where(it => it.Type != item.Type)
                        .SelectMany(it => it.Pos.GetSurrounding())
                        .Where(it => map[it.X, it.Y])
                        .Where(it => !entities.Any(e => e.Pos.IsEqual(it)))
                        .ToArray();

                    var target = GetNearest(item.Pos, potentialTargets, map, entities);
                    if (target == null)
                        continue;

                    //Have target, now need to find most direct route

                    var toMoveTo = GetNearest(target, item.Pos.GetSurrounding(), map, entities);
                    if (toMoveTo == null)
                        continue;

                    item.Pos.X = toMoveTo.X;
                    item.Pos.Y = toMoveTo.Y;

                    //attack if next to something now
                    var newTargets = NextTo(item, entities);
                    if (newTargets.Any())
                    {
                        Attack(newTargets, entities);
                    }
                }

                totalRounds++;
            }

            var result = totalRounds * entities.Sum(it => it.Health);
            var result2 = (totalRounds - 1) * entities.Sum(it => it.Health);

            return result2;
        }

        [Solution(15, 2)]
        public int Problem2(string input)
        {
            var start = 3;
            int? successResult = null;

            while (successResult == null)
            {
                ++start;
                successResult = RunAndCheckLosses(input, start);
            }

            return successResult.Value;
        }

        private int? RunAndCheckLosses(string input, int currentPower)
        {
            var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            var map = new bool[lines[0].Length, lines.Length];
            var entities = new List<Entity>();

            for (var i = 0; i < lines.Length; i++)
            {
                for (var j = 0; j < lines[i].Length; j++)
                {
                    map[j, i] = lines[i][j] != '#';

                    if (lines[i][j] == 'E')
                        entities.Add(new Entity(new Point(j, i), EntityType.Elf, currentPower));
                    else if (lines[i][j] == 'G')
                        entities.Add(new Entity(new Point(j, i), EntityType.Goblin));
                }
            }

            var totalRounds = 0;
            var startingElves = entities.Count(it => it.Type == EntityType.Elf);

            while (entities.Any(it => it.Type == EntityType.Elf) && entities.Any(it => it.Type == EntityType.Goblin))
            {
                var ordered = entities.OrderBy(it => it.Pos.Y).ThenBy(it => it.Pos.X).ToList();

                System.Diagnostics.Debug.WriteLine("Round " + (totalRounds + 1));

                foreach (var item in ordered)
                {
                    // ignore if already defeated
                    if (!entities.Contains(item))
                        continue;

                    //Attack
                    var targets = NextTo(item, entities);
                    if (targets.Any())
                    {
                        Attack(targets, entities, item.Power);
                        continue;
                    }

                    //Move
                    //Find nearest
                    var potentialTargets = entities
                        .Where(it => it.Type != item.Type)
                        .SelectMany(it => it.Pos.GetSurrounding())
                        .Where(it => map[it.X, it.Y])
                        .Where(it => !entities.Any(e => e.Pos.IsEqual(it)))
                        .ToArray();

                    var target = GetNearest(item.Pos, potentialTargets, map, entities);
                    if (target == null)
                        continue;

                    //Have target, now need to find most direct route
                    var toMoveTo = GetNearest(target, item.Pos.GetSurrounding(), map, entities);
                    if (toMoveTo == null)
                        continue;

                    item.Pos.X = toMoveTo.X;
                    item.Pos.Y = toMoveTo.Y;

                    //attack if next to something now
                    var newTargets = NextTo(item, entities);
                    if (newTargets.Any())
                    {
                        Attack(newTargets, entities, item.Power);
                    }
                }

                if (entities.Count(it => it.Type == EntityType.Elf) != startingElves)
                    return null;

                totalRounds++;
            }

            var result = totalRounds * entities.Sum(it => it.Health);
            var result2 = (totalRounds - 1) * entities.Sum(it => it.Health);

            return result2;
        }
        
        private List<Entity> NextTo(Entity current, List<Entity> fullList)
        {
            var points = current.Pos.GetSurrounding();
            var output = new List<Entity>();
            foreach(var item in points)
                output = output.Concat(fullList.Where(it => it.Pos.IsEqual(item) && it.Type != current.Type)).ToList();

            return output;
        }

        private void Attack(List<Entity> targets, List<Entity> fullList, int power = 3)
        {
            var minHealth = targets.Min(it => it.Health);
            var toAttack = targets.Where(it => it.Health == minHealth).OrderBy(it => it.Pos.Y).ThenBy(it => it.Pos.X).First();
            toAttack.Health -= power;
            if (toAttack.Health <= 0)
                fullList.Remove(toAttack);
        }

        private bool IsPointInAnything(Point point, List<Entity> entities, bool[,] map)
        {
            if (!map[point.X, point.Y])
                return true;

            return entities.Any(it => it.Pos.IsEqual(point));
        }

        private Point GetNearest(Point startPoint, Point[] targetPoints, bool[,] map, List<Entity> entities)
        {
            if (targetPoints.Any(it => it.IsEqual(startPoint)))
                return startPoint;

            var checkedGrid = new bool[map.GetLength(0), map.GetLength(1)];
            checkedGrid[startPoint.X, startPoint.Y] = true;
            var currentPoints = new List<Point> { startPoint };
            var currentHitPoints = new List<Point>();

            while(!currentHitPoints.Any() && currentPoints.Any())
            {
                var nextCurrentPoints = new List<Point>();

                foreach(var point in currentPoints)
                {
                    var surrounding = point.GetSurrounding().Where(it => !IsPointInAnything(it, entities, map)).Where(it => !checkedGrid[it.X, it.Y]).ToList();
                    foreach (var newPoint in surrounding)
                        checkedGrid[newPoint.X, newPoint.Y] = true;

                    nextCurrentPoints.AddRange(surrounding);
                }

                currentPoints = nextCurrentPoints;
                currentHitPoints.AddRange(currentPoints.Where(it => targetPoints.Any(tp => tp.IsEqual(it))));
            }

            if (!currentHitPoints.Any())
                return null;

            return currentHitPoints.OrderBy(it => it.Y).ThenBy(it => it.X).First();
        }

        private class Entity
        {
            public Point Pos { get; }
            public EntityType Type { get; }
            public int Health { get; set; }
            public int Power { get; }

            public Entity(Point pos, EntityType entityType)
            {
                this.Pos = pos;
                this.Type = entityType;
                this.Health = 200;
                this.Power = 3;
            }

            public Entity(Point pos, EntityType entityType, int power)
            {
                this.Pos = pos;
                this.Type = entityType;
                this.Health = 200;
                this.Power = power;
            }

            public override string ToString()
            {
                return $"{(Type == EntityType.Elf ? 'E' : 'G')}({Health}) @ ({Pos.X},{Pos.Y})";
            }
        }

        private enum EntityType
        {
            Elf,
            Goblin
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

            public bool IsEqual(Point point)
            {
                return point.X == X && point.Y == Y;
            }

            public override string ToString()
            {
                return $"{X}, {Y}";
            }

            public Point[] GetSurrounding()
            {
                return new Point[4]
                {
                    new Point(X, Y - 1),
                    new Point(X + 1, Y),
                    new Point(X, Y + 1),
                    new Point(X - 1, Y),
                };
            }
        }
    }
}
