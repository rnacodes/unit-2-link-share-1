using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmerPuzzle
{
    enum Side { Left, Right }

    // Represents the state of the puzzle at any given point
    record State(Side Farmer, Side Fox, Side Goose, Side Grain)
    {
        public bool IsValid()
        {
            // Fox cannot be left with Goose without the Farmer
            if (Fox == Goose && Farmer != Fox) return false;
            // Goose cannot be left with Grain without the Farmer
            if (Goose == Grain && Farmer != Goose) return false;
            return true;
        }

        public bool IsGoal() => 
            Farmer == Side.Right && Fox == Side.Right && Goose == Side.Right && Grain == Side.Right;
            
        public Side Opposite(Side side) => side == Side.Left ? Side.Right : Side.Left;
    }

    class Program
    {
        static void Main()
        {
            var initialState = new State(Side.Left, Side.Left, Side.Left, Side.Left);
            var queue = new Queue<(State State, List<string> Path)>();
            var visited = new HashSet<State>();

            queue.Enqueue((initialState, new List<string>()));
            visited.Add(initialState);

            while (queue.Count > 0)
            {
                var (current, path) = queue.Dequeue();

                if (current.IsGoal())
                {
                    Console.WriteLine("Solution found in " + path.Count + " moves:");
                    for (int i = 0; i < path.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {path[i]}");
                    }
                    return;
                }

                foreach (var move in GetPossibleMoves(current))
                {
                    if (!visited.Contains(move.NextState))
                    {
                        visited.Add(move.NextState);
                        var newPath = new List<string>(path) { move.Description };
                        queue.Enqueue((move.NextState, newPath));
                    }
                }
            }

            Console.WriteLine("No solution found.");
        }

        static IEnumerable<(State NextState, string Description)> GetPossibleMoves(State s)
        {
            Side nextSide = s.Opposite(s.Farmer);

            // Option 1: Farmer crosses alone
            var alone = s with { Farmer = nextSide };
            if (alone.IsValid())
                yield return (alone, $"Farmer crosses alone to the {nextSide}");

            // Option 2: Farmer takes Fox
            if (s.Fox == s.Farmer)
            {
                var next = s with { Farmer = nextSide, Fox = nextSide };
                if (next.IsValid()) yield return (next, $"Farmer takes the Fox to the {nextSide}");
            }

            // Option 3: Farmer takes Goose
            if (s.Goose == s.Farmer)
            {
                var next = s with { Farmer = nextSide, Goose = nextSide };
                if (next.IsValid()) yield return (next, $"Farmer takes the Goose to the {nextSide}");
            }

            // Option 4: Farmer takes Grain
            if (s.Grain == s.Farmer)
            {
                var next = s with { Farmer = nextSide, Grain = nextSide };
                if (next.IsValid()) yield return (next, $"Farmer takes the Grain to the {nextSide}");
            }
        }
    }
}