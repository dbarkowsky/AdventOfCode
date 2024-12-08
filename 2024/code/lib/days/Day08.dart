import 'package:code/days/Day.dart';

class Day08 extends Day {
  Day08(super.fileName, super.useTestData);

  // Finding antinodes when they don't repeat
  void part1() {
    print(countAntinodes());
  }

  // Finding them when they do repeat
  void part2() {
    print(countAntinodes(repeating: true));
  }

  // Counts the antinodes on the input
  int countAntinodes({bool repeating = false}){
    // Make a map of antenna types and their locations
    Map<String, List<(int, int)>> antennas = buildAntennaMap();
    // Recording unique antinode locations
    Set<String> uniqueAntinodes = {};

    for (final entry in antennas.entries) {
      // Get all the possible antenna pairs
      List<List<(int, int)>> pairCombos = getPairsCombos(entry.value);
      for (final pair in pairCombos) {
        // For each pair, get all the possible antinode points
        List<(int, int)> possibleAntinodes = getPossibleAntinodePointsForPair(
            pair.first, pair.last,
            maxX: input.length, maxY: input[0].length, repeating: repeating);
        // And add them to the Set
        uniqueAntinodes
            .addAll(possibleAntinodes.map((el) => "${el.$1},${el.$2}"));
      }
    }
    return(uniqueAntinodes.length);
  }

  // Gets all possible antinode points given two antenna locations
  List<(int, int)> getPossibleAntinodePointsForPair((int, int) a, (int, int) b,
      {int maxX = 12, int maxY = 12, bool repeating = false}) {
    List<(int, int)> possiblePoints = [];
    (int, int) offset = (a.$1 - b.$1, a.$2 - b.$2);

    (int, int) point1 = (a.$1 + offset.$1, a.$2 + offset.$2);
    (int, int) point2 = (b.$1 - offset.$1, b.$2 - offset.$2);

    // If not repeating, it will only find the first one (not including antenna itself)
    if (repeating) {
      // Include the antennas and outside antinodes (but not those between antennas)
      possiblePoints.addAll([a, b]);      
      // As long as one is within bounds, we keep goind
      while (isWithinBounds(point1, maxX, maxY) || isWithinBounds(point2, maxX, maxY)){
        possiblePoints.addAll(
          [point1, point2]);
        point1 = (point1.$1 + offset.$1, point1.$2 + offset.$2);
        point2 = (point2.$1 - offset.$1, point2.$2 - offset.$2);
      }
    } else {
      // Non-repeating option
      possiblePoints.addAll(
          [point1, point2]);
    }
    // filter out those not on the board
    return possiblePoints.where((el) => isWithinBounds(el, maxX, maxY)).toList();
  }

  bool isWithinBounds((int, int) location, int maxX, int maxY) =>
      location.$1 >= 0 &&
      location.$1 < maxX &&
      location.$2 >= 0 &&
      location.$2 < maxY;

  // function to create map of all frequencies of antennas with their locations
  Map<String, List<(int, int)>> buildAntennaMap() {
    Map<String, List<(int, int)>> antennas = {};
    for (int i = 0; i < input.length; i++) {
      List<String> row = input[i].split("");
      for (int j = 0; j < row.length; j++) {
        if (row[j] == ".") continue; // Don't care about empty spaces
        antennas.update(row[j], (el) => [...el, (i, j)],
            ifAbsent: () => [(i, j)]);
      }
    }
    return antennas;
  }

  // function to get all possible pairs of antennas with matching frequencies
  List<List<(int, int)>> getPairsCombos(List<(int, int)> locations) {
    List<List<(int, int)>> combos = [];
    for (int i = 0; i < locations.length - 1; i++) {
      for (int j = 1; j < locations.length; j++) {
        // Can't pair with itself
        if (locations[i].$1 == locations[j].$1 &&
            locations[i].$2 == locations[j].$2) continue;
        // xy pair is the same as yx pair, so don't add
        // Doing a weird thing here where we try to get the reverse. Continue if we find, but add if we don't.
        try {
          // .firstWhere throws an error if not found
          combos.firstWhere((List<(int, int)> el) =>
              el.first.$1 == locations[j].$1 &&
              el.first.$2 == locations[j].$2 &&
              el.last.$1 == locations[i].$1 &&
              el.last.$2 == locations[i].$2);
          continue;
        } catch (e) {
          combos.add([
            (locations[i].$1, locations[i].$2),
            (locations[j].$1, locations[j].$2)
          ]);
        }
      }
    }
    return combos;
  }
}
