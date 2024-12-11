import 'dart:collection';

import 'package:code/days/Day.dart';

final class Rock<int> extends LinkedListEntry<Rock<int>> {
  int value;
  int id;
  Rock(this.value, this.id);

  @override
  String toString() {
    return '$value';
  }
}

class Day11 extends Day {
  LinkedList<Rock<int>> rocks = LinkedList();
  Day11(super.fileName, super.useTestData);

  LinkedList<Rock<int>> buildRockList() {
    LinkedList<Rock<int>> tempRocks = LinkedList();
    List<int> rockInts = input.first.split(' ').map(int.parse).toList();
    for (int i = 0; i < rockInts.length; i++) {
      tempRocks.add(Rock(rockInts[i], i));
    }
    return tempRocks;
  }

  void part1() {
    rocks = buildRockList();
    int blinks = 25;
    int rockIndex = rocks.length;
    for (int i = 0; i < blinks; i++) {
      Rock<int>? currentRock = rocks.first;
      while (currentRock != null) {
        if (currentRock.value == 0) {
          currentRock.value = 1;
        } else if (currentRock.value.toString().length % 2 == 0) {
          String rockString = currentRock.value.toString();
          int rockLength = rockString.length;
          String left = rockString.substring(0, rockLength ~/ 2);
          String right = rockString.substring(rockLength ~/ 2);
          currentRock.insertBefore(Rock(int.parse(left), rockIndex));
          rockIndex++;
          currentRock.value = int.parse(right);
        } else {
          currentRock.value *= 2024;
        }
        currentRock = currentRock.next;
      }
      // if (i % 5 == 0) print('Blink $i');
    }
    print(rocks.length);
  }

  // This doesn't work. Too many rocks. Need to cache
  void part2() {
    rocks = buildRockList();
    int blinks = 75;
    int rockIndex = rocks.length;
    Rock<int>? currentRock = rocks.first;
    int totalRocks = 0;
    while (currentRock != null) {
      print('current rock $currentRock');
      LinkedList<Rock<int>> localRockList = LinkedList();
      localRockList.add(Rock<int>(currentRock.value, 0));
      for (int i = 0; i < blinks; i++) {
        Rock<int>? localCurrentRock = localRockList.first;
        while (localCurrentRock != null) {
          // print('local current rock $localCurrentRock');
          if (localCurrentRock.value == 0) {
            localCurrentRock.value = 1;
          } else if (localCurrentRock.value.toString().length % 2 == 0) {
            String rockString = localCurrentRock.value.toString();
            int rockLength = rockString.length;
            String left = rockString.substring(0, rockLength ~/ 2);
            String right = rockString.substring(rockLength ~/ 2);
            localCurrentRock.insertBefore(Rock(int.parse(left), rockIndex));
            rockIndex++;
            localCurrentRock.value = int.parse(right);
          } else {
            localCurrentRock.value *= 2024;
          }
          localCurrentRock = localCurrentRock.next;
        }
              if (i % 5 == 0) print('Blink $i');

      }
      totalRocks += localRockList.length;
      currentRock = currentRock.next;
      // print(localRockList);
    }
    print(totalRocks);
  }
}
