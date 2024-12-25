import 'package:code/days/Day.dart';

class Day25 extends Day {
  List<List<int>> locks = [];
  List<List<int>> keys = [];
  Day25(super.fileName, super.useTestData) {
    // Convert grids to keys/locks
    List<List<String>> grid = [];
    for (final row in input) {
      if (row.isEmpty) {
        save(grid);
        grid = [];
        continue;
      }
      // Break row and add to grid
      grid.add(row.split(""));
    }
    // Save one more time to get the last lock/key
    save(grid);
  }

  // Saves the grid as either a lock or a key after converting to numbers
  void save(List<List<String>> grid){
    bool isLock = grid.first.every((s) => s =='#');
        // Transpose grid and add as key/lock
        List<List<String>> transposedInput = [];
        for (int i = 0; i < grid[0].length; i++) {
          String vertical = "";
          for (int j = 0; j < grid.length; j++) {
            vertical += grid[j][i];
          }
          transposedInput.add(vertical.split(""));
        }
        List<int> entry = transposedInput.map(countHash).toList();
        if (isLock){
          // Is a lock
          locks.add(entry);
        } else {
          // Is a key
          keys.add(entry);
        }
  }

  int countHash(List<String> row){
    int count = -1;
    for (final el in row){
      if (el == '#') count++;
    }
    return count;
  }

  void part1() {
    // Check all keys in all locks. Count how many fit.
    int count = 0;
    for (final lock in locks){
      for (final key in keys){
        if (doesKeyFit(key: key, lock: lock)) count++;
      }
    }
    print(count);
  }

  // Part 2 is based on whether you have all previous stars
  void part2() {}

  // Key fits if its height is less than the remainder from the lock's pin
  bool doesKeyFit({required List<int> key, required List<int> lock}){
    int maxPin = 6;
    for (int i = 0; i < key.length; i++){
      if (maxPin - lock[i] <= key[i]) return false;
    }
    return true;
  }
}
