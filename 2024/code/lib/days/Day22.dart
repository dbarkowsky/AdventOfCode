import 'package:code/days/Day.dart';

class Day22 extends Day {
  Day22(super.fileName, super.useTestData);

  void part1() {
    int sum = 0;
    for (final secretString in input) {
      int current = int.parse(secretString);
      int goal = 2000;
      for (int i = 0; i < goal; i++) {
        current = prune(mix(current, current * 64));
        current = prune(mix(current, (current / 32).floor()));
        current = prune(mix(current, current * 2048));
      }
      sum += current;
    }
    print(sum);
  }

  void part2() {
    Map<String, int> priceSums = {};
    for (final secretString in input) {
      int current = int.parse(secretString);
      int goal = 2000;
      List<int> prices = [current % 10];
      List<int> priceDiffs = [];
      for (int i = 0; i < goal - 1; i++) {
        current = prune(mix(current, current * 64));
        current = prune(mix(current, (current / 32).floor()));
        current = prune(mix(current, current * 2048));
        prices.add(current % 10);
        priceDiffs.add(prices.last - prices[prices.length - 2]);
      }
      Map<String, int> highestPrices = {};
      int priceIndex = 4; // First time that we have four consecutive changes
      for (int diffIndex = 3; diffIndex < priceDiffs.length; diffIndex++) {
        String key = priceDiffs.sublist(diffIndex - 3, diffIndex + 1).join(",");
        highestPrices.update(key, (value) => value,
            ifAbsent: () => prices[priceIndex]);
        priceIndex++;
      }
      // Add these new totals to the price tracker
      for (final entry in highestPrices.entries) {
        priceSums.update(entry.key, (value) => value + entry.value,
            ifAbsent: () => entry.value);
      }
    }
    // Identify combo with highest sum
    List<(String, int)> highestPricesList =
        priceSums.entries.map((el) => (el.key, el.value)).toList();
    highestPricesList.sort((a, b) => b.$2 - a.$2);
    print(highestPricesList.first);
  }

  int mix(int secret, int value) {
    return value ^ secret;
  }

  int prune(int secret) {
    return secret % 16777216;
  }
}
