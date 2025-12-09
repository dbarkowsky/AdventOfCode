package aoc.days;

import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.Objects;
import java.util.Queue;
import java.util.Set;

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

  // Bad idea for part 2
  // Tried to populate all the coordinates, but the amount is way too much
  // Ran out of RAM.
  // Wanted to use BitSet for this, but its indexes are only int-sized.
  public void part2() {
    System.out.println("Day 09, Part 2");
    // Potentially crazy solution to try and record all coordinates
    int minSize = Integer.MAX_VALUE;
    int maxSize = 0;
    for (Point tile : redTiles) {
      minSize = Math.min(Math.min(tile.x, tile.y), minSize);
      maxSize = Math.max(Math.max(tile.x, tile.y), maxSize);
    }

    // Create grid set of coordinate points
    Set<Point> grid = new HashSet<>();
    // Populate grid with red tiles and their connections
    for (int i = 0; i < redTiles.size() - 1; i++){
      makeLineBetweenPoints(grid, redTiles.get(i), redTiles.get(i + 1));
    }
    // Then connect the first and last
    makeLineBetweenPoints(grid, redTiles.get(0), redTiles.get(redTiles.size() - 1));
    
    // Flood fill the rest of them.
    // But where to start?
    // Assumption: if we cut across the centre, we will hit the side of the shape
    // and can start on other side.
    // Just hope we don't hit a horizontal edge
    int x = maxSize / 2;
    int y = 0;
    boolean edgeFound = false;
    Point startingPoint = null;
    while (!edgeFound){
      Point testPoint = new Point(x, y);
      if (grid.contains(testPoint)){
        // Found an edge. Assume the next one is where we'd start
        startingPoint = new Point(x, y + 1);
        edgeFound = true;
      }
      y++;
    }
    // Now start the flood fill from here.
    floodFill(grid, startingPoint);
    // For each pair of red tiles, create a new set
    long rectangleArea = 0;
    for (int i = 0; i < redTiles.size(); i++) {
      for (int j = 1 + i; j < redTiles.size(); j++) {
        // Add all points in the rectangle to the set
        Set<Point> rectangleSet = new HashSet<>();
        addRectangleToSet(rectangleSet, redTiles.get(i), redTiles.get(j));

        // Determine if this set fits in larger set
        if (containsAll(grid, rectangleSet)) {
          // Get size between these two and compare to previous max
          rectangleArea = Math.max(rectangleArea, getRectangleSize(redTiles.get(i), redTiles.get(j)));
        }
        // Otherwise it's ignored
      }
    }
    System.out.println(rectangleArea);
  }

  private long getRectangleSize(Point a, Point b) {
    long xDifference = 1 + (Math.abs(a.x - b.x));
    long yDifference = 1 + (Math.abs(a.y - b.y));

    return xDifference * yDifference;
  }

  private void addRectangleToSet(Set<Point> grid, Point a, Point b){
    int xMin = Math.min(a.x, b.x);
    int xMax = Math.max(a.x, b.x);

    int yMin = Math.min(a.y, b.y);
    int yMax = Math.max(a.y, b.y);

    for (int y = yMin; y <= yMax; y++) {
        for (int x = xMin; x <= xMax; x++) {
            grid.add(new Point(x, y));
        }
    }
  }

  private void floodFill(Set<Point> grid, Point start){
    Queue<Point> queue = new ArrayDeque<Point>();
    queue.add(start);

    while (!queue.isEmpty()){
      Point current = queue.poll();
      // Add to grid
      grid.add(current);
      // Check all four directions and add to queue if not in grid
      //  Up
      if (!grid.contains(new Point(current.x - 1, current.y))){
        queue.add(new Point(current.x - 1, current.y));
      }
      // Down
      if (!grid.contains(new Point(current.x + 1, current.y))){
        queue.add(new Point(current.x + 1, current.y));
      }
      // Left
      if (!grid.contains(new Point(current.x, current.y - 1))){
        queue.add(new Point(current.x, current.y - 1));
      }
      // Right
      if (!grid.contains(new Point(current.x, current.y + 1))){
        queue.add(new Point(current.x, current.y + 1));
      }
    }
  }

  private boolean containsAll(Set<Point> big, Set<Point> small) {
    return big.containsAll(small);
  }

  private void makeLineBetweenPoints(Set<Point> grid, Point a, Point b){
    // Line can only go one of four directions.
    if (a.x < b.x){
      // Increase x as we go
      for (int i = a.x; i <= b.x; i ++){
        grid.add(new Point(i, a.y));
      }
    } else if (a.y < b.y){
      // Increase y as we go
      for (int i = a.y; i <= b.y; i ++){
        grid.add(new Point(a.x, i));
      }
    } else if (b.x < a.x) {
      // Start at bx instead
      for (int i = b.x; i <= a.x; i ++){
        grid.add(new Point(i, b.y));
      }
    } else {
      // b.y must be smaller than a.y
      for (int i = b.y; i <= a.y; i ++){
        grid.add(new Point(b.x, i));
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

     @Override
    public boolean equals(Object other) {
      if (this == other)
        return true;
      if (other == null || getClass() != other.getClass())
        return false;
      Point otherPoint = (Point) other;
      return this.x == otherPoint.x && this.y == otherPoint.y;
    }

    @Override
    public int hashCode() {
      return Objects.hash(this.x, this.y);
    }
  }
}
