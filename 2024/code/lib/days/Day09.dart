import 'package:code/days/Day.dart';

class Day09 extends Day {
  List<int> diskMap = [];
  Day09(super.fileName, super.useTestData) {
    diskMap.addAll(input[0].split("").map(int.parse).toList());
  }

  int? intOrNull(String item) {
    if (item == ".") return null;
    return int.parse(item);
  }

  void part1() {
    List<int?> hdd = diskMapToHdd(diskMap);
    List<int> defragmented = defragmentHdd(hdd);
    print(calculateChecksum(defragmented));
  }

  void part2() {}

  // Convert disk map to hdd with null spaces
  List<int?> diskMapToHdd(List<int> diskMap){
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

  // Shuffle files down to the null spaces
  List<int> defragmentHdd(List<int?> hdd){
    List<int?> defragmentedDrive = List.from(hdd);
    int currentIndex = 0;
    int lastNonNullIndex = defragmentedDrive.length - 1;
    while (currentIndex < defragmentedDrive.length){
      if (defragmentedDrive[currentIndex] == null){
        while (defragmentedDrive[lastNonNullIndex] == null){
          lastNonNullIndex--;
        }
        int endEl = defragmentedDrive.elementAt(lastNonNullIndex)!;
        defragmentedDrive[currentIndex] = endEl;
         defragmentedDrive.removeAt(lastNonNullIndex)!;
        lastNonNullIndex--;
      }
      currentIndex++;
    }
    print(defragmentedDrive);
    return defragmentedDrive.whereType<int>().toList();
  }

  int calculateChecksum(List<int> fileSystem){
    int sum = 0;
    for (int i = 0; i < fileSystem.length; i++){
      sum += i * fileSystem[i];
    }
    return sum;
  }
}
