package aoc.days;

import java.util.ArrayList;

public class Day06 {
  // We'll make parallel arrays. Store numbers here
  private ArrayList<ArrayList<Integer>> numbers = new ArrayList<>();
  // Store operators in this array
  private ArrayList<String> operators = new ArrayList<>();

  public Day06(ArrayList<String> input) {
    // Need to first transpose the input.

    // Add some blank arraylists
    for (int i = 0; i < input.size() - 1; i++) {
      numbers.add(new ArrayList<>());
    }
    int lastIndex = input.size() - 1;

    for (String operator : input.get(lastIndex).split("[ ]+")) {
      operators.add(operator);
    }
    // Store numbers in a grid.
    for (int rowIndex = input.size() - 2; rowIndex >= 0; rowIndex--) {
      // Start from the bottom and work our way up.
      for (String stringValue : input.get(rowIndex).trim().split("[ ]+")) {
        numbers.get(rowIndex).add(Integer.parseInt(stringValue));
      }
    }
  }

  public void part1() {
    System.out.println("Day 06, Part 1");
    long sum = 0;
    // For as many entries as we have
    for (int entryIndex = 0; entryIndex < operators.size(); entryIndex++){
      // Perform the operation on all numbers on that location
      String operator = operators.get(entryIndex);
      // First one is here.
      long result = numbers.get(0).get(entryIndex);
      for (int numberIndex = 1; numberIndex < numbers.size(); numberIndex++){
        if (operator.equals("+")){
          result += numbers.get(numberIndex).get(entryIndex);
        } else {
          // Only other option is *
          result *= numbers.get(numberIndex).get(entryIndex);
        }
      }
      sum += result;
    }
    System.out.println(sum);
  }

  public void part2() {
    System.out.println("Day 06, Part 2");

  }
}
