import 'dart:convert';
import 'dart:io';

Stream<String> fileToArray({required String fileName, bool useTestData = false}) {
  final file = File('./input${useTestData ? '/test' : ''}/${fileName}'); // Path works from project root
  Stream<String> lines = file.openRead().transform(utf8.decoder).transform(LineSplitter());
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
