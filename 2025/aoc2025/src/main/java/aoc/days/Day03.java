package aoc.days;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Stack;

public class Day03 {
  private ArrayList<String> lines;

  public Day03(ArrayList<String> input) {
    lines = input;
  }

  // Turns on the batteries to make the biggest two-digit number
  public void part1() {
    System.out.println("Day 03, Part 1");
    int joltageSum = 0;
    // For each battery bank (row)
    for (String bank : lines) {
      // Break it into a list of numbers
      List<String> batteriesAsStrings = Arrays.asList(bank.split(""));
      ArrayList<Integer> batteries = new ArrayList<Integer>();
      for (String item : batteriesAsStrings) {
        batteries.add(Integer.parseInt(item));
      }

      // Find the biggest number in that list
      int biggestNumber = -1;
      int biggestNumberIndex = -1;
      // size - 1 here because we can't use the last battery as the first of two
      // numbers.
      for (int i = 0; i < batteries.size() - 1; i++) {
        if (batteries.get(i) > biggestNumber) {
          biggestNumber = batteries.get(i);
          biggestNumberIndex = i;
        }
      }
      // We now have the first biggest number and its position. Do it again, but
      // starting after this number.
      int nextNumber = -1;
      for (int i = biggestNumberIndex + 1; i < batteries.size(); i++) {
        if (batteries.get(i) > nextNumber) {
          nextNumber = batteries.get(i);
        }
      }
      // We should now have both numbers. Combine them like strings and add to total.
      String biggestAsString = String.valueOf(biggestNumber) + String.valueOf(nextNumber);
      joltageSum += Integer.parseInt(biggestAsString);
    }
    System.out.println(joltageSum);
  }

  // Second attempt after some research.
  // Totally different approach.
  public void part2() {
    System.out.println("Day 03, Part 2");
    long joltageSum = 0;
    // For each battery bank (row)
    for (String bank : lines) {
      int targetLength = 12;
      Stack<Character> stack = new Stack<>();
      // Iterate left to right over number.
      for (int i = 0; i < bank.length(); i++) {
        char currentNum = bank.charAt(i);

        // This has a few parts:
        // 1. The stack obviously can't be empty
        // 2. We need to make sure there's still 12 items on the stack and there's
        // potential to fill it up to 12 still in the row.
        // That's the stack.size() + (bank.length() - i) > targetLength
        // 3. If the top item on the stack is smaller, we remove it so it can be
        // replaced.
        while (!stack.isEmpty() &&
            stack.size() + (bank.length() - i) > targetLength &&
            stack.peek() < currentNum) {
          stack.pop();
        }

        // Then, if there's room, we replace it with the bigger number.
        if (stack.size() < targetLength) {
          stack.push(currentNum);
        }
      }
      
      // Combining stack to final value here
      String result = "";
      for (char num : stack) {
        result += num;
      }
      joltageSum += Long.valueOf(result);
    }
    System.out.println(joltageSum);
  }

  // Same concept, but turns on twelve batteries.
  // This approach didn't work. It's very greedy, and it messes up with numbers
  // like 234234234234278.
  // It will prioritize the higher number for some reason, so misses smaller
  // numbers that would result in an overall bigger value.
  public void part2Original() {
    // Similar concept to start.
    System.out.println("Day 03, Part 2");
    long joltageSum = 0;
    // For each battery bank (row)
    for (String bank : lines) {
      // Break it into a list of numbers
      List<String> batteriesAsStrings = Arrays.asList(bank.split(""));
      ArrayList<Integer> batteries = new ArrayList<Integer>();
      for (String item : batteriesAsStrings) {
        batteries.add(Integer.parseInt(item));
      }

      // Here it changes.
      // Should be able to get the biggest number by identifying the highest value,
      // then turning that on, right to left.
      // Repeat with next highest value until 12 batteries are turned on.
      // Suspected step that was missing: There's a step missing. After a biggest
      // number is found and all turned on, locate the last point and see if there are
      // sufficient numbers behind it to turn on.
      // If yes, that's our new starting point. Turning on anything left of this point
      // will only make the final number smaller.
      // If no, move left to the next on number and check this again.

      // Store which ones are on in this list. True = on
      boolean[] onOffList = new boolean[batteries.size()];

      int targetOnBatteries = 12;
      int searchStartIndex = 0;
      while (countOnBatteries(onOffList) < targetOnBatteries) {
        // Find highest value
        int biggestNumber = -1;
        for (int i = searchStartIndex; i < batteries.size(); i++) {
          // Don't count as biggest if already on. Prevents getting the same biggest every
          // time.
          if (batteries.get(i) > biggestNumber && !onOffList[i]) {
            biggestNumber = batteries.get(i);
          }
        }

        // Start turning on batteries with that number, right to left.
        // Don't exceed total number allowed on.
        for (int i = batteries.size() - 1; i >= 0 && countOnBatteries(onOffList) < targetOnBatteries; i--) {
          if (batteries.get(i) == biggestNumber) {
            onOffList[i] = true;
          }
        }

        // If we're at the target, we can stop.
        int onBatteries = countOnBatteries(onOffList);
        if (onBatteries == targetOnBatteries)
          break;

        // Identify where the next starting point is. We can't turn numbers on left of
        // an existing ON number if it would make it smaller, but we
        // can't avoid this if there are fewer than the remaining number of OFF numbers
        // to the right of this point.

        int remainingNeeded = targetOnBatteries - onBatteries;
        int batteriesAfterLast = -1;
        int indexOfLastOnBattery = getLastOnBattery(onOffList);
        do {
          // Are there enough numbers after this to get all 12?
          batteriesAfterLast = countOffBatteriesAfterIndex(indexOfLastOnBattery, onOffList);
          if (batteriesAfterLast >= remainingNeeded) {
            // Things are okay, reduce the search space.
            searchStartIndex = indexOfLastOnBattery + 1;
          } else {
            // Not enough numbers to the right, check previous ON number
            indexOfLastOnBattery = getLastOnBattery(onOffList, indexOfLastOnBattery);
          }
        } while (batteriesAfterLast < remainingNeeded);
      }

      // Should now have 12 batteries turned on. Just need to combine them and add to
      // sum
      ArrayList<String> finalNumber = new ArrayList<String>();
      for (int i = 0; i < onOffList.length; i++) {
        if (onOffList[i]) {
          finalNumber.add(batteriesAsStrings.get(i));
        }
      }
      System.out.println(constructNumberAsLong(finalNumber));
      joltageSum += constructNumberAsLong(finalNumber);
    }

    System.out.println(joltageSum);
  }

  private int countOffBatteriesAfterIndex(int index, boolean[] batteries) {
    int count = 0;
    for (int i = index + 1; i < batteries.length; i++) {
      if (!batteries[i])
        count++;
    }
    return count;
  }

  private int getLastOnBattery(boolean[] batteries) {
    return getLastOnBattery(batteries, batteries.length);
  }

  // Assumes at least one is on.
  private int getLastOnBattery(boolean[] batteries, int endingPoint) {
    int index = -1;
    for (int i = 0; i < endingPoint; i++) {
      if (batteries[i])
        index = i;
    }
    return index;
  }

  private String constructNumberAsString(ArrayList<String> nums) {
    String returnValue = "";
    for (String num : nums) {
      returnValue += num;
    }
    return returnValue;
  }

  private long constructNumberAsLong(ArrayList<String> nums) {
    return Long.parseLong(constructNumberAsString(nums));
  }

  private int countOnBatteries(boolean[] batteries) {
    int count = 0;
    for (boolean cell : batteries) {
      if (cell)
        count++;
    }
    return count;
  }
}
