import 'package:code/fileReader.dart' as fileReader;

void main(List<String> arguments) {
  final input = fileReader.fileToArray(fileName: './input/test');
  fileReader.printArray(array: input);
}
