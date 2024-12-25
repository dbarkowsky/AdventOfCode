import 'dart:io';

import 'package:code/days/Day.dart';

enum Operation { AND, OR, XOR }

typedef Instruction = ({String left, Operation op, String right});

class Day24 extends Day {
  Day24(super.fileName, super.useTestData) {
    reset();
  }
  Map<String, int> register = {};
  Map<String, Instruction> tree = {};

  void reset() {
    register = {};
    tree = {};
    int inputIndex = 0;
    while (input[inputIndex] != "") {
      var [code, value] = input[inputIndex].split(": ");
      register[code] = int.parse(value);
      inputIndex++;
    }
    inputIndex++; // skip blank
    while (inputIndex < input.length) {
      var [instruction, key] = input[inputIndex].split(" -> ");
      var [left, op, right] = instruction.split(" ");
      tree[key] = (left: left, op: stringToOperation(op), right: right);
      inputIndex++;
    }
  }

  void run() {
    // Evaluate all the tree entries to build register values
    for (final entry in tree.entries) {
      register[entry.key] = getRegisterValue(entry.key);
    }
  }

  int getBinaryForLetter(String letter) {
    // Get all letter entries
    List<MapEntry<String, int>> entries =
        register.entries.where((e) => e.key.startsWith(letter)).toList();
    entries.sort((a, b) => b.key.compareTo(a.key));
    // Assemble binary string
    String binaryString = "";
    for (final entry in entries) {
      binaryString += entry.value.toString();
    }
    // Convert to decimal
    int binary = int.parse(binaryString, radix: 2);
    return binary;
  }

  void part1() {
    run();
    int binary = getBinaryForLetter('z');
    print(binary);
  }

  void part2() {
    reset();
    // Don't want to include the last z entry in this swap list
    List<int> zNums = tree.keys
        .where((k) => k.startsWith('z'))
        .map((s) => s.substring(1))
        .map(int.parse)
        .toList();
    int highest = 0;
    for (int num in zNums) {
      if (num > highest) {
        highest = num;
      }
    }
    // If the output goes to a z, the operation must be XOR
    List<String> badZGates = tree.entries
        .where((entry) => entry.key.startsWith("z"))
        .where((entry) => tree[entry.key]!.op != Operation.XOR)
        .where((entry) => entry.key != 'z$highest')
        .map((e) => e.key)
        .toList();
    // If output does not go to z, and input is not from x and y, then it should not be XOR
    List<String> badOtherGates = tree.entries
        .where((entry) => !entry.key.startsWith("z"))
        .where((entry) => (!tree[entry.key]!.left.startsWith("x") &&
            !tree[entry.key]!.right.startsWith("x") &&
            !tree[entry.key]!.left.startsWith("y") &&
            !tree[entry.key]!.right.startsWith("y")))
        .where((entry) => tree[entry.key]!.op == Operation.XOR)
        .map((e) => e.key)
        .toList();

    // That should be 6 gates to swap so far, but how do we find the last two?
    // final falseCarryGates = () {
    //   // For each gate in badZGates, swap it with the other bad gate that uses it as an input
    //   for (var a in badOtherGates) {
    //     final b = badZGates
    //         .firstWhere((g) => register[g] == register[firstWhereThatUsesOutput(a)]);
    //     Instruction temp = tree[a]!;
    //     tree[a] = tree[b]!;
    //     tree[b] = temp;
    //   }

    //   run();
    //   final expectedResult = getBinaryForLetter('x') + getBinaryForLetter('y');
    //   final actualResult = getBinaryForLetter('z');
    //   final falseCarryBit = countTrailingZeroBits(expectedResult ^ actualResult)
    //       .toString()
    //       .padLeft(2, '0');

    //   final filteredWires = tree.values
    //       .where((wire) =>
    //           wire.left.endsWith(falseCarryBit) &&
    //           wire.right.endsWith(falseCarryBit))
    //       .toList();

    //   assert(filteredWires.length == 2,
    //       "Did not find exactly two false carry gates to swap.");
    //   return filteredWires;
    // }();

    // print(falseCarryGates);

    List<String> remainingNodes = tree.keys
        .where((k) => !badZGates.contains(k) && !badOtherGates.contains(k))
        .toList();
    int swapIndexA = 0;
    while (swapIndexA < remainingNodes.length - 1) {
      for (int swapIndexB = swapIndexA + 1;
          swapIndexB < remainingNodes.length;
          swapIndexB++) {
        reset();
        // Swap known bad nodes
        for (int i = 0; i < badZGates.length; i++) {
          swapOutputs(badZGates[i], badOtherGates[i]);
        }
        // Swap two other nodes at random
        swapOutputs(remainingNodes[swapIndexA], remainingNodes[swapIndexB]);
        run();

        if (thisSwapWorked()){
          List<String> swappedNodes = [...badZGates, ...badOtherGates, remainingNodes[swapIndexA], remainingNodes[swapIndexB]];
          swappedNodes.sort();
          print(swappedNodes.join(","));
          exit(0);
        }
      }
      swapIndexA++;
    }
    print('Failure');
  }

  bool thisSwapWorked(){
    return getBinaryForLetter('x') ^ getBinaryForLetter('y') == getBinaryForLetter('z');
  }

  void swapOutputs(String a, String b) {
    Instruction temp = tree[a]!;
    tree[a] = tree[b]!;
    tree[b] = temp;
  }

  String firstWhereThatUsesOutput(String out) {
    // Filter gates that use 'out' in their left or right output
    final candidates = tree.entries
        .where((gate) => gate.value.left == out || gate.value.right == out)
        .toList();

    // Check if we have a gate with an output starting with 'z'
    final zGate = candidates.firstWhere((gate) => gate.key.startsWith('z'),
        orElse: () =>
            // If no 'z' gate is found, recursively call for the next gate that uses the output
            candidates.firstWhere(
              (gate) => tree.containsKey(gate.key),
            ));
    // If a 'z' gate is found, modify its output as per the original Kotlin logic
    final newOut = "z${int.parse(zGate.key.substring(1))}".padLeft(2, '0');
    return newOut;
  }

  int countTrailingZeroBits(int value) {
    int count = 0;
    while ((value & 1) == 0 && value != 0) {
      count++;
      value >>= 1; // Shift right by one bit.
    }
    return count;
  }

  int getRegisterValue(String key) {
    if (register.containsKey(key)) {
      return register[key]!;
    }
    // Hasn't been calculated yet, get the sum of its parts
    Instruction instruction = tree[key]!;
    if (instruction.op == Operation.AND) {
      return getRegisterValue(instruction.left) &
          getRegisterValue(instruction.right);
    }
    if (instruction.op == Operation.OR) {
      return getRegisterValue(instruction.left) |
          getRegisterValue(instruction.right);
    }
    return getRegisterValue(instruction.left) ^
        getRegisterValue(instruction.right);
  }

  Operation stringToOperation(String s) {
    switch (s) {
      case "AND":
        return Operation.AND;
      case "OR":
        return Operation.OR;
      case "XOR":
        return Operation.XOR;
      default:
        return Operation.AND; // should never hit
    }
  }
}
