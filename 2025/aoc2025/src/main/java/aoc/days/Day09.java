package aoc.days;

import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
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

  // This part was hard.
  // Create the outside of the shape, but only check the edges of the rectangle.
  // Edges alone should determine if it falls within the larger shape.
  public void part2() {
    System.out.println("Day 09, Part 2");
    // Create grid set of coordinate points
    Set<Point> boundary = new HashSet<>();
    // Populate grid with red tiles and their connections (aka draw the shape)
    for (int i = 0; i < redTiles.size(); i++) {
      makeLineBetweenPoints(boundary, redTiles.get(i), redTiles.get((i + 1) % redTiles.size()));
    }

    // Now that I have this set, I eventually want to make a Map
    // key = y aka the row, but value is a list of ranges where the shape is filled
    // in that row
    // Those values are really just x indexes in the grid

    // Make buckets of points for each row
    ArrayList<Point> sortedPoints = new ArrayList<>(boundary);
    sortedPoints.sort((a, b) -> a.y - b.y);
    Map<Integer, ArrayList<Integer>> rangeBuckets = new HashMap<>();
    for (Point p : sortedPoints) {
      ArrayList<Integer> currentRow = rangeBuckets.computeIfAbsent(p.y, k -> new ArrayList<>());
      currentRow.add(p.x);
    }
    // These buckets will look like {1: [2,3,4,11]}
    // But we want it to look like {1: [Range(2-11)]}

    // Now coalese these into ranges
    Map<Integer, ArrayList<Range>> boundingRanges = new HashMap<Integer, ArrayList<Range>>();
    for (Map.Entry<Integer, ArrayList<Integer>> entry : rangeBuckets.entrySet()) {
      ArrayList<Integer> xs = entry.getValue();
      int minX = Collections.min(xs);
      int maxX = Collections.max(xs);

      // Originally I thought there would be multiple ranges.
      // that would be the case if the big shape had more cutouts...
      // but we got away with a single one per entry, which makes the list obsolete.
      ArrayList<Range> list = new ArrayList<>();
      list.add(new Range(minX, maxX));
      boundingRanges.put(entry.getKey(), list);
    }

    long biggestArea = 0;
    for (int i = 0; i < redTiles.size(); i++) {
      for (int j = 1 + i; j < redTiles.size(); j++) {
        // Only check fit if it's bigger
        long rectangleSize = getRectangleSize(redTiles.get(i), redTiles.get(j));
        if (rectangleSize > biggestArea) {
          // Check if rectangle fits inside bigger shape, recorded in grid
          if (rectangleFits(boundingRanges, redTiles.get(i), redTiles.get(j))) {
            // Get size between these two and compare to previous max
            biggestArea = Math.max(biggestArea, rectangleSize);
          }
        }
        // Otherwise it's ignored
      }
    }
    System.out.println(biggestArea);
  }

  // Determines if a rectangle fits based on its edges.
  private boolean rectangleFits(Map<Integer, ArrayList<Range>> boundingRanges, Point a, Point b) {

    int minX = Math.min(a.x, b.x);
    int maxX = Math.max(a.x, b.x);
    int minY = Math.min(a.y, b.y);
    int maxY = Math.max(a.y, b.y);

    // Top row: horizontal segment between minX..maxX must be inside
    ArrayList<Range> topRanges = boundingRanges.get(minY);
    if (topRanges == null || !rowCovers(topRanges, minX, maxX)) {
      return false;
    }

    // Bottom row: same
    ArrayList<Range> bottomRanges = boundingRanges.get(maxY);
    if (bottomRanges == null || !rowCovers(bottomRanges, minX, maxX)) {
      return false;
    }

    // Vertical edges: for every row between minY and maxY,
    // left X and right X must lie inside some range on that row.
    for (int y = minY; y <= maxY; y++) {
      ArrayList<Range> row = boundingRanges.get(y);
      if (row == null)
        return false;

      if (!rowContainsX(row, minX))
        return false;
      if (!rowContainsX(row, maxX))
        return false;
    }

    return true;
  }

  private boolean rowContainsX(ArrayList<Range> ranges, int x) {
    for (Range r : ranges) {
      if (x >= r.start && x <= r.end) {
        return true;
      }
    }
    return false;
  }

  private boolean rowCovers(ArrayList<Range> ranges, int start, int end) {
    for (Range r : ranges) {
      if (start >= r.start && end <= r.end) {
        return true;
      }
    }
    return false;
  }

  // This was originally to try and get multiranges from a single list... 
  // Ended up not being used.
  ArrayList<ArrayList<Integer>> splitRanges(ArrayList<Integer> values) {
    ArrayList<ArrayList<Integer>> ranges = new ArrayList<>();
    if (values.isEmpty())
      return ranges;

    values.sort(Integer::compareTo);

    ArrayList<Integer> current = new ArrayList<>();
    current.add(values.get(0));

    for (int i = 1; i < values.size(); i++) {
      int prev = values.get(i - 1);
      int curr = values.get(i);

      boolean consecutive = (curr == prev + 1);

      if (consecutive) {
        // Continue current range
        current.add(curr);
      } else {
        // Not consecutive: check if curr is isolated or starts a new cluster
        boolean isolated = true;

        if (i + 1 < values.size()) {
          int next = values.get(i + 1);
          if (next == curr + 1) {
            // curr starts a new consecutive cluster
            isolated = false;
          }
        }

        if (isolated) {
          // Absorb isolated value into current range
          current.add(curr);
        } else {
          // End current range, start new one
          ranges.add(new ArrayList<>(current));
          current.clear();
          current.add(curr);
        }
      }
    }

    // Add final range
    ranges.add(new ArrayList<>(current));

    return ranges;
  }

  private class Range {
    public final int start, end;

    public Range(int start, int end) {
      this.start = start;
      this.end = end;
    }

    public boolean contains(int x) {
      return x >= start && x <= end;
    }

    @Override
    public String toString() {
      return "[" + start + "," + end + "]";
    }
  }

  // Bad idea for part 2
  // Tried to populate all the coordinates, but the amount is way too much
  // Ran out of RAM.
  // Wanted to use BitSet for this, but its indexes are only int-sized.
  public void part2v1() {
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
    for (int i = 0; i < redTiles.size() - 1; i++) {
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
    while (!edgeFound) {
      Point testPoint = new Point(x, y);
      if (grid.contains(testPoint)) {
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

  private void addRectangleToSet(Set<Point> grid, Point a, Point b) {
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

  private void floodFill(Set<Point> grid, Point start) {
    Queue<Point> queue = new ArrayDeque<Point>();
    queue.add(start);

    while (!queue.isEmpty()) {
      Point current = queue.poll();
      // Add to grid
      grid.add(current);
      // Check all four directions and add to queue if not in grid
      // Up
      if (!grid.contains(new Point(current.x - 1, current.y))) {
        queue.add(new Point(current.x - 1, current.y));
      }
      // Down
      if (!grid.contains(new Point(current.x + 1, current.y))) {
        queue.add(new Point(current.x + 1, current.y));
      }
      // Left
      if (!grid.contains(new Point(current.x, current.y - 1))) {
        queue.add(new Point(current.x, current.y - 1));
      }
      // Right
      if (!grid.contains(new Point(current.x, current.y + 1))) {
        queue.add(new Point(current.x, current.y + 1));
      }
    }
  }

  private boolean containsAll(Set<Point> big, Set<Point> small) {
    return big.containsAll(small);
  }

  private void makeLineBetweenPoints(Set<Point> grid, Point a, Point b) {
    // Line can only go one of four directions.
    if (a.x < b.x) {
      // Increase x as we go
      for (int i = a.x; i <= b.x; i++) {
        grid.add(new Point(i, a.y));
      }
    } else if (a.y < b.y) {
      // Increase y as we go
      for (int i = a.y; i <= b.y; i++) {
        grid.add(new Point(a.x, i));
      }
    } else if (b.x < a.x) {
      // Start at bx instead
      for (int i = b.x; i <= a.x; i++) {
        grid.add(new Point(i, b.y));
      }
    } else {
      // b.y must be smaller than a.y
      for (int i = b.y; i <= a.y; i++) {
        grid.add(new Point(b.x, i));
      }
    }
  }

  private int getGridIndex(Point p, int N) {
    return p.y * N + p.x;
  }

  private class Point {
    int x;
    int y;

    public Point(int x, int y) {
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
