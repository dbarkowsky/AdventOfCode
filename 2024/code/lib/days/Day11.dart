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
  Map<String, int> cache = {}; // Need this for part 2.
  Day11(super.fileName, super.useTestData);

  // Turns the input into a linked list of Rocks
  LinkedList<Rock<int>> buildRockList() {
    LinkedList<Rock<int>> tempRocks = LinkedList();
    List<int> rockInts = input.first.split(' ').map(int.parse).toList();
    for (int i = 0; i < rockInts.length; i++) {
      tempRocks.add(Rock(rockInts[i]));
    }
    return tempRocks;
  }

  // This only works for a small number of blinks. List gets too long otherwise.
  // See part 2 for a method that is much more scalable.
  void part1() {
    rocks = buildRockList();
    int blinks = 25; // Don't increase this. Your computer will explode.
    // For the number of blinks
    for (int i = 0; i < blinks; i++) {
      // Get the first rock, and traverse until there is no more next rock
      Rock<int>? currentRock = rocks.first;
      while (currentRock != null) {
        // Follow blink rules to get next value/rocks
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
        // And move on to next rock
        currentRock = currentRock.next;
      }
    }
    // How many rocks do we have at the end?
    print(rocks.length);
  }

  // Uses the Map cache to remember previously calculated results.
  // This avoids doing the calculation when we already know the outcome.
  void part2() {
    rocks = buildRockList();
    int blinks = 75;
    Rock<int>? currentRock = rocks.first;
    int totalRocks = 0;
    // Still traverse through rocks.
    while (currentRock != null) {
      // We want to store the result based on rock value and blinks remaining
      String key = '${currentRock.value}-$blinks';
      // Get the count of the final number of rocks for this value rock
      int rockCount = performBlinksForCount(currentRock, blinks);
      // Add to total
      totalRocks += rockCount;
      // Cache result for later
      cache[key] = rockCount;
      // Do next rock
      currentRock = currentRock.next;
    }
    print(totalRocks);
  }

  // Recursive function to get and store count values for rock given x amount of blinks
  int performBlinksForCount(Rock<int> rock, int remainingBlinks) {
    String key = '${rock.value}-$remainingBlinks';
    // If we've already calculated this, just return the end result
    if (cache.containsKey(key)) {
      return cache[key]!;
    }
    // Otherwise, we have to calculate. Follows blink rules.
    int count = 0;
    if (remainingBlinks == 0) {
      count = 1;
    } else if (rock.value == 0) {
      count = performBlinksForCount(Rock(1), remainingBlinks - 1);
    } else if (rock.value.toString().length % 2 == 0) {
      String rockString = rock.value.toString();
      int rockLength = rockString.length;
      String left = rockString.substring(0, rockLength ~/ 2);
      String right = rockString.substring(rockLength ~/ 2);
      count = performBlinksForCount(
              Rock(int.parse(left)), remainingBlinks - 1) +
          performBlinksForCount(Rock(int.parse(right)), remainingBlinks - 1);
    } else {
      count =
          performBlinksForCount(Rock(rock.value * 2024), remainingBlinks - 1);
    }
    // Save the result to the cache before returning it
    cache[key] = count;
    return count;
  }
}
