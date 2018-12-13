using AdventOfCode2018.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2018.Solutions
{
    class Day13
    {
        [Solution(13, 1)]
        public string Problem1(string input)
        {
            var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var grid = new Track[lines[0].Length, lines.Length];
            var cartList = new List<Cart>();

            for (var y = 0; y < lines.Length; y++)
            {
                for(var x = 0; x < lines[y].Length; x++)
                {
                    switch (lines[y][x])
                    {
                        case '|':
                            grid[x, y] = Track.Vert;
                            break;
                        case '-':
                            grid[x, y] = Track.Hori;
                            break;
                        case '/':
                            grid[x, y] = Track.ForwardTurn;
                            break;
                        case '\\':
                            grid[x, y] = Track.BackTurn;
                            break;
                        case '+':
                            grid[x, y] = Track.Intersection;
                            break;
                        case '^':
                            grid[x, y] = Track.Hori;
                            cartList.Add(new Cart(x, y, Direction.North));
                            break;
                        case '>':
                            grid[x, y] = Track.Vert;
                            cartList.Add(new Cart(x, y, Direction.East));
                            break;
                        case 'v':
                            grid[x, y] = Track.Hori;
                            cartList.Add(new Cart(x, y, Direction.South));
                            break;
                        case '<':
                            grid[x, y] = Track.Vert;
                            cartList.Add(new Cart(x, y, Direction.West));
                            break;
                        default:
                            grid[x, y] = Track.None;
                            break;
                    }
                }
            }

            var carts = cartList.ToArray();
            while (true)
            {
                foreach(var item in carts)
                {
                    item.UpdatePos();
                    item.UpdateDir(grid[item.X, item.Y]);
                    if (item.IsColliding(carts))
                        return $"{item.X},{item.Y}";
                }
            }
        }

        [Solution(13, 2)]
        public string Problem2(string input)
        {
            var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var grid = new Track[lines[0].Length, lines.Length];
            var cartList = new List<Cart>();

            for (var y = 0; y < lines.Length; y++)
            {
                for (var x = 0; x < lines[y].Length; x++)
                {
                    switch (lines[y][x])
                    {
                        case '|':
                            grid[x, y] = Track.Vert;
                            break;
                        case '-':
                            grid[x, y] = Track.Hori;
                            break;
                        case '/':
                            grid[x, y] = Track.ForwardTurn;
                            break;
                        case '\\':
                            grid[x, y] = Track.BackTurn;
                            break;
                        case '+':
                            grid[x, y] = Track.Intersection;
                            break;
                        case '^':
                            grid[x, y] = Track.Hori;
                            cartList.Add(new Cart(x, y, Direction.North));
                            break;
                        case '>':
                            grid[x, y] = Track.Vert;
                            cartList.Add(new Cart(x, y, Direction.East));
                            break;
                        case 'v':
                            grid[x, y] = Track.Hori;
                            cartList.Add(new Cart(x, y, Direction.South));
                            break;
                        case '<':
                            grid[x, y] = Track.Vert;
                            cartList.Add(new Cart(x, y, Direction.West));
                            break;
                        default:
                            grid[x, y] = Track.None;
                            break;
                    }
                }
            }

            while (true)
            {
                var toRemove = new List<Cart>();
                cartList = cartList.OrderBy(it => it.Y).ThenBy(it => it.X).ToList();

                for(var i = 0; i < cartList.Count; i++)
                {
                    var cart = cartList[i];
                    if (toRemove.Contains(cart))
                        continue;
                    cart.UpdatePos();
                    cart.UpdateDir(grid[cart.X, cart.Y]);
                    var colliding = cartList.FirstOrDefault(it => it != cart && it.X == cart.X && it.Y == cart.Y);
                    if(colliding != null)
                    {
                        toRemove.Add(cart);
                        toRemove.Add(colliding);
                    }
                }

                foreach (var item in toRemove)
                    cartList.Remove(item);

                if(cartList.Count <= 1)
                {

                    var last = cartList[0];

                    return $"{last.X},{last.Y}";
                }
                    
            }
        }

        private enum Track
        {
            None,
            Vert,
            Hori,
            BackTurn,
            ForwardTurn,
            Intersection
        }

        private enum Direction
        {
            North,
            East,
            South,
            West
        }

        private enum NextTurn
        {
            Left,
            Straight,
            Right
        }

        private class Cart
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Direction Direction { get; set; }

            private NextTurn nextTurn;

            public Cart(int x, int y, Direction direction)
            {
                this.X = x;
                this.Y = y;
                this.Direction = direction;
                this.nextTurn = NextTurn.Left;
            }

            public void UpdatePos()
            {
                switch (Direction)
                {
                    case Direction.North:
                        this.Y--;
                        break;
                    case Direction.East:
                        this.X++;
                        break;
                    case Direction.South:
                        this.Y++;
                        break;
                    case Direction.West:
                        this.X--;
                        break;
                }
            }

            public void UpdateDir(Track trackType)
            {
                switch (trackType)
                {
                    case Track.BackTurn:
                        if (Direction == Direction.North)
                            Direction = Direction.West;
                        else if (Direction == Direction.East)
                            Direction = Direction.South;
                        else if (Direction == Direction.South)
                            Direction = Direction.East;
                        else if (Direction == Direction.West)
                            Direction = Direction.North;
                        break;
                    case Track.ForwardTurn:
                        if (Direction == Direction.North)
                            Direction = Direction.East;
                        else if (Direction == Direction.East)
                            Direction = Direction.North;
                        else if (Direction == Direction.South)
                            Direction = Direction.West;
                        else if (Direction == Direction.West)
                            Direction = Direction.South;
                        break;
                    case Track.Intersection:
                        if (nextTurn == NextTurn.Left)
                            Direction = (Direction)((((int)Direction) + 3) % 4);
                        else if (nextTurn == NextTurn.Right)
                            Direction = (Direction)((((int)Direction) + 1) % 4);

                        nextTurn = (NextTurn)((((int)nextTurn) + 1) % 3);
                        break;
                }
            }

            public bool IsColliding(Cart[] carts)
            {
                return carts.Any(it => it != this && it.X == X && it.Y == Y);
            }
        }
    }
}
