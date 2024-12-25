import 'package:code/days/Day01.dart';
import 'package:code/days/Day02.dart';
import 'package:code/days/Day03.dart';
import 'package:code/days/Day04.dart';
import 'package:code/days/Day05.dart';
import 'package:code/days/Day06.dart';
import 'package:code/days/Day07.dart';
import 'package:code/days/Day08.dart';
import 'package:code/days/Day09.dart';
import 'package:code/days/Day10.dart';
import 'package:code/days/Day11.dart';
import 'package:code/days/Day12.dart';
import 'package:code/days/Day13.dart';
import 'package:code/days/Day14.dart';
import 'package:code/days/Day15.dart';
import 'package:code/days/Day16.dart';
import 'package:code/days/Day17.dart';
import 'package:code/days/Day18.dart';
import 'package:code/days/Day19.dart';
import 'package:code/days/Day20.dart';
import 'package:code/days/Day21.dart';
import 'package:code/days/Day22.dart';
import 'package:code/days/Day23.dart';
import 'package:code/days/Day24.dart';
import 'package:code/days/Day25.dart';

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
      break;
    case 5:
      Day05 day05 = Day05('day05', useTestData);
      day05.part1();
      day05.part2();
      break;
    case 6:
      Day06 day06 = Day06('day06', useTestData);
      day06.part1();
      day06.part2();
      break;
    case 7:
      Day07 day07 = Day07('day07', useTestData);
      day07.part1();
      day07.part2();
      break;
    case 8:
      Day08 day08 = Day08('day08', useTestData);
      day08.part1();
      day08.part2();
      break;
    case 9:
      Day09 day09 = Day09('day09', useTestData);
      day09.part1();
      day09.part2();
      break;
    case 10:
      Day10 day10 = Day10('day10', useTestData);
      day10.part1();
      day10.part2();
      break;
    case 11:
      Day11 day11 = Day11('day11', useTestData);
      day11.part1();
      day11.part2();
      break;
    case 12:
      Day12 day12 = Day12('day12', useTestData);
      day12.part1();
      day12.part2();
      break;
    case 13:
      Day13 day13 = Day13('day13', useTestData);
      day13.part1();
      day13.part2();
      break;
    case 14:
      Day14 day14 = Day14('day14', useTestData);
      day14.part1();
      day14.part2();
      break;
    case 15:
      Day15 day15 = Day15('day15', useTestData);
      day15.part1();
      day15.part2();
      break;
    case 16:
      Day16 day16 = Day16('day16', useTestData);
      day16.part1();
      day16.part2();
      break;
    case 17:
      Day17 day17 = Day17('day17', useTestData);
      day17.part1();
      day17.part2();
      break;
    case 18:
      Day18 day18 = Day18('day18', useTestData);
      day18.part1();
      day18.part2();
      break;
    case 19:
      Day19 day19 = Day19('day19', useTestData);
      day19.part1();
      day19.part2();
      break;
    case 20:
      Day20 day20 = Day20('day20', useTestData);
      day20.part1();
      day20.part2();
      break;
    case 21:
      Day21 day21 = Day21('day21', useTestData);
      day21.part1();
      day21.part2();
      break;
    case 22:
      Day22 day22 = Day22('day22', useTestData);
      day22.part1();
      day22.part2();
      break;
    case 23:
      Day23 day23 = Day23('day23', useTestData);
      day23.part1();
      day23.part2();
      break;
    case 24:
      Day24 day24 = Day24('day24', useTestData);
      day24.part1();
      day24.part2();
      break;
    case 25:
      Day25 day25 = Day25('day25', useTestData);
      day25.part1();
      day25.part2();
      break;
    default:
  }
}
