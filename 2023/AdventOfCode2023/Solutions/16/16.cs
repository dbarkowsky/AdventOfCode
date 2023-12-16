using Tools;

namespace Solutions
{
  public enum Direction
  {
    UP,
    DOWN,
    LEFT,
    RIGHT
  }
  public class Space
  {
    public char mirror;
    public List<Direction> passedDirections; // To know if we've already energized in this direction
    public Space(char mirror)
    {
      this.mirror = mirror;
      passedDirections = new();
    }
  }
  public class Day16
  {
    List<string> strings = new List<string>();
    List<List<Space>> grid = new();


    public Day16(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      ResetGrid();
    }

    private void ResetGrid()
    {
      grid = new();
      foreach (string line in strings)
      {
        grid.Add(line.ToCharArray().Select(character => new Space(character)).ToList());
      }
    }

    public int PartOne()
    {
      HashSet<(int x, int y)> energizedSpaces = new(); ;
      ShootLight(0, 0, Direction.RIGHT, energizedSpaces);
      return energizedSpaces.Count;
    }

    public int PartTwo()
    {
      // List to store counts of energized tiles
      List<int> energizedCounts = new();
      // List of all starting points and directions
      List<(int x, int y, Direction d)> startingList = new();
      // Get all edges, add them to the list
      for (int x = 0; x < grid.Count; x++)
      {
        startingList.Add((x, 0, Direction.RIGHT));
        startingList.Add((x, grid[x].Count - 1, Direction.LEFT));
      }
      for (int y = 0; y < grid.First().Count; y++)
      {
        startingList.Add((0, y, Direction.DOWN));
        startingList.Add((grid.Count - 1, y, Direction.UP));
      }
      // For each starting location, get the number of energized spaces, and add to counts
      foreach ((int x, int y, Direction d) location in startingList)
      {
        ResetGrid();
        HashSet<(int x, int y)> energizedSpaces = new(); ;
        ShootLight(location.x, location.y, location.d, energizedSpaces);
        energizedCounts.Add(energizedSpaces.Count);
      }
      return energizedCounts.Max();
    }

    private (int x, int y, Direction d) GetLocationFromDirection(int x, int y, Direction d)
    {
      switch (d)
      {
        case Direction.RIGHT:
          return (x, y + 1, Direction.RIGHT);
        case Direction.LEFT:
          return (x, y - 1, Direction.LEFT);
        case Direction.DOWN:
          return (x + 1, y, Direction.DOWN);
        case Direction.UP:
          return (x - 1, y, Direction.UP);
        default:
          return (-1, -1, Direction.RIGHT);
      }
    }

    // Uses current mirror and direction to get next location
    // Returns a list in case the beam is split
    private List<(int x, int y, Direction d)> GetNextForQueue(int x, int y, Direction d)
    {
      char mirror = grid[x][y].mirror;
      List<(int x, int y, Direction d)> toReturn = new();
      switch (mirror)
      {
        case '/':
          switch (d)
          {
            case Direction.UP:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.RIGHT));
              break;
            case Direction.DOWN:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.LEFT));
              break;
            case Direction.LEFT:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.DOWN));
              break;
            case Direction.RIGHT:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.UP));
              break;
          }
          break;
        case '\\':
          switch (d)
          {
            case Direction.UP:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.LEFT));
              break;
            case Direction.DOWN:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.RIGHT));
              break;
            case Direction.LEFT:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.UP));
              break;
            case Direction.RIGHT:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.DOWN));
              break;
          }
          break;
        case '|':
          switch (d)
          {
            case Direction.LEFT:
            case Direction.RIGHT:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.DOWN));
              toReturn.Add(GetLocationFromDirection(x, y, Direction.UP));
              break;
            case Direction.UP:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.UP));
              break;
            case Direction.DOWN:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.DOWN));
              break;
          }
          break;
        case '-':
          switch (d)
          {
            case Direction.UP:
            case Direction.DOWN:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.LEFT));
              toReturn.Add(GetLocationFromDirection(x, y, Direction.RIGHT));
              break;
            case Direction.LEFT:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.LEFT));
              break;
            case Direction.RIGHT:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.RIGHT));
              break;
          }
          break;
        case '.':
          switch (d)
          {
            case Direction.LEFT:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.LEFT));
              break;
            case Direction.RIGHT:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.RIGHT));
              break;
            case Direction.UP:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.UP));
              break;
            case Direction.DOWN:
              toReturn.Add(GetLocationFromDirection(x, y, Direction.DOWN));
              break;
          }
          break;
      }
      return toReturn;
    }

    private void ShootLight(int startingX, int startingY, Direction startingDirection, HashSet<(int x, int y)> energizedSpaces)
    {
      Queue<(int x, int y, Direction d)> shootQueue = new();
      shootQueue.Enqueue((startingX, startingY, startingDirection));

      while (shootQueue.Count > 0)
      {
        (int x, int y, Direction d) current = shootQueue.Dequeue();
        // Is this a valid space on the grid?
        if (current.x >= 0 && current.x < grid.Count && current.y >= 0 && current.y < grid.First().Count)
        {
          // Console.WriteLine($"{current.x},{current.y}");
          // Is this space already energized in this direction?
          if (grid[current.x][current.y].passedDirections.Contains(current.d))
          {
            // Then we've already passed through in this direction and don't need to do this again
            continue;
          }
          else
          {
            // Energize this space
            grid[current.x][current.y].passedDirections.Add(current.d);
            // Add to set of energized spaces
            energizedSpaces.Add((current.x, current.y));
            // What do we add to the queue next?
            List<(int x, int y, Direction d)> nextForQueue = GetNextForQueue(current.x, current.y, current.d);
            foreach ((int x, int y, Direction d) instruction in nextForQueue)
            {
              shootQueue.Enqueue(instruction);
            }
          }
        }
      }
    }
  }
}


