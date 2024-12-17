import 'dart:math';
import 'package:code/days/Day.dart';

class Day17 extends Day {
  int a = 0;
  int b = 0;
  int c = 0;
  List<int> program = [];
  int instructionPointer = 0;
  List<int> output = [];
  Day17(super.fileName, super.useTestData);

  // Called to rebuild class variables from input
  void reset() {
    program = [];
    output = [];
    instructionPointer = 0;
    int lineNum = 0;
    RegExp numberReg = RegExp(r"([\d]+)");
    for (final line in input) {
      List<String?> matches =
          numberReg.allMatches(line).map((match) => match.group(0)).toList();
      switch (lineNum) {
        case 0:
          a = int.parse(matches.first!);
          break;
        case 1:
          b = int.parse(matches.first!);
          break;
        case 2:
          c = int.parse(matches.first!);
          break;
        case 4:
          program = matches.map((match) => int.parse(match!)).toList();
          break;
        default:
      }
      lineNum++;
    }
  }

  // Just following the instructions
  void part1() {
    reset();
    run();
    print('A: $a\nB: $b\nC: $c');
    print(output.join(","));
  }

  // Trying to find the correct value for A that reproduces the program list as output
  void part2() {
    reset();
    // We start with an i that matches the last index of the program
    // This is because that defines the maximum number of steps to possibly create a program of the same size.
    print(findInitialA(0, program.length - 1));
  }

  // I couldn't have done this without peeking at other answers.
  // It would not have crossed my mind to use bitwise operators
  // Kudos to u/Polaric_Spiral for pointing this out
  int findInitialA(int nextValue, int i){
    // If < 0 we've exhausted this path
    if (i < 0) return nextValue;
    // This part is tricky.
    // aVal determines what place we're trying to find in the output/program match
    // By shifting it 3 we start looking for the first position.
    // Shifting it in subsequent calls looks for the next positions
    // Iterating from aVal to aVal + 8 covers all 8 bits at that location, which corresponds
    // to the 8 possible opcodes/operands 
    // Eventually, an aVal will produce the correct output for this location (i) in the original program
    for (int aVal = nextValue << 3; aVal < (nextValue << 3) + 8; aVal++){
      reset(); // Must reset every round
      a = aVal; // For next run, set a and try to run with that
      run();
      // We're building the program backwards, so the first time, the first element of the output 
      // must equal the last program element. Every time i goes down, it is checking to make sure
      // the next addition to our output is aligned with the expected program
      if (output[0] == program[i]){
        int finalValue = findInitialA(aVal, i - 1);
        if (finalValue >= 0) return finalValue; // Don't do negatives
      }
    }
    return -1;
  }

  // Runs the program
  void run() {
    while (instructionPointer < program.length) {
      int opcode = program[instructionPointer];
      int operand = program[instructionPointer + 1];
      int comboOperand = getComboOperand(operand);
      switch (opcode) {
        case 0:
          adv(comboOperand: comboOperand);
          break;
        case 1:
          bxl(literalOperand: operand);
          break;
        case 2:
          bst(comboOperand: comboOperand);
          break;
        case 3:
          jnz(literalOperand: operand);
          continue; // b/c we don't increase the pointer here
        case 4:
          bxc();
          break;
        case 5:
          out(comboOperand: comboOperand);
          break;
        case 6:
          bdv(comboOperand: comboOperand);
          break;
        case 7:
          cdv(comboOperand: comboOperand);
          break;
        default:
      }
      instructionPointer += 2;
    }
  }

  int getComboOperand(int operand) {
    switch (operand) {
      case 0:
      case 1:
      case 2:
      case 3:
        return operand;
      case 4:
        return a;
      case 5:
        return b;
      case 6:
        return c;
      default:
        return -1;
    }
  }

  //**
  // I've named these functions after their names in the instructions.
  // */
  void adv({required int comboOperand}) {
    int numerator = a;
    int denominator = pow(2, comboOperand).toInt();
    double result = numerator / denominator;
    a = result.toInt();
  }

  void bxl({required int literalOperand}) {
    b = b ^ literalOperand;
  }

  void bst({required int comboOperand}) {
    b = comboOperand % 8;
  }

  void jnz({required int literalOperand}) {
    if (a == 0) {
      instructionPointer += 2;
    } else {
      instructionPointer = literalOperand;
    }
  }

  void bxc() {
    b = b ^ c;
  }

  void out({required int comboOperand}) {
    int value = comboOperand % 8;
    output.add(value);
  }

  void bdv({required int comboOperand}) {
    int numerator = a;
    int denominator = pow(2, comboOperand).toInt();
    b = numerator ~/ denominator;
  }

  void cdv({required int comboOperand}) {
    int numerator = a;
    int denominator = pow(2, comboOperand).toInt();
    c = numerator ~/ denominator;
  }
}
