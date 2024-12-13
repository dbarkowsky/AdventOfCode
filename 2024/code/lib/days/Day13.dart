import 'package:collection/collection.dart';
import 'package:code/days/Day.dart';

class Game {
  int aX = 0;
  int aY = 0;
  int bX = 0;
  int bY = 0;
  int prizeX = 0;
  int prizeY = 0;
  Game(this.aX, this.aY, this.bX, this.bY, this.prizeX, this.prizeY);

  @override
  String toString() {
    return '\tA: $aX, $aY\n  \tB: $bX, $bY\n  \tPrize: $prizeX, $prizeY\n';
  }
}

class Day13 extends Day {
  List<Game> games = [];
  Day13(super.fileName, super.useTestData) {
    input.add("");
    int aX = 0;
    int aY = 0;
    int bX = 0;
    int bY = 0;
    int prizeX = 0;
    int prizeY = 0;
    RegExp buttonXReg = RegExp(r"X\+(\d+)");
    RegExp buttonYReg = RegExp(r"Y\+(\d+)");
    RegExp prizeXReg = RegExp(r"X=(\d+)");
    RegExp prizeYReg = RegExp(r"Y=(\d+)");
    for (int lineNum = 0; lineNum < input.length; lineNum++) {
      int modulo = lineNum % 4;
      switch (modulo) {
        case 0:
          aX = int.parse(buttonXReg.firstMatch(input[lineNum])!.group(1)!);
          aY = int.parse(buttonYReg.firstMatch(input[lineNum])!.group(1)!);
          break;
        case 1:
          bX = int.parse(buttonXReg.firstMatch(input[lineNum])!.group(1)!);
          bY = int.parse(buttonYReg.firstMatch(input[lineNum])!.group(1)!);
          break;
        case 2:
          prizeX = int.parse(prizeXReg.firstMatch(input[lineNum])!.group(1)!);
          prizeY = int.parse(prizeYReg.firstMatch(input[lineNum])!.group(1)!);
          break;
        case 3:
          games.add(Game(aX, aY, bX, bY, prizeX, prizeY));
          break;
        default:
      }
    }
  }

  void part1() {
    int aCost = 3;
    int bCost = 1;
    int total = 0;
    for (final game in games) {
      HeapPriorityQueue<int> costHeap =
          HeapPriorityQueue<int>((a, b) => a.compareTo(b));
      for (int a = 0; a <= 100; a++) {
        for (int b = 0; b <= 100; b++) {
          int xTotal = (a * game.aX) + (b * game.bX);
          int yTotal = (a * game.aY) + (b * game.bY);
          // print('$xTotal $yTotal ${game.prizeX} ${game.prizeY}');
          if (xTotal == game.prizeX && yTotal == game.prizeY) {
            // Add cost
            int cost = (a * aCost) + (b * bCost);
            costHeap.add(cost);
          }
        }
      }
      if (costHeap.isNotEmpty){
        total += costHeap.first;
      }
    }

    print(total);
  }

  void part2() {}
}
