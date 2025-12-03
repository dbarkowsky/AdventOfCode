package aoc.days;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class Day03 {
  private ArrayList<String> lines;

  public Day03(ArrayList<String> input) {
    lines = input;
  }

  // Turns on the batteries to make the biggest two-digit number
  public void Part1() {
    System.out.println("Day 03, Part 1");
    int joltageSum = 0;
    // For each battery bank (row)
    for (String bank : lines){
      // Break it into a list of numbers
      List<String> batteriesAsStrings = Arrays.asList(bank.split(""));
      ArrayList<Integer> batteries = new ArrayList<Integer>();
      for (String item : batteriesAsStrings){
        batteries.add(Integer.parseInt(item));
      }

      // Find the biggest number in that list
      int biggestNumber = -1;
      int biggestNumberIndex = -1;
      // size - 1 here because we can't use the last battery as the first of two numbers.
      for (int i = 0; i < batteries.size() - 1; i ++) {
        if (batteries.get(i) > biggestNumber){
          biggestNumber = batteries.get(i);
          biggestNumberIndex = i;
        }
      }
      // We now have the first biggest number and its position. Do it again, but starting after this number.
      int nextNumber = -1;
      for (int i = biggestNumberIndex + 1; i < batteries.size(); i ++) {
        if (batteries.get(i) > nextNumber){
          nextNumber = batteries.get(i);
        }
      }
      // We should now have both numbers. Combine them like strings and add to total. 
      String biggestAsString = String.valueOf(biggestNumber) + String.valueOf(nextNumber);
      joltageSum += Integer.parseInt(biggestAsString);
    }
    System.out.println(joltageSum);
  }

  // Same concept, but turns on twelve batteries.
  public void Part2(){

  }
}
