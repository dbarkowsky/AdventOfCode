import 'package:code/days/Day.dart';

class Day22 extends Day {
  Day22(super.fileName, super.useTestData);

  // Get the sum of the 2000th secret number for each monkey
  void part1() {
    int sum = 0;
    for (final secretString in input) {
      int current = int.parse(secretString);
      int goal = 2000;
      // This is just the pattern for each step in the secret number calculation
      for (int i = 0; i < goal; i++) {
        current = prune(mix(current, current * 64));
        current = prune(mix(current, (current / 32).floor()));
        current = prune(mix(current, current * 2048));
      }
      sum += current;
    }
    print(sum);
  }

  // Which code will result in the best overall return from the monkeys
  void part2() {
    // Store the sum for each code
    Map<String, int> priceSums = {};
    // For each monkey's secret
    for (final secretString in input) {
      int current = int.parse(secretString);
      int goal = 2000;
      List<int> prices = [current % 10];
      List<int> priceDiffs = [];
      for (int i = 0; i < goal - 1; i++) {
        current = prune(mix(current, current * 64));
        current = prune(mix(current, (current / 32).floor()));
        current = prune(mix(current, current * 2048));
        // Add the price and the difference for each step
        prices.add(current % 10);
        priceDiffs.add(prices.last - prices[prices.length - 2]);
      }
      // Now that we know prices and differences, we can loop through 4 differences at a time
      // We'll use those differences together as the key, then update the highest price seen for that key
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
