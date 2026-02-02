//GPT 4.1
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    enum Side { Left, Right }

    class State
    {
        public Side Farmer, Fox, Goose, Grain;
        public State Parent;
        public string Move;

        public State(Side farmer, Side fox, Side goose, Side grain, State parent = null, string move = null)
        {
            Farmer = farmer; Fox = fox; Goose = goose; Grain = grain;
            Parent = parent; Move = move;
        }

        public bool IsGoal() =>
            Farmer == Side.Right && Fox == Side.Right && Goose == Side.Right && Grain == Side.Right;

        public override bool Equals(object obj)
        {
            if (obj is State s)
                return Farmer == s.Farmer && Fox == s.Fox && Goose == s.Goose && Grain == s.Grain;
            return false;
        }

        public override int GetHashCode() =>
            (int)Farmer | ((int)Fox << 2) | ((int)Goose << 4) | ((int)Grain << 6);

        public bool IsValid()
        {
            // If farmer is not with fox and goose are together, fox eats goose
            if (Farmer != Fox && Fox == Goose) return false;
            // If farmer is not with goose and grain are together, goose eats grain
            if (Farmer != Goose && Goose == Grain) return false;
            return true;
        }

        public IEnumerable<State> GetNextStates()
        {
            var items = new[] { "Fox", "Goose", "Grain", "None" };
            foreach (var item in items)
            {
                Side newFarmer = Opposite(Farmer);
                Side newFox = Fox, newGoose = Goose, newGrain = Grain;
                string moveDesc = $"Farmer crosses alone";

                if (item == "Fox" && Farmer == Fox)
                {
                    newFox = Opposite(Fox);
                    moveDesc = "Farmer takes Fox";
                }
                else if (item == "Goose" && Farmer == Goose)
                {
                    newGoose = Opposite(Goose);
                    moveDesc = "Farmer takes Goose";
                }
                else if (item == "Grain" && Farmer == Grain)
                {
                    newGrain = Opposite(Grain);
                    moveDesc = "Farmer takes Grain";
                }
                else if (item != "None")
                {
                    continue; // Can't take item not on same side
                }

                var next = new State(newFarmer, newFox, newGoose, newGrain, this, moveDesc);
                if (next.IsValid())
                    yield return next;
            }
        }

        private Side Opposite(Side s) => s == Side.Left ? Side.Right : Side.Left;
    }

    static void Main()
    {
        var initial = new State(Side.Left, Side.Left, Side.Left, Side.Left);
        var queue = new Queue<State>();
        var visited = new HashSet<State>();
        queue.Enqueue(initial);

        State solution = null;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current.IsGoal())
            {
                solution = current;
                break;
            }
            foreach (var next in current.GetNextStates())
            {
                if (!visited.Contains(next))
                {
                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }
        }

        if (solution != null)
        {
            var path = new Stack<string>();
            for (var s = solution; s.Parent != null; s = s.Parent)
                path.Push(s.Move);
            Console.WriteLine("Solution:");
            foreach (var step in path)
                Console.WriteLine(step);
        }
        else
        {
            Console.WriteLine("No solution found.");
        }
    }
}