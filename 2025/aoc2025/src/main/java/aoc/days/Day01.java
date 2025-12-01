package aoc.days;

import java.util.ArrayList;

public class Day01 {

  private ArrayList<String> lines;

  public Day01(ArrayList<String> input) {
    lines = input;
  }

  public void Part1() {
    // Implement Part 1 logic here
    System.out.println("Day 01, Part 1");
    int currentValue = 50;
    int zeroCount = 0;
    for (String line : lines) {
      // System.out.println(line);
      // Direction is first character
      String direction = line.substring(0, 1);
      int steps = Integer.parseInt(line.substring(1, line.length()));
      // Turn dial left or right
      int reducedMovement = steps % 100;
      if (direction.equals("R")) {
        currentValue += reducedMovement;
        if (currentValue >= 100)
          currentValue = currentValue % 100;
      } else {
        currentValue -= reducedMovement;
        if (currentValue < 0) {
          currentValue = 100 + currentValue;
        }
      }
      // System.out.printf("Current value: %d\n", currentValue);
      if (currentValue == 0)
        zeroCount++;
    }
    System.out.println(zeroCount);
  }

  public void Part2() {
    // Implement Part 1 logic here
    System.out.println("Day 01, Part 1");
    int currentValue = 50;
    int zeroCount = 0;
    for (String line : lines) {
      // System.out.println(line);
      // Direction is first character
      String direction = line.substring(0, 1);
      int steps = Integer.parseInt(line.substring(1, line.length()));
      // Turn dial left or right
      int reducedMovement = steps % 100;
      // System.out.printf("%s, %d, %d\n", line, currentValue, reducedMovement);
      // Changed how we look for zero counts.
      if (direction.equals("R")) {
        // Right is easy. We just look at how many times the whole thing can be divided
        // by the dial size.
        zeroCount += (currentValue + steps) / 100;
        currentValue = (currentValue + reducedMovement) % 100;
      } else {
        // Left sucks.
        // We don't want to count the case where we start at 0 then move left as a
        // roll-over.
        // This only counts if we aren't starting at 0.
        int firstHit = (currentValue == 0 ? 100 : currentValue);
        if (steps >= firstHit) {
          zeroCount += 1 + (steps - firstHit) / 100; // Note minimum of 1.
        }
        // Then update current value
        currentValue -= reducedMovement;
        if (currentValue < 0) {
          currentValue = 100 + currentValue;
        }
      }
      // System.out.printf("Zero count: %d\n", zeroCount);
      // System.out.printf("Current value: %d\n", currentValue);
    }
    System.out.println(zeroCount);
  }
}
