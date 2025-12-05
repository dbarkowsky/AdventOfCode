package aoc.days;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

public class Day05 {
  Map<Long, Long> rangeMap = new HashMap<Long, Long>();
  ArrayList<Long> ingredients = new ArrayList<>();

  public Day05(ArrayList<String> input) {
    // Build the map out without any overlaps.
    int inputIndex = 0;
    while (input.get(inputIndex) != "") {
      long start = Long.parseLong(input.get(inputIndex).split("-")[0]);
      long end = Long.parseLong(input.get(inputIndex).split("-")[1]);
      // // For the first one, just add it
      // if (rangeMap.isEmpty()){
      // rangeMap.put(start, end);
      // }
      // // Not empty, find the first entry that's lower than this one.
      addToMap(rangeMap, start, end);
      inputIndex++;
    }
    // Then put all the ingredients in a list
    inputIndex++;
    for (int i = inputIndex; i < input.size(); i++) {
      ingredients.add(Long.parseLong(input.get(i)));
    }
  }

  public void part1() {
    System.out.println("Day 05, Part 1");
    int freshIngredientCount = 0;
    ArrayList<Long> keys = new ArrayList<Long>(rangeMap.keySet());
    keys.sort((a, b) -> b.compareTo(a)); // Reverse order
    for (long ingredient : ingredients) {
      if (isFresh(keys, ingredient)){
        freshIngredientCount++;
      }
    }
    System.out.println(freshIngredientCount);
  }

  public void part2() {
    System.out.println("Day 05, Part 2");
  }

  private boolean isFresh(ArrayList<Long> keys, long ingredient) {
    for (long start : keys) {
      long end = rangeMap.get(start);
      // Does this ingredient fit in the range?
      if (start <= ingredient) {
        if (ingredient <= end)
          return true;
        // Then can return false if not. Cannot be in lower ranges
        return false;
      }
    }
    return false;
  }

  // Did this same thing for 2024 Day 05 recently.
  // Should add a key:value range to the map while dealing with any overlap
  // Consolidates ranges that overlap entry.
  private void addToMap(Map<Long, Long> rangeMap, long start, long end) {
    // Copies so we can update in this method
    long s = start;
    long e = end;

    ArrayList<Long> keys = new ArrayList<Long>(rangeMap.keySet());
    keys.sort((a, b) -> a.compareTo(b));
    for (Long key : keys) {
      long value = rangeMap.get(key);
      if (value + 1 < s)
        continue;
      if (e + 1 < key)
        break;

      s = Math.min(s, key);
      e = Math.max(e, value);
      rangeMap.remove(key);
    }

    rangeMap.put(s, e);
  }
}
