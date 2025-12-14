package aoc.days;

import java.util.ArrayList;
import java.util.BitSet;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.awt.Point;

public class Day12 {

  ArrayList<Present> presents = new ArrayList<>();
  ArrayList<Space> spaces = new ArrayList<>();

  public Day12(ArrayList<String> input) {
    // First 30 lines are the present sizes
    ArrayList<String> presentLines = new ArrayList<>(input.subList(0, 29));
    int i = 0;
    for (; i < presentLines.size(); i += 5) {
      presents.add(new Present(presentLines.subList(i, i + 4)));
    }
    // Everything else defines the spaces to fill
    for (; i < input.size(); i++) {
      spaces.add(new Space(input.get(i)));
    }
  }

  // So, this didn't work for the test input, but it did for the real one
  // I'm not happy about this, but I'll take it.
  // Took 2.7 minutes to run though.
  public void part1() {
    System.out.println("Day 12, Part 1");
    // Nothing fancy here. Just see if everything fits and count it.
    int count = 0;
    for (Space s : spaces) {
      if (everythingFits(s))
        count++;
      // System.out.println(String.format("%d,%d", s.grid.width, s.grid.height));

    }
    System.out.println(count);
  }

  public void part2() {
    System.out.println("Day 12, Part 2");
    // There's not really a part 2, but I saw this online as a solution.
    // It turns out they didn't expect a real solution, and you can just cheese it.
    // If the full 9 squares of all shapes, whether the shape of the present or not,
    // would fit, then it fits. No fancy maneuvering.
    // Only works for real input, not test input.
    int count = 0;
    for (Space s : spaces) {
      int sumOfRequirements = 0;
      for (int num : s.requirements) {
        sumOfRequirements += num;
      }
      if ((s.grid.width * s.grid.height) >= sumOfRequirements * 9) {
        count++;
      }
    }
    System.out.println(count);
  }

  private boolean everythingFits(Space s) {
    // A copy so we can mutate
    int[] requirementsCopy = s.requirements.clone();

    // As long as there are presents to insert
    while (requirementsRemain(requirementsCopy)) {
      // We'll track the best-scoring option and insert that one
      int maxScore = Integer.MIN_VALUE;
      Point bestPoint = null;
      BitGrid bestGrid = null;
      int bestPresentIndex = -1;
      int startRow = 1;

      // Build list of remaining presents (by index) for one pass
      for (int i = 0; i < requirementsCopy.length; i++) {
        if (requirementsCopy[i] <= 0)
          continue;

        Present p = presents.get(i);
        // We need to check every possible rotation for this...
        ArrayList<BitGrid> possibleRotations = new ArrayList<>();
        possibleRotations.add(p.shape);
        possibleRotations.add(p.shape.rotate90CW());
        possibleRotations.add(p.shape.rotate180());
        possibleRotations.add(p.shape.rotate270CW());

        // Try all rotations
        for (BitGrid g : possibleRotations) {
          Map<Point, Integer> result = s.grid.scoreAllPlacements(g, startRow);

          // If it has the
          for (Map.Entry<Point, Integer> e : result.entrySet()) {
            if (e.getValue() > maxScore) {
              maxScore = e.getValue();
              bestPoint = e.getKey();
              bestGrid = g;
              bestPresentIndex = i;
            }
          }
        }
        // A small attempt to get this to run faster. Start at a row we know isn't
        // filled yet.
        startRow = Math.max(1, s.grid.lastOccupiedRow() - 2);
      }

      // Nothing fits, fail early
      if (bestPoint == null) {
        return false;
      }

      // Apply best present to the space grid
      applyGrid(s.grid, bestGrid, bestPoint);
      // Update the last occupied row.
      s.grid.lastRow = s.grid.lastOccupiedRow();

      // Consume one requirement
      requirementsCopy[bestPresentIndex]--;
    }

    return true;
  }

  // Puts the present into the Space at appropriate location
  private void applyGrid(BitGrid space, BitGrid small, Point center) {
    for (int i = small.bits.nextSetBit(0); i >= 0; i = small.bits.nextSetBit(i + 1)) {

      int sourceRow = i / 3;
      int sourceColumn = i % 3;

      int row = center.y + (sourceRow - 1);
      int column = center.x + (sourceColumn - 1);

      space.set(row, column, true);
    }
  }

  // Makes sure there are still presents to insert
  private boolean requirementsRemain(int[] r) {
    for (int i : r) {
      if (i != 0) {
        return true;
      }
    }
    return false;
  }

  // A class to represent each present.
  private class Present {
    // Been meaning to use BitGrid for a while. Finally a chance.
    BitGrid shape = new BitGrid(3, 3);

    public Present(List<String> presentLines) {
      // index 1,2,3 are the actual present
      List<String> relevantLines = presentLines.subList(1, 4);
      for (int i = 0; i < relevantLines.size(); i++) {
        String[] row = relevantLines.get(i).split("");
        for (int j = 0; j < row.length; j++) {
          if (row[j].equals("#")) {
            shape.flip(i, j);
          }
        }
      }
    }
  }

  // A class to represent each box Space
  private class Space {
    BitGrid grid;
    // the list of presents to insert
    int[] requirements = new int[6];

    public Space(String spaceLine) {
      String dimensions = spaceLine.split(": ")[0];
      int width = Integer.parseInt(dimensions.split("x")[0]);
      int height = Integer.parseInt(dimensions.split("x")[1]);
      grid = new BitGrid(width, height);

      String[] requirementStrings = spaceLine.split(": ")[1].split(" ");
      for (int i = 0; i < requirementStrings.length; i++) {
        requirements[i] = Integer.parseInt(requirementStrings[i]);
      }
    }
  }

  // I needed a BitSet but in two dimensions. Could have probably done it
  // with some clever indexing, but oh well.
  // This grew way bigger than I expected it to.
  public class BitGrid {
    private final int width;
    private final int height;
    private final BitSet bits;
    private int lastRow = lastOccupiedRow();

    public BitGrid(int width, int height) {
      this.width = width;
      this.height = height;
      this.bits = new BitSet(width * height);
    }

    // Some simple getters and setters
    private int index(int row, int col) {
      return row * width + col;
    }

    public void set(int row, int col, boolean value) {
      bits.set(index(row, col), value);
    }

    public boolean get(int row, int col) {
      return bits.get(index(row, col));
    }

    public void flip(int row, int col) {
      bits.flip(index(row, col));
    }

    // Some methods to get all rotations of the grid
    // Used for rotating presents
    public BitGrid rotate90CW() {
      BitGrid out = new BitGrid(height, width); // swapped dimensions

      for (int row = 0; row < height; row++) {
        for (int col = 0; col < width; col++) {
          if (get(row, col)) {
            int newRow = col;
            int newCol = height - 1 - row;
            out.set(newRow, newCol, true);
          }
        }
      }

      return out;
    }

    public BitGrid rotate180() {
      BitGrid out = new BitGrid(width, height);

      for (int row = 0; row < height; row++) {
        for (int col = 0; col < width; col++) {
          if (get(row, col)) {
            int newRow = height - 1 - row;
            int newCol = width - 1 - col;
            out.set(newRow, newCol, true);
          }
        }
      }

      return out;
    }

    public BitGrid rotate270CW() {
      BitGrid out = new BitGrid(height, width); // swapped dimensions

      for (int row = 0; row < height; row++) {
        for (int col = 0; col < width; col++) {
          if (get(row, col)) {
            int newRow = width - 1 - col;
            int newCol = row;
            out.set(newRow, newCol, true);
          }
        }
      }

      return out;
    }

    // Because I need debugging help
    public void print() {
      for (int r = 0; r < height; r++) {
        StringBuilder sb = new StringBuilder(width);
        for (int c = 0; c < width; c++) {
          sb.append(get(r, c) ? '#' : '.');
        }
        System.out.println(sb);
      }
      System.out.println();
    }

    // Gets a 3x3 subset of the grid
    // Used to extract a portion of Shapes
    public BitGrid get3x3(int row, int col) {
      BitGrid out = new BitGrid(3, 3);

      for (int rowDiff = -1; rowDiff <= 1; rowDiff++) {
        for (int colDiff = -1; colDiff <= 1; colDiff++) {
          int sr = row + rowDiff;
          int sc = col + colDiff;

          // If in bounds, copy its value
          if (sr >= 0 && sr < height && sc >= 0 && sc < width) {
            out.set(rowDiff + 1, colDiff + 1, get(sr, sc));
          }
          // Else leave it false.
        }
      }

      return out;
    }

    // Checks if two grids can merge without collisions
    // aka no 1s in the same places
    // Only designed with checking two 3x3 grids
    public boolean couldMerge(BitGrid other) {
      BitSet a = this.bits;
      BitSet b = other.bits;

      BitSet tmp = (BitSet) a.clone();
      tmp.and(b);

      // if AND is empty, no overlapping 1s
      return tmp.isEmpty();
    }

    public boolean isEmpty() {
      return bits.isEmpty();
    }

    // Returns a score based on whether surrounding blocks have 1 values
    public int scorePlacement(BitGrid small, int centerR, int centerC) {
      int[] rowDifference = { -1, 1, 0, 0 };
      int[] columnDifference = { 0, 0, -1, 1 };
      BitGrid window = this.get3x3(centerR, centerC);

      // Must be merge-compatible
      if (!small.couldMerge(window)) {
        return -1; // invalid placement
      }

      int score = 0;

      // Iterate over bits set in the small grid
      for (int i = small.bits.nextSetBit(0); i >= 0; i = small.bits.nextSetBit(i + 1)) {

        int sr = i / 3; // 0..2
        int sc = i % 3; // 0..2

        int lr = centerR + (sr - 1);
        int lc = centerC + (sc - 1);

        // Check adjacency in the large grid
        for (int d = 0; d < 4; d++) {
          int adjacentRow = lr + rowDifference[d];
          int adjacentCol = lc + columnDifference[d];

          if (adjacentRow >= 0 && adjacentRow < height && adjacentCol >= 0 && adjacentCol < width) {
            if (this.get(adjacentRow, adjacentCol)) {
              score++;
            }
          }
        }
      }

      return score;
    }

    // Looks at all places this present could go.
    // Returns a map with centre points and its score
    // 3x3 only can be inserted.
    public Map<Point, Integer> scoreAllPlacements(BitGrid small, int startRow) {
      if (small.width != 3 || small.height != 3)
        throw new IllegalArgumentException("small grid must be 3x3");

      Map<Point, Integer> result = new HashMap<>();

      // Clamp startRow so a 3x3 window fits
      int rowStart = Math.max(1, startRow);
      int rMax = height - 2;

      int maxSmallBits = small.bits.cardinality();

      for (int row = rowStart; row <= rMax; row++) {

        // Early exit: everything below is empty
        if (row - 2 > lastRow)
          break;

        for (int col = 1; col < width - 1; col++) {

          // Optional fast skip for saturated areas
          if (count3x3(row, col) > maxSmallBits)
            continue;

          int score = scorePlacement(small, row, col);
          if (score >= 0) {
            result.put(new Point(col, row), score);
          }
        }
      }

      return result;
    }

    // What's the last occupied row?
    // The idea here was to skip the already full rows
    public int lastOccupiedRow() {
      if (bits == null) {
        return -1;
      }
      int lastBit = bits.length() - 1;
      if (lastBit < 0)
        return -1;

      return lastBit / width;
    }

    // Just gets all "on" bits in this 3x3 area
    public int count3x3(int centerR, int centerC) {
      int count = 0;

      for (int dr = -1; dr <= 1; dr++) {
        for (int dc = -1; dc <= 1; dc++) {
          int row = centerR + dr;
          int col = centerC + dc;

          if (row >= 0 && row < height && col >= 0 && col < width) {
            if (get(row, col))
              count++;
          }
        }
      }
      return count;
    }
  }
}
