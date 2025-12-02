package aoc.days;

import java.util.ArrayList;

public class Day02 {

  private String[] ranges;

  public Day02(ArrayList<String> input) {
    ranges = input.get(0).split(",");
  }

  public void Part1() {
    long repeatingValueSum = 0;
   // For each range
   for (String range : ranges){
    // Get start and end of range
    long start = Long.parseLong(range.split("-")[0]);
    long end = Long.parseLong(range.split("-")[1]);

    // Loop through range and look for repeating (invalid) IDs
    for (long i = start; i <= end;){
      if (IsRepeating(i)) repeatingValueSum += i;
      i++;
      // If the size is odd, we can skip to the next number of even size.
      // if (String.valueOf(i).length() % 2 == 1){
      //   i = 5 * (String.valueOf(i).length());
      // }
    }
   }
   System.out.println(repeatingValueSum);
  }

  public void Part2() {
    
  }

  private boolean IsRepeating(long num){
    String stringNum = String.valueOf(num);
    // Can't be repeating if odd length
    if (stringNum.length() % 2 == 1) return false;
    String left = stringNum.substring(0, stringNum.length() / 2);
    String right = stringNum.substring((stringNum.length() / 2));
    if (left.equals(right)) return true;
    return false;
  }
}
