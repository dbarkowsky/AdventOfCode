import 'package:code/days/Day.dart';

class Day10 extends Day {
  List<List<int>> map = [];
  Day10(super.fileName, super.useTestData) {
    map = input.map((row) => row.split("").map(int.parse).toList()).toList();
  }

  void part1() {
    List<(int, int)> zeros = findAllZeros(map);

    int count = 0;

    for (final zero in zeros) {
      Set<String> uniquePeaks = {};
      List<List<bool>> visited = [];
      for (int i = 0; i < map.length; i++) {
        List<bool> row = [];
        for (int j = 0; j < map[i].length; j++) {
          row.add(false);
        }
        visited.add(row);
      }
      List<(int, int)> peaks = countPathsFrom(zero, visited);
      for (final peak in peaks) {
        uniquePeaks.add(peak.toString());
      }
      count += uniquePeaks.length;
    }

    print(count);
  }

  void part2() {
    List<(int, int)> zeros = findAllZeros(map);

    int count = 0;

    for (final zero in zeros) {
      List<String> pathEnds = [];
      List<List<bool>> visited = [];
      for (int i = 0; i < map.length; i++) {
        List<bool> row = [];
        for (int j = 0; j < map[i].length; j++) {
          row.add(false);
        }
        visited.add(row);
      }
      List<(int, int)> peaks = countPathsFrom(zero, visited);
      for (final peak in peaks) {
        pathEnds.add(peak.toString());
      }
      count += pathEnds.length;
    }

    print(count);
  }

  List<(int, int)> countPathsFrom((int, int) start, List<List<bool>> visited,
      {int increment = 1, int endValue = 9, (int, int)? origin}) {
    if (map[start.$1][start.$2] == endValue) return [start];
    List<List<bool>> localVisited = List.from(visited);
    printVisited(localVisited);
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
      if (map[location.$1][location.$2] - map[start.$1][start.$2] != increment)
        continue;
      // If an origin was specified, it can't be from there.
      if (origin != null &&
          location.$1 == origin.$1 &&
          location.$2 == origin.$2) continue;

      // Otherwise, add
      nextLocations.add(location);
      // print('$start $location');
    }
    if (nextLocations.isEmpty) return [];
    List<(int, int)> foundPaths = [];
    for (final next in nextLocations) {
      foundPaths.addAll(countPathsFrom(next, localVisited, origin: start));
    }
    return foundPaths;
  }

  List<(int, int)> findAllZeros(List<List<int>> map) {
    List<(int, int)> zeros = [];
    for (int x = 0; x < map.length; x++) {
      for (int y = 0; y < map[x].length; y++) {
        if (map[x][y] == 0) zeros.add((x, y));
      }
    }
    return zeros;
  }

  void printVisited(List<List<bool>> visited){
    for (final row in visited){
      print(row.map((el) => el ? "X" : ".").join(""));
    }
    print("");
  }
}
