package aoc.days;

import java.util.ArrayList;
import java.util.stream.Collectors;

public class Day06 {
  // We'll make parallel arrays. Store numbers here
  private ArrayList<ArrayList<Integer>> numbers = new ArrayList<>();
  // Store operators in this array
  private ArrayList<String> operators = new ArrayList<>();
  // For part 2
  private ArrayList<ArrayList<Integer>> transposedEntries = new ArrayList<>();

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

    // Realized for part 2 you also need to know the whitespace.
    // New input specifically for that part.
    createPart2NumberList(input);
  }

  private void createPart2NumberList(ArrayList<String> input) {
    // Need to do this based on number of spaces...
    // Order should be the same, so we'll use the operators from before
    // Fortunately, all the rows are the same length
    ArrayList<String[]> rowsAsArrays = new ArrayList<>();
    for (int rowIndex = 0; rowIndex < input.size() - 1; rowIndex++) {
      rowsAsArrays.add(input.get(rowIndex).split(""));
    }

    // Let's move left to right.
    // Build out the new entries as we go
    // If all values in this column are blank, add to the list and proceed.
    ArrayList<String> currentEntryNumbers = new ArrayList<>();

    for (int columnIndex = 0; columnIndex < rowsAsArrays.get(0).length; columnIndex++) {
      // Build the vertical number
      String verticalNumber = "";
      for (int rowIndex = 0; rowIndex < rowsAsArrays.size(); rowIndex++) {
        verticalNumber += rowsAsArrays.get(rowIndex)[columnIndex];
      }
      if (verticalNumber.isBlank()) {
        // Add current entry numbers to numbers with whitespace list
        transposedEntries.add(convertStringListToIntList(currentEntryNumbers));
        // Clear current entry numbers
        currentEntryNumbers.clear();
      } else {
        // include this number in this entry
        currentEntryNumbers.add(verticalNumber);
      }
    }
    // It's possible we get to the end of the columns but there's no whitespace
    // Add the last value just in case
    transposedEntries.add(convertStringListToIntList(currentEntryNumbers));
  }

  private ArrayList<Integer> convertStringListToIntList(ArrayList<String> list) {
    return new ArrayList<>(list.stream().map(item -> Integer.parseInt(item.trim())).collect(Collectors.toList()));
  }

  // First, just perform operations on a column of numbers
  public void part1() {
    System.out.println("Day 06, Part 1");
    long sum = 0;
    // For as many entries as we have
    for (int entryIndex = 0; entryIndex < operators.size(); entryIndex++) {
      // Perform the operation on all numbers on that location
      String operator = operators.get(entryIndex);
      // First one is here.
      long result = numbers.get(0).get(entryIndex);
      for (int numberIndex = 1; numberIndex < numbers.size(); numberIndex++) {
        if (operator.equals("+")) {
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

  // Attempt 2
  // Realized that the whitespace mattered, because not all numbers are
  // right-aligned.
  // This time, each position is a column of numbers. So...
  // 13
  // 45
  // +
  // Is actually 35 + 14; Thankfully it's still only * and +
  public void part2() {
    System.out.println("Day 06, Part 2");
    long sum = 0;
    // We have numbers transposed already
    for (int entryIndex = 0; entryIndex < operators.size(); entryIndex++) {
      // Perform the operation on all numbers on that location
      String operator = operators.get(entryIndex);
      // Get new numbers made from columns
      ArrayList<Integer> transposedNumbers = transposedEntries.get(entryIndex);
      // First one is here.
      long result = transposedNumbers.get(0);
      for (int numberIndex = 1; numberIndex < transposedNumbers.size(); numberIndex++) {
        if (operator.equals("+")) {
          result += transposedNumbers.get(numberIndex);
        } else {
          // Only other option is *
          result *= transposedNumbers.get(numberIndex);
        }
      }
      sum += result;
    }
    System.out.println(sum);
  }

  // Given a list of numbers, create a new list of numbers
  // based on the column reading direction
  // Original transpose when I thought things were right-aligned
  // Now unused
  @SuppressWarnings("unused")
  private ArrayList<Integer> transposeToColumns(int entryIndex) {
    ArrayList<Integer> returnNumbers = new ArrayList<>();
    // For this entry, get all numbers and put them in a list
    // Do so as a list of String though.
    ArrayList<String> currentNumbers = new ArrayList<>();
    for (int numberIndex = 0; numberIndex < numbers.size(); numberIndex++) {
      currentNumbers.add(String.valueOf(numbers.get(numberIndex).get(entryIndex)));
    }
    // Then pop off the last value if there is one until all three arrays are empty
    // Concat these values to as new number, then insert that number in the list
    while (!everyStringIsBlank(currentNumbers)) {
      String newNumber = "";
      for (int i = 0; i < currentNumbers.size(); i++) {
        if (!currentNumbers.get(i).isBlank()) {
          String currentNumber = currentNumbers.get(i);
          newNumber += currentNumber.substring(currentNumber.length() - 1);
          // Then set this trimmed number back in list
          currentNumbers.remove(i);
          currentNumbers.add(i, currentNumber.substring(0, currentNumber.length() - 1));
        }
      }
      returnNumbers.add(Integer.parseInt(newNumber));
    }

    return returnNumbers;
  }

  private boolean everyStringIsBlank(ArrayList<String> list) {
    for (String entry : list) {
      if (!entry.isBlank())
        return false;
    }
    return true;
  }
}
