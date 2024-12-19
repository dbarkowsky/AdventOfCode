import 'package:code/days/Day.dart';

class Day19 extends Day {
  Map<String, int> patternCount = {};
  List<String> existingPatterns = [];
  List<String> desiredPatterns = [];
  Day19(super.fileName, super.useTestData) {
    existingPatterns = input[0].split(", ");
    for (final pattern in existingPatterns) {
      patternCount[pattern] = 1;
    }
    desiredPatterns.addAll(input.sublist(2));
  }

  void part1() {
    int count = 0;
    for (String desiredPattern in desiredPatterns) {
      print(desiredPattern);
      if (getValueFromCache(desiredPattern) != 0) {
        count++;
      }
    }
    print(count);
  }

  void part2() {
    List<int> counts = desiredPatterns.map(getValueFromCache).toList();
    print(patternCount);
    print(counts.reduce((a, b) => a + b));
  }

  int countValidPatternsWithin(String desiredPattern) {
    if (desiredPattern.isEmpty) return 1;
    Set<String> choppedLeftovers = <String>{
      ...existingPatterns
          .where(
              (existingPattern) => desiredPattern.startsWith(existingPattern))
          .map((existingPattern) =>
              desiredPattern.substring(existingPattern.length)),
      ...existingPatterns
          .where((existingPattern) => desiredPattern.endsWith(existingPattern))
          .map((existingPattern) => desiredPattern.substring(
              0, desiredPattern.length - existingPattern.length - 1))
    };

    List<int> values = choppedLeftovers
        .toList()
        .map((choppedLeftover) => getValueFromCache(choppedLeftover))
        .toList();
    int count = values.isEmpty ? 0 : values.reduce((a, b) => a + b);

    return count;
  }

  int getValueFromCache(String pattern) {
    if (patternCount.containsKey(pattern)) {
      return patternCount[pattern]!;
    }
    int count = countValidPatternsWithin(pattern);
    patternCount[pattern] = count;
    return patternCount[pattern]!;
  }
}
