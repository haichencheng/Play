using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace RobotRecovery
{
    class Path
    {
        public Path()
        {
            Moves = new List<Move>();
        }
        public List<Move> Moves { get; set; }

        public int Length {  get { return Moves.Count; } }

        public override string ToString()
        {
            string str = $"{Length}:";
            foreach (var move in Moves)
            {
                switch (move)
                {
                    case Move.Up:
                        str += "U";
                        break;
                    case Move.Down:
                        str += "D";
                        break;
                    case Move.Left:
                        str += "L";
                        break;
                    case Move.Right:
                        str += "R";
                        break;
                }
            }

            return str;
        }
    }


    class BotMazeState
    {
        public string FromStateKey { get; set; }

        public Move FromMove { get; set; }
        
        public int FromLength {get; set;}

        public List<Room> BotInRooms { get; set; }
        
        public int BestToLength {get; set;}

        public Move BestToMove {get; set;}

        public bool Reached {get; set;}
        
        public string Key { get; set;}
        
        public int MaxBotDistance { get; set; }
        
        public HashSet<Move> UsedMoves {get;set;} = new HashSet<Move>();
        public HashSet<Room> UsedBotRooms {get;set;} = new HashSet<Room>();

        public override string ToString()
        {
            return $"{Key} From:{FromLength}, MaxBotDistance={MaxBotDistance}, BestToLength={BestToLength}";
        }

        public BotMazeState(
            List<Room> p_botInRooms,
            string p_fromStateKey,
            Move p_fromMove,
            int p_fromLength, 
            MazeShortestPathMap p_shortPathMap)
        {
            FromStateKey = p_fromStateKey;
            FromMove = p_fromMove;
            FromLength = p_fromLength;
            BestToLength = Maze.Max_Distance;
            BestToMove = Move.None;
            Reached = false;

            BotInRooms = p_botInRooms;
            BotInRooms.Sort(Room.CompareByDistanceDesc);
            foreach (var room in BotInRooms)
            {
                Key += $"{room.GetHashCode():D6}";
            }

            MaxBotDistance = 0;
            foreach (var room in BotInRooms)
            {
                room.DistanceToEntrance = p_shortPathMap.DistanceMap[room.Y, room.X].Distance;
                if (MaxBotDistance < room.DistanceToEntrance)
                {
                    MaxBotDistance = room.DistanceToEntrance;
                }
            }
            
            if (BotInRooms.Count == 1)
            {
                Reached = true;
                Room room = BotInRooms[0];
                var shortPath = p_shortPathMap.DistanceMap[room.Y, room.X];
                BestToLength = shortPath.Distance;
                BestToMove = MazeShortestPathMap.Opposite(shortPath.LastMove);
            }
        }

        public BotMazeState MoveBy(Move p_move, MazeShortestPathMap p_shortPathMap)
        {
            HashSet<int> roomHashs = new HashSet<int>();
            List<Room> nextBotInRooms = new List<Room>();
            foreach(var room in BotInRooms)
            {
                var nextRoom = room.MoveBy(p_move);
                if (!p_shortPathMap.InputMaze.IsValidRoom(nextRoom))
                {
                    nextRoom = room;
                }

                if (!roomHashs.Contains(nextRoom.GetHashCode())
                    && !p_shortPathMap.InputMaze.IsEntrance(nextRoom))
                {
                    roomHashs.Add(nextRoom.GetHashCode());
                    nextBotInRooms.Add(nextRoom);
                }
            }

            return new BotMazeState(nextBotInRooms, Key, p_move, this.FromLength+1, p_shortPathMap);
        }

        public Room GetNextBotInRoom()
        {
            foreach (var room in BotInRooms)
            {
                if (!UsedBotRooms.Contains(room))
                {
                    return room;
                }
            }

            return null;
        }

        public bool IsComplete(int p_moveCap)
        {
            if (BotInRooms.Count <= 1)
            {
                return true;
            }

            if (FromLength + MaxBotDistance >= p_moveCap)
            {
                return true;
            }

            if (UsedMoves.Count >= 4)
            {
                return true;
            }
            if (!UsedMoves.Contains(FromMove) && UsedMoves.Count>=3)
            {
                return true;
            }
            if (UsedBotRooms.Count == BotInRooms.Count)
            {
                return true;
            }

            return false;
        }

    }


}
