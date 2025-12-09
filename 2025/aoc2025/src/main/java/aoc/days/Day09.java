package aoc.days;

import java.util.ArrayList;

public class Day09 {
  ArrayList<String> redTiles = new ArrayList<>();
   public Day09(ArrayList<String> input) {
    for (String line : input){
      redTiles.add(line);
    }
  }

  // Find largest rectange where red tiles are opposite corners
  public void part1() {
    System.out.println("Day 09, Part 1");
    long rectangleArea = 0;
    for (int i = 0; i < redTiles.size(); i++){
      for (int j = 1 + i; j < redTiles.size(); j++){
        // Get size between these two and compare to previous max
        rectangleArea = Math.max(rectangleArea, getRectangleSize(redTiles.get(i), redTiles.get(j)));
      }
    }
    System.out.println(rectangleArea);
  }

  public void part2() {
    System.out.println("Day 09, Part 2");
  
  }

  private long getRectangleSize(String a, String b){
    long aX = Long.valueOf(a.split(",")[0]);
    long aY = Long.valueOf(a.split(",")[1]);
    long bX = Long.valueOf(b.split(",")[0]);
    long bY = Long.valueOf(b.split(",")[1]);

    long xDifference = 1 + (Math.abs(aX - bX));
    long yDifference = 1 + (Math.abs(aY - bY));

    return xDifference * yDifference;
  }
}
