import 'dart:collection';

import 'package:code/days/Day.dart';

typedef Coordinate = ({int x, int y});

class SearchState {
  Coordinate location;
  int picoseconds = 0;
  Coordinate previous = (x: -1, y: -1);
  Set<Coordinate> visited = {};
  int remainingCheats = 0;
  SearchState(
      {required this.location,
      required this.picoseconds,
      Coordinate? previous,
      Set<Coordinate>? visited,
      int? remainingCheats})
      : previous = previous ?? (x: -1, y: -1),
        visited = visited ?? {},
        remainingCheats = remainingCheats ?? 0;
}

class Day20 extends Day {
  Set<Coordinate> walls = {};
  Coordinate start = (x: -1, y: -1);
  Coordinate end = (x: -1, y: -1);
  Day20(super.fileName, super.useTestData) {
    for (int x = 0; x < input.length; x++) {
      List<String> row = input[x].split("").toList();
      for (int y = 0; y < row.length; y++) {
        if (row[y] == "#")
          walls.add((x: x, y: y));
        else if (row[y] == "S")
          start = (x: x, y: y);
        else if (row[y] == "E") end = (x: x, y: y);
      }
    }
  }

  // Just wants how many patterns were possible
  void part1() {
    int picoseconds = 2;
    int cheats = 1;

    // First, find fastest path with no cheating
    Map<int, int> initialMap = getPathLength(start, end, 0);
    print(initialMap);
    int noCheatLength = initialMap.entries.first.key;

    Map<int, int> cheatMap = getPathLength(start, end, 1, noCheatCount: noCheatLength);
    print(cheatMap);
  }

  void part2() {}

  Map<int, int> getPathLength(Coordinate start, Coordinate end, int cheatCount,
      {int? noCheatCount}) {
    // Map to count how often cheats occur
    // Key is picoseconds saved, value is number of cheats that save that amount
    Map<int, int> cheatCounter = {};
    // queue for search and set to remember visited spaces
    Queue<SearchState> queue = Queue();
    // Add start and mark as visited
    queue.add(SearchState(
        location: start,
        picoseconds: 0,
        visited: {start},
        remainingCheats: cheatCount));

    while (queue.isNotEmpty) {
      SearchState current = queue.removeFirst();
      if (current.location == end) {
        cheatCounter.update(current.picoseconds, (value) => value + 1,
            ifAbsent: () => 1);
        continue;
      }
      // Gets valid neighbours, then filters by visited and obstacles
      List<Coordinate> neighbours =
          getNextNeighbours(current.location, input.length, current.previous)
              .where((n) => !current.visited.contains(n))
              .where((coord) => !walls.contains(coord))
              .toList();
      // Mark these as visited now to avoid returning to space (direction doesn't matter)
      queue.addAll(neighbours
          .map((n) => SearchState(
              location: n,
              picoseconds: current.picoseconds + 1,
              previous: current.location,
              visited: {...current.visited, n},
              remainingCheats: current.remainingCheats))
          .where((state) => noCheatCount != null
              ? noCheatCount - current.picoseconds + 1 > 100
              : true));
      // Also queue wall spaces if we have cheats left
      if (current.remainingCheats > 0) {
        List<Coordinate> neighbourWalls =
            getNextNeighbours(current.location, input.length, current.previous)
                .where((n) => !current.visited.contains(n))
                .where((coord) => walls.contains(coord))
                .toList();
        queue.addAll(neighbourWalls
            .map((n) => SearchState(
                location: n,
                picoseconds: current.picoseconds + 1,
                previous: current.location,
                visited: {...current.visited, n},
                remainingCheats: current.remainingCheats - 1))
            .where((state) => noCheatCount != null
                ? noCheatCount - current.picoseconds + 1 > 100
                : true));
      }
    }
    return cheatCounter;
  }

  List<Coordinate> getNextNeighbours(
      Coordinate start, int max, Coordinate? origin) {
    List<Coordinate> offsets = [
      (x: -1, y: 0),
      (x: 1, y: 0),
      (x: 0, y: -1),
      (x: 0, y: 1)
    ];
    List<Coordinate> neighbours = offsets
        .map((offset) => (x: start.x + offset.x, y: start.y + offset.y))
        .where((coord) =>
            coord.x >= 0 && coord.y >= 0 && coord.x <= max && coord.y <= max)
        .toList();

    if (origin != null) {
      return neighbours
          .where((coord) => !(coord.x == origin.x && coord.y == origin.y))
          .toList();
    }
    return neighbours;
  }
}
