using Tools;

namespace Solutions
{
  public class Day10
  {
    List<string> strings = new List<string>();
    List<List<char>> grid = new();
    (int x, int y) start = (x: 0, y: 0);
    char startPipe = 'S';

    enum Direction
    {
      NORTH,
      SOUTH,
      EAST,
      WEST,
      NONE
    }

    public Day10(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      foreach (string row in strings)
      {
        grid.Add(row.ToList());
      }
      // Where is start?
      start = FindStart();
      // What kind of pipe is start?
      startPipe = GetPipeFromSurroundings(start.x, start.y);
    }

    public int PartOne()
    {
      Direction[] directions = GetDirectionsFromPipe(startPipe);
      // Move the first steps
      (int x, int y) currentPipe1 = GetNextLocation(start.x, start.y, directions.First());
      // Console.WriteLine($"curr1: {currentPipe1.x}, {currentPipe1.y}");
      (int x, int y) currentPipe2 = GetNextLocation(start.x, start.y, directions.Last());
      // Console.WriteLine($"curr2: {currentPipe2.x}, {currentPipe2.y}");
      Direction dir1 = GetNextDirection(currentPipe1.x, currentPipe1.y, GetOppositeDirection(directions.First()));
      // Console.WriteLine(dir1.ToString());
      Direction dir2 = GetNextDirection(currentPipe2.x, currentPipe2.y, GetOppositeDirection(directions.Last()));
      // Console.WriteLine(dir2.ToString());

      int steps = 1;

      // Continue moving steps until they are the same position
      while (currentPipe1 != currentPipe2)
      {
        // Console.WriteLine(currentPipe1.x + "," + currentPipe1.y + ", " + dir1.ToString());
        // Move pipe location 1
        currentPipe1 = GetNextLocation(currentPipe1.x, currentPipe1.y, dir1);
        // Determine next direction
        dir1 = GetNextDirection(currentPipe1.x, currentPipe1.y, GetOppositeDirection(dir1));

        // Move pipe location 2
        currentPipe2 = GetNextLocation(currentPipe2.x, currentPipe2.y, dir2);
        // Determine next direction
        dir2 = GetNextDirection(currentPipe2.x, currentPipe2.y, GetOppositeDirection(dir2));

        steps++;
      }

      return steps;
    }

    public int PartTwo()
    {
      return -1;
    }

    private Direction GetNextDirection(int x, int y, Direction from)
    {
      char pipe = grid[x][y];
      Direction[] pipeDirections = GetDirectionsFromPipe(pipe);
      Console.WriteLine($"{from.ToString()} {pipeDirections.First().ToString()}, {pipeDirections.Last().ToString()}");
      // Remove direction that would go back to origin
      pipeDirections = pipeDirections.Where(direction => direction != from).ToArray();
      // Remaining direction is the only way to go
      Direction nextDirection = pipeDirections.First();
      Console.WriteLine(nextDirection.ToString());
      return nextDirection;
    }

    private (int, int) GetNextLocation(int currX, int currY, Direction dir)
    {
      // return the coordinates of that next location
      if (dir == Direction.NORTH) return (currX - 1, currY);
      if (dir == Direction.SOUTH) return (currX + 1, currY);
      if (dir == Direction.WEST) return (currX, currY - 1);
      return (currX, currY + 1);
    }

    private (int, int) FindStart()
    {
      foreach ((List<char> row, int x) in grid.WithIndex())
      {
        foreach ((char pipe, int y) in row.WithIndex())
        {
          if (pipe == 'S')
          {
            return (x, y);
          }
        }
      }
      return (0, 0);
    }

    private char GetPipeFromSurroundings(int x, int y)
    {
      List<Direction> pipeDirections = new();
      // Check North
      if (x > 0)
      {
        Direction[] northDirections = GetDirectionsFromPipe(grid[x - 1][y]);
        if (northDirections.Contains(Direction.SOUTH)) pipeDirections.Add(Direction.NORTH);
      }

      // Check South
      if (x < grid.Count - 1)
      {
        Direction[] southDirections = GetDirectionsFromPipe(grid[x + 1][y]);
        if (southDirections.Contains(Direction.NORTH)) pipeDirections.Add(Direction.SOUTH);
      }

      // Check West
      if (y > 0)
      {
        Direction[] westDirections = GetDirectionsFromPipe(grid[x][y - 1]);
        if (westDirections.Contains(Direction.EAST)) pipeDirections.Add(Direction.WEST);
      }

      // Check East
      if (y < grid[x].Count - 1)
      {
        Direction[] eastDirections = GetDirectionsFromPipe(grid[x][y + 1]);
        if (eastDirections.Contains(Direction.WEST)) pipeDirections.Add(Direction.EAST);
      }

      // Use directions to get pipe type
      return GetPipeFromDirections(pipeDirections);
    }

    private Direction[] GetDirectionsFromPipe(char pipe)
    {
      switch (pipe)
      {
        case '|':
          return new Direction[] { Direction.NORTH, Direction.SOUTH };
        case '-':
          return new Direction[] { Direction.EAST, Direction.WEST };
        case 'F':
          return new Direction[] { Direction.SOUTH, Direction.EAST };
        case 'J':
          return new Direction[] { Direction.NORTH, Direction.WEST };
        case 'L':
          return new Direction[] { Direction.NORTH, Direction.EAST };
        case '7':
          return new Direction[] { Direction.SOUTH, Direction.WEST };
        default:
          return new Direction[] { Direction.NONE, Direction.NONE };
      }
    }

    private char GetPipeFromDirections(List<Direction> directions)
    {
      if (directions.Contains(Direction.NORTH) && directions.Contains(Direction.SOUTH)) return '|';
      if (directions.Contains(Direction.EAST) && directions.Contains(Direction.WEST)) return '-';
      if (directions.Contains(Direction.EAST) && directions.Contains(Direction.SOUTH)) return 'F';
      if (directions.Contains(Direction.NORTH) && directions.Contains(Direction.EAST)) return 'L';
      if (directions.Contains(Direction.NORTH) && directions.Contains(Direction.WEST)) return 'J';
      if (directions.Contains(Direction.WEST) && directions.Contains(Direction.SOUTH)) return '7';
      return '.';
    }

    private Direction GetOppositeDirection(Direction dir)
    {
      switch (dir)
      {
        case Direction.NORTH:
          return Direction.SOUTH;
        case Direction.SOUTH:
          return Direction.NORTH;
        case Direction.EAST:
          return Direction.WEST;
        default:
          return Direction.EAST;
      }
    }
  }
}


