﻿using System;
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
        public Move FromMove { get; set; }
        
        public int FromLength {get; set;}

        public List<Room> BotInRooms { get; set; }
        
        public int BestToLength {get; set;}

        public Move BestToMove {get; set;}

        public bool Reached {get; set;}
        
        public String Key { get; set;}
        
        public int MaxBotDistance { get; set; }

        public BotMazeState(
            List<Room> p_botInRooms, 
            Move p_fromMove,
            int p_fromLength, 
            MazeShortestPathMap p_shortPathMap)
        {
            FromMove = p_fromMove;
            FromLength = p_fromLength;
            BotInRooms = p_botInRooms;
            BotInRooms.Sort(Room.CompareByYX);
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
                BestToLength = p_shortPathMap.DistanceMap[room.Y, room.X].Distance;
                BestToMove = MazeShortestPathMap.Opposite(p_shortPathMap.DistanceMap[room.Y, room.X].LastMove);
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

            return new BotMazeState(nextBotInRooms, p_move, this.FromLength+1, p_shortPathMap);
        }


        public bool IsComplete(int p_moveCap)
        {
            if (BotInRooms.Count <= 1)
            {
                return true;
            }

            if (p_moveCap <= FromLength + MaxBotDistance)
            {
                return true;
            }

            return false;
        }

    }


}
