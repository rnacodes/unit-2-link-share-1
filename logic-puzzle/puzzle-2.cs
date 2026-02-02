//Claude Haiku 4.5
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    enum Item { Farmer, Fox, Goose, Grain }
    
    class RiverState
    {
        public HashSet<Item> Left;
        public HashSet<Item> Right;
        public RiverState Parent;
        public string Action;

        public RiverState()
        {
            Left = new HashSet<Item> { Item.Farmer, Item.Fox, Item.Goose, Item.Grain };
            Right = new HashSet<Item>();
            Parent = null;
            Action = null;
        }

        private RiverState(HashSet<Item> left, HashSet<Item> right, RiverState parent, string action)
        {
            Left = new HashSet<Item>(left);
            Right = new HashSet<Item>(right);
            Parent = parent;
            Action = action;
        }

        public bool IsGoal() => Left.Count == 0;

        public bool IsValid()
        {
            // Check left side
            if (Left.Contains(Item.Fox) && Left.Contains(Item.Goose) && !Left.Contains(Item.Farmer))
                return false;
            if (Left.Contains(Item.Goose) && Left.Contains(Item.Grain) && !Left.Contains(Item.Farmer))
                return false;

            // Check right side
            if (Right.Contains(Item.Fox) && Right.Contains(Item.Goose) && !Right.Contains(Item.Farmer))
                return false;
            if (Right.Contains(Item.Goose) && Right.Contains(Item.Grain) && !Right.Contains(Item.Farmer))
                return false;

            return true;
        }

        public IEnumerable<RiverState> GetNextStates()
        {
            if (Left.Contains(Item.Farmer))
            {
                // Farmer moves from left to right
                // Farmer alone
                var next = new RiverState(Left, Right, this, "Farmer crosses alone to the right");
                next.Left.Remove(Item.Farmer);
                next.Right.Add(Item.Farmer);
                if (next.IsValid())
                    yield return next;

                // Farmer with each item on left
                foreach (var item in new[] { Item.Fox, Item.Goose, Item.Grain })
                {
                    if (Left.Contains(item))
                    {
                        var next2 = new RiverState(Left, Right, this, $"Farmer takes {item} to the right");
                        next2.Left.Remove(Item.Farmer);
                        next2.Left.Remove(item);
                        next2.Right.Add(Item.Farmer);
                        next2.Right.Add(item);
                        if (next2.IsValid())
                            yield return next2;
                    }
                }
            }
            else
            {
                // Farmer moves from right to left
                // Farmer alone
                var next = new RiverState(Left, Right, this, "Farmer crosses alone to the left");
                next.Right.Remove(Item.Farmer);
                next.Left.Add(Item.Farmer);
                if (next.IsValid())
                    yield return next;

                // Farmer with each item on right
                foreach (var item in new[] { Item.Fox, Item.Goose, Item.Grain })
                {
                    if (Right.Contains(item))
                    {
                        var next2 = new RiverState(Left, Right, this, $"Farmer takes {item} to the left");
                        next2.Right.Remove(Item.Farmer);
                        next2.Right.Remove(item);
                        next2.Left.Add(Item.Farmer);
                        next2.Left.Add(item);
                        if (next2.IsValid())
                            yield return next2;
                    }
                }
            }
        }

        public string GetStateString() =>
            $"Left: {string.Join(", ", Left.OrderBy(x => x.ToString()))} | Right: {string.Join(", ", Right.OrderBy(x => x.ToString()))}";

        public override bool Equals(object obj)
        {
            if (obj is RiverState rs)
                return Left.SetEquals(rs.Left) && Right.SetEquals(rs.Right);
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var item in Left)
                hash ^= item.GetHashCode();
            return hash;
        }
    }

    static void Main()
    {
        var initial = new RiverState();
        var queue = new Queue<RiverState>();
        var visited = new HashSet<RiverState>();
        queue.Enqueue(initial);
        visited.Add(initial);

        RiverState solution = null;
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
            var moves = new Stack<string>();
            for (var state = solution; state.Parent != null; state = state.Parent)
                moves.Push(state.Action);

            Console.WriteLine("=== Fox, Goose, and Grain River Crossing Puzzle ===\n");
            Console.WriteLine("Initial State:");
            Console.WriteLine("Left: Farmer, Fox, Goose, Grain | Right: (empty)\n");

            int step = 1;
            foreach (var move in moves)
            {
                Console.WriteLine($"Step {step}: {move}");
                step++;
            }

            Console.WriteLine($"\nFinal State:");
            Console.WriteLine("Left: (empty) | Right: Farmer, Fox, Goose, Grain");
            Console.WriteLine($"\nTotal moves: {moves.Count}");
        }
        else
        {
            Console.WriteLine("No solution found.");
        }
    }
}