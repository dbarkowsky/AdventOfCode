import 'package:code/days/Day.dart';

class Day22 extends Day {
  Day22(super.fileName, super.useTestData);

  void part1() {
    int sum = 0;
    for (final secretString in input){
      int current = int.parse(secretString);
      int goal = 2000;
      for (int i = 0; i < goal; i++){
        current = prune(mix(current, current * 64));
        current = prune(mix(current, (current / 32).floor()));
        current = prune(mix(current, current * 2048));
      }
      sum += current;
    }
    print(sum);
  }

  void part2() {
    
  }

  int mix(int secret, int value){
    return value^secret;
  }

  int prune(int secret){
    return secret % 16777216;
  }
}
