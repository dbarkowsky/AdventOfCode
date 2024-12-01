import 'dart:io';

List<String> fileToArray({required String fileName, bool useTestData = false}) {
  final file = File('./input${useTestData ? '/test' : ''}/$fileName'); // Path works from project root
  List<String> lines = file.readAsLinesSync();
  return lines;
}

void printArray({array= Stream<String>}) async {
  try {
    await for (var element in array) {
      print(element);
    }
  } catch (e) {
    print('Error: $e');
  }
}
