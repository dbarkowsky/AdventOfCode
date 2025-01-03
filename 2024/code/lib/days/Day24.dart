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

  // Gets the decimal value of all registers with a given prefix
  // Only used for x, y, and z
  int getDecimalForLetter(String letter) {
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
    int decimal = int.parse(binaryString, radix: 2);
    return decimal;
  }

  String getBinaryForLetter(String letter) {
    List<MapEntry<String, int>> letterEntries =
        register.entries.where((e) => e.key.startsWith(letter)).toList();
    letterEntries.sort((a, b) => b.key.compareTo(a.key));
    return letterEntries.map((e) => e.value.toString()).join("");
  }

  // Just get the z outcome
  void part1() {
    run();
    int binary = getDecimalForLetter('z');
    print(binary);
  }

  // There are 4 crossed wires that prevent the correct outcome
  // Which wires are these?
  // This is my initial solution. It's not good, but produces
  // multiple possible answers, one of which is correct.
  void part2BruteForce() {
    // Doesn't work for test input
    if (useTestData) {
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
    return getDecimalForLetter('x') + getDecimalForLetter('y') ==
        getDecimalForLetter('z');
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

  // This is a much better way of solving the problem
  // Based off another solution I read online
  void part2() {
    reset();
    // Set x to 0 and y to 1. Makes things easier to test.
    for (final key in register.keys) {
      if (key.startsWith("x"))
        register[key] = 0;
      else if (key.startsWith("y")) register[key] = 1;
    }
    run();
    // Record swaps for later
    List<({String a, String b})> swaps = [];
    // Find which wires are causing a bad result
    // This is basically just detecting which full adders give the wrong output
    List<String> badWires = getBadWires();
    while (badWires.isNotEmpty) {
      // Select the next wire
      String currentName = badWires.first;
      Instruction currentInstruction = tree[currentName]!;
      // This is the register that will feed this adder
      String inputName = currentName.replaceRange(0, 1, 'x');
      // Output wires (z) should be the result of a XOR
      if (currentInstruction.op != Operation.XOR) {
        // Not a XOR? This is a problem. Must find the related XOR
        // In full adder, x XOR y should lead to another XOR -> z
        String firstXor = tree.entries
            .where((e) => isXorWithInput(e.value, inputName))
            .first
            .key;
        String secondXor = tree.entries
            .where((e) => isXorWithInput(e.value, firstXor))
            .first
            .key;
        // Now we have the initial z output and the element of the XOR, we can swap them
        swaps.add((a: currentName, b: secondXor));
      } else {
        // It is a XOR. It should be fed by the result of an OR op (previous full adder)
        // and the result of the XOR op
        Instruction gateInput1 = tree[currentInstruction.left]!;
        Instruction gateInput2 = tree[currentInstruction.right]!;

        // If either of the instructions that led to this are AND, we know there's a problem
        MapEntry<String, Instruction>? badWire;
        if (gateInput1.op == Operation.AND) {
          badWire = MapEntry(currentInstruction.left, gateInput1);
        } else if (gateInput2.op == Operation.AND) {
          badWire = MapEntry(currentInstruction.right, gateInput2);
        }

        // If we didn't find AND, it's not a bad wire maybe?
        if (badWire == null) throw Exception("No bad gate here");

        // Otherwise, we should have a bad wire and
        // this bad wire should have XOR with x and y inputs
        String inputXor = tree.entries
            .where((e) => isXorWithInput(e.value, inputName))
            .first
            .key;

        // We can now record the swap of the bad wire with the input XOR
        swaps.add((a: inputXor, b: badWire.key));
      }

      // Refresh which wires are the bad ones to find the next issue
      // This works because we should have just fixed the last one with swapping
      // So running again will go until next issue is found or we reach the end
      reset();
      for (final key in register.keys) {
        if (key.startsWith("x"))
          register[key] = 0;
        else if (key.startsWith("y")) register[key] = 1;
      }
      for (final swap in swaps) {
        swapOutputs(swap.a, swap.b);
      }
      run();
      badWires = getBadWires();
    }
    // Print the answer
    List<String> swapKeys = swaps.expand((e) => [e.a, e.b]).toList();
    swapKeys.sort();
    print(swapKeys.join(","));
  }

  // Helper function to see if instruction matches XOR with key
  bool isXorWithInput(Instruction i, String key) {
    return i.op == Operation.XOR && (i.left == key || i.right == key);
  }

  // Returns a list of wire keys. 
  // Relies on indices of each bit in the z output
  List<String> getBadWires() {
    return getBadIndices()
        .map((i) => "z${i.toString().padLeft(2, '0')}")
        .toList();
  }

  // This is the key to finding the bad full adders
  List<int> getBadIndices() {
    int x = getDecimalForLetter("x");
    int y = getDecimalForLetter("y");
    String zAsBinary = getBinaryForLetter("z");
    String expectedBinary =
        (x + y).toRadixString(2).padLeft(zAsBinary.length, "0");
    List<String> expectedBits = expectedBinary.split("");
    List<String> actualBits = zAsBinary.split("");
    // Compare the actual bits to the expected bits.
    // If they don't match, we add that index to the list of bad ones.
    List<int> badIndices = [];
    for (int i = 0; i < expectedBits.length; i++) {
      if (expectedBits[expectedBits.length - i - 1] !=
          actualBits[actualBits.length - i - 1]) {
        badIndices.add(i);
      }
    }
    return badIndices;
  }
}
