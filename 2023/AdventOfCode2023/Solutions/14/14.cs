using Microsoft.VisualBasic;
using Tools;

namespace Solutions
{
  public class Day14
  {
    List<string> strings = new List<string>();
    List<List<char>> grid = new();


    public Day14(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
    }

    public int PartOne()
    {
      ResetGrid();
      // Rocks need to go NORTH
      TiltNorth();
      // Sum the weight from each line
      return GetLoad();
    }

    public int PartTwo()
    {
      ResetGrid();
      int iterations = 1000; // Seems to be enough... Don't need 1 billion
      for (int i = 0; i < iterations; i++)
      {
        Cycle();
      }

      return GetLoad();
    }

    private void ResetGrid()
    {
      grid = new();
      foreach (string line in strings)
      {
        grid.Add(line.ToCharArray().ToList());
      }
    }

    private int GetLoad()
    {
      int multiplier = 1;
      int sum = 0;
      for (int x = grid.Count - 1; x >= 0; x--)
      {
        int rockCount = grid[x].Aggregate(0, (acc, cur) =>
        {
          if (cur == 'O')
          {
            return acc + multiplier;
          }
          return acc;
        });
        sum += rockCount;
        multiplier++;
      }
      return sum;
    }

    private void Cycle()
    {
      TiltNorth();
      TiltWest();
      TiltSouth();
      TiltEast();
    }

    private void TiltNorth()
    {
      // Traverse grid each column from top to bottom
      for (int y = 0; y < grid.First().Count; y++)
      {
        for (int x = 0; x < grid.Count; x++)
        {
          if (grid[x][y] == 'O')
          {
            // If we find an O rock, look up the column until we find another O, a #, or the edge
            (int x, int y) rockPosition = (x, y);
            int currentX = x - 1;
            while (currentX >= 0 && grid[currentX][y] != 'O' && grid[currentX][y] != '#')
            {
              currentX--;
            }
            // Then swap the current rock with the last known free position
            if (currentX + 1 != rockPosition.x)
            {
              grid[currentX + 1][y] = 'O';
              grid[rockPosition.x][rockPosition.y] = '.';
            }
          }
        }
      }
    }

    private void TiltSouth()
    {
      // Traverse grid each column from bottom to top
      for (int y = 0; y < grid.First().Count; y++)
      {
        for (int x = grid.Count - 1; x >= 0; x--)
        {
          if (grid[x][y] == 'O')
          {
            // If we find an O rock, look down the column until we find another O, a #, or the edge
            (int x, int y) rockPosition = (x, y);
            int currentX = x + 1;
            while (currentX < grid.Count && grid[currentX][y] != 'O' && grid[currentX][y] != '#')
            {
              currentX++;
            }
            // Then swap the current rock with the last known free position
            if (currentX - 1 != rockPosition.x)
            {
              grid[currentX - 1][y] = 'O';
              grid[rockPosition.x][rockPosition.y] = '.';
            }
          }
        }
      }
    }

    private void TiltWest()
    {
      for (int x = 0; x < grid.Count; x++)
      {
        for (int y = 0; y < grid.First().Count; y++)
        {
          if (grid[x][y] == 'O')
          {
            // If we find an O rock, look down the column until we find another O, a #, or the edge
            (int x, int y) rockPosition = (x, y);
            int currentY = y - 1;
            while (currentY >= 0 && grid[x][currentY] != 'O' && grid[x][currentY] != '#')
            {
              currentY--;
            }
            // Then swap the current rock with the last known free position
            if (currentY + 1 != rockPosition.y)
            {
              grid[x][currentY + 1] = 'O';
              grid[rockPosition.x][rockPosition.y] = '.';
            }
          }
        }
      }
    }

    private void TiltEast()
    {
      for (int x = 0; x < grid.Count; x++)
      {
        for (int y = grid.First().Count - 1; y >= 0; y--)
        {
          if (grid[x][y] == 'O')
          {
            // If we find an O rock, look down the column until we find another O, a #, or the edge
            (int x, int y) rockPosition = (x, y);
            int currentY = y + 1;
            while (currentY < grid.First().Count && grid[x][currentY] != 'O' && grid[x][currentY] != '#')
            {
              currentY++;
            }
            // Then swap the current rock with the last known free position
            if (currentY - 1 != rockPosition.y)
            {
              grid[x][currentY - 1] = 'O';
              grid[rockPosition.x][rockPosition.y] = '.';
            }
          }
        }
      }
    }

    private void PrintGrid()
    {
      for (int x = 0; x < grid.Count; x++)
      {
        for (int y = 0; y < grid[x].Count; y++)
        {
          Console.Write(grid[x][y]);
        }
        Console.WriteLine("");
      }
    }
  }
}


