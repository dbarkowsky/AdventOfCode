import 'package:code/days/Day.dart';

enum Operator { ADD, MULTIPLY }

class Day07 extends Day {
  List<int> testValues = [];
  List<List<int>> components = [];
  Day07(super.fileName, super.useTestData) {
    for (final line in input) {
      var [testValueAsString, componentsAsString] = line.split(": ");
      List<int> componentList =
          componentsAsString.split(" ").map(int.parse).toList();
      int testValue = int.parse(testValueAsString);
      testValues.add(testValue);
      components.add(componentList);
    }
  }

  void part1() {
    int sum = 0;
    for (int i = 0; i < testValues.length; i++) {
      List<List<Operator>> possibleOperatorConfigs =
        generateOperators(components[i].length - 1);
      for (List<Operator> config in possibleOperatorConfigs) {
        if (isEquivalent(testValues[i], components[i], config)) {
          sum += testValues[i];
          break;
        }
      }
    }
    
    print(sum);
  }

  void part2() {}

  bool isEquivalent(int testValue, List<int> nums, List<Operator> ops) {
    int result = nums.first;
    for (int i = 0; i < ops.length; i++) {
      switch (ops[i]) {
        case Operator.ADD:
          result += nums[i + 1];
          break;
        case Operator.MULTIPLY:
          result *= nums[i + 1];
          break;
        default:
          break;
      }
    }
    return result == testValue;
  }

  List<List<Operator>> generateOperators(int length) {
    List<String> ops = [];
    String maxString = List.filled(length, "1").join("");
    int max = int.parse(maxString, radix: 2);

    for (int i = 0; i <= max; i++) {
      ops.add(i.toRadixString(2).padLeft(length, "0"));
    }
    List<List<Operator>> convertedOps = [];
    for (String op in ops) {
      List<Operator> onePossibleConfig = op.split("").map(stringToOp).toList();
      convertedOps.add(onePossibleConfig);
    }
    return convertedOps;
  }

  Operator stringToOp(String s) {
    int n = int.parse(s);
    if (n == Operator.ADD.index) {
      return Operator.ADD;
    } else {
      return Operator.MULTIPLY;
    }
  }
}
