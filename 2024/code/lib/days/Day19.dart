import 'package:code/days/Day.dart';

class Day19 extends Day {
  Set<String> possiblePatterns = {};
  List<String> desiredPatterns = [];
  Day19(super.fileName, super.useTestData) {
    possiblePatterns.addAll(input[0].split(", "));
    desiredPatterns.addAll(input.sublist(2));
  }

  void part1() {
    int count = 0;
    for (String desiredPattern in desiredPatterns) {
      final result = isPatternPossible(desiredPattern);
      if (result.isPossible) {
        print(desiredPattern);
        possiblePatterns.addAll(result.newPatterns);
        count++;
      }
    }
    print(count);
  }

  ({bool isPossible, List<String> newPatterns}) isPatternPossible(
      String pattern) {
    String desiredPattern = pattern;
    // Start with the whole piece. Is that a valid pattern?
    int start = 0;
    int end = desiredPattern.length - 1;
    List<String> parts = [];
    while (end >= start) {
      print('$start, $end');
      String slice = desiredPattern.substring(start, end);
      while (!possiblePatterns.contains(slice) && end > start) {
        // If not, remove shrink it down until it is.
        end--;
        slice = desiredPattern.substring(start, end);
        if (slice.isEmpty) {
          return (isPossible: false, newPatterns: []);
        }
        print('slice: $slice');
      }

      // Otherwise, we assume we have a valid pattern
      parts.add(slice);
      desiredPattern = desiredPattern.substring(slice.length);
      print('desiredPatter: $desiredPattern');
      end = desiredPattern.length - 1;

      if (desiredPattern.length == 1) {
        if (possiblePatterns.contains(desiredPattern)) {
          parts.add(desiredPattern);
          return (isPossible: true, newPatterns: parts);
        } else {
          return (isPossible: false, newPatterns: []);
        }
      }
    }
    return (isPossible: false, newPatterns: []);
  }

  void part2() {}
}
