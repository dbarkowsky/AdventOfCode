import 'package:code/days/Day.dart';

class Day02 extends Day {
  Day02(super.fileName, super.useTestData);

  void part1({int allowedFaults = 0}) async {
    int sum = 0;

    // Checking if each line is valid
    for (String line in input) {
      List<int> items = line.split(" ").map((el) => int.parse(el)).toList();
      if (await lineIsGood(items, allowedFaults)) {
        sum++;
      }
    }
    print(sum);
  }

  Future<bool> lineIsGood(List<int> items, int allowedFaults)async{
    bool? generalAscending; // Will determine during loop
    int prev = items[0];
    for (int i = 1; i < items.length; i++) {
        bool? localAscending = items[i] > prev
            ? true
            : items[i] < prev
                ? false
                : null;
        generalAscending ??= localAscending; // only if still undetermined (null)
        int difference = (prev - items[i]).abs();
        // Is this row valid? 
        if (isInvalid(localAscending, generalAscending, difference)) {
          // No remaining faults. No good.
          if (allowedFaults == 0){
            return false;
          }

          // If we have allowed faults still... recursion!

          // Making copies of the list with either value removed
          List<int> withLeftRemoved = List<int>.from(items);
          withLeftRemoved.removeAt(i-1);
          List<int> withRightRemoved = List<int>.from(items);
          withRightRemoved.removeAt(i);
          // A list of possible outcomes depending on which value was removed and one less fault tolerance
          List<Future<bool>> possibleFutures = [lineIsGood(withLeftRemoved, allowedFaults - 1),lineIsGood(withRightRemoved, allowedFaults - 1)];

          // Horrible edge case where we might have to remove the first value further back
          // e.g. 48 46 47 49 51 54 56  -> it's the 48 that must be removed, but we won't find error until 46/47 check
          if (i - 2 >= 0){
            List<int> withFarLeftRemoved = List<int>.from(items);
            withFarLeftRemoved.removeAt(i - 2);
            possibleFutures.add(lineIsGood(withFarLeftRemoved, allowedFaults - 1));
          }
          // Wait for futures to be resolved, then see if any are true.
          List<bool> childrenAreGood = await Future.wait(possibleFutures);
          return childrenAreGood.any((child) => child);
        } else {
          // Just moving down the list.
          prev = items[i];
        }
      }
    return true; // All numbers in this line were good!
  }

  // Checks if the row only ascends/descends and has a difference from 1-3
  bool isInvalid(bool? localAscending, bool? generalAscending, int difference) {
    return localAscending != generalAscending ||
        difference > 3 || difference <= 0;
  }

  void part2() {
    part1(allowedFaults: 1);
  }
}
