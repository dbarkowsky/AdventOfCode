import 'dart:math';

import 'package:code/days/Day.dart';

enum Operator { ADD, MULTIPLY, CONCAT }

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

  // Dealing with all possible combos of + and *
  void part1() {
    int sum = 0;
    for (int i = 0; i < testValues.length; i++) {
      List<List<Operator>> possibleOperatorConfigs =
        generateBinaryOperators(components[i].length - 1);
      for (List<Operator> config in possibleOperatorConfigs) {
        if (isEquivalent(testValues[i], components[i], config)) {
          sum += testValues[i];
          break;
        }
      }
    }
    
    print(sum);
  }

  // Now concatenation is also possible, so ternary options
  void part2() {
    int sum = 0;
    for (int i = 0; i < testValues.length; i++) {
      List<List<Operator>> possibleOperatorConfigs =
        generateTernaryOperators(components[i].length - 1);
      for (List<Operator> config in possibleOperatorConfigs) {
        if (isEquivalent(testValues[i], components[i], config)) {
          sum += testValues[i];
          break;
        }
      }
    }
    
    print(sum);
  }

  // Does appropriate calculation and then checks against test number
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
        case Operator.CONCAT:
          result = int.parse('$result${nums[i + 1]}');
          break;
        default:
          break;
      }
    }
    return result == testValue;
  }

  // Generates all possible options of the ADD or MULTIPLY lists given a possible length.
  // Could cache this to speed things up.
  List<List<Operator>> generateBinaryOperators(int length) {
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

  // Same as above, but now CONCAT is a possible option as well
  List<List<Operator>> generateTernaryOperators(int length) {
    List<List<Operator>> convertedOps = [];

    int max = (pow(3, length) - 1).toInt(); // Calculate 3^length - 1.

    for (int i = 0; i <= max; i++) {
      String ternaryString = toTernary(i).padLeft(length, "0");
      List<Operator> onePossibleConfig = ternaryString.split("").map(stringToOp).toList();
      convertedOps.add(onePossibleConfig);
    }
    return convertedOps;
  }

  // Converts an integer to a ternary (base-3) string
String toTernary(int num) {
  if (num == 0) return "0";
  String ternary = "";
  while (num > 0) {
    ternary = (num % 3).toString() + ternary;
    num ~/= 3; // This is int division, but by 3. So num = num / 3 using int math.
  }
  return ternary;
}

  Operator stringToOp(String s) {
    int n = int.parse(s);
    if (n == Operator.ADD.index) {
      return Operator.ADD;
    } else if (n == Operator.MULTIPLY.index){
      return Operator.MULTIPLY;
    } else {
      return Operator.CONCAT;
    }
  }
}
