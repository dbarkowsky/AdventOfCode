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
        if (horizontal == 0)
        {
          sum += vertical;
        }
      }
      return sum;
    }

    // I tried to do a similar thing here originally, but it's not that simple.
    // We can't look one row/col at a time, we need to compare matrices to see if they are off by one.
    public int PartTwo()
    {
      int sum = 0;
      foreach (List<string> grid in grids)
      {
        int horizontal = FindMatchingHorizontals(grid);
        int vertical = FindMatchingVerticals(grid);
        sum += horizontal * 100;
        if (horizontal == 0)
        {
          sum += vertical;
        }
      }
      return sum;
    }

    private int FindHorizontalMirror(List<string> grid)
    {
      // Move through indices of grid. Get all lines on either side of split, and compare to find if there's a off-by-one error.
      for (int i = 1; i < grid.Count; i++)
      {
        
      }
    }

    

    private int FindMatchingVerticals(List<string> grid)
    {
      int width = grid.First().Length;
      for (int y = 1; y < width; y++)
      {
        // If we find adjacent matching columns
        if (VerticalHowManyDifferences(y - 1, y, grid) <= 0)
        {
          int left = y - 1;
          int right = y;
          int adjustment = 0;
          // As long as we're on the grid and each next column has a mirror
          while (left - adjustment >= 0 && right + adjustment < grid.First().Length && VerticalHowManyDifferences(left - adjustment, right + adjustment, grid) <= 0)
          {
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
        if (HorizontalHowManyDifferences(grid[x - 1], grid[x]) <= 0)
        {
          int top = x - 1;
          int bottom = x;
          int adjustment = 0;
          // As long as we're on the grid and each next row has a mirror (or in part2 has only one difference)
          while (top - adjustment >= 0 && bottom + adjustment < grid.Count && HorizontalHowManyDifferences(grid[top - adjustment], grid[bottom + adjustment]) <= 0)
          {
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

    private int HorizontalHowManyDifferences(string row1, string row2)
    {
      int differences = 0;
      for (int i = 0; i < row1.Length; i++)
      {
        if (row1[i] != row2[i])
        {
          differences++;
        }
      }
      return differences;
    }

    private int VerticalHowManyDifferences(int col1, int col2, List<string> grid)
    {
      // Check if the specified columns are within bounds
      if (col1 < 0 || col1 >= grid.First().Length || col2 < 0 || col2 >= grid.First().Length)
      {
        return 100;
      }
      int differences = 0;
      // Iterate through each row and compare the values in the specified columns
      for (int row = 0; row < grid.Count; row++)
      {
        string valueCol1 = grid[row][col1].ToString();
        string valueCol2 = grid[row][col2].ToString();

        if (valueCol1 != valueCol2)
        {
          // Columns are not identical in this row
          differences++;
        }
      }
      return differences;
    }
  }
}
