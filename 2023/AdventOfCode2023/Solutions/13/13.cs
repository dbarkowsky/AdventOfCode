using Tools;

namespace Solutions
{
  public class Day13
  {
    List<string> strings = new List<string>();
    List<List<string>> grids = new();


    public Day13(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      List<string> grid = new();
      foreach (string line in strings)
      {
        if (line == "")
        {
          grids.Add(grid);
          grid = new();
        }
        else
        {
          grid.Add(line);
        }
      }
      grids.Add(grid);
    }

    public int PartOne()
    {
      int sum = 0;
      foreach (List<string> grid in grids)
      {
        int horizontal = FindMatchingHorizontals(grid);
        int vertical = FindMatchingVerticals(grid);
        sum += horizontal * 100;
        sum += vertical;
      }
      return sum;
    }

    public int PartTwo()
    {

      return -1;
    }

    private bool ColumnsMatch(List<string> grid, int col1, int col2)
    {
      // Check if the specified columns are within bounds
      if (col1 < 0 || col1 >= grid.First().Length || col2 < 0 || col2 >= grid.First().Length)
      {
        return false;
      }

      // Iterate through each row and compare the values in the specified columns
      for (int row = 0; row < grid.Count; row++)
      {
        string valueCol1 = grid[row][col1].ToString();
        string valueCol2 = grid[row][col2].ToString();

        if (valueCol1 != valueCol2)
        {
          // Columns are not identical in this row
          return false;
        }
        Console.WriteLine($"{grid[row]} {row},{col1} {row},{col2}");
      }

      // All rows have identical values in the specified columns
      return true;
    }

    private int FindMatchingVerticals(List<string> grid)
    {
      int width = grid.First().Length;
      for (int y = 1; y < width; y++)
      {
        // If we find adjacent matching columns
        if (ColumnsMatch(grid, y - 1, y))
        {
          Console.WriteLine($"Found Match y {y - 1},{y}");
          int left = y - 1;
          int right = y;
          int adjustment = 0;
          // As long as we're on the grid and each next column has a mirror
          while (left - adjustment >= 0 && right + adjustment < grid.First().Length && ColumnsMatch(grid, left - adjustment, right + adjustment))
          {
            // Console.WriteLine($"Adjustment {adjustment}");
            adjustment++; // Move away from the original match
          }
          // Back it up
          adjustment--;
          // Were we at the edge of the grid?
          if (left - adjustment == 0 || right + adjustment == grid.First().Length - 1)
          {
            return left + 1; // Return number of columns from left up
          }
        }
      }
      return 0;
    }

    private int FindMatchingHorizontals(List<string> grid)
    {
      // For every row in the grid
      for (int x = 1; x < grid.Count; x++)
      {
        // If we find adjacent matching rows
        if (grid[x - 1] == grid[x])
        {
          Console.WriteLine($"Found Match x {x - 1},{x}");
          int top = x - 1;
          int bottom = x;
          int adjustment = 0;
          // As long as we're on the grid and each next row has a mirror
          while (top - adjustment >= 0 && bottom + adjustment < grid.Count && grid[top - adjustment] == grid[bottom + adjustment])
          {
            // Console.WriteLine($"Adjustment {adjustment}");
            adjustment++; // Move away from the original match
          }
          // Back it up
          adjustment--;
          // Were we at the edge of the grid?
          if (top - adjustment == 0 || bottom + adjustment == grid.Count - 1)
          {
            return top + 1; // Return number of rows from top up
          }
        }
      }
      return 0;
    }
  }
}


