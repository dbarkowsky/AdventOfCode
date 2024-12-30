import 'dart:collection';
import 'dart:core';
import 'dart:math';
import 'package:code/days/Day.dart';

// My good friend, Coordinate, back again.
typedef Coordinate = ({int x, int y});

class SearchState {
  Coordinate location;
  List<Coordinate> visited = [];
  SearchState(this.location, this.visited);
}

class Day21 extends Day {
  Day21(super.fileName, super.useTestData);

  // Some hardcoded coordinates for the two different pads
  Map<String, Coordinate> numPad = {
    '7': (x: 0, y: 0),
    '8': (x: 0, y: 1),
    '9': (x: 0, y: 2),
    '4': (x: 1, y: 0),
    '5': (x: 1, y: 1),
    '6': (x: 1, y: 2),
    '1': (x: 2, y: 0),
    '2': (x: 2, y: 1),
    '3': (x: 2, y: 2),
    '0': (x: 3, y: 1),
    'A': (x: 3, y: 2),
  };

  Map<String, Coordinate> dirPad = {
    '^': (x: 0, y: 1),
    'A': (x: 0, y: 2),
    '<': (x: 1, y: 0),
    'v': (x: 1, y: 1),
    '>': (x: 1, y: 2),
  };

  // This cache remembers what the final length of an input will be
  // key is "directions,depth"  e.g. "<<A,3"
  Map<String, int> cache = {};
  void part1() {
    print(solve(2));
  }

  void part2() {
    print(solve(25));
  }

  int solve(int steps) {
    // Reset cache, just in case
    cache = {};
    int total = 0;
    // For each input code
    for (final code in input) {
      // We always start at A
      String start = 'A';
      int directionLength = 0;
      // Progress along the code
      for (int i = 0; i < code.length; i++) {
        String end = code[i];
        // Get initial inputs for possible keypad presses
        List<List<Coordinate>> paths = getShortestPaths(start, end, numPad);
        // Convert to direction strings
        List<List<String>> directions = paths
            .map(convertPathToInstructions)
            .where(hasNoInefficiencies) // Removes things like <,^,<
            .toList();
        // Use converter to get smallest amount of steps at x depth of robots
        // Have to check all possible outcomes, because it's not always clear which one is best at any given depth
        directionLength +=
            directions.map((d) => layerRobots(steps, d.join(""))).reduce(min);
        // Move to next symbol
        start = code[i];
      }
      // Get complexity score
      RegExp number = RegExp(r"[\d]+");
      int numericPart = int.parse(number.firstMatch(code)!.group(0)!);
      total += directionLength * numericPart;
    }
    return total;
  }

  // Checks if a direction list has things like <,^.< (bad)
  // Should only keep things like <,<,^ instead (good)
  bool hasNoInefficiencies(List<String> directions) {
    if (directions.length < 4) return true;
    if (directions[0] == directions[2] && directions[0] != directions[1]) {
      return false;
    }
    return true;
  }

  // Another cache that tracks start and end of the direction pad
  // Returns the converted path possibilities
  Map<String, List<List<String>>> dirCache = {};

  // Recursive function that finds the length of the final command needed to
  // command x number of robot layers
  int layerRobots(int remainingLayers, String directions) {
    if (directions.isEmpty) return 0;
    if (remainingLayers == 0) {
      return directions.length;
    }
    // key is for the cache
    String key = '$directions,$remainingLayers';
    // Have we seen this one before at this depth?
    if (cache.containsKey(key)) {
      return cache[key]!;
    } else {
      // This is a newly encountered iteration, continue to get final length
      // Split this instruction into pieces e.g. <<A^^A => [<<A, ^^A]
      List<String> dividedDirections =
          directions.split("A").map((e) => '${e}A').toList();
      List<String> trimmedDirections =
          dividedDirections.sublist(0, dividedDirections.length - 1);

      // For each of these direction pieces
      int totalLength = 0;
      for (final direction in trimmedDirections) {
        // Move through the direction, get paths, and repeat
        List<String> splitDirection = direction.split("");
        String start = 'A';
        for (int i = 0; i < splitDirection.length; i++) {
          String end = splitDirection[i];
          String key = '$start,$end';
          // Get initial inputs for possible keypad presses
          // Convert to direction strings
          List<List<String>> newDirections = dirCache.containsKey(key) ? dirCache[key]! : getShortestPaths(start, end, dirPad)
              .map(convertPathToInstructions)
              .where(hasNoInefficiencies)
              .toList();

          // Add to cache if not already there
          if (!dirCache.containsKey(key)) dirCache[key] = newDirections;
          // For each of these new directions possibilities, we have to check the outcome
          // But we only care about the smallest
          int minLength = newDirections
              .map((d) => layerRobots(remainingLayers - 1, d.join("")))
              .reduce(min);
          totalLength += minLength;
          start = direction[i];
        }
      }
      // Make sure we save this result before returning it
      cache[key] = totalLength;
      return totalLength;
    }
  }

  // Takes a coordinate path and converts to a list of string directions
  List<String> convertPathToInstructions(List<Coordinate> path) {
    List<String> instructions = [];
    for (int i = 1; i < path.length; i++) {
      Coordinate a = path[i - 1];
      Coordinate b = path[i];

      if (b.x > a.x) {
        // moving down
        instructions.add("v");
      } else if (a.x > b.x) {
        // moving up
        instructions.add("^");
      } else if (b.y > a.y) {
        // moving right
        instructions.add(">");
      } else if (a.y > b.y) {
        // moving left
        instructions.add("<");
      }
    }
    instructions.add('A');
    return instructions;
  }

  // An additional cache so we don't have to continue to calculate shortest paths
  Map<String, List<List<Coordinate>>> pathCache = {};
  // Gets the shortest path between two pad locations
  // Basically a BFS
  List<List<Coordinate>> getShortestPaths(
      String from, String to, Map<String, Coordinate> pad) {
    String padName = pad == numPad ? "num" : "dir";
    String key = '$from,$to,$padName';
    if (pathCache.containsKey(key)){
      return pathCache[key]!;
    }
    List<List<Coordinate>> paths = [];
    Coordinate start = pad[from]!;
    Coordinate end = pad[to]!;
    Queue<SearchState> queue = Queue();
    queue.add(SearchState(start, [start]));
    while (queue.isNotEmpty) {
      SearchState current = queue.removeFirst();
      if (current.location == end) {
        paths.add(current.visited);
        continue;
      }

      List<Coordinate> neighbours =
          getNextNeighbours(current.location, 3, current.visited)
              .where((n) => pad.containsValue(n))
              .toList();
      queue.addAll(
          neighbours.map((n) => SearchState(n, [...current.visited, n])));
    }
    int min = paths.first.length;
    for (final path in paths) {
      if (path.length < min) {
        min = path.length;
      }
    }
    List<List<Coordinate>> returnPaths = paths.where((p) => p.length == min).toList();
    pathCache[key] = returnPaths;
    return returnPaths;
  }

  // Borrowing from earlier day.
  // Gets neighbouring coordinates based on params
  List<Coordinate> getNextNeighbours(
      Coordinate start, int max, List<Coordinate> exclude,
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
