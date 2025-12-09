package aoc.days;

import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.BitSet;
import java.util.HashMap;
import java.util.Queue;

public class Day09 {
  ArrayList<Point> redTiles = new ArrayList<>();

  public Day09(ArrayList<String> input) {
    for (String line : input) {
      int x = Integer.valueOf(line.split(",")[0]);
      int y = Integer.valueOf(line.split(",")[1]);
      redTiles.add(new Point(x, y));
    }
  }

  // Find largest rectange where red tiles are opposite corners
  public void part1() {
    System.out.println("Day 09, Part 1");
    long rectangleArea = 0;
    for (int i = 0; i < redTiles.size(); i++) {
      for (int j = 1 + i; j < redTiles.size(); j++) {
        // Get size between these two and compare to previous max
        rectangleArea = Math.max(rectangleArea, getRectangleSize(redTiles.get(i), redTiles.get(j)));
      }
    }
    System.out.println(rectangleArea);
  }

  public void part2() {
    System.out.println("Day 09, Part 2");
    // Potentially crazy solution to try and record all coordinates
    // Using a BitSet, as I expect a lot of values and want small mem usage
    int minSize = Integer.MAX_VALUE;
    int maxSize = 0;
    for (Point tile : redTiles) {
      minSize = Math.min(Math.min(tile.x, tile.y), minSize);
      maxSize = Math.max(Math.max(tile.x, tile.y), maxSize);
    }
    // Min size is now our offset
    int offset = minSize;
    final int N = maxSize; // - minSize;

    // Create BitSet
    BitSet grid = new BitSet(N * N);
    // Populate grid with red tiles and their connections
    for (int i = 0; i < redTiles.size() - 1; i++){
      makeLineBetweenPoints(grid, redTiles.get(i), redTiles.get(i + 1), offset);
    }
    // Then connect the first and last
    makeLineBetweenPoints(grid, redTiles.get(0), redTiles.get(redTiles.size() - 1), offset);
    
    // Flood fill the rest of them.
    // But where to start?
    // Assumption: if we cut across the centre, we will hit the side of the shape
    // and can start on other side.
    // Just hope we don't hit a horizontal edge
    int x = N / 2;
    int y = 0;
    boolean edgeFound = false;
    Point startingPoint = null;
    while (!edgeFound){
      int index = y * N + x;
      if (grid.get(index)){
        // Found an edge. Assume the next one is where we'd start
        startingPoint = new Point(x, y + 1);
        edgeFound = true;
      }
      y++;
    }
    // Now start the flood fill from here.
    floodFill(grid, startingPoint);
    // For each pair of red tiles, create a new BitSet
    long rectangleArea = 0;
    for (int i = 0; i < redTiles.size(); i++) {
      for (int j = 1 + i; j < redTiles.size(); j++) {
        // This will be a lot bigger than needed
        // But I think it must match for the comparison
        BitSet rectangleSet = new BitSet(N * N);

        // Determine if this BitSet fits in larger BitSet
        if (containsAll(grid, rectangleSet)) {
          // Get size between these two and compare to previous max
          rectangleArea = Math.max(rectangleArea, getRectangleSize(redTiles.get(i), redTiles.get(j)));
        }
        // Otherwise it's ignored
      }
    }
    System.out.println();
  }

  private long getRectangleSize(Point a, Point b) {
    long xDifference = 1 + (Math.abs(a.x - b.x));
    long yDifference = 1 + (Math.abs(a.y - b.y));

    return xDifference * yDifference;
  }

  private void floodFill(BitSet grid, Point start){
    Queue<Point> queue = new ArrayDeque<Point>();
    queue.add(start);

    while (!queue.isEmpty()){
      Point current = queue.poll();
      // Add to grid
      grid.set(getGridIndex(current, grid.size()));
      // Check all four directions and add to queue if not in grid
      //  Up
      if (!grid.get(getGridIndex(new Point(current.x - 1, current.y), grid.size()))){
        queue.add(new Point(current.x - 1, current.y));
      }
      // Down
      if (!grid.get(getGridIndex(new Point(current.x + 1, current.y), grid.size()))){
        queue.add(new Point(current.x + 1, current.y));
      }
      // Left
      if (!grid.get(getGridIndex(new Point(current.x, current.y - 1), grid.size()))){
        queue.add(new Point(current.x, current.y - 1));
      }
      // Right
      if (!grid.get(getGridIndex(new Point(current.x, current.y + 1), grid.size()))){
        queue.add(new Point(current.x, current.y + 1));
      }
    }
  }

  private boolean containsAll(BitSet big, BitSet small) {
    BitSet tmp = (BitSet) small.clone();
    tmp.andNot(big);
    return tmp.isEmpty();
  }

  private void makeLineBetweenPoints(BitSet grid, Point a, Point b, int offset){
    // Line can only go one of four directions.
    if (a.x < b.x){
      // Increase x as we go
      for (int i = a.x; i <= b.x; i ++){
        int index = a.y * grid.size() + i;
        grid.set(index);
      }
    } else if (a.y < b.y){
      // Increase y as we go
      for (int i = a.y; i <= b.y; i ++){
        int index = i * grid.size() + a.x;
        grid.set(index);
      }
    } else if (b.x < a.x) {
      // Start at bx instead
      for (int i = b.x; i <= a.x; i ++){
        int index = b.y * grid.size() + i;
        grid.set(index);
      }
    } else {
      // b.y must be smaller than a.y
      for (int i = b.y; i <= a.y; i ++){
        int index = i * grid.size() + b.x;
        grid.set(index);
      }
    }
  }

  private int getGridIndex(Point p, int N){
    return p.y * N + p.x;
  }

  private class Point{
    int x;
    int y;
    public Point(int x, int y){
      this.x = x; 
      this.y = y;
    }
  }

  

}
