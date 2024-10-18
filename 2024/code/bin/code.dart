import 'package:code/days/Day01.dart';

void main(List<String> arguments) {
  if (arguments.isEmpty) {
    print('No day was provided. Run "dart run code <day number>" to select a day.');
  }
  final day = int.parse(arguments.first);
  bool useTestData = arguments.length > 1 ? arguments[1] == 'test' : false;
  switch (day) {
    case 1:
      Day01.part1(useTestData);
      break;
    default:
  }
}
