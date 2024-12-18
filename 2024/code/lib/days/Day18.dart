import 'dart:collection';
import 'package:code/days/Day.dart';

typedef Coordinate = ({int x, int y});

class SearchState {
  Coordinate location;
  int steps = 0;
  Coordinate previous = (x: -1, y: -1);
  SearchState(this.location, this.steps, this.previous);
}

class Day18 extends Day {
  int gridSize = 0;
  Day18(super.fileName, super.useTestData) {
    gridSize = useTestData ? 6 : 70;
  }

  Set<Coordinate> buildObstacles(int max) {
    Set<Coordinate> obstacles = {};
    obstacles.addAll(input
        .sublist(0, max)
        .map((row) => row.split(",").map(int.parse))
        .toList()
        .map((row) => (x: row.last, y: row.first)));
    return obstacles;
  }

  // Just getting shortest path
  void part1() {
    Set<Coordinate> obstacles = buildObstacles(useTestData ? 12 : 1024);
    print(bfs(obstacles));
  }

  // Testing when the path may be blocked by adding an obstacle
  // Uses brute force. Could be optimized
  void part2() {
    print("Part 2 can take a few seconds. Be patient.");
    // Start at last known good path
    int obstacleCount = useTestData ? 12 : 1024;
    Set<Coordinate> obstacles = buildObstacles(obstacleCount);
    // Until it can't reach the end, try again with more obstacles
    while (bfs(obstacles) != -1) {
      obstacleCount++;
      obstacles = buildObstacles(obstacleCount);
    }
    print(input[obstacleCount - 1]); // -1 because index vs count
  }

  void printGrid(Set<Coordinate> visited, Set<Coordinate> obstacles) {
    for (int x = 0; x <= gridSize; x++) {
      List<String> row = [];
      for (int y = 0; y <= gridSize; y++) {
        if (visited.contains((x: x, y: y))) {
          row.add("O");
        } else if (obstacles.contains((x: x, y: y))) {
          row.add("X");
        } else {
          row.add(".");
        }
      }
      print(row.join(""));
    }
    print("");
  }

  // breadth-first search
  int bfs(Set<Coordinate> obstacles) {
    Coordinate start = (x: 0, y: 0);
    Coordinate end = (x: gridSize, y: gridSize);
    // queue for search and set to remember visited spaces
    Queue<SearchState> queue = Queue();
    Set<Coordinate> visited = {};
    // Add start and mark as visited
    queue.add(SearchState(start, 0, (x: -1, y: -1)));
    visited.add(start);

    while (queue.isNotEmpty) {
      SearchState current = queue.removeFirst();
      if (current.location == end) {
        return current.steps;
      }
      // Gets valid neighbours, then filters by visited and obstacles
      List<Coordinate> neighbours =
          getNextNeighbours(current.location, gridSize, current.previous)
              .where((n) => !visited.contains(n))
              .where((coord) => !obstacles.contains(coord))
              .toList();
      // Mark these as visited now to avoid returning to space (direction doesn't matter)
      visited.addAll(neighbours);
      queue.addAll(neighbours
          .map((n) => SearchState(n, current.steps + 1, current.location)));
    }
    // No path?
    return -1;
  }

  List<Coordinate> getNextNeighbours(
      Coordinate start, int max, Coordinate origin) {
    List<Coordinate> offsets = [
      (x: -1, y: 0),
      (x: 1, y: 0),
      (x: 0, y: -1),
      (x: 0, y: 1)
    ];
    return offsets
        .map((offset) => (x: start.x + offset.x, y: start.y + offset.y))
        .where((coord) =>
            coord.x >= 0 && coord.y >= 0 && coord.x <= max && coord.y <= max)
        .where((coord) => !(coord.x == origin.x && coord.y == origin.y))
        .toList();
  }
}
