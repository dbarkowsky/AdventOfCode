import 'package:code/days/Day.dart';

class Day05 extends Day {
  // Track which numbers must come before. The element in the value array must come before the key.
  Map<int, List<int>> orders = {};
  List<List<int>> updates = []; // int list of updates
  List<List<int>> incorrectUpdates = []; // Populated in part 1

  Day05(super.fileName, super.useTestData) {
    // Use constructor to parse input
    int index = 0;
    // Get all the order pairs.
    // Makes map where key points to array of numbers that should be before itself
    while (input[index] != "") {
      List<int> orderPair = input[index].split("|").map(int.parse).toList();
      orders.update(orderPair[1], (existing) => [...existing, orderPair[0]],
          ifAbsent: () => [orderPair[0]]);
      index++;
    }
    index++; // to skip blank line
    // Add the update lines
    for (int i = index; i < input.length; i++) {
      updates.add(input[i].split(",").map(int.parse).toList());
    }
  }

  // Check if updates are correct, sum middle number
  void part1() {
    int total = 0;
    for (final update in updates) {
      if (isCorrect(update)) {
        total += getMiddleNumber(update);
      }
    }
    print(total);
  }

  // Take incorrect updates, sort to correct, sum middle number
  void part2() {
    int total = 0;
    for (final update in incorrectUpdates) {
      update.sort((int a, int b) => comparePages(a, b));
      total += getMiddleNumber(update);
    }
    print(total);
  }

  // Compares numbers using the Map of orders
  int comparePages(int a, int b) {
    if (!orders.containsKey(a)) {
      return 0;
    }
    if (orders[a]!.contains(b)) {
      return 1;
    }
    return -1;
  }

  // Checks if an update line is correct
  bool isCorrect(List<int> update) {
    // Travel along update pages
    for (int i = 0; i < update.length; i++) {
      int currentNumber = update[i];
      int currentIndex = i + 1;
      // Check if each subsequent number is actually supposed to be before
      List<int> numbersBefore = orders[currentNumber] ?? [];
      while (currentIndex < update.length) {
        if (numbersBefore.contains(update[currentIndex])) {
          incorrectUpdates.add(update); // For part 2
          return false; // Has something out of order
        }
        currentIndex++;
      }
    }
    return true; // Is already in order
  }

  // Just returns middle (based on index) number of a list
  int getMiddleNumber(List<int> update) {
    int middleIndex = update.length ~/ 2; // How to do integer math
    return update[middleIndex];
  }
}
