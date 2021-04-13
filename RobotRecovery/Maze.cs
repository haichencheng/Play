using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace RobotRecovery
{
    enum RoomType
    {
        Empty = 0,
        Wall = 1,
        Entrance = 2,
        Robot = 3,
    };

    class ShortPath
    {
        public int Distance { get; set; }

        public Move LastMove { get; set; }
    }

    class Maze
    {
        public Maze(int p_yCount, int p_xCount, int p_robotCount)
        {
            YCount = p_yCount;
            XCount = p_xCount;
            RobotCount = p_robotCount;

            RoomTypeMap = new RoomType[YCount, XCount];
        }


        public bool IsValidRoom(Room p_room)
        {

            if (p_room.Y < 0 || p_room.Y >= YCount
                || p_room.X < 0 || p_room.X >= XCount)
            {
                return false;
            }

            return RoomType.Wall != RoomTypeMap[p_room.Y, p_room.X];
        }

        public bool IsEntrance(Room p_room)
        {

            if (p_room.Y < 0 || p_room.Y >= YCount
                || p_room.X < 0 || p_room.X >= XCount)
            {
                return false;
            }

            return RoomType.Entrance == RoomTypeMap[p_room.Y, p_room.X];
        }


        public const int Infinite_Distance = 9999999;
        public const int Max_Distance = 1000000;
        
        public int YCount { get; set; }
        public int XCount { get; set; }

        public int RobotCount { get; set; }


        public RoomType[,] RoomTypeMap { get; set; }

    };


    class MazeShortestPathMap
    {
        public MazeShortestPathMap(Maze p_maze)
        {
            InputMaze = p_maze;
        }

        public static Move Opposite(Move p_move)
        {
            Move result = Move.Up;
            switch (p_move)
            {
                case Move.Up:
                    result = Move.Down;
                    break;
                case Move.Down:
                    result = Move.Up;
                    break;
                case Move.Left:
                    result = Move.Right;
                    break;
                case Move.Right:
                    result = Move.Left;
                    break;
            }
            return result;
        }


        public void Solve()
        {
            DistanceMap = new ShortPath[InputMaze.YCount, InputMaze.XCount];
            for (int y = 0; y < InputMaze.YCount; y++)
            {
                for (int x = 0; x < InputMaze.XCount; x++)
                {
                    switch (InputMaze.RoomTypeMap[y, x])
                    {
                        case RoomType.Entrance:
                            DistanceMap[y, x] = new ShortPath() { Distance = 0, LastMove = Move.None };
                            Entrance = new Room(y, x);
                            break;
                        case RoomType.Wall:
                            DistanceMap[y, x] = new ShortPath() { Distance = 9999, LastMove = Move.None };
                            break;
                        default:
                            DistanceMap[y, x] = new ShortPath() { Distance = 9999, LastMove = Move.None };
                            break;
                    }
                }
            }


            Queue<Room> bfsQueue = new Queue<Room>();
            bfsQueue.Enqueue(Entrance);

            Room room;
            while (bfsQueue.TryDequeue(out room))
            {
                var roomShortPath = DistanceMap[room.Y, room.X];

                for (var tryMove = Move.Up; tryMove < Move.None; tryMove++)
                {
                    var nextRoom = room.MoveBy(tryMove);

                    if (!InputMaze.IsValidRoom(nextRoom))
                    {
                        continue;
                    }

                    if (DistanceMap[nextRoom.Y, nextRoom.X].Distance <= roomShortPath.Distance + 1)
                    {
                        // have shorter path
                        continue;
                    }

                    // to explore 
                    DistanceMap[nextRoom.Y, nextRoom.X].Distance = roomShortPath.Distance + 1;
                    DistanceMap[nextRoom.Y, nextRoom.X].LastMove = tryMove;
                    bfsQueue.Enqueue(nextRoom);
                }

                // PrintDistanceMap();
            }
        }

        public void PrintDistanceMap()
        {
            Console.WriteLine("\nDistanceMap:");
            for (int y =0; y< InputMaze.YCount; y++)
            {
                Console.Write($"{y:D2}=>");
                for (int x=0; x<InputMaze.XCount;x++)
                {
                    Console.Write($"{x}={DistanceMap[y, x].Distance:D4}.");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public Path GetPathToEntrance(Room p_room)
        {
            //PrintDistanceMap();

            Path shortest = new Path();
            var room = new Room(p_room.Y, p_room.X);
            while (DistanceMap[room.Y, room.X].Distance != 0)
            {
                shortest.Moves.Add(Opposite(DistanceMap[room.Y, room.X].LastMove));
                switch (DistanceMap[room.Y, room.X].LastMove)
                {
                    case Move.Up:
                        room.Y++;
                        break;
                    case Move.Down:
                        room.Y--;
                        break;
                    case Move.Left:
                        room.X++;
                        break;
                    case Move.Right:
                        room.X--;
                        break;
                }
            }

            return shortest;
        }

        public List<Room> GetRobotInRooms()
        {
            List<Room> rooms = new List<Room>();
            for (int y = 0; y < InputMaze.YCount; y++)
            {
                for (int x = 0; x < InputMaze.XCount; x++)
                {
                    if (InputMaze.RoomTypeMap[y,x] == RoomType.Robot)
                    {
                        rooms.Add(new Room(y, x));
                    }
                }
            }
            return rooms;
        }

        public Maze InputMaze { get; set; }

        Room Entrance { get; set; }

        public ShortPath[,] DistanceMap { get; set; }
    }
}
