package aoc.days;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.Set;

public class Day07 {
  ArrayList<String[]> rows = new ArrayList<String[]>();
  final String BEAM = "|";
  final String SPLITTER = "^";
  final String START = "S";

  public Day07(ArrayList<String> input) {
    // Insert array lists for each row
    for (String row: input){
      rows.add(row.split(""));
    }
  }

  // Counting number of splits
  public void part1() {
    System.out.println("Day 07, Part 1");
    int splits = 0;
    // Going to track each row outcome in a set.
    // Iterate through each previous row and check if there's anything below
    // Build new set based on whether there's a beam above and a splitter on this row.
    // Loop until all rows are analysed
    Set<Integer> currentRowBeamsIndex = new HashSet<>();
    // Why a set? Because two beams cannot double up in the same space
    // Find start
    String[] firstRow = rows.get(0);
    for (int column = 0; column < firstRow.length; column ++){
      if (firstRow[column].equals(START)){
        currentRowBeamsIndex.add(column);
      }
    }
    // Begin loop, starting at second row
    for (int rowIndex = 1; rowIndex < rows.size(); rowIndex++){
      // Copy set to avoid editing by reference
      Set<Integer> previousRowBeamsIndex = new HashSet<>(currentRowBeamsIndex);
      currentRowBeamsIndex = new HashSet<>();
      // We only need to check existing entries in previous row
      for (int columnIndex : previousRowBeamsIndex){
        // Two cases.
        // Either this row has a splitter or it doesn't
        if (rows.get(rowIndex)[columnIndex].equals(SPLITTER)){
          splits++;
          currentRowBeamsIndex.add(columnIndex + 1);
          currentRowBeamsIndex.add(columnIndex - 1);
        } else {
          // No splitter, just continue beam
          currentRowBeamsIndex.add(columnIndex);
        }
      }
    }
    System.out.println(splits);
  }

  // Detect total possible unique paths of beam
  public void part2() {
    System.out.println("Day 07, Part 2");
    // Easier than it sounds.
    // If you add all the paths of the beam at each level it splits, it equals the number of unique paths
    // It is fortunate that the beam only splits on specific rows

    // Use the same algo as part 1
    Set<Integer> currentRowBeamsIndex = new HashSet<>();
    String[] firstRow = rows.get(0);
    for (int column = 0; column < firstRow.length; column ++){
      if (firstRow[column].equals(START)){
        currentRowBeamsIndex.add(column);
      }
    }
    int timelineSum = 0;
    for (int rowIndex = 1; rowIndex < rows.size(); rowIndex++){
      Set<Integer> previousRowBeamsIndex = new HashSet<>(currentRowBeamsIndex);
      currentRowBeamsIndex = new HashSet<>();
      boolean rowContainsSplit = false;
      for (int columnIndex : previousRowBeamsIndex){
        if (rows.get(rowIndex)[columnIndex].equals(SPLITTER)){
          currentRowBeamsIndex.add(columnIndex + 1);
          currentRowBeamsIndex.add(columnIndex - 1);
          // NEW FOR PART 2
          // This row contained a split. Record this
          rowContainsSplit = true;
        } else {
          currentRowBeamsIndex.add(columnIndex);
        }
      }
      // NEW FOR PART 2
      if (rowContainsSplit){
        // Count the number of beams in this row and add to total
        timelineSum += currentRowBeamsIndex.size();
      }
    }
    System.out.println(timelineSum);
  }

  private class ManifoldUnit {

  }
}
