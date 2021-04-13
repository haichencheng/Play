using System;
using System.IO;

namespace RobotRecovery
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("Data/robotrecovery.txt");

                //Read the first line of text
                line = sr.ReadLine();
                string[] fields = line.Split(' ');

                int yCount = int.Parse(fields[0]);
                int xCount = int.Parse(fields[1]);
                int robotCount = int.Parse(fields[2]);

                Maze maze = new Maze(yCount, xCount, robotCount);

                for (int y = 0; y < yCount; y++)
                {
                    line = sr.ReadLine();
                    var roomStates = line.ToCharArray();
                    for (int x = 0; x< xCount; x++)
                    {
                        if ('.' == roomStates[x])
                        {
                            maze.RoomTypeMap[y, x] = RoomType.Empty;
                        }
                        else if ('X' == roomStates[x])
                        {
                            maze.RoomTypeMap[y, x] = RoomType.Wall;
                        }
                        else if ('E' == roomStates[x])
                        {
                            maze.RoomTypeMap[y, x] = RoomType.Entrance;
                        }
                        else if ('R' == roomStates[x])
                        {
                            maze.RoomTypeMap[y, x] = RoomType.Robot;
                        }
                    }
                }
                //close the file
                sr.Close();
                var pathFinder = new PathFinder(maze);
                pathFinder.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
    }
}
