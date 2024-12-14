import 'package:code/fileReader.dart' as fileReader;

abstract class Day {
  List<String> input = [];
  bool useTestData = false;

  Day(String fileName, this.useTestData) {
    input =
        fileReader.fileToArray(fileName: fileName, useTestData: useTestData);
  }

  void printInput() {
    fileReader.printArray(array: input);
  }
}
