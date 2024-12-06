import 'package:code/days/Day.dart';

enum Direction { Up, Down, Left, Right }

// Handle operations with positions
class Position {
  int x;
  int y;
  Position(this.x, this.y);

  update(int x, int y) {
    this.x = x;
    this.y = y;
  }

  @override
  String toString() {
    return '$x,$y';
  }

  @override
  bool operator ==(covariant Position other) {
    return x == other.x && y == other.y;
  }
}

// Ended up using this to bundle the position and direction together.
class PositionDirection {
  Position pos;
  Direction dir;
  PositionDirection(this.pos, this.dir);

  @override
  bool operator ==(covariant PositionDirection other) {
    return other.pos == pos && other.dir == dir;
  }
}

class Day06 extends Day {
  Position guardStart = Position(0, 0);

  Day06(super.fileName, super.useTestData);

  List<List<String>> buildGrid() {
    List<List<String>> grid = [];

    for (int x = 0; x < input.length; x++) {
      List<String> splitLine = input[x].split("");
      grid.add(splitLine);
      int y = splitLine.indexOf("^");
      if (y != -1) {
        guardStart.update(x, y);
      }
    }
    return grid;
  }

  void part1() {
    List<List<String>> grid = buildGrid();
    // Let guard walk its course
    var (visited, _) = walk(guardStart, Direction.Up, grid);
    // This is the end. Count spaces guard has travelled to.
    print(countUniqueVisited(visited));
  }

  int countUniqueVisited(List<PositionDirection> visited) {
    Set<String> positions = {};
    for (final entry in visited) {
      positions.add(entry.pos.toString());
    }
    return positions.length;
  }

  // Guard walks his path, based on starting point, starting direction, and a grid with obstacles
  (List<PositionDirection>, bool) walk(
      Position guardStart, Direction directionStart, List<List<String>> grid) {
    Direction guardFacing = directionStart;
    Position guard = guardStart;

    // Track where the guard has been, and what direction he was travelling
    List<PositionDirection> visited = [];
    visited.add(PositionDirection(guard, guardFacing));
    // Get next possible position of guard
    Position potentialNextPosition = getNextPosition(guardFacing, guard);
    // Go until he leaves the grid
    while (positionIsOnGrid(potentialNextPosition, grid)) {
      // But if we've visited this spot in this direction, we're in a loop
      if (visited
          .contains(PositionDirection(potentialNextPosition, guardFacing))) {
        return (visited, true);
      }

      if (grid[potentialNextPosition.x][potentialNextPosition.y] == "#") {
        // Hit an obstacle, turn right
        guardFacing = getRightTurn(guardFacing);
        // Update potential position
        potentialNextPosition = getNextPosition(guardFacing, guard);
      } else {
        // No issues with obstacles, update guard's position
        guard = potentialNextPosition;
        visited.add(PositionDirection(guard, guardFacing));
        potentialNextPosition = getNextPosition(guardFacing, guard);
      }
    }
    return (visited, false);
  }

  bool positionIsOnGrid(Position pos, List<List<String>> grid) {
    if (pos.x < 0) return false;
    if (pos.x >= grid.length) return false;
    if (pos.y < 0) return false;
    if (pos.y >= grid[0].length) return false;

    return true;
  }

  Direction getRightTurn(Direction current) {
    switch (current) {
      case Direction.Up:
        return Direction.Right;
      case Direction.Down:
        return Direction.Left;
      case Direction.Left:
        return Direction.Up;
      case Direction.Right:
        return Direction.Down;
      default:
        return current;
    }
  }

  Position getNextPosition(Direction guardFacing, Position guard) {
    switch (guardFacing) {
      case Direction.Up:
        return Position(guard.x - 1, guard.y);
      case Direction.Down:
        return Position(guard.x + 1, guard.y);
      case Direction.Left:
        return Position(guard.x, guard.y - 1);
      case Direction.Right:
        return Position(guard.x, guard.y + 1);
      default:
        return guard;
    }
  }

  void printGrid(List<List<String>> grid) {
    for (final row in grid) {
      print(row.join(""));
    }
  }

  // Need to find all the possible locations where obstructions (rock?) could go.
  // This approach is brute force. Can still be optimized.
  void part2() {
    print("Part 2 takes some time. Be patient.");
    List<List<String>> grid = buildGrid();
    Set<Position> possibleRocks = {};
    var (visited, _) = walk(guardStart, Direction.Up, grid);

    // For each position visited, would placing a rock there instead cause a loop?
    for (int i = 0; i < visited.length; i++) {
      // Place a rock on copied grid
      Position rockLocation = visited[i].pos;
      // Is this rock on a location we would have to visit to get here? Then abandon
      List<Position> visitedToHere =
          visited.sublist(0, i).map((el) => el.pos).toList();
      if (visitedToHere.contains(rockLocation)) continue;
      List<List<String>> copiedGrid = buildGrid();
      if (positionIsOnGrid(rockLocation, copiedGrid)) {
        copiedGrid[rockLocation.x][rockLocation.y] = "#";
        // With rock on temp grid, walk it. See if a loop exists.
        if (walk(guardStart, Direction.Up, copiedGrid).$2) {
          possibleRocks.add(rockLocation);
        }
      }
    }

    print(possibleRocks.length);
  }
}
