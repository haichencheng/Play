using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace RobotRecovery
{

    class PathFinder
    {
        Maze InputMaze { get; set; }
        MazeShortestPathMap ShortPathMap { get; set; }

        Dictionary<string, BotMazeState> KeyToStateMap;

        Path BestPath { get; set; }

        int Cap
        {
            get
            {
                if (null == BestPath)
                {
                    return Maze.Max_Distance;
                }
                return BestPath.Length;
            }
        }

        public PathFinder(Maze p_maze)
        {
            InputMaze = p_maze;
            ShortPathMap = new MazeShortestPathMap(p_maze);
            KeyToStateMap = new Dictionary<string, BotMazeState>();
        }

        public Path Run()
        {
            ShortPathMap = new MazeShortestPathMap(InputMaze);
            ShortPathMap.Solve();

            BotMazeState start = GetStartState();
            int maxPathlength = start.MaxBotDistance;

            while (maxPathlength > 0)
            {
                FollowBots(start, maxPathlength, 0);
                maxPathlength = maxPathlength*2/3;
            }

            Console.WriteLine($"\nFollowBots Finished Cap {Cap}");

            Console.WriteLine($"\n\nBestPath {BestPath}");
            return BestPath;
        }


        private void FollowBots(BotMazeState p_state, int p_leg, int p_depth)
        {
            Nodes++;
                  
            if (CheckComplete(p_state))
            {
                BackwardUpdate(p_state);
                return;
            }
            for (int i=0; i< p_state.BotInRooms.Count; i++)
            {
                var botInRoom = p_state.BotInRooms[i];
                int maxMoveCount = botInRoom.DistanceToEntrance < p_leg ? botInRoom.DistanceToEntrance : p_leg;
                BotMazeState afterState = FollowBotShortPath(botInRoom, maxMoveCount, p_state);
                if (!CheckComplete(afterState))
                {
                    if (Nodes % 10000 == 0)
                    {
                        Console.WriteLine($"\n{DateTime.Now} leg {p_leg} Cap {Cap} from {p_state} to {afterState}");
                    }

                    // Depth first recursion
                    FollowBots(afterState, p_leg, p_depth + 1);
                }
            }
        }


        void BackwardUpdate(BotMazeState p_state)
        {
        }

        bool CheckComplete(BotMazeState p_state)
        {
            if (!p_state.IsComplete(Cap))
            {
                return false;
            }
            
            if (p_state.Reached)
            {
            /*print best path
                BestPath = new Path();
                BestPath.Moves.AddRange(p_state.PathFromStart.Moves);
                BestPath.Moves.AddRange(p_state.PathToEntrance.Moves);

                Console.WriteLine($"\n{DateTime.Now} Step {Nodes}, {BestPath.Length}");
                Console.WriteLine($"{BestPath}\n");

                using (StreamWriter sw = new StreamWriter("ShortPaths.txt", true))
                {
                    sw.WriteLine($"{DateTime.Now} Step {Nodes}, {BestPath.Length}");
                    sw.WriteLine($"{BestPath}");
                    sw.WriteLine();
                }

                */
            }
            return true;
        }

        BotMazeState RecordState(BotMazeState p_state)
        {
            if (KeyToStateMap.ContainsKey(p_state.Key))
            {
                var savedState = KeyToStateMap[p_state.Key];
                if (savedState.FromLength < p_state.FromLength)
                {
                    return savedState;
                }
            }

            KeyToStateMap[p_state.Key] = p_state;
            return p_state;
        }


        private BotMazeState FollowBotShortPath(Room p_botInRoom, int p_maxMoveCount, BotMazeState p_state)
        {
            Debug.Assert(p_state.BotInRooms.Count > 1);
            var botPath = ShortPathMap.GetPathToEntrance(p_botInRoom);

            BotMazeState state = p_state;
            for (int i = 0; i < p_maxMoveCount; i++) 
            {
                var tryMove = botPath.Moves[i];
                var stateAfterMove = state.MoveBy(tryMove, ShortPathMap);
                stateAfterMove = RecordState(stateAfterMove);

                if (stateAfterMove.IsComplete(Cap))
                {
                    return stateAfterMove;
                }
                state = stateAfterMove;
            }

            return state;
        }

        BotMazeState GetStartState()
        {
            return new BotMazeState(ShortPathMap.GetRobotInRooms(), Move.None, 0, ShortPathMap);
        }

        int Nodes { get; set; }
    }

}
