import 'dart:collection';
import 'dart:core';
import 'package:code/days/Day.dart';

typedef Coordinate = ({int x, int y});

class SearchState {
  Coordinate location;
  int picoseconds = 0;
  Coordinate previous = (x: -1, y: -1);
  Set<Coordinate> visited = {};
  bool hasCheated = false;
  SearchState(
      {required this.location,
      required this.picoseconds,
      Coordinate? previous,
      Set<Coordinate>? visited,
      bool? hasCheated})
      : previous = previous ?? (x: -1, y: -1),
        visited = visited ?? {},
        hasCheated = hasCheated ?? false;
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
    int acceptableCheatCount = 0;
    int acceptableSavings = useTestData ? 1: 100;
    int cheatDistance = 2;
    // First, find fastest path with no cheating
    (Map<Coordinate, int>, Map<int, int>) startRun = getPathLengths(start, 1, true);
    // (Map<Coordinate, int>, Map<int, int>) endRun = getPathLengths(end, 1, true);
    // int noCheatLength = startRun.$1[end]!;
    // print(noCheatLength);
    for (MapEntry<({int x, int y}), int> pairA in startRun.$1.entries){
      for (MapEntry<({int x, int y}), int> pairB in startRun.$1.entries){
        if (pairA == pairB) continue;
        int distance = distanceBetween(pairA.key, pairB.key);
        if (distance > cheatDistance) continue;
        int timeSaved = (pairA.value - pairB.value) - distance;
        if (timeSaved >= acceptableSavings){
          // print('${pairA.key} - ${pairB.key}: $timeSaved');
          acceptableCheatCount++;
        }
    }
    }
    print(acceptableCheatCount);
  }

  void part2() {
       int acceptableCheatCount = 0;
    int acceptableSavings = useTestData ? 50: 100;
    int cheatDistance = 20;
    // First, find fastest path with no cheating
    (Map<Coordinate, int>, Map<int, int>) startRun = getPathLengths(start, 1, true);
    // (Map<Coordinate, int>, Map<int, int>) endRun = getPathLengths(end, 1, true);
    // int noCheatLength = startRun.$1[end]!;
    // print(noCheatLength);
    for (MapEntry<({int x, int y}), int> pairA in startRun.$1.entries){
      for (MapEntry<({int x, int y}), int> pairB in startRun.$1.entries){
        if (pairA == pairB) continue;
        int distance = distanceBetween(pairA.key, pairB.key);
        if (distance > cheatDistance) continue;
        int timeSaved = (pairA.value - pairB.value) - distance;
        if (timeSaved >= acceptableSavings){
          // print('${pairA.key} - ${pairB.key}: $timeSaved');
          acceptableCheatCount++;
        }
    }
    }
    print(acceptableCheatCount);
  }

  (Map<Coordinate, int>, Map<int, int>) getPathLengths(Coordinate start, int neighbourDistance, bool initialRun) {
    // Map to count how often cheats occur
    // Key is picoseconds saved, value is number of cheats that save that amount
    Map<int, int> secondsSaved = {};
    // Map to track number of picoseconds to normally get to each space
    // Used to see if it's worth going to this one (saves time)
    Map<Coordinate, int> secondsToCoord = {};
    // queue for search and set to remember visited spaces
    Queue<SearchState> queue = Queue();
    // Add start and mark as visited
    queue.add(SearchState(
        location: start, picoseconds: 0, visited: {start}, hasCheated: false));

    while (queue.isNotEmpty) {
      SearchState current = queue.removeFirst();
      if (initialRun) secondsToCoord[current.location] = current.picoseconds;

      // Gets valid neighbours, then filters by visited and walls
      // The neighbours could be far away depending on neighbour distance and cheat status
      List<Coordinate> neighbours = getNextNeighbours(
              current.location, input.length, current.visited,
              withinDistance: current.hasCheated ? 1 : neighbourDistance)
          .where((coord) => !walls.contains(coord))
          .toList();
      // Mark these as visited now to avoid returning to space (direction doesn't matter)
      queue.addAll(neighbours.map((n) => SearchState(
          location: n,
          picoseconds: current.picoseconds + 1,
          previous: current.location,
          visited: {...current.visited, n},
          hasCheated: current.hasCheated
              ? true
              : distanceBetween(current.location, n) > 1)));
    }
    return (secondsToCoord, secondsSaved);
  }

  int distanceBetween(Coordinate a, Coordinate b) {
    return (a.x - b.x).abs() + (a.y - b.y).abs();
  }

  List<Coordinate> getNextNeighbours(
      Coordinate start, int max, Set<Coordinate> exclude,
      {int withinDistance = 1}) {
    List<Coordinate> offsets = [];
    for (int x = -withinDistance; x <= withinDistance; x++) {
      for (int y = -withinDistance; y <= withinDistance; y++){
        if (x.abs() + y.abs() <= withinDistance) offsets.add((x: x, y: y));
      }
    }
    return offsets
        .map((offset) => (x: start.x + offset.x, y: start.y + offset.y))
        .where((coord) =>
            coord.x >= 0 && coord.y >= 0 && coord.x <= max && coord.y <= max)
        .where((coord) => !exclude.contains(coord))
        .toList();
  }
}
