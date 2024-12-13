import 'package:collection/collection.dart';
import 'package:code/days/Day.dart';
import 'dart:core';

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
    input.add(""); // For even loops
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
          // Blank line, we add the game
          games.add(Game(aX, aY, bX, bY, prizeX, prizeY));
          break;
        default:
      }
    }
  }

  // Brute forced part 1. Find lowest possible outcome
  // Expected part 2 would ask for all outcomes, so stored them in heap
  void part1() {
    int aCost = 3;
    int bCost = 1;
    int total = 0;
    for (final game in games) {
      HeapPriorityQueue<int> costHeap =
          HeapPriorityQueue<int>((a, b) => a.compareTo(b));
      // For every possibility, see if it matches prize values
      for (int a = 0; a <= 100; a++) {
        for (int b = 0; b <= 100; b++) {
          int xTotal = (a * game.aX) + (b * game.bX);
          int yTotal = (a * game.aY) + (b * game.bY);
          if (xTotal == game.prizeX && yTotal == game.prizeY) {
            // Add cost
            int cost = (a * aCost) + (b * bCost);
            costHeap.add(cost);
          }
        }
      }
      if (costHeap.isNotEmpty) {
        total += costHeap.first;
      }
    }

    print(total);
  }

  // Couldn't do this with brute force. Had to look up the math required.
  // Linear equations used to find ideal point of prizeX, prizeY
  void part2() {
    int aCost = 3;
    int bCost = 1;
    int total = 0;
    int goalAdjustment = 10000000000000; // 10 trillion :'(
    for (final game in games) {
      // Starting with these formulae: (aX)a + (bX)b = goalX  and (aY)a + (bY)b = goalY
      // So b = (goalX - (aX)a) / bX; but also b = (goalY - (aY)a) / bY;
      // this means that: (goalX - (aX)a) / bX = (goalY - (aY)a) / bY
      // Multiply the bottom terms to get: (goalX * bY) − (a * aX * bY) = (goalY * bX) − (a * aY *bX)
      // Isolate b:  (aY * bX)a + (goalX * bY) - (aX * bY)a = goalY * bX
      // Group terms: ((aY * bX) - (bY * aX))a = goalY * bX - (goalX * bY)
      // Then islolate a: a = (goalY * bX - (goalX * bY)) / ((aY * bX) - (bY * aX))
      int goalX = goalAdjustment + game.prizeX;
      int goalY = goalAdjustment + game.prizeY;
      int numerator = (game.bX * goalY - game.bY * goalX);
      int denominator = (game.bX * game.aY - game.bY * game.aX);
      // If there is a greatest common denominator...
      if (numerator.gcd(denominator) != 1) {
        // We can assume integer math will work
        int a = numerator ~/ denominator;
        int b = (goalY - game.aY * a) ~/ game.bY;
        // Then resume like part 1
        int xTotal = (a * game.aX) + (b * game.bX);
        int yTotal = (a * game.aY) + (b * game.bY);
        if (xTotal == goalX && yTotal == goalY) {
          // Add cost
          int cost = (a * aCost) + (b * bCost);
          total += cost;
        }
      }
    }
    print(total);
  }
}
