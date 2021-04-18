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
                  
            if (p_state.IsComplete(Cap, p_fromStart))
            {
                ProcessReached(p_state);
                return;
            }
            for (int i=0; i< p_state.BotInRooms.Count; i++)
            {
                var botInRoom = p_state.BotInRooms[i];
                int maxMoveCount = botInRoom.DistanceToEntrance < p_leg ? botInRoom.DistanceToEntrance : p_leg;
                BotMazeState afterState = FollowBotShortPath(botInRoom, maxMoveCount, p_state, p_fromStart);
                if (!afterState.IsComplete(Cap, p_fromStart))
                {
                    if (ExploredNodeCount % 1000000 == 0)
                    {
                        Console.WriteLine($"\n{DateTime.Now} {ExploredNodeCount/1000000} M states={KeyToStateMap.Count} leg {p_leg} fromStart {p_fromStart} Cap {Cap} from [{p_state}] to [{afterState}]");
                    }

                    // Depth first recursion
                    FollowBots(afterState, p_leg, p_fromStart + maxMoveCount);
                }
            }
        }


        void ProcessReached(BotMazeState p_state)
        {
            if (!p_state.Reached)
            {
                return;
            }

            var state = p_state;
            int backCount = 0;
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
                backCount++;
            }
            
            if (backCount > 0)
            {
                Console.WriteLine($"\nBack {backCount} from [{p_state}] to [{state}]");
            }
            if (state.Key != Start.Key)
            {
                return;
            }

            if (null != BestPath
                && p_state.BestToLength + p_state.FromLength >= BestPath.Length)
            {
                return;
            }

            BestPath = new Path();
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


        BotMazeState RecordState(BotMazeState p_state)
        {
            if (KeyToStateMap.ContainsKey(p_state.Key))
            {
                var savedState = KeyToStateMap[p_state.Key];
                if (savedState.FromLength > p_state.FromLength)
                {
                   // Console.WriteLine($"Shorten {p_state.Key} FromLength {savedState.FromLength} to {p_state.FromLength}");

                    savedState.FromLength = p_state.FromLength;
                    savedState.FromMove = p_state.FromMove;
                    savedState.FromStateKey = p_state.FromStateKey;

                    ProcessReached(savedState);
                }
                return savedState;
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
                    ProcessReached(stateAfterMove);
                    return stateAfterMove;
                }
                state = stateAfterMove;
            }

            return state;
        }

        int ExploredNodeCount { get; set; }
    }

}
