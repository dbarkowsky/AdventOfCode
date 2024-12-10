import 'package:code/days/Day.dart';

class Day10 extends Day {
  List<List<int>> map = [];
  Day10(super.fileName, super.useTestData) {
    map = input.map((row) => row.split("").map(int.parse).toList()).toList();
  }

  // Counting unique endpoints for each trailhead
  void part1() {
    List<(int, int)> zeros = findAllZeros(map);

    int count = 0;

    for (final zero in zeros) {
      // Using Set so that each peak is only counted once, even if multiple paths
      Set<String> uniquePeaks = {};
      List<List<bool>> visited = makeVisited();
      List<(int, int)> peaks = countPathsFrom(zero, visited);
      for (final peak in peaks) {
        uniquePeaks.add(peak.toString());
      }
      count += uniquePeaks.length;
    }

    print(count);
  }

  // Counting all possible routes from trailhead (0) to end (9)
  void part2() {
    List<(int, int)> zeros = findAllZeros(map);

    int count = 0;

    for (final zero in zeros) {
      List<String> pathEnds =
          []; // This is the key difference. We count all possible routes
      List<List<bool>> visited = makeVisited();
      List<(int, int)> peaks = countPathsFrom(zero, visited);
      for (final peak in peaks) {
        pathEnds.add(peak.toString());
      }
      count += pathEnds.length;
    }

    print(count);
  }

  // Makes grid of visited locations
  List<List<bool>> makeVisited() {
    List<List<bool>> visited = [];
    for (int i = 0; i < map.length; i++) {
      List<bool> row = [];
      for (int j = 0; j < map[i].length; j++) {
        row.add(false);
      }
      visited.add(row);
    }
    return visited;
  }

  // Recursive function to do a Breadth-first search
  List<(int, int)> countPathsFrom((int, int) start, List<List<bool>> visited,
      {int increment = 1, int endValue = 9, (int, int)? origin}) {
    // Need to copy list like this to make actual deep copy
    List<List<bool>> localVisited = List.from(visited.map((row) => [...row]));
    // If it's the end value, we're found what we're looking for.
    if (map[start.$1][start.$2] == endValue) return [start];
    // Mark as visited
    localVisited[start.$1][start.$2] = true;
    List<(int, int)> nextLocations = [];
    // Add locations on all sides
    List<(int, int)> offsets = [(0, 1), (0, -1), (1, 0), (-1, 0)];
    for (final offset in offsets) {
      (int, int) location = (start.$1 + offset.$1, start.$2 + offset.$2);
      // It must be on the map
      if (location.$1 < 0 ||
          location.$2 < 0 ||
          location.$1 >= map.length ||
          location.$2 >= map[0].length) continue;
      // It can't have been visited
      if (localVisited[location.$1][location.$2]) continue;
      // It can't be a different difference than the increment
      if (map[location.$1][location.$2] - map[start.$1][start.$2] !=
          increment) {
        continue;
      }
      // Otherwise, add
      nextLocations.add(location);
    }
    if (nextLocations.isEmpty) return [];
    // Concatenate all found paths together
    List<(int, int)> foundPaths = [];
    for (final next in nextLocations) {
      foundPaths.addAll(countPathsFrom(next, localVisited, origin: start));
    }
    return foundPaths;
  }

  // Find and list all coodinates with 0
  List<(int, int)> findAllZeros(List<List<int>> map) {
    List<(int, int)> zeros = [];
    for (int x = 0; x < map.length; x++) {
      for (int y = 0; y < map[x].length; y++) {
        if (map[x][y] == 0) zeros.add((x, y));
      }
    }
    return zeros;
  }

  // For testing purposes
  void printVisited(List<List<bool>> visited) {
    for (final row in visited) {
      print(row.map((el) => el ? "X" : ".").join(""));
    }
    print("");
  }
}
