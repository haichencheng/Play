using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace RobotRecovery
{
    enum Move
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
    
        None = 4,
    };

    class Room
    {
        public Room(int p_y, int p_x)
        {

            this.Y = p_y;
            this.X = p_x;
        }
        public int X { get; set; }
        public int Y { get; set; }

        public Room MoveBy(Move p_move)
        {
            switch (p_move)
            {
                case Move.Up:
                    return new Room(Y - 1, X);
                case Move.Down:
                    return new Room(Y + 1, X);
                case Move.Left:
                    return new Room(Y, X - 1);
                case Move.Right:
                    return new Room(Y, X + 1);
                default:
                    return this;
            }
        }

        public int DistanceToEntrance { get; set;}
        public static Room FromHash(int p_hash)
        {
           return new Room(p_hash/ MaxWidth, p_hash % MaxWidth);
        }

        public override string ToString()
        {
            return $"R({GetHashCode():D6}:{DistanceToEntrance})";
        }

        const int MaxWidth = 1000;
        public override int GetHashCode()
        {
            return MaxWidth * Y + X;
        }

        public static int CompareByYX(Room p_left, Room p_right)
        {   
            if (p_left.Y < p_right.Y)
            {
                return -1;
            }
            if (p_left.Y > p_right.Y)
            {
                return 0;
            }
            if (p_left.X < p_right.X)
            {
                return -1;
            }
            if (p_left.X > p_right.X)
            {
                return 1;
            }
            return 0;
        }

        public static int CompareByDistanceDesc(Room p_left, Room p_right)
        {
            if (p_left.DistanceToEntrance > p_right.DistanceToEntrance)
            {
                return -1;
            }
            else if (p_left.DistanceToEntrance < p_right.DistanceToEntrance)
            {
                return 1;
            }

            return 0;
        }

        public static int CompareByDistance(Room p_left, Room p_right)
        {
            if (p_left.DistanceToEntrance < p_right.DistanceToEntrance)
            {
                return -1;
            }
            else if (p_left.DistanceToEntrance > p_right.DistanceToEntrance)
            {
                return 1;
            }

            return 0;
        }
    }

}
