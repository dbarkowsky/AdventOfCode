import 'package:code/days/Day.dart';

class Day19 extends Day {
  List<String> initialPatterns = [];
  List<String> desiredPatterns = [];
  Map<String, int> cache = {}; // This tracks every known pattern and how many ways it can be assembled.
  Day19(super.fileName, super.useTestData) {
    initialPatterns = input[0].split(", ");
    desiredPatterns.addAll(input.sublist(2));
    // One run to identify all patterns and add to cache
    analyze();
  }

  // Just wants how many patterns were possible
  void part1() {
    int total = 0;
    for (final pattern in desiredPatterns){
      if (cache.containsKey(pattern)){
        total += cache[pattern] == 0 ? 0 : 1;
      }
    }
    print(total);
  }

  // Wants to know how many ways the patterns can be assembled.
  void part2() {
    int total = 0;
    for (final pattern in desiredPatterns){
      if (cache.containsKey(pattern)){
        total += cache[pattern]!;
      }
    }
    print(total);
  }

  void analyze(){
    // Start with the initial patterns provided that we know exist
    int biggestKnownLength = initialPatterns.map((p) => p.length).reduce((a, b) => a > b ? a : b);
    // Grouping by length of pattern
    for (int len = 1; len <= biggestKnownLength; len++){
      List<String> possibleMatchingLen = initialPatterns.where((p) => p.length == len).toList();
      // If the pattern is 1 long, it can only be assembled one way
      if (len == 1){
        for (final match in possibleMatchingLen){
          cache[match] = 1;
        }
      } else {
        // Otherwise, check if it is possible. 
        for (final match in possibleMatchingLen){
          cache[match] = isPossible(match) + 1;
        }
      }
    }

    // Now for all the desired/unknown patterns.
    for (final pattern in desiredPatterns){
      isPossible(pattern);
    }
  }

  // Determines if a string is possible to be assembled given known patterns
  // Returns the number of ways it can be assembled.
  int isPossible(String testingPattern){
    // If we already know this, don't do it, just get value from cache
    if (cache.containsKey(testingPattern)){
      return cache[testingPattern]!;
    }
    // If it's a blank string, it can't do anything.
    if (testingPattern.isEmpty) return 0;

    // It's an unknown still.
    int count = 0;
    // For each of the initial patterns, see if it starts with this pattern
    for (int i = 0; i < initialPatterns.length; i++){
      if (testingPattern.startsWith(initialPatterns[i])){
        // It does? then remove the known part and repeat this process with the unknown part
        String subPattern = testingPattern.substring(initialPatterns[i].length);
        count += isPossible(subPattern); // It is assembled in ways equal to the sum of its parts
      }
    }

    // Save that for later.
    cache[testingPattern] = count;
    return count;
  }
}
