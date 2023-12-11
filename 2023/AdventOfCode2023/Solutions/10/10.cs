using Tools;

namespace Solutions
{
  public class Day10
  {
    List<string> strings = new List<string>();
    List<List<char>> grid = new();
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
    }

    public int PartOne()
    {
      // Where is start?
      (int x, int y) start = FindStart(grid);
      // Follow that pipe
      FollowPipe(grid, (start.x, start.y), out int steps);
      return steps;
    }

    // Original Part 2 attempt. Doesn't work
    // Tries to expand the grid so I can flood fill it and see what's actually in the loop
    public int PartTwo()
    {
      // Make a bigger, expanded grid so flood can fill between pipes
      List<List<char>> expandedGrid = new();
      // First, add a row of just ground
      List<char> groundRow = new(Enumerable.Repeat('z', (grid.First().Count * 2) + 1));
      expandedGrid.Add(groundRow);
      foreach ((List<char> row, int x) in grid.WithIndex())
      {
        // Add a piece of ground at start
        List<char> expandedRow = new();

        // Add row, extending horizontal pipes
        foreach ((char pipe, int y) in row.WithIndex())
        {
          // Can we add a horizontal pipe?
          if (y > 0 && y < row.Count - 1 && (GetDirectionsFromPipe(grid[x][y - 1]).Contains(Direction.EAST) || grid[x][y - 1] == 'S') && (GetDirectionsFromPipe(grid[x][y]).Contains(Direction.WEST) || grid[x][y] == 'S'))
          {
            expandedRow.Add('-');
          }
          // Else, add an inserted value
          else
          {
            expandedRow.Add('z');
          }
          // Then add the real value
          expandedRow.Add(pipe);
        }
        // Add extra space on right side
        expandedRow.Add('z');
        // Add to grid
        expandedGrid.Add(expandedRow);

        // Add row extending vertical pipes
        List<char> verticalRow = new();
        // Can we add a vertical pipe?
        foreach ((char pipe, int y) in expandedRow.WithIndex())
        {
          if (GetDirectionsFromPipe(expandedGrid.Last()[y]).Contains(Direction.SOUTH) || expandedGrid.Last()[y] == 'S')
          {
            verticalRow.Add('|');
          }
          else
          {
            verticalRow.Add('z');
          }
        }

        // Add to grid
        expandedGrid.Add(verticalRow);
      }
      // Add row of ground at the bottom too
      expandedGrid.Add(groundRow);

      // Find the start of the expanded grid
      (int x, int y) start = FindStart(expandedGrid);

      // Follow this bigger pipe
      HashSet<(int x, int y)> pipeLocations = FollowPipe(expandedGrid, start, out int steps);

      // Flood the map from the outside corner. Only locations part of pipeLocations block the flood.
      // Add flooded locations to this Set:
      HashSet<(int x, int y)> floodedLocations = new();
      Flood(floodedLocations, (0, 0), expandedGrid, pipeLocations);

      // Go through all locations. If not part of the pipe loop, not flooded, and not added ('z'), then count it as within the pipe loop
      int locationsWithinPipe = GetInsidePipeCount(expandedGrid, floodedLocations, pipeLocations);
      return locationsWithinPipe;
    }

    // Second attempt at Part 2 after doing some reading.
    // Just uses any pipe that goes North to flip whether we are inside or outside of the loop.
    // Iterates through grid and counts inside locations
    public int PartTwov2()
    {
      // Where is start?
      (int x, int y) start = FindStart(grid);
      // Follow that pipe
      HashSet<(int x, int y)> pipeLocations = FollowPipe(grid, (start.x, start.y), out int steps);
      int count = 0;
      bool inside = false;
      foreach ((List<char> row, int x) in grid.WithIndex())
      {
        foreach ((char pipe, int y) in row.WithIndex())
        {
          if (pipeLocations.Contains((x, y)) && (GetDirectionsFromPipe(grid[x][y]).Contains(Direction.NORTH) || (grid[x][y] == 'S' && GetDirectionsFromPipe(GetPipeFromSurroundings(x, y, grid)).Contains(Direction.NORTH))))
          {
            inside = !inside;
          }
          if (inside && !pipeLocations.Contains((x, y)))
          {
            count++;
          }
        }
      }
      return count;
    }

    // Part of failed Part 2
    // Tries to count locations on a flooded grid that aren't pipe, flood, or inserted locations
    private int GetInsidePipeCount(List<List<char>> grid, HashSet<(int x, int y)> floodedLocations, HashSet<(int x, int y)> pipeLocations)
    {
      int count = 0;
      foreach ((List<char> row, int x) in grid.WithIndex())
      {
        foreach ((char pipe, int y) in row.WithIndex())
        {
          if (pipe != 'z' && !floodedLocations.Contains((x, y)) && !pipeLocations.Contains((x, y)))
          {
            count++;
          }
        }
      }
      return count;
    }

    // Part of failed Part 2
    // From the top left corner, tries to flood the map, adding those locations to floodedLocations
    private void Flood(HashSet<(int x, int y)> floodedLocations, (int x, int y) start, List<List<char>> grid, HashSet<(int x, int y)> pipeLocations)
    {
      Queue<(int x, int y)> floodQueue = new();
      floodQueue.Enqueue(start);
      floodedLocations.Add((start.x, start.y));

      // Use the queue to loop through new locations
      while (floodQueue.Count > 0)
      {
        (int x, int y) current = floodQueue.Dequeue();

        if (current.x > 0 && !floodedLocations.Contains((current.x - 1, current.y)) && !pipeLocations.Contains((current.x - 1, current.y)))
        {
          floodQueue.Enqueue((current.x - 1, current.y));
          floodedLocations.Add((current.x - 1, current.y));
        }

        if (current.x < grid.Count - 1 && !floodedLocations.Contains((current.x + 1, current.y)) && !pipeLocations.Contains((current.x + 1, current.y)))
        {
          floodQueue.Enqueue((current.x + 1, current.y));
          floodedLocations.Add((current.x + 1, current.y));
        }

        if (current.y > 0 && !floodedLocations.Contains((current.x, current.y - 1)) && !pipeLocations.Contains((current.x, current.y - 1)))
        {
          floodQueue.Enqueue((current.x, current.y - 1));
          floodedLocations.Add((current.x, current.y - 1));
        }

        if (current.y < grid.First().Count - 1 && !floodedLocations.Contains((current.x, current.y + 1)) && !pipeLocations.Contains((current.x, current.y + 1)))
        {
          floodQueue.Enqueue((current.x, current.y + 1));
          floodedLocations.Add((current.x, current.y + 1));
        }
      }
    }

    // Given a grid and a starting location, follows a pipe loop
    // Returns the HashSet, but also exposes the int steps
    private HashSet<(int x, int y)> FollowPipe(List<List<char>> grid, (int x, int y) start, out int steps)
    {
      // Set to track pipe locations
      HashSet<(int x, int y)> pipeLocations = new() { (start.x, start.y) };

      // What kind of pipe is start?
      startPipe = GetPipeFromSurroundings(start.x, start.y, grid);
      Direction[] directions = GetDirectionsFromPipe(startPipe);

      // Move the first steps
      (int x, int y) currentPipe1 = GetNextLocation(start.x, start.y, directions.First());
      (int x, int y) currentPipe2 = GetNextLocation(start.x, start.y, directions.Last());
      Direction dir1 = GetNextDirection(currentPipe1.x, currentPipe1.y, GetOppositeDirection(directions.First()), grid);
      Direction dir2 = GetNextDirection(currentPipe2.x, currentPipe2.y, GetOppositeDirection(directions.Last()), grid);

      steps = 1;

      // Continue moving steps until they are the same position
      while (currentPipe1 != currentPipe2)
      {
        // Add them to the set
        pipeLocations.Add((currentPipe1.x, currentPipe1.y));
        pipeLocations.Add((currentPipe2.x, currentPipe2.y));
        // Move pipe location 1
        currentPipe1 = GetNextLocation(currentPipe1.x, currentPipe1.y, dir1);
        // Determine next direction
        dir1 = GetNextDirection(currentPipe1.x, currentPipe1.y, GetOppositeDirection(dir1), grid);

        // Move pipe location 2
        currentPipe2 = GetNextLocation(currentPipe2.x, currentPipe2.y, dir2);
        // Determine next direction
        dir2 = GetNextDirection(currentPipe2.x, currentPipe2.y, GetOppositeDirection(dir2), grid);

        steps++;
      }
      pipeLocations.Add((currentPipe1.x, currentPipe1.y));

      return pipeLocations;
    }

    // Gets the next direction to go in after arriving at a pipe by removing the direction we entered from
    private Direction GetNextDirection(int x, int y, Direction from, List<List<char>> grid)
    {
      char pipe = grid[x][y];
      Direction[] pipeDirections = GetDirectionsFromPipe(pipe);
      // Remove direction that would go back to origin
      pipeDirections = pipeDirections.Where(direction => direction != from).ToArray();
      // Remaining direction is the only way to go
      Direction nextDirection = pipeDirections.First();
      return nextDirection;
    }

    // Returns coordinates based on current location and direction
    private (int, int) GetNextLocation(int currX, int currY, Direction dir)
    {
      // return the coordinates of that next location
      if (dir == Direction.NORTH) return (currX - 1, currY);
      if (dir == Direction.SOUTH) return (currX + 1, currY);
      if (dir == Direction.WEST) return (currX, currY - 1);
      return (currX, currY + 1);
    }

    // Finds the S on the grid
    private (int, int) FindStart(List<List<char>> grid)
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

    // Infers the type of pipe the S represents.
    // Technically works on any location, but S is the primary case
    private char GetPipeFromSurroundings(int x, int y, List<List<char>> grid)
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

    // Convert pipe char to directions
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

    // Convert directions to pipe char
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

    // Returns the opposite of a direction
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


