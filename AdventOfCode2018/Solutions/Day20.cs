using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day20
    {

        // 7815 Too low for part 2
        [Solution(20, 1)]
        public int Problem1(string input)
        {
            var startingRoom = new Room(0, 0);
            startingRoom.Dist = 0;
            var seenRooms = new Dictionary<long, Room>();
            seenRooms.Add(startingRoom.GetPos(), startingRoom);
            var path = new LinkedList<char>();

            foreach(var item in input)
            {
                if (item == '^' || item == '$')
                    continue;
                path.AddLast(item);
            }

            var mapped = EnumerateRoute(seenRooms, path.First, startingRoom, 0);

            var test = seenRooms.Values.Where(it => it.Dist >= 1000).Count();
            Console.WriteLine($"for solution 2: {test}");

            return seenRooms.Values.Max(it => it.Dist);
        }

        private PosInfo[] EnumerateRoute(Dictionary<long, Room> seenRooms, LinkedListNode<char> path, Room room, int dist)
        {
            var currentPath = path;
            var currentRoom = room;
            var currentDist = dist;

            while(currentPath != null && currentPath.Value != ')' && currentPath.Value != '|')
            {
                //Console.WriteLine($"Heading {currentPath.Value} at dist of {currentDist}");

                if ("NSEW".Contains(currentPath.Value))
                {
                    Room newRoom = null;

                    //if (currentDist == 16 && currentPath.Value == 'N')
                    //    Console.WriteLine("Debugging");

                    if(currentPath.Value == 'N')
                    {
                        if (currentRoom.North != null)
                            newRoom = currentRoom.North;
                        else
                        {
                            newRoom = GetRoom(currentRoom.Pos.X, currentRoom.Pos.Y - 1, seenRooms);
                            newRoom.South = currentRoom;
                            currentRoom.North = newRoom;
                        }
                    }
                    else if (currentPath.Value == 'E')
                    {
                        if (currentRoom.East != null)
                            newRoom = currentRoom.East;
                        else
                        {
                            newRoom = GetRoom(currentRoom.Pos.X + 1, currentRoom.Pos.Y, seenRooms);
                            newRoom.West = currentRoom;
                            currentRoom.East = newRoom;
                        }
                    }
                    else if (currentPath.Value == 'S')
                    {
                        if (currentRoom.South != null)
                            newRoom = currentRoom.South;
                        else
                        {
                            newRoom = GetRoom(currentRoom.Pos.X, currentRoom.Pos.Y + 1, seenRooms);
                            newRoom.North = currentRoom;
                            currentRoom.South = newRoom;
                        }
                    }
                    else // West
                    {
                        if (currentRoom.West != null)
                            newRoom = currentRoom.West;
                        else
                        {
                            newRoom = GetRoom(currentRoom.Pos.X - 1, currentRoom.Pos.Y, seenRooms);
                            newRoom.East = currentRoom;
                            currentRoom.West = newRoom;
                        }
                    }

                    currentDist++;
                    if (newRoom.Dist > currentDist)
                        newRoom.Dist = currentDist;
                    else
                        currentDist = newRoom.Dist;
                        

                    currentPath = currentPath.Next;
                    currentRoom = newRoom;
                    continue;
                }
                else if (currentPath.Value == '(')
                {
                    var subPaths = new List<PosInfo>();
                    var subStr = currentPath;
                    while(subStr.Value != ')')
                    {
                        //Console.WriteLine("Starting sub path");
                        var newSubPaths = EnumerateRoute(seenRooms, subStr.Next, currentRoom, currentDist);
                        subPaths.AddRange(newSubPaths);
                        subStr = newSubPaths[0].CurrentPath;
                    }


                    var routes = subPaths.Select(it => EnumerateRoute(seenRooms, subStr.Next, it.CurrentRoom, it.CurrentDist))
                        .SelectMany(it => it)
                        .GroupBy(it => it.CurrentRoom.Pos);

                    var toFollow = routes.Select(it => it.OrderBy(r => r.CurrentDist).First()).ToArray();

                    return toFollow;

                    //var routes = subPaths.Select(it => EnumerateRoute(seenRooms, subStr.Next, it.CurrentRoom, it.CurrentDist)).SelectMany(it => it);
                    //var minRoute = routes.OrderBy(it => it.CurrentDist).First();
                    //var lastRoute = routes.Last();

                    //return new PosInfo[]
                    //{
                    //    new PosInfo
                    //    {
                    //        CurrentRoom = currentRoom,
                    //        CurrentPath = lastRoute.CurrentPath,
                    //        CurrentDist = minRoute.CurrentDist
                    //    }
                    //};
                }
            }

            return new PosInfo[]
            {
                new PosInfo {
                    CurrentRoom = currentRoom,
                    CurrentPath = currentPath,
                    CurrentDist = currentDist
                }
            };
        }

        private Room GetRoom(int x, int y, Dictionary<long, Room> seenRooms)
        {
            //var count = seenRooms.Count;

            //if (count % 100 == 0)
            //{
            //    Console.WriteLine($"Seen {count} rooms");

            //    if(count % 1000 == 0)
            //        Console.WriteLine($"Current max: {seenRooms.Values.Max(it => it.Dist)}");
            //}
            //if (count > 9100)
            //{
            //    Console.WriteLine($"Seen {count} rooms");
            //    Console.WriteLine($"Current max: {seenRooms.Values.Max(it => it.Dist)}");
            //}
            //if(count == 9117)
            //{
            //    Console.WriteLine("For solution 2:");
            //    Console.WriteLine(seenRooms.Values.Where(it => it.Dist >= 1000).Count());
            //}

            var newRoom = new Room(x, y);
            var key = newRoom.GetPos();
            var matching = seenRooms.ContainsKey(key);

            if (matching)
                return seenRooms[key];

            seenRooms.Add(key, newRoom);

            return newRoom;
        }

        private class Room
        {
            public int Dist { get; set; }
            public Pos Pos { get; set; }
            public Room North { get; set; }
            public Room East { get; set; }
            public Room South { get; set; }
            public Room West { get; set; }

            public Room(int x, int y)
            {
                this.Pos = new Pos
                {
                    X = x,
                    Y = y
                };
                Dist = int.MaxValue;
            }

            public long GetPos()
            {
                long res = Pos.X;
                res = res << 32;
                res = res | (long)(uint)Pos.Y;

                return res;
            }

            //public override bool Equals(object obj)
            //{
            //    if (!(obj is Room))
            //        return false;

            //    var casted = (Room)obj;

            //    return casted.Pos.Equals(Pos);
            //}
        }

        private class PosInfo
        {
            public Room CurrentRoom { get; set; }
            public LinkedListNode<char> CurrentPath { get; set; }
            public int CurrentDist { get; set; }
        }

        private struct Pos
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
