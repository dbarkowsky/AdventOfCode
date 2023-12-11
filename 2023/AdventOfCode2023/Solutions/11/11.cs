using Tools;

namespace Solutions
{
  public class Day11
  {
    List<string> strings = new List<string>();
    List<List<char>> grid = new();
    struct Point
    {
      public int x;
      public int y;
      public Point(int x, int y)
      {
        this.x = x;
        this.y = y;
      }
    }
    public Day11(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      foreach (string row in strings)
      {
        grid.Add(row.ToCharArray().ToList());
      }
    }

    public int PartOne()
    {
      // Find the points
      List<Point> points = FindPoints();
      // Expand the grid and get new points
      List<Point> expandedPoints = ExpandGrid(1, points);
      // Join those points into point pairs
      List<(Point p1, Point p2)> pointPairs = GetJoinedPoints(expandedPoints);
      // For each point pair, get the distance between
      // Sum those distances
      int sum = 0;
      foreach ((Point p1, Point p2) pointPair in pointPairs)
      {
        sum += GetDistanceBetween(pointPair.p1, pointPair.p2);
      }

      return sum;
    }

    public long PartTwo()
    {
      // Find the points
      List<Point> points = FindPoints();
      // Expand the grid and get new points
      List<Point> expandedPoints = ExpandGrid(1000000, points);
      // Join those points into point pairs
      List<(Point p1, Point p2)> pointPairs = GetJoinedPoints(expandedPoints);
      // For each point pair, get the distance between
      // Sum those distances
      long sum = 0;
      foreach ((Point p1, Point p2) pointPair in pointPairs)
      {
        sum += GetDistanceBetween(pointPair.p1, pointPair.p2);
      }

      return sum;
    }

    // Get's difference between two Points
    private int GetDistanceBetween(Point p1, Point p2)
    {
      return Math.Abs(p2.x - p1.x) + Math.Abs(p2.y - p1.y);
    }

    // Copies the list of points, finds rows that can be expanded
    // Modifies the points x and y to reflect expansion amount
    private List<Point> ExpandGrid(int numExpansions, List<Point> points)
    {
      List<Point> expandedPoints = points.ToList();
      // Which horizontal lines can be expanded?
      List<int> horizontalExpansions = new();
      for (int x = 0; x < grid.Count; x++)
      {
        if (grid[x].All(point => point == '.')) horizontalExpansions.Add(x);
      }
      // Which vertical lines can be expanded?
      List<int> verticalExpansions = new();
      for (int y = 0; y < grid.First().Count; y++)
      {
        bool goodCandidate = true;
        for (int x = 0; x < grid.Count; x++)
        {
          if (grid[x][y] != '.')
          {
            goodCandidate = false;
            break;
          }
        }
        if (goodCandidate) verticalExpansions.Add(y);
      }

      // For each point
      foreach ((Point point, int i) in points.WithIndex())
      {
        // For each horizontal line expanded that's < x, x = x + numExpansions
        foreach (int row in horizontalExpansions)
        {
          if (row < point.x) expandedPoints[i] = new Point(expandedPoints[i].x + Math.Max(1, numExpansions - 1), expandedPoints[i].y);
        }
        // For each vertical line expanded that's < y, y = y + numExpansions
        foreach (int column in verticalExpansions)
        {
          if (column < point.y) expandedPoints[i] = new Point(expandedPoints[i].x, expandedPoints[i].y + Math.Max(1, numExpansions - 1));
        }
      }

      // Return new list of Points
      return expandedPoints;
    }

    // Finds all the # points on the grid
    private List<Point> FindPoints()
    {
      List<Point> points = new();
      foreach ((List<char> row, int x) in grid.WithIndex())
      {
        foreach ((char point, int y) in row.WithIndex())
        {
          if (point == '#') points.Add(new Point(x, y));
        }
      }
      return points;
    }

    // Creates a list of points with no reversed or repeating pairs
    private List<(Point p1, Point p2)> GetJoinedPoints(List<Point> points)
    {
      List<(Point p1, Point p2)> pointPairs = new();
      foreach (Point p1 in points)
      {
        foreach (Point p2 in points)
        {
          if (!(p1.x == p2.x && p1.y == p2.y))
          {
            if (!pointPairs.Contains((p1, p2)) && !pointPairs.Contains((p2, p1)))
            {
              pointPairs.Add((p1, p2));
            }
          }
        }
      }

      return pointPairs;
    }
  }
}


