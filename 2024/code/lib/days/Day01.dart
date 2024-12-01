import 'package:code/days/Day.dart';

class Day01 extends Day {
  Day01(super.fileName, super.useTestData);

  @override
  void part1() {
    List<num> lefts = [];
    List<num> rights = [];
    for (String pair in input) {
      final left = pair.split("   ")[0];
      final right = pair.split("   ")[1];

      lefts.add(int.parse(left));
      rights.add(int.parse(right));
    }

    lefts.sort();
    rights.sort();
    num sum = 0;
    for (int i = 0; i < lefts.length; i++) {
      sum += (lefts[i] - rights[i]).abs();
    }
    print(sum);
  }

  @override
  void part2() {
    Map<num, num> tracker = {};
    List<num> lefts = [];
    List<num> rights = [];
    for (String pair in input) {
      final left = pair.split("   ")[0];
      final right = pair.split("   ")[1];

      lefts.add(int.parse(left));
      rights.add(int.parse(right));
    }

    for (int i = 0; i < rights.length; i++) {
      tracker.update(rights[i], (existingValue) {
        return existingValue + 1;
      }, ifAbsent: () => 1);
    }

    num sum = 0;
    for (var leftNum in lefts) {
      if (tracker.containsKey(leftNum)) {
        sum += tracker[leftNum]! * leftNum;
      }
    }
    print(sum);
  }
}
