using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day22
    {
        private const int YZeroXMultiplier = 16807;
        private const int XZeroYMultiplier = 48271;
        private const int ErosionLevelModulo = 20183;

        [Solution(22, 1)]
        public int Problem1(string input)
        {
            var info = GetInfo(input);

            var erosionLevels = new int[info.Target.X + 1, info.Target.Y + 1];
            erosionLevels[0, 0] = info.Depth % ErosionLevelModulo;

            for(var x = 1; x <= info.Target.X; x++)
            {
                var gi = x * YZeroXMultiplier;
                erosionLevels[x, 0] = (gi + info.Depth) % ErosionLevelModulo;
            }

            for(var y = 1; y <= info.Target.Y; y++)
            {
                var gi = y * XZeroYMultiplier;
                erosionLevels[0, y] = (gi + info.Depth) % ErosionLevelModulo;
            }

            for(var y = 1; y <= info.Target.Y; y++)
            {
                for(var x = 1; x <= info.Target.X; x++)
                {
                    var gi = erosionLevels[x - 1, y] * erosionLevels[x, y - 1];
                    erosionLevels[x, y] = (gi + info.Depth) % ErosionLevelModulo;
                }
            }

            erosionLevels[info.Target.X, info.Target.Y] = erosionLevels[0, 0];

            var sumOfMap = 0;
            for(var y = 0; y <= info.Target.Y; y++)
            {
                for(var x = 0; x <= info.Target.X; x++)
                {
                    var value = erosionLevels[x, y] % 3;
                    sumOfMap += value;
                }
            }

            return sumOfMap;
        }

        [Solution(22, 2)]
        public int Problem2(string input)
        {
            var info = GetInfo(input);

            var thread = new Thread(new ThreadStart(Prob2Start), 1024 * 1024 * 10);
            thread.Start();

            //var system = new CaveSystem(info);
            ////system.DumpMap();
            //system.Start();

            //return system.currentMinTimeToTarget;
            return 0;
        }

        private void Prob2Start()
        {
            var info = GetInfo("depth: 10647\ntarget: 7,770");
            Console.WriteLine("Running in thread");
            var system = new CaveSystem(info);
            //system.DumpMap();
            system.Start();
            Console.WriteLine("Result: " + system.currentMinTimeToTarget);
        }

        private class CaveSystem
        {
            private const int ToChange = 7;

            private CaveSquare origin;
            private CaveSquare peakX;
            private CaveSquare peakY;
            private CaveSquare target;
            
            private (int Depth, Point Target) info;

            public int currentMinTimeToTarget { get; set; }

            public CaveSystem((int Depth, Point Target) info)
            {
                this.info = info;
                var originEl = info.Depth % ErosionLevelModulo;
                origin = new CaveSquare(0, 0, ToTileType(originEl), originEl);
                peakX = origin;
                peakY = origin;

                for (var x = 1; x <= info.Target.X; x++)
                {
                    var gi = x * YZeroXMultiplier;
                    var el = (gi + info.Depth) % ErosionLevelModulo;
                    var square = new CaveSquare(x, 0, ToTileType(el), el);
                    square.West = peakX;
                    peakX.East = square;
                    peakX = square;
                }

                for (var y = 1; y <= info.Target.Y; y++)
                {
                    var gi = y * XZeroYMultiplier;
                    var el = (gi + info.Depth) % ErosionLevelModulo;
                    var square = new CaveSquare(0, y, ToTileType(el), el);
                    square.North = peakY;
                    peakY.South = square;
                    peakY = square;
                }

                for (var y = 1; y <= info.Target.Y; y++)
                {
                    for (var x = 1; x <= info.Target.X; x++)
                    {
                        var left = FindSquare(x - 1, y);
                        var above = left.North.East;
                        var gi = left.ErosionLevel * above.ErosionLevel;
                        var el = (gi + info.Depth) % ErosionLevelModulo;
                        var square = new CaveSquare(x, y, ToTileType(el), el);
                        square.West = left;
                        left.East = square;
                        square.North = above;
                        above.South = square;
                    }
                }

                target = FindSquare(info.Target.X, info.Target.Y);
                target.ErosionLevel = origin.ErosionLevel;
                target.Type = origin.Type;

                // Hypothetical peak min time to target
                currentMinTimeToTarget = (target.Position.X + target.Position.Y) * 8;
            }

            public void DumpMap()
            {
                var current = origin;
                while(current != null)
                {
                    var currentX = current;
                    var sb = new StringBuilder();
                    while(currentX != null)
                    {
                        if (currentX.Type == TileType.Rocky)
                            sb.Append(".");
                        else if (currentX.Type == TileType.Wet)
                            sb.Append("=");
                        else if (currentX.Type == TileType.Narrow)
                            sb.Append("|");
                        else
                            sb.Append("#");

                        currentX = currentX.East;
                    }

                    Console.WriteLine(sb.ToString());
                    current = current.South;
                }
            }

            public void Start()
            {
                ExploreSquare(origin, Equiped.Torch, 0, 0);
            }

            public void ExploreSquare(CaveSquare square, Equiped equiped, int currentTime, int depth)
            {
                //history = history.Concat(new List<CaveSquare> { square }).ToList();
                depth++;

                if (!IsVaild(square, equiped))
                    return;

                if (currentTime > currentMinTimeToTarget)
                    return;

                var options = SetTimes(square, equiped, currentTime);

                if(square == target)
                {
                    
                    var getTorch = equiped == Equiped.Torch ? currentTime : currentTime + ToChange;
                    var newMinTime = Math.Min(getTorch, currentMinTimeToTarget);

                    if(currentMinTimeToTarget > newMinTime)
                    {
                        currentMinTimeToTarget = newMinTime;
                        Console.WriteLine("Got new min time, " + currentMinTimeToTarget);
                    }

                    //if(currentMinTimeToTarget == 55)
                    //{
                    //    foreach (var item in history)
                    //        Console.WriteLine($"Pos: {item.Position}, Gear: {item.GearDist} Torch: {item.TorchDist} None: {item.NoneDist}");

                    //    Console.WriteLine("Debugging");
                    //}

                    return;
                }

                if (!options.Any())
                    return;

                if (square.East == null)
                    AddRowRight();
                if (square.South == null)
                    AddRowDown();

                // Following options
                // Upper left of position
                if(square.Position.X <= target.Position.X && square.Position.Y <= target.Position.Y)
                {
                    foreach (var item in options)
                        ExploreSquare(square.South, item.equiped, item.time, depth);
                    foreach (var item in options)
                        ExploreSquare(square.East, item.equiped, item.time, depth);

                    if(square.North != null)
                    {
                        foreach (var item in options)
                            ExploreSquare(square.North, item.equiped, item.time, depth);
                    }

                    if (square.West != null)
                        foreach (var item in options)
                            ExploreSquare(square.West, item.equiped, item.time, depth);
                }
                // Upper right of position
                else if (square.Position.X > target.Position.X && square.Position.Y <= target.Position.Y)
                {
                    foreach (var item in options)
                        ExploreSquare(square.West, item.equiped, item.time, depth);
                    foreach (var item in options)
                        ExploreSquare(square.South, item.equiped, item.time, depth);
                    if(square.North != null)
                    {
                        foreach (var item in options)
                            ExploreSquare(square.North, item.equiped, item.time, depth);
                    }
                    foreach (var item in options)
                        ExploreSquare(square.East, item.equiped, item.time, depth);
                }
                // Lower left of position
                else if (square.Position.X <= target.Position.X && square.Position.Y > target.Position.Y)
                {
                    foreach (var item in options)
                        ExploreSquare(square.East, item.equiped, item.time, depth);
                    foreach (var item in options)
                        ExploreSquare(square.North, item.equiped, item.time, depth);
                    if(square.West != null)
                    {
                        foreach (var item in options)
                            ExploreSquare(square.West, item.equiped, item.time, depth);
                    }
                    foreach (var item in options)
                        ExploreSquare(square.South, item.equiped, item.time, depth);
                }
                // Lower right of position
                else
                {
                    foreach (var item in options)
                        ExploreSquare(square.West, item.equiped, item.time, depth);
                    foreach (var item in options)
                        ExploreSquare(square.North, item.equiped, item.time, depth);
                    foreach (var item in options)
                        ExploreSquare(square.South, item.equiped, item.time, depth);
                    foreach (var item in options)
                        ExploreSquare(square.East, item.equiped, item.time, depth);
                }
            }

            private bool IsVaild(CaveSquare square, Equiped equiped)
            {
                if (square.Type == TileType.Rocky && equiped == Equiped.None)
                    return false;

                if (square.Type == TileType.Wet && equiped == Equiped.Torch)
                    return false;

                if (square.Type == TileType.Narrow && equiped == Equiped.Gear)
                    return false;

                return true;
            }

            private List<(Equiped equiped, int time)> SetTimes(CaveSquare square, Equiped equiped, int currentTime)
            {
                var output = new List<(Equiped equiped, int time)>();
                if (ShouldEquipContinue(square, equiped, currentTime))
                    output.Add((equiped, currentTime + 1));

                if (equiped == Equiped.Gear)
                    square.GearDist = GetNewDist(square.GearDist, currentTime);
                else if (equiped == Equiped.Torch)
                    square.TorchDist = GetNewDist(square.TorchDist, currentTime);
                else if (equiped == Equiped.None)
                    square.NoneDist = GetNewDist(square.NoneDist, currentTime);

                var altEquip = GetAltEqupment(square, equiped);
                var newTime = currentTime + ToChange;

                if (ShouldEquipContinue(square, altEquip, newTime))
                    output.Add((altEquip, newTime + 1));

                if (altEquip == Equiped.Gear)
                    square.GearDist = GetNewDist(square.GearDist, newTime);
                else if (altEquip == Equiped.Torch)
                    square.TorchDist = GetNewDist(square.TorchDist, newTime);
                else if (altEquip == Equiped.None)
                    square.NoneDist = GetNewDist(square.NoneDist, newTime);

                return output;
            }

            private bool ShouldEquipContinue(CaveSquare square, Equiped equiped, int currentTime)
            {
                if (equiped == Equiped.Gear)
                    return square.GearDist == null || square.GearDist.Value > currentTime;
                else if (equiped == Equiped.Torch)
                    return square.TorchDist == null || square.TorchDist.Value > currentTime;
                else if (equiped == Equiped.None)
                    return square.NoneDist == null || square.NoneDist.Value > currentTime;

                return false;
            }

            private TileType ToTileType(int erosionLevel)
            {
                return (TileType)((erosionLevel % 3) + 1);
            }

            private CaveSquare FindSquare(int x, int y)
            {
                var current = origin;
                for (var i = 0; i < x; i++)
                    current = current.East;

                for (var i = 0; i < y; i++)
                    current = current.South;

                return current;
            }

            private void AddRowRight()
            {
                var gi = (peakX.Position.X + 1) * YZeroXMultiplier;
                var el = (gi + info.Depth) % ErosionLevelModulo;
                var square = new CaveSquare(peakX.Position.X + 1, 0, ToTileType(el), el);
                square.West = peakX;
                peakX.East = square;
                peakX = square;

                while(square.West.South != null)
                {
                    gi = square.West.South.ErosionLevel * square.ErosionLevel;
                    el = (gi + info.Depth) % ErosionLevelModulo;
                    var newSquare = new CaveSquare(square.Position.X, square.Position.Y + 1, ToTileType(el), el);
                    newSquare.North = square;
                    square.South = newSquare;
                    newSquare.West = square.West.South;
                    square.West.South.East = newSquare;
                    square = newSquare;
                }
            }

            private void AddRowDown()
            {
                var gi = (peakY.Position.Y + 1) * XZeroYMultiplier;
                var el = (gi + info.Depth) % ErosionLevelModulo;
                var square = new CaveSquare(0, peakY.Position.Y + 1, ToTileType(el), el);
                square.North = peakY;
                peakY.South = square;
                peakY = square;

                while(square.North.East != null)
                {
                    gi = square.ErosionLevel * square.North.East.ErosionLevel;
                    el = (gi + info.Depth) % ErosionLevelModulo;
                    var newSquare = new CaveSquare(square.Position.X + 1, square.Position.Y, ToTileType(el), el);
                    newSquare.West = square;
                    square.East = newSquare;
                    newSquare.North = square.North.East;
                    square.North.East.South = newSquare;
                    square = newSquare;
                }
            }

            private Equiped GetAltEqupment(CaveSquare square, Equiped currentEquiped)
            {
                if (square.Type == TileType.Rocky)
                    return currentEquiped == Equiped.Torch ? Equiped.Gear : Equiped.Torch;
                else if (square.Type == TileType.Wet)
                    return currentEquiped == Equiped.Gear ? Equiped.None : Equiped.Gear;
                else
                    return currentEquiped == Equiped.Torch ? Equiped.None : Equiped.Torch;
            }

            private int GetNewDist(int? current, int newDist)
            {
                if (current == null)
                    return newDist;

                return Math.Min(current.Value, newDist);
            }
        }

        private class CaveSquare
        {
            public Point Position { get; }
            public TileType Type { get; set; }
            public int ErosionLevel { get; set; }

            public CaveSquare North { get; set; }
            public CaveSquare East { get; set; }
            public CaveSquare South { get; set; }
            public CaveSquare West { get; set; }

            public int? TorchDist { get; set; }
            public int? GearDist { get; set; }
            public int? NoneDist { get; set; }

            public CaveSquare(int x, int y, TileType tileType, int erosionLevel)
            {
                this.Position = new Point(x, y);
                this.Type = tileType;
                this.ErosionLevel = erosionLevel;
            }
        }


        private (int Depth, Point Target) GetInfo(string input)
        {
            var items = input.Split(new[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return (Depth: int.Parse(items[1]), Target: new Point(items[3]));
        }

        private struct Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(string input)
            {
                var items = input.Split(',');
                X = int.Parse(items[0]);
                Y = int.Parse(items[1]);
            }

            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public override string ToString()
            {
                return $"({X},{Y})";
            }
        }

        private enum TileType
        {
            Unknown = 0,
            Rocky = 1,
            Wet = 2,
            Narrow = 3
        }

        private enum Equiped
        {
            Torch,
            Gear,
            None
        }
    }
}
