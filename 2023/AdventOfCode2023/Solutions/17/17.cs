using Tools;

namespace Solutions
{
  public class Day17
  {

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
      List<Node> path = FollowPaths(start, end);

      return path.Aggregate(0, (acc, cur) => acc + cur.heatloss);
    }

    public int PartTwo()
    {
      return -1;
    }

    private List<Node> FollowPaths((int x, int y) start, (int x, int y) end)
    {
      List<Node> path = new();
      return path;
    }
  }
}


