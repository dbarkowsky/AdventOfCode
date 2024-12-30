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

    List<String> remainingNodes = tree.keys
        .where((k) => !badZGates.contains(k) && !badOtherGates.contains(k))
        .toList();
    // What are all the possible combinations of swaps for these known bad gates?
    // Should be 9 total
    List<({String a, String b})> combinations = [];
    for (String item1 in badZGates) {
      for (String item2 in badOtherGates) {
        combinations.add((a: item1, b: item2));
      }
    }
    // Combine these into triplets of the swaps to take place
    List<List<({String a, String b})>> swapTriplets = [];
    swapTriplets.add([combinations[0], combinations[4], combinations[8]]);
    swapTriplets.add([combinations[0], combinations[5], combinations[7]]);
    swapTriplets.add([combinations[1], combinations[3], combinations[8]]);
    swapTriplets.add([combinations[1], combinations[5], combinations[6]]);
    swapTriplets.add([combinations[2], combinations[3], combinations[7]]);
    swapTriplets.add([combinations[2], combinations[4], combinations[6]]);

    for (final triplet in swapTriplets) {
      int swapIndexA = 0;
      while (swapIndexA < remainingNodes.length - 1) {
        for (int swapIndexB = swapIndexA + 1;
            swapIndexB < remainingNodes.length;
            swapIndexB++) {
          reset();
          // Swap known bad nodes
          for (final swap in triplet) {
            swapOutputs(swap.a, swap.b);
          }
          // Swap two other nodes at random
          swapOutputs(remainingNodes[swapIndexA], remainingNodes[swapIndexB]);
          run();

          if (thisSwapWorked()) {
            List<String> swappedNodes = [
              ...badZGates,
              ...badOtherGates,
              remainingNodes[swapIndexA],
              remainingNodes[swapIndexB]
            ];
            swappedNodes.sort();
            print(swappedNodes.join(","));
            exit(0);
          }
        }
        swapIndexA++;
      }
    }
    print('Failure');
  }

  bool thisSwapWorked() {
    return getBinaryForLetter('x') ^ getBinaryForLetter('y') ==
        getBinaryForLetter('z');
  }

  void swapOutputs(String a, String b) {
    // print('$a - $b');
    // Swap which gates make up this entry
    Instruction temp = tree[a]!;
    tree[a] = tree[b]!;
    tree[b] = temp;
    // Any entries that reference this gate must be updated as well
    for (final entry in tree.entries) {
      if (entry.value.left == a) {
        tree.update(entry.key, (e) => (left: b, op: e.op, right: e.right));
      } else if (entry.value.right == a) {
        tree.update(entry.key, (e) => (left: e.left, op: e.op, right: b));
      } else if (entry.value.left == b) {
        tree.update(entry.key, (e) => (left: a, op: e.op, right: e.right));
      } else if (entry.value.right == b) {
        tree.update(entry.key, (e) => (left: e.left, op: e.op, right: a));
      }
    }
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
