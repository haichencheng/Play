using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace RobotRecovery
{

    class PathFinder
    {
        MazeShortestPathMap ShortPathMap { get; set; }
        BotMazeState Start {get; set;}
        Dictionary<string, BotMazeState> KeyToStateMap { get; set; } = new Dictionary<string, BotMazeState>();
        Dictionary<int, HashSet<string>> FromLengthToStateKeySetMap { get; set; } = new Dictionary<int, HashSet<string>>();

        int FromCursor { get; set; } = 0;

        Path BestPath { get; set; }

        int ExploreCount { get; set; } = 0;

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
            ShortPathMap = new MazeShortestPathMap(p_maze);
        }

        public Path Run()
        {
            ShortPathMap.Solve();
            Start = new BotMazeState(ShortPathMap.GetRobotInRooms(), "", Move.None, 0, ShortPathMap);
            RecordInFromLengthToStateSetMap(Start);
            RecordInKeyToStateMap(Start);

            BotMazeState state = GetNextState();
            while (null != state)
            {
                ExploreState(state);

                state = GetNextState();
            }

            Console.WriteLine($"\nFollowBots Finished Cap {Cap}");
            Console.WriteLine($"\nBestPath {BestPath}");
            return BestPath;
        }

        void ExploreState(BotMazeState p_state)
        {
            ExploreCount++;
            if (0 == ExploreCount % 10000)
            {
                Console.WriteLine($"\n{DateTime.Now} {ExploreCount} Cap {Cap} states={KeyToStateMap.Count} [{p_state}]");
            }
            if (p_state.IsComplete(Cap))
            {
                ProcessReached(p_state);
                return;
            }

            var state = p_state;
            while (true)
            {
                var botInRoom = state.GetNextBotInRoom();
                if (null == botInRoom)
                {
                    return;
                }
                BotMazeState afterState = FollowBotShortPath(botInRoom, state);
                if (null == afterState)
                {
                    continue;
                }
                if (afterState.IsComplete(Cap))
                {
                    return;
                }
                state = afterState;
            }
        }

        BotMazeState GetNextState()
        {
            while (true)
            {
                if (0 == FromLengthToStateKeySetMap.Count)
                {
                    return null;
                }

                while (FromLengthToStateKeySetMap.Count > 0 && !FromLengthToStateKeySetMap.ContainsKey(FromCursor))
                {
                    FromCursor++;
                }

                var fromSet = FromLengthToStateKeySetMap[FromCursor];
                foreach (var key in fromSet)
                {
                    var state = KeyToStateMap[key];
                    if (!state.IsComplete(Cap))
                    {
                        return state;
                    }
                    ProcessReached(state);
                    fromSet.Remove(key);
                    break;
                }
                if (0 == fromSet.Count)
                {
                    FromLengthToStateKeySetMap.Remove(FromCursor);
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
                //Console.WriteLine($"\nBack {backCount} from [{p_state}] to [{state}]");
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

            Console.WriteLine($"\n{DateTime.Now} ExploreCount={ExploreCount}, BestPath.Length={BestPath.Length}");
            Console.WriteLine($"{BestPath}\n");

            using (StreamWriter sw = new StreamWriter("ShortPaths.txt", true))
            {
                sw.WriteLine($"{DateTime.Now} ExploreCount={ExploreCount}, BestPath.Length={BestPath.Length}");
                sw.WriteLine($"{BestPath}");
                sw.WriteLine();
            }
        }


        void RecordInFromLengthToStateSetMap(BotMazeState p_state)
        {
            if (!FromLengthToStateKeySetMap.ContainsKey(p_state.FromLength))
            {
                FromLengthToStateKeySetMap[p_state.FromLength] = new HashSet<string>();
            }
            FromLengthToStateKeySetMap[p_state.FromLength].Add(p_state.Key);
        }


        BotMazeState RecordInKeyToStateMap(BotMazeState p_state)
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


        private BotMazeState FollowBotShortPath(Room p_botInRoom, BotMazeState p_state)
        {
            Debug.Assert(p_state.BotInRooms.Count > 1);
            var botPath = ShortPathMap.GetPathToEntrance(p_botInRoom);
            p_state.UsedBotRooms.Add(p_botInRoom);

            BotMazeState state = p_state;
            foreach(var tryMove in botPath.Moves)
            {
                if (state.UsedMoves.Contains(tryMove) || tryMove == MazeShortestPathMap.Opposite(state.FromMove))
                {
                    return null;
                }

                var stateAfterMove = state.MoveBy(tryMove, ShortPathMap);
                state.UsedMoves.Add(tryMove);
                stateAfterMove = RecordInKeyToStateMap(stateAfterMove);
                RecordInFromLengthToStateSetMap(stateAfterMove);

                if (stateAfterMove.IsComplete(Cap))
                {
                    ProcessReached(stateAfterMove);
                    return stateAfterMove;
                }

                if (stateAfterMove.FromLength < state.FromLength+1)
                {
                    return null;
                }

                state = stateAfterMove;
            }

            return state;
        }
    }

}
