package aoc.days;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
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
    // It is fortunate that the beam only splits on specific rows
    // Every time it splits, it needs to be recorded, keeping the existing quantity of beams there.
    // For this reason, we keep a map, not a set

    // Almost the same, but we track both column index and quantity
    Map<Integer, Long> currentRowBeams = new HashMap<>();
    String[] firstRow = rows.get(0);
    for (int column = 0; column < firstRow.length; column ++){
      if (firstRow[column].equals(START)){
        currentRowBeams.put(column, Long.valueOf(1));
      }
    }

    for (int rowIndex = 1; rowIndex < rows.size(); rowIndex++){
      Map<Integer, Long> previousRowBeams = new HashMap<>(currentRowBeams);
      currentRowBeams = new HashMap<>();
      for (Map.Entry<Integer, Long> record : previousRowBeams.entrySet()){
        int columnIndex = record.getKey();
        long quantity = record.getValue();
        if (rows.get(rowIndex)[columnIndex].equals(SPLITTER)){
          // Is there already an entry for this location? Need to add the quantities
          // Left first
          long newLeftQuantity  = quantity + (currentRowBeams.get(columnIndex - 1) != null ? currentRowBeams.get(columnIndex - 1) : 0);
          currentRowBeams.put(columnIndex - 1, newLeftQuantity);
          // Then right
          long newRightQuantity  = quantity + (currentRowBeams.get(columnIndex + 1) != null ? currentRowBeams.get(columnIndex + 1) : 0);
          currentRowBeams.put(columnIndex + 1, newRightQuantity);
        } else {
          // There could be a case here where a beam comes from above as well as from the sides...
          long newQuantity = quantity + (currentRowBeams.get(columnIndex) != null ? currentRowBeams.get(columnIndex) : 0);
          currentRowBeams.put(columnIndex, newQuantity);
        }
      }
    }
    // At the end, it should just be the total sum of all quantities.
    long sum = 0;
    for (Map.Entry<Integer, Long> record : currentRowBeams.entrySet()){
      sum += record.getValue();
    }
    System.out.println(sum);
  }
}
