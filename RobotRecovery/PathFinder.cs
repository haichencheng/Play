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
        BotMazeState Start {get; set;}
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

            Start = new BotMazeState(ShortPathMap.GetRobotInRooms(), "", Move.None, 0, ShortPathMap);
            KeyToStateMap[Start.Key] = Start;

            int maxPathlength = Start.MaxBotDistance;
            while (maxPathlength > 0)
            {
                FollowBots(Start, maxPathlength, 0);
                maxPathlength = maxPathlength*2/3;
            }

            Console.WriteLine($"\nFollowBots Finished Cap {Cap}");

            Console.WriteLine($"\n\nBestPath {BestPath}");
            return BestPath;
        }


        private void FollowBots(BotMazeState p_state, int p_leg, int p_fromStart)
        {
            ExploredNodeCount++;
                  
            if (CheckComplete(p_state, p_fromStart))
            {
                return;
            }
            for (int i=0; i< p_state.BotInRooms.Count; i++)
            {
                var botInRoom = p_state.BotInRooms[i];
                int maxMoveCount = botInRoom.DistanceToEntrance < p_leg ? botInRoom.DistanceToEntrance : p_leg;
                BotMazeState afterState = FollowBotShortPath(botInRoom, maxMoveCount, p_state, p_fromStart);
                if (!CheckComplete(afterState, p_fromStart))
                {
                    //if (ExploredNodeCount % 10000 == 0)
                    {
                        Console.WriteLine($"\n{DateTime.Now} {ExploredNodeCount} leg {p_leg} fromStart {p_fromStart} Cap {Cap} from [{p_state}] to [{afterState}]");
                    }

                    // Depth first recursion
                    FollowBots(afterState, p_leg, p_fromStart + maxMoveCount);
                }
            }
        }


        BotMazeState BackwardUpdate(BotMazeState p_state)
        {
            var state = p_state;
            while (state.FromLength > 0)
            {
                var fromState = KeyToStateMap[state.FromStateKey];
                if (fromState.BestToLength <= state.BestToLength + 1)
                {
                    break;
                }

                fromState.BestToLength = state.BestToLength + 1;
                fromState.BestToMove = state.FromMove;
                fromState.Reached = true;

                state = fromState;
            }

            return state;
        }

        bool CheckComplete(BotMazeState p_state, int p_fromStart)
        {
            if (!p_state.IsComplete(Cap, p_fromStart))
            {
                return false;
            }
            
            if (p_state.Reached)
            {
                var lastUpdated = BackwardUpdate(p_state);
                if (lastUpdated.Key != Start.Key)
                {
                    return true;
                }

                if (null != BestPath
                    && p_state.BestToLength + p_state.FromLength >= BestPath.Length)
                {
                    return true;
                }

                BestPath = new Path();
                var state = lastUpdated;
                while (state.BotInRooms.Count > 1)
                {
                    BestPath.Moves.Add(state.BestToMove);
                    var toState = state.MoveBy(state.BestToMove, ShortPathMap);
                    state = KeyToStateMap[toState.Key];
                }

                var lastBotInRoom = state.BotInRooms[0];
                var pathToEntrance = ShortPathMap.GetPathToEntrance(lastBotInRoom);
                BestPath.Moves.AddRange(pathToEntrance.Moves);

                Console.WriteLine($"\n{DateTime.Now} Step {ExploredNodeCount}, BestPath Len {BestPath.Length}");
                Console.WriteLine($"{BestPath}\n");

                using (StreamWriter sw = new StreamWriter("ShortPaths.txt", true))
                {
                    sw.WriteLine($"{DateTime.Now} Step {ExploredNodeCount}, {BestPath.Length}");
                    sw.WriteLine($"{BestPath}");
                    sw.WriteLine();
                }
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


        private BotMazeState FollowBotShortPath(Room p_botInRoom, int p_maxMoveCount, BotMazeState p_state, int p_fromStart)
        {
            Debug.Assert(p_state.BotInRooms.Count > 1);
            var botPath = ShortPathMap.GetPathToEntrance(p_botInRoom);

            BotMazeState state = p_state;
            for (int i = 0; i < p_maxMoveCount; i++) 
            {
                var tryMove = botPath.Moves[i];
                var stateAfterMove = state.MoveBy(tryMove, ShortPathMap);
                stateAfterMove = RecordState(stateAfterMove);

                if (stateAfterMove.IsComplete(Cap, p_fromStart))
                {
                    return stateAfterMove;
                }
                state = stateAfterMove;
            }

            return state;
        }

        int ExploredNodeCount { get; set; }
    }

}
