import 'package:code/days/Day01.dart';
import 'package:code/days/Day02.dart';
import 'package:code/days/Day03.dart';
import 'package:code/days/Day04.dart';

void main(List<String> arguments) {
  if (arguments.isEmpty) {
    print(
        'No day was provided. Run "dart run code <day number>" to select a day.');
  }
  final day = int.parse(arguments.first);
  bool useTestData = arguments.length > 1 ? arguments[1] == 'test' : false;
  switch (day) {
    case 1:
      Day01 day01 = Day01('day01', useTestData);
      day01.part1();
      day01.part2();
      break;
    case 2:
      Day02 day02 = Day02('day02', useTestData);
      day02.part1();
      day02.part2();
      break;
    case 3:
      Day03 day03 = Day03('day03', useTestData);
      day03.part1();
      day03.part2();
      break;
    case 4:
      Day04 day04 = Day04('day04', useTestData);
      day04.part1();
      day04.part2();
    default:
  }
}
