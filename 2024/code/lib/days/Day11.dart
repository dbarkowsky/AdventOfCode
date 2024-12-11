import 'dart:collection';

import 'package:code/days/Day.dart';

final class Rock<int> extends LinkedListEntry<Rock<int>> {
  int value;
  Rock(this.value);

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
      tempRocks.add(Rock(rockInts[i]));
    }
    return tempRocks;
  }

  void part1() {
    rocks = buildRockList();
    int blinks = 25;
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
          currentRock.insertBefore(Rock(int.parse(left)));
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
    Rock<int>? currentRock = rocks.first;
    Map<String, int> cache = {};
    int totalRocks = 0;
    while (currentRock != null) {
      int rockCount = countRockWithCache(currentRock, blinks, cache);
      totalRocks += rockCount;
      currentRock = currentRock.next;
    }
    print(totalRocks);
  }

  int countRockWithCache(Rock<int> rock, int blinks, Map<String, int> cache) {
    String key = '${rock.value}-$blinks';
    if (cache.containsKey(key)) {
      return cache[key]!;
    } else {
      int rockCount = performBlinksForCount(rock, blinks, cache);
      cache[key] = rockCount;
      return rockCount;
    }
  }

  int performBlinksForCount(
      Rock<int> rock, int remainingBlinks, Map<String, int> cache) {
    int count = 0;
    String key = '${rock.value}-$remainingBlinks';

    if (remainingBlinks == 0)
      count = 1;
    else if (cache.containsKey(key)) {
      return cache[key]!;
    }
    if (rock.value == 0) {
      return performBlinksForCount(Rock(1), remainingBlinks - 1, cache);
    } else if (rock.value.toString().length % 2 == 0) {
      String rockString = rock.value.toString();
      int rockLength = rockString.length;
      String left = rockString.substring(0, rockLength ~/ 2);
      String right = rockString.substring(rockLength ~/ 2);
      return performBlinksForCount(
              Rock(int.parse(left)), remainingBlinks - 1, cache) +
          performBlinksForCount(
              Rock(int.parse(right)), remainingBlinks - 1, cache);
    } else {
      return performBlinksForCount(
          Rock(rock.value * 2024), remainingBlinks - 1, cache);
    }
  }
}
