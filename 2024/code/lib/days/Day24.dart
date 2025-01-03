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

  // Resets maps back to base input
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

  // Parses the maps to get the final values of all registers
  void run() {
    // Evaluate all the tree entries to build register values
    for (final entry in tree.entries) {
      int value = getRegisterValue(entry.key, entry.key, true);
      // If we get -1, there was an issue (found a cycle)
      if (value == -1) {
        throw Error();
      }
      register[entry.key] = value;
    }
  }

  // Gets the binary value of all registers with a given prefix
  // Only used for x, y, and z
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

  // Just get the z outcome
  void part1() {
    run();
    int binary = getBinaryForLetter('z');
    print(binary);
  }

  // There are 4 crossed wires that prevent the correct outcome
  // Which wires are these?
  // This is my initial solution. It's not good, but produces
  // multiple possible answers, one of which is correct.
  void part2() {
    // Doesn't work for test input
    if (useTestData){
      print("Not intended for test input");
      exit(0);
    }
    reset();

    // Don't want to include the last z entry in this swap list
    // It's the one that would be the end of the flow
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
    // Learned about these following conditions from online forums.
    // It relates to the full adder configuration
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

    // There should be one more swap unrelated to the z registers
    List<String> remainingNodes = tree.keys
        .where((k) =>
            !badZGates.contains(k) &&
            !badOtherGates.contains(k) &&
            k != 'z$highest')
        .toList();

    // What are all the possible combinations of swaps for these known bad gates?
    List<({String a, String b})> combinations = [];
    for (String item1 in badZGates) {
      for (String item2 in badOtherGates) {
        combinations.add((a: item1, b: item2));
      }
    }

    // Generate all groups of 3 pairs with unique values
    List<List<({String a, String b})>> groups = [];
    for (int i = 0; i < combinations.length; i++) {
      for (int j = i + 1; j < combinations.length; j++) {
        for (int k = j + 1; k < combinations.length; k++) {
          var group = [combinations[i], combinations[j], combinations[k]];

          // Check if all values in the group are unique
          Set<String> usedValues = {};
          bool isValid = true;
          for (var pair in group) {
            if (usedValues.contains(pair.a) || usedValues.contains(pair.b)) {
              isValid = false;
              break;
            }
            usedValues.add(pair.a);
            usedValues.add(pair.b);
          }

          if (isValid) {
            groups.add(group);
          }
        }
      }
    }

    // Brute force through each possible combination
    for (final triplet in groups) {
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
          try {
            run();
          } catch (e) {
            // loop detected. Abort and try next
            continue;
          }
          // If x + y = z, then this is a possible solution
          if (thisSwapWorked()) {
            List<String> swappedNodes = [
              triplet[0].a,
              triplet[0].b,
              triplet[1].a,
              triplet[1].b,
              triplet[2].a,
              triplet[2].b,
              remainingNodes[swapIndexA],
              remainingNodes[swapIndexB]
            ];
            swappedNodes.sort();
            print(swappedNodes.join(","));
          }
        }
        swapIndexA++;
      }
    }
  }

  bool thisSwapWorked() {
    return getBinaryForLetter('x') + getBinaryForLetter('y') ==
        getBinaryForLetter('z');
  }

  void swapOutputs(String a, String b) {
    // Swap which gates make up this entry
    Instruction temp = tree[a]!;
    tree[a] = tree[b]!;
    tree[b] = temp;
  }

  // Recursive function that gets a register's value or goes deeper to calculate it
  int getRegisterValue(String key, String start, bool initial) {
    // If we discover a full cycle, abort. It would be an endless loop otherwise.
    if (key == start && !initial) return -1;
    if (register.containsKey(key)) {
      return register[key]!;
    }
    // Hasn't been calculated yet, get the sum of its parts
    Instruction instruction = tree[key]!;
    if (instruction.op == Operation.AND) {
      return getRegisterValue(instruction.left, start, false) &
          getRegisterValue(instruction.right, start, false);
    }
    if (instruction.op == Operation.OR) {
      return getRegisterValue(instruction.left, start, false) |
          getRegisterValue(instruction.right, start, false);
    }
    return getRegisterValue(instruction.left, start, false) ^
        getRegisterValue(instruction.right, start, false);
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
