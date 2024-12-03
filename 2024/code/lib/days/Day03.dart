import 'package:code/days/Day.dart';

class Day03 extends Day {
  Day03(super.fileName, super.useTestData);

  // Any mul(#,#) is valid
  void part1() {
    num total = 0;

    for (String line in input) {
      RegExp mulMatch = RegExp(r'mul\(([\d]{1,3}),([\d]{1,3})\)');
      List<RegExpMatch> matches = mulMatch.allMatches(line).toList();

      // For each match, add the product
      for (final match in matches) {
        total += (int.parse(match.group(1)!) * int.parse(match.group(2)!));
      }
    }

    print(total);
  }

  // do and don't change whether mul(#,#) is valid
  void part2() {
    num total = 0;
    bool doing = true; // Starts valid
    for (String line in input) {
      RegExp mulMatch =
          RegExp(r"mul\(([\d]{1,3}),([\d]{1,3})\)|do\(\)|don't\(\)");
      List<RegExpMatch> matches = mulMatch.allMatches(line).toList();

      for (final match in matches) {
        // For each match, is this a do or don't?
        bool justDo = RegExp(r"do\(\)").hasMatch(match.group(0)!);
        bool justDont = RegExp(r"don't\(\)").hasMatch(match.group(0)!);

        // Turn off or on, or just add the product
        if (justDo) {
          doing = true;
        } else if (justDont) {
          doing = false;
        } else {
          if (doing) {
            total += (int.parse(match.group(1)!) * int.parse(match.group(2)!));
          }
        }
      }
    }

    print(total);
  }
}
