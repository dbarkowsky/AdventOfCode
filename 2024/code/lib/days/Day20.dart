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
  Map<Coordinate, int> picosecondsFromStart = {};

  Day20(super.fileName, super.useTestData) {
    // Initialize the grid by saving wall locations
    for (int x = 0; x < input.length; x++) {
      List<String> row = input[x].split("").toList();
      for (int y = 0; y < row.length; y++) {
        if (row[y] == "#") {
          walls.add((x: x, y: y));
        } else if (row[y] == "S") {
          start = (x: x, y: y);
        } else if (row[y] == "E") {
          end = (x: x, y: y);
        }
      }
    }
    // Identify seconds from the starting location for each space
    picosecondsFromStart = getPathLengths(start, 1);
  }

  // Just wants how many patterns were possible
  void part1() {
    print(countAcceptableCheats(useTestData ? 1 : 100, 2));
  }

  // Also wants possible patterns, but with bigger cheat distance
  void part2() {
    print(countAcceptableCheats(useTestData ? 50 : 100, 20));
  }

  // Counts cheat instances that result in the minimum savings required given an allowed cheat distance
  // Feels brute-force-like. Would like a more efficient way to do it
  int countAcceptableCheats(int minimumSavings, int cheatDistance) {
    int acceptableCheatCount = 0;
    // Going to look at every pair of non-wall nodes
    for (MapEntry<({int x, int y}), int> pairA
        in picosecondsFromStart.entries) {
      for (MapEntry<({int x, int y}), int> pairB
          in picosecondsFromStart.entries) {
        // If they are the same location, don't bother
        if (pairA == pairB) continue;
        // If the distance is greater than we can cheat or if the node is adjacent, don't bother
        int distance = distanceBetween(pairA.key, pairB.key);
        if (distance > cheatDistance || distance == 1) continue;
        // If we saved an acceptable amount of time, count it
        int timeSaved = (pairA.value - pairB.value) - distance;
        if (timeSaved >= minimumSavings) {
          acceptableCheatCount++;
        }
      }
    }
    return acceptableCheatCount;
  }

  // I way overcomplicated this originally thinking part 2 would ask for bigger cheat allowances, but I thought you could break up the cheat.
  // I've taken some of that overcomplication out, but the memory remains.
  Map<Coordinate, int> getPathLengths(Coordinate start, int neighbourDistance) {
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
      // Track how long it took to get here
      secondsToCoord[current.location] = current.picoseconds;

      // Gets valid neighbours, then filters by visited and walls
      // The neighbours could be far away depending on neighbour distance and cheat status
      List<Coordinate> neighbours = getNextNeighbours(
              current.location, input.length, current.visited,
              withinDistance: current.hasCheated ? 1 : neighbourDistance)
          .where((coord) => !walls.contains(coord))
          .toList();
      // Add them to the queue, marking as visited, adding time, and detecting cheats
      queue.addAll(neighbours.map((n) => SearchState(
          location: n,
          picoseconds: current.picoseconds + 1,
          previous: current.location,
          visited: {...current.visited, n},
          hasCheated: current.hasCheated
              ? true
              : distanceBetween(current.location, n) > 1)));
    }
    return secondsToCoord;
  }

  // Manhattan distance function
  int distanceBetween(Coordinate a, Coordinate b) {
    return (a.x - b.x).abs() + (a.y - b.y).abs();
  }

  // A more complicated version of the getNeighbours functions I have been using in past days
  // This one includes all neighbours up to a certain distance, and takes a Set of excluded coordinates
  List<Coordinate> getNextNeighbours(
      Coordinate start, int max, Set<Coordinate> exclude,
      {int withinDistance = 1}) {
    List<Coordinate> offsets = [];
    for (int x = -withinDistance; x <= withinDistance; x++) {
      for (int y = -withinDistance; y <= withinDistance; y++) {
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
