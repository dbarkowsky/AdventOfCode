package aoc.days;

import java.util.ArrayList;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class Day02 {

  private String[] ranges;

  public Day02(ArrayList<String> input) {
    ranges = input.get(0).split(",");
  }

  /**
   * Looking for repeating numbers, were one half is the same as the other. e.g.
   * 123123.
   */
  public void Part1() {
    System.out.println("Day 02, Part 1");
    long repeatingValueSum = 0;
    // For each range
    for (String range : ranges) {
      // Get start and end of range
      long start = Long.parseLong(range.split("-")[0]);
      long end = Long.parseLong(range.split("-")[1]);

      // Loop through range and look for repeating (invalid) IDs
      for (long i = start; i <= end;) {
        if (HasRepeatingHalves(i)) {
          repeatingValueSum += i;
        }
        i++;
        // If the size is odd, we can skip to the next number of even size.
        // Not true for part 2
        if (String.valueOf(i).length() % 2 == 1) {
          String nextValue = "1";
          for (int j = 0; j < String.valueOf(i).length(); j++) {
            nextValue += "0";
          }
          i = Long.valueOf(nextValue);
        }
      }
    }
    System.out.println(repeatingValueSum);
  }

  /**
   * Now any digit entirely made of repeating numbers counts. e.g. 121212.
   */
  public void Part2() {
    System.out.println("Day 02, Part 2");
    long repeatingValueSum = 0;
    // For each range
    for (String range : ranges) {
      // Get start and end of range
      long start = Long.parseLong(range.split("-")[0]);
      long end = Long.parseLong(range.split("-")[1]);

      // Loop through range and look for repeating (invalid) IDs
      for (long i = start; i <= end;) {
        if (HasRepeatingChunks(i)) {
          repeatingValueSum += i;
        }
        i++;
      }
    }
    System.out.println(repeatingValueSum);
  }

  // Just checks if both halves are the same string
  private boolean HasRepeatingHalves(long num) {
    String stringNum = String.valueOf(num);
    // Can't be repeating if odd length
    if (stringNum.length() % 2 == 1)
      return false;
    String left = stringNum.substring(0, stringNum.length() / 2);
    String right = stringNum.substring((stringNum.length() / 2));
    if (left.equals(right))
      return true;
    return false;
  }

  // Starts with the half division, but then breaks it down into smaller and
  // smaller divisions.
  private boolean HasRepeatingChunks(long num) {
    String stringNum = String.valueOf(num);
    // Could now potentially repeat if odd. e.g. 111
    // Start with halves, but if a repeating value isn't found, break it up by the
    // next viable division
    // Starts as 2 if even, but we'll check if that divides the value evenly
    int validDivision = 2;
    while (validDivision <= stringNum.length()) {
      // If it doesn't divide by this division evenly, then it can't be repeating.
      // Break into more divisions and try again.
      if (stringNum.length() % validDivision != 0) {
        validDivision++;
        continue;
      }
      // Break num into these chunks.
      int divisionSize = stringNum.length() / validDivision;
      ArrayList<String> chunks = new ArrayList<String>();
      for (int i = 0; i < stringNum.length(); i += divisionSize) {
        chunks.add(stringNum.substring(i, i + divisionSize));
      }
      // Assume true to start, identify if all the chunks match.
      boolean allMatch = true;
      String firstChunk = chunks.get(0);
      for (int i = 0; i < chunks.size(); i++) {
        if (!chunks.get(i).equals(firstChunk)) {
          allMatch = false;
          break;
        }
      }

      // If matching, we're done, if not, divide more.
      if (allMatch)
        return true;
      validDivision++;
    }
    return false;
  }

  // Then I realized regex was an option
  // I thought this would be faster, but it's actually slower somehow.
  public void UseRegex() {
    Pattern part1Pattern = Pattern.compile("^(.+)\\1$");
    Pattern part2Pattern = Pattern.compile("^(.+)\\1+$");
    long part1Sum = 0;
    long part2Sum = 0;
    // For each range
    for (String range : ranges) {
      // Get start and end of range
      long start = Long.parseLong(range.split("-")[0]);
      long end = Long.parseLong(range.split("-")[1]);

      // Loop through range and look for repeating (invalid) IDs
      for (long i = start; i <= end; i++) {
        if (part1Pattern.matcher(String.valueOf(i)).find()) part1Sum += i;
        if (part2Pattern.matcher(String.valueOf(i)).find()) part2Sum += i;
      }
    }
    System.out.printf("Part 1: %d\n", part1Sum);
    System.out.printf("Part 2: %d\n", part2Sum);
  }
}
