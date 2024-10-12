import 'dart:convert';
import 'dart:io';

Stream<String> fileToArray({fileName= String}) {
  final file = File(fileName);
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
