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
      // Console.WriteLine(distances);
      // foreach (KeyValuePair<Point, int> kvp in distances)
      // {
      //   Console.WriteLine(kvp.Key + ": " + kvp.Value);
      // }
      return distances[end] * -1;
    }

    // Same concept, finding longest path, but this time the > restrictions don't matter.
    // This maybe creates a problem of cyclic graphs, which a topological sort can't handle.
    // Either use a DFS and brute force, or use BFS and track a bunch of independant states
    // In either case there's a potential limit to maximum memory usage. I think I can reduce that with BFS by tracking paths
    public int PartTwo()
    {
      return -1;
    }

    private List<Point> TopologicalSort()
    {
      // Only one starting node, so we can shortcut this a bit
      HashSet<Point> visited = new();
      List<Point> ordering = new();

      // Essentially a depth-first search from here.
      DFS(start, visited, ordering);

      return ordering;
    }

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


