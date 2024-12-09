import 'package:code/days/Day.dart';

class Day09 extends Day {
  List<int> diskMap = [];
  Day09(super.fileName, super.useTestData) {
    diskMap.addAll(input[0].split("").map(int.parse).toList());
  }

  void part1() {
    List<int?> hdd = diskMapToHdd(diskMap);
    List<int> defragmented = defragmentHdd(hdd);
    print(calculateChecksum(defragmented));
  }

  void part2() {
    List<int?> hdd = diskMapToHdd(diskMap);
    List<int?> defragmented = defragmentHddInChunks(hdd);
    print(calculateChecksum(defragmented));
  }

  // Convert disk map to hdd with null spaces. e.g. [00..11..2] where . = null
  List<int?> diskMapToHdd(List<int> diskMap) {
    List<int?> hdd = [];
    int diskMapIndex = 0;
    int fileId = 0;
    while (diskMapIndex < diskMap.length) {
      int count = diskMap[diskMapIndex];
      int? insertValue = diskMapIndex % 2 == 0 ? fileId : null;
      if (insertValue == null) fileId++;
      hdd.addAll(List.filled(count, insertValue));
      diskMapIndex++;
    }
    return hdd;
  }

  // Shuffle files down to the null spaces, but breaks them up if needed
  List<int> defragmentHdd(List<int?> hdd) {
    List<int?> defragmentedDrive = List.from(hdd);
    int currentIndex = 0;
    int lastNonNullIndex =
        defragmentedDrive.lastIndexWhere((test) => test != null);
    bool sorted = false;
    while (!sorted) {
      // If we find a null space...
      if (defragmentedDrive[currentIndex] == null) {
        // Swap current and lastNonNull
        int temp = defragmentedDrive[lastNonNullIndex]!;
        defragmentedDrive[currentIndex] = temp;
        defragmentedDrive[lastNonNullIndex] = null;
        // Move last pointer to next non-null
        lastNonNullIndex =
            defragmentedDrive.lastIndexWhere((test) => test != null);
      }
      // Advance to next space
      currentIndex++;
      // We're done if we've passed all the non-null spaces
      if (currentIndex >= lastNonNullIndex) sorted = true;
    }
    return defragmentedDrive.whereType<int>().toList();
  }

  // Shuffle files to start of drive, but don't break those files up
  List<int?> defragmentHddInChunks(List<int?> hdd) {
    List<int?> defragmentedDrive = List.from(hdd);
    // Find last group of numbers
    (int, int) lastGroup = findLastGroup(defragmentedDrive, defragmentedDrive.length - 1);
      // Find starting gap start and end
      (int, int) nullSpace = findNullSpace(defragmentedDrive);
    while (lastGroup.$1 > 0) {
      bool suitableNullSpaceFound = (nullSpace.$2 - nullSpace.$1) >= (lastGroup.$2 - lastGroup.$1);
      // If this null space is past the group
      if (nullSpace.$1 > lastGroup.$2) {
        // We didn't find anything, so go to next group
        lastGroup = findLastGroup(defragmentedDrive, lastGroup.$1 - 1);
        // And reset null space to first one
        nullSpace = findNullSpace(defragmentedDrive);
        // If this is the first/last group, we're done
        if (lastGroup.$1 == 0) break;
      } else
      // If the group doesn't fit in the space
      if (!suitableNullSpaceFound) {
        // Find the next null space that it will fit
        nullSpace = findNullSpace(defragmentedDrive, startingAt: nullSpace.$2 + 1);
      } else {
        // Group would fit in current null space
        // Copy group id into null spaces
        int groupId = defragmentedDrive[lastGroup.$1]!;
        for (int i = nullSpace.$1; i < (nullSpace.$1 + (lastGroup.$2 - lastGroup.$1 + 1)); i++){
          defragmentedDrive[i] = groupId;
        }
        // Nullify the original location
        for (int i = lastGroup.$1; i < (lastGroup.$1 + (lastGroup.$2 - lastGroup.$1 + 1)); i++){
          defragmentedDrive[i] = null;
        }
        // Identify next group and first null space
        lastGroup = findLastGroup(defragmentedDrive, lastGroup.$1 - 1);
        nullSpace = findNullSpace(defragmentedDrive);
      }
    }

    return defragmentedDrive;
  }

  // Finds the start and end of the last group of non-null items in the list
  (int, int) findLastGroup(List<int?> drive, int startingAt) {
    if (startingAt <= 0) return (-1, -1); // Can't start before list
    int endGroup = drive.lastIndexWhere((test) => test != null, startingAt);
    if (endGroup == -1) return (-1, -1); // If there was no non-null character, why are we here?
    int groupIcon = drive[endGroup]!;
    int startGroup = endGroup;
    // Where know where the last one is, but where does this group start?
    while (startGroup > 0 && drive[startGroup] != null && drive[startGroup] == groupIcon) {
      startGroup--;
    }
    startGroup++; // To make it not one extra
    return (startGroup, endGroup);
  }

  // Finds the start and end of the first null group of indexes
  (int, int) findNullSpace(List<int?> drive, {int startingAt = 0}) {
    int startNull = drive.indexWhere((test) => test == null, startingAt);
    int endNull = startNull;
    while (endNull < drive.length && drive[endNull] == null) {
      endNull++;
    }
    endNull--; // To make it not one extra
    return (startNull, endNull);
  }

  // Checksum is the sum of every (index * the file's id number)
  int calculateChecksum(List<int?> fileSystem) {
    int sum = 0;
    for (int i = 0; i < fileSystem.length; i++) {
      if (fileSystem[i] != null) {
        sum += i * fileSystem[i]!;
      }
    }
    return sum;
  }
}
