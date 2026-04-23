using System.Drawing;
using Tools;

namespace Solutions
{
  public class Day23
  {
    List<string> strings = new List<string>();
    Dictionary<Point, string> maze = new Dictionary<Point, string>();
    Point start;
    Point end;

    public Day23(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      for (int x = 0; x < strings.Count; x++)
      {
        for (int y = 0; y < strings[x].Length; y++)
        {
          maze.Add(new Point(x, y), strings[x].ElementAt(y).ToString());
        }
      }
      start = new Point(0, 1); // Top row, one over from left
      end = new Point(strings.Count - 1, strings[0].Length - 2); // Bottom row, one over from right
    }

    // Find the longest path through the maze
    // We can take advantage of the fact that all forks in the maze are governed by uni-directional tiles
    // They prevent us from going back the wrong way on a path
    // DO NOT count start as a step, but count the end
    public int PartOne()
    {
      // We can find longest path via topological sort + distance calculations
      // This might be overkill, as each distance between adjacent tiles is just 1 for now.
      List<Point> sortedTiles = TopologicalSort();

      Dictionary<Point, int> distances = new();
      foreach (Point tile in sortedTiles)
      {
        distances.Add(tile, int.MaxValue);
      }
      distances[start] = 0; // No steps to get to start

      // For each node, get edges and replace distance if it's smaller than previously calculated one
      sortedTiles.ForEach(sortedTile =>
      {
        if (distances.ContainsKey(sortedTile))
        {
          List<(Point, string)> nextTiles = GetNextTiles(sortedTile);
          nextTiles.ForEach(nextTile =>
          {
            // Distance is distance so far + the cost of the edge.
            int newDistance = distances[sortedTile] + -1; // -1 because we want the longest. Consider it inverted from regular distance of 1. Otherwise would get shortest.
            Point nextPoint = nextTile.Item1;
            distances[nextPoint] = Math.Min(distances[nextPoint], newDistance);
          });
        }
      });
      // The longest path will be the value at the end, but inverted
      return distances[end] * -1;
    }

    // Same concept, finding longest path, but this time the > restrictions don't matter.
    // This maybe creates a problem of cyclic graphs, which a topological sort can't handle.
    // Compress graph to just junction nodes, then DFS with backtracking.
    // Essentially, make a high-level graph to eliminate all the 1-distance edges.
    public int PartTwo()
    {
      // First, we only care about places where the path can split, aka junctions
      // Everywhere else is just a series of nodes in the grid with only one path
      // Why start and end? They are the first and last nodes in our new graph
      HashSet<Point> junctions = new() { start, end };
      foreach (KeyValuePair<Point, string> kvp in maze)
      {
        if (kvp.Value == "#") continue;
        if (GetNextTiles(kvp.Key, ignoreSlopes: true).Count >= 3)
          junctions.Add(kvp.Key);
      }

      // BFS from each junction to find adjacent junctions and their distances
      // We're finding these so that we can make a superset graph, where each junction is a node, 
      // and the edges now have bigger weights (distances)
      Dictionary<Point, List<(Point neighbor, int distance)>> graph = new();
      foreach (Point junction in junctions)
      {
        graph[junction] = new List<(Point, int)>();
        Queue<(Point, int)> queue = new();
        HashSet<Point> seen = new() { junction };
        queue.Enqueue((junction, 0));

        while (queue.Count > 0)
        {
          (Point currentTile, int distance) = queue.Dequeue();
          // If we've reached a different junction, record the edge and stop expanding from here
          if (distance > 0 && junctions.Contains(currentTile))
          {
            graph[junction].Add((currentTile, distance));
            continue;
          }
          foreach ((Point next, _) in GetNextTiles(currentTile, ignoreSlopes: true))
          {
            // If we haven't visited this yet, mark it as so and track the distance.
            // Remember, seen set is reset each junction, so we're not carrying it over between BFS runs
            if (!seen.Contains(next))
            {
              seen.Add(next);
              queue.Enqueue((next, distance + 1));
            }
          }
        }
      }

      // DFS with backtracking on the compressed graph
      HashSet<Point> visited = new();
      return LongestPath(graph, visited, start);
    }

    // Just a simple DFS based on high-level graph of junctions
    private int LongestPath(Dictionary<Point, List<(Point, int)>> graph, HashSet<Point> visited, Point current)
    {
      if (current == end) return 0;

      visited.Add(current);
      int best = int.MinValue;

      foreach ((Point next, int dist) in graph[current])
      {
        if (!visited.Contains(next))
        {
          // Recurse if we haven't checked this route yet
          int sub = LongestPath(graph, visited, next);
          if (sub != int.MinValue)
            best = Math.Max(best, sub + dist); // Keep the longest found route
        }
      }

      // This is the backtracking key. 
      // Remove this from visited before returning, then another path can always visit it again.
      visited.Remove(current);
      return best;
    }

    // Sort designed to order all tile (nodes) in the maze (graph)
    // Learned this in the graph theory course
    private List<Point> TopologicalSort()
    {
      // Only one starting node, so we can shortcut this a bit
      HashSet<Point> visited = new();
      List<Point> ordering = new();

      // Essentially a depth-first search from here.
      DFS(start, visited, ordering);

      return ordering;
    }

    // A simple DFS to work with the topological sort.
    // Always appends new nodes to the start, because this was supposed to be a queue-like structure,
    // but C# doesn't do .unshift like Javascript does.
    private void DFS(Point at, HashSet<Point> visited, List<Point> ordering)
    {
      visited.Add(at);
      List<(Point, string)> nextTiles = GetNextTiles(at);
      // Any non-visited edges? Continue dfs
      foreach ((Point, string) tile in nextTiles)
      {
        if (!visited.Contains(tile.Item1))
        {
          DFS(tile.Item1, visited, ordering);
        }
      }
      ordering.Insert(0, at);
    }

    // Just gets adjacent tiles from a specified point.
    // Had to add the ignoreSlopes argument because of part 2
    private List<(Point, string)> GetNextTiles(Point from, bool ignoreSlopes = false)
    {
      List<(Point, string)> tiles = new();
      List<(int, int)> adjecentCoords = new() { (0, -1), (0, 1), (-1, 0), (1, 0) };
      foreach ((int, int) coord in adjecentCoords)
      {
        Point p = new(from.X + coord.Item1, from.Y + coord.Item2);
        if (maze.ContainsKey(p))
        {
          // Let's only include non # blocks, because # can't be traversed.
          if (maze[p] == "#") continue;
          // If this is a > and pointing towards our current block, then we also continue
          if (!ignoreSlopes)
          {
            if (
              (maze[p] == ">" && p == new Point(from.X, from.Y - 1)) || // left
              (maze[p] == "<" && p == new Point(from.X, from.Y + 1)) || // right
              (maze[p] == "v" && p == new Point(from.X - 1, from.Y)) || // up
              (maze[p] == "^" && p == new Point(from.X + 1, from.Y))    // down, but this doesn't actually exist in input
            ) continue;
          }
          // Otherwise, add this to possible next tiles
          tiles.Add((p, maze[p]));
        }
      }
      return tiles;
    }
  }
}


