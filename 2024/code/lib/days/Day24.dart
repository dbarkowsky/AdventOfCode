import 'package:code/days/Day.dart';

enum Operation{
  AND,
  OR,
  XOR
}

typedef Instruction = ({String left, Operation op, String right});

class Day24 extends Day {
  Day24(super.fileName, super.useTestData){
    int inputIndex = 0;
    while (input[inputIndex] != ""){
      var [code, value] = input[inputIndex].split(": ");
      register[code] = int.parse(value);
      inputIndex++;
    }
    inputIndex++; // skip blank
    while (inputIndex < input.length){
      var [instruction, key] = input[inputIndex].split(" -> ");
      var [left, op, right] = instruction.split(" ");
      tree[key] = (left: left, op: stringToOperation(op), right: right);
      inputIndex++;
    }
  }
  Map<String, int> register = {};
  Map<String, Instruction> tree = {};

  void part1() {
    // Evaluate all the tree entries to build register values
    for (final entry in tree.entries){
      register[entry.key] = getRegisterValue(entry.key);
    }
    print(register);
    // Get all z entries
    List<MapEntry<String, int>> entries = register.entries.where((e) => e.key.startsWith("z")).toList();
    entries.sort((a, b) => b.key.compareTo(a.key));
    // Assemble binary string
    String binaryString = "";
    for (final entry in entries){
      binaryString += entry.value.toString();
    }
    // Convert to decimal
    int binary = int.parse(binaryString, radix: 2);
    print(binary);
  }

  void part2() {
  
  }

  int getRegisterValue(String key){
    if (register.containsKey(key)){
      return register[key]!;
    }
    // Hasn't been calculated yet, get the sum of its parts
    Instruction instruction = tree[key]!;
    if (instruction.op == Operation.AND){
      return getRegisterValue(instruction.left) & getRegisterValue(instruction.right);
    }
    if (instruction.op == Operation.OR){
      return getRegisterValue(instruction.left) | getRegisterValue(instruction.right);
    }
    return getRegisterValue(instruction.left) ^ getRegisterValue(instruction.right);
  }

  Operation stringToOperation(String s){
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
