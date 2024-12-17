import 'dart:math';

import 'package:code/days/Day.dart';

class Day17 extends Day {
  int a = 0;
  int b = 0;
  int c = 0;
  List<int> program = [];
  int instructionPointer = 0;
  List<int> output = [];
  Day17(super.fileName, super.useTestData) {
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

  void part1() {
    while (instructionPointer < program.length) {
      int opcode = program[instructionPointer];
      int operand = program[instructionPointer + 1];
      int comboOperand = getComboOperand(operand);
      // print('$instructionPointer, $opcode, $operand');
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
    print('A: $a\nB: $b\nC: $c');
    print(output.join(","));
  }

  void part2() {}

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
  void adv({required int comboOperand}){
    int numerator = a;
    int denominator = pow(2, comboOperand).toInt();
    a = numerator ~/ denominator;
  }

  void bxl({required int literalOperand}){
    b = b ^ literalOperand;
  }

  void bst({required int comboOperand}){
    b = comboOperand % 8;
  }

  void jnz({required int literalOperand}){
    if (a == 0){
      instructionPointer += 2;
    } else {
      instructionPointer = literalOperand;
    }
  }

  void bxc(){
    b = b ^ c;
  }

  void out({required int comboOperand}){
    int value = comboOperand % 8;
    output.add(value);
  }

  void bdv({required int comboOperand}){
    int numerator = a;
    int denominator = pow(2, comboOperand).toInt();
    b = numerator ~/ denominator;
  }

  void cdv({required int comboOperand}){
    int numerator = a;
    int denominator = pow(2, comboOperand).toInt();
    c = numerator ~/ denominator;
  }
}
