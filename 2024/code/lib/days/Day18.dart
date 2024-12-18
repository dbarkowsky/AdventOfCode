import 'dart:collection';
import 'dart:math';
import 'package:code/days/Day.dart';
import 'package:code/days/Day13.dart';
import 'package:code/days/Day15.dart';

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

  void part1() {
    Set<Coordinate> obstacles = buildObstacles(useTestData ? 12 : 1024);
    Coordinate start = (x: 0, y: 0);
    Coordinate end = (x: gridSize, y: gridSize);
    Queue<SearchState> queue = Queue();
    Set<Coordinate> visited = {};
    queue.add(SearchState(start, 0, (x: -1, y: -1)));
    printGrid(visited, obstacles);
    while (queue.isNotEmpty) {
      SearchState current = queue.removeFirst();
      visited.add(current.location);
      printGrid(visited, obstacles);
      if (current.location == end) {
        print(current.steps);
        printGrid(visited, obstacles);
        break;
      }
      List<Coordinate> neighbours =
          getNextNeighbours(current.location, gridSize, current.previous)
              .where((n) => !visited.contains(n))
              .where((coord) => !obstacles.contains(coord))
              .toList();
      // print(neighbours);
      queue.addAll(neighbours
          .map((n) => SearchState(n, current.steps + 1, current.location)));
    }
  }

  void part2() {}

    void printGrid(Set<Coordinate> visited, Set<Coordinate> obstacles){
    for(int x = 0; x <= gridSize; x++){
      List<String> row = [];
      for(int y = 0; y <= gridSize; y++){
        if (visited.contains((x: x, y: y))){
          row.add("O");
        }else if (obstacles.contains((x: x, y: y))){
          row.add("X");
        } else {
          row.add(".");
        }
      }
      print(row.join(""));
    }
          print("");
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
