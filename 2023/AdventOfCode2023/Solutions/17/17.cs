using Tools;

namespace Solutions
{
  public class Day17
  {

    public enum Direction
    {
      UP,
      DOWN,
      LEFT,
      RIGHT
    }

    class Node
    {
      public int x { get; }
      public int y { get; }
      public int heatloss { get; }
      public Node(int x, int y, int h)
      {
        this.x = x;
        this.y = y;
        heatloss = h;
      }

      public override string ToString()
      {
        return $"{x},{y}: {heatloss}";
      }
    }
    List<string> strings = new List<string>();
    List<List<Node>> grid = new();

    public Day17(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      foreach ((string line, int x) in strings.WithIndex())
      {
        List<Node> newLine = new();
        foreach ((char number, int y) in line.ToArray().WithIndex())
        {
          newLine.Add(new Node(x, y, int.Parse(number.ToString())));
        }
        grid.Add(newLine);
      }
    }

    public int PartOne()
    {
      (int x, int y) start = (0, 0);
      (int x, int y) end = (grid.Count - 1, grid.First().Count - 1);
      Dictionary<string, Node?> pathRecord = AStarPath(grid[start.x][start.y], grid[end.x][end.y]);

      foreach (Node node in pathRecord.Values)
      {
        Console.WriteLine(node);
      }
      // Starting from end, work way backwards to build path.
      List<Node> path = new List<Node>();
      Node? current = pathRecord[$"{end.x},{end.y}"];
      while (current != null)
      {
        Console.WriteLine(current.ToString());
        path.Add(current);
        current = pathRecord[$"{current.x},{current.y}"];
      }
      // Console.WriteLine(pathRecord[$"12,12"]);
      return path.Aggregate(0, (acc, curr) => acc + curr.heatloss);
    }

    public int PartTwo()
    {
      return -1;
    }

    private Dictionary<string, Node?> AStarPath(Node start, Node end)
    {
      // Set up priority queue for A* algorithm
      // High priority is bad, comes later
      PriorityQueue<Node, int> frontier = new PriorityQueue<Node, int>();
      frontier.Enqueue(start, grid[start.x][start.y].heatloss);
      // Dictionary keeps track of where we came from
      Dictionary<string, Node?> cameFrom = new Dictionary<string, Node?>();
      cameFrom[$"{start.x},{start.y}"] = null;
      // Dictionary keeps track of cost so far per node
      Dictionary<string, int> costSoFar = new Dictionary<string, int>();
      costSoFar[$"{start.x},{start.y}"] = 0;

      while (frontier.Count > 0)
      {
        Node current = frontier.Dequeue();

        // Get all neighbouring nodes
        List<Node> neighbours = GetNeighbours(current);
        // Console.WriteLine($"{current}'s neighbours:");
        // foreach (Node neighbour in neighbours)
        // {
        //   Console.WriteLine(neighbour);
        // }
        // For each node, determine priority and add to frontier
        foreach (Node next in neighbours)
        {
          Console.WriteLine(next);

          // New cost is what we've done so far plus the heat cost of next Node
          int newCost = costSoFar[$"{current.x},{current.y}"] + next.heatloss;
          // Check if we've already moved three consecutive spaces
          int consecutiveMoves = ConsecutiveMoves(current, cameFrom[$"{current.x},{current.y}"], cameFrom);
          // Console.WriteLine($"Consecutive moves: {consecutiveMoves}");
          const int MAX_CONSECUTIVE_MOVES = 3;
          if (next.x == end.x && next.y == end.y && consecutiveMoves < MAX_CONSECUTIVE_MOVES)
          {
            cameFrom[$"{next.x},{next.y}"] = current;
            return cameFrom;
          }
          // If we haven't already done 3 consecutive moves in this direction... then
          // If the next node isn't the node we just came from... then
          // If we haven't visited this node, or if this new cost is better than a previous route to this node
          if (
            consecutiveMoves < MAX_CONSECUTIVE_MOVES &&
            // (cameFrom[$"{current.x},{current.y}"] == null || !(next.x == cameFrom[$"{current.x},{current.y}"].x && next.y == cameFrom[$"{current.x},{current.y}"].y)) &&
            (!costSoFar.ContainsKey($"{next.x},{next.y}") || newCost <= costSoFar[$"{next.x},{next.y}"])
          )
          {
            Console.WriteLine($"New or better entry: {next} - {newCost}");
            // Set the cost
            costSoFar[$"{next.x},{next.y}"] = newCost;
            // Determine priority and store in queue
            int priority = newCost; //+ ConsecutiveMovePenalty(next, end);
            frontier.Enqueue(next, priority);
            // Mark that the next node came from the current node
            cameFrom[$"{next.x},{next.y}"] = current;
          }
        }
      }
      return cameFrom;
    }

    private int ConsecutiveMoves(Node current, Node? previous, Dictionary<string, Node?> cameFrom, Direction? d = null)
    {
      // Console.WriteLine($"Current: {current.ToString()}");
      // Console.WriteLine($"previous: {(previous != null ? previous.ToString() : "null")}");
      // Console.WriteLine(d.ToString());

      if (previous == null) return 0;
      // What direction are we coming from?
      if (current.x - previous.x == 1 && (d == null || d == Direction.UP))
      {
        // up
        return 1 + ConsecutiveMoves(previous, cameFrom[$"{previous.x},{previous.y}"], cameFrom, Direction.UP);
      }
      else if (current.x - previous.x == -1 && (d == null || d == Direction.DOWN))
      {
        // down
        return 1 + ConsecutiveMoves(previous, cameFrom[$"{previous.x},{previous.y}"], cameFrom, Direction.DOWN);
      }
      else if (current.y - previous.y == 1 && (d == null || d == Direction.LEFT))
      {
        // left
        return 1 + ConsecutiveMoves(previous, cameFrom[$"{previous.x},{previous.y}"], cameFrom, Direction.LEFT);
      }
      else if (current.y - previous.y == -1 && (d == null || d == Direction.RIGHT))
      {
        // right
        return 1 + ConsecutiveMoves(previous, cameFrom[$"{previous.x},{previous.y}"], cameFrom, Direction.RIGHT);
      }
      return 0;
    }

    private List<Node> GetNeighbours(Node current)
    {
      List<Node> neighbours = new List<Node>();
      // above
      if (current.x > 0)
      {
        neighbours.Add(grid[current.x - 1][current.y]);
      }
      // below
      if (current.x < grid.Count - 1)
      {
        neighbours.Add(grid[current.x + 1][current.y]);
      }
      // left
      if (current.y > 0)
      {
        neighbours.Add(grid[current.x][current.y - 1]);
      }
      // right
      if (current.y < grid.First().Count - 1)
      {
        neighbours.Add(grid[current.x][current.y + 1]);
      }
      return neighbours;
    }
  }
}


