import 'package:code/fileReader.dart' as fileReader;

class Day01 {
  static void part1(bool useTestData){
    String fileName = 'day01';
    final input = fileReader.fileToArray(fileName: fileName, useTestData: useTestData);
    fileReader.printArray(array: input);
  }
}
