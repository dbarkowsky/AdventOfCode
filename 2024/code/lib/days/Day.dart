import 'package:code/fileReader.dart' as fileReader;

abstract class Day {
  late List<String> input;
  Day(String fileName, bool useTestData) {
    input =
        fileReader.fileToArray(fileName: fileName, useTestData: useTestData);
  }

  void printInput() {
    fileReader.printArray(array: input);
  }
}
