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

  // Thoughts:
  // need a way to score where a present goes
  // Better score for having more sides against the boundary or against other
  // presents
  // or cost: higher cost for adjacent unfilled spaces...
  public void part1() {
    System.out.println("Day 12, Part 1");
    // For each space, we'll try to add one of each remaining presents from the list
    // of requirements
    // We can check each one in each direction
    // If it fits, (no overlap), record space of center and score
    // Fortunately, all presents are 3x3
    int count = 0;
    for (Space s : spaces) {
      if (everythingFits(s))
        count++;
    }
    System.out.println(count);
  }

  private boolean everythingFits(Space s) {
    int[] requirementsCopy = s.requirements.clone();

    while (requirementsRemain(requirementsCopy)) {
      int maxScore = Integer.MIN_VALUE;
      Point bestPoint = null;
      BitGrid bestGrid = null;
      int bestPresentIndex = -1;

      // Build list of remaining presents (by index)
      for (int i = 0; i < requirementsCopy.length; i++) {
        if (requirementsCopy[i] <= 0)
          continue;

        Present p = presents.get(i);

        ArrayList<BitGrid> possibleRotations = new ArrayList<>();
        possibleRotations.add(p.shape);
        possibleRotations.add(p.shape.rotate90CW());
        possibleRotations.add(p.shape.rotate180());
        possibleRotations.add(p.shape.rotate270CW());

        // Try all rotations
        for (BitGrid g : possibleRotations) {
          int startRow = 0;//Math.max(1, s.grid.firstOccupiedRow() - 1);
          Map<Point, Integer> result = s.grid.scoreAllPlacements(g, startRow);

          for (Map.Entry<Point, Integer> e : result.entrySet()) {
            if (e.getValue() > maxScore) {
              maxScore = e.getValue();
              bestPoint = e.getKey();
              bestGrid = g;
              bestPresentIndex = i;
            }
          }
        }
      }

      // Nothing fits, fail early
      if (bestPoint == null) {
        return false;
      }

      // Apply best present to the space grid
      applyGrid(s.grid, bestGrid, bestPoint);

      // Consume one requirement
      requirementsCopy[bestPresentIndex]--;
    }

    return true;
  }

  private void applyGrid(BitGrid space, BitGrid small, Point center) {
    for (int idx = small.bits.nextSetBit(0); idx >= 0; idx = small.bits.nextSetBit(idx + 1)) {

      int sr = idx / 3;
      int sc = idx % 3;

      int r = center.y + (sr - 1);
      int c = center.x + (sc - 1);

      space.set(r, c, true);
    }
  }

  private boolean requirementsRemain(int[] r) {
    for (int i : r) {
      if (i != 0) {
        return true;
      }
    }
    return false;
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

  private class Present {
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

  private class Space {
    BitGrid grid;
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

  public class BitGrid {
    private final int width;
    private final int height;
    private final BitSet bits;
    int lastRow = lastOccupiedRow();

    public BitGrid(int width, int height) {
      this.width = width;
      this.height = height;
      this.bits = new BitSet(width * height);
    }

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

    public BitGrid rotate90CW() {
      BitGrid out = new BitGrid(height, width); // swapped dimensions

      for (int r = 0; r < height; r++) {
        for (int c = 0; c < width; c++) {
          if (get(r, c)) {
            int nr = c;
            int nc = height - 1 - r;
            out.set(nr, nc, true);
          }
        }
      }

      return out;
    }

    public BitGrid rotate180() {
      BitGrid out = new BitGrid(width, height);

      for (int r = 0; r < height; r++) {
        for (int c = 0; c < width; c++) {
          if (get(r, c)) {
            int nr = height - 1 - r;
            int nc = width - 1 - c;
            out.set(nr, nc, true);
          }
        }
      }

      return out;
    }

    public BitGrid rotate270CW() {
      BitGrid out = new BitGrid(height, width); // swapped dimensions

      for (int r = 0; r < height; r++) {
        for (int c = 0; c < width; c++) {
          if (get(r, c)) {
            int nr = width - 1 - c;
            int nc = r;
            out.set(nr, nc, true);
          }
        }
      }

      return out;
    }

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

    public BitGrid get3x3(int r, int c) {
      BitGrid out = new BitGrid(3, 3);

      for (int dr = -1; dr <= 1; dr++) {
        for (int dc = -1; dc <= 1; dc++) {
          int sr = r + dr;
          int sc = c + dc;

          // If in bounds, copy its value
          if (sr >= 0 && sr < height && sc >= 0 && sc < width) {
            out.set(dr + 1, dc + 1, get(sr, sc));
          }
          // Else leave it false.
        }
      }

      return out;
    }

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

    public int scorePlacement(BitGrid small, int centerR, int centerC) {
      int[] DR = { -1, 1, 0, 0 };
      int[] DC = { 0, 0, -1, 1 };
      BitGrid window = this.get3x3(centerR, centerC);

      // Must be merge-compatible
      if (!small.couldMerge(window)) {
        return -1; // invalid placement
      }

      int score = 0;

      // Iterate over bits set in the small grid
      for (int idx = small.bits.nextSetBit(0); idx >= 0; idx = small.bits.nextSetBit(idx + 1)) {

        int sr = idx / 3; // 0..2
        int sc = idx % 3; // 0..2

        int lr = centerR + (sr - 1);
        int lc = centerC + (sc - 1);

        // Check adjacency in the large grid
        for (int d = 0; d < 4; d++) {
          int ar = lr + DR[d];
          int ac = lc + DC[d];

          if (ar >= 0 && ar < height && ac >= 0 && ac < width) {
            if (this.get(ar, ac)) {
              score++;
            }
          }
        }
      }

      return score;
    }

    public Map<Point, Integer> scoreAllPlacements(BitGrid small, int startRow) {
      if (small.width != 3 || small.height != 3)
        throw new IllegalArgumentException("small grid must be 3x3");

      Map<Point, Integer> result = new HashMap<>();

      // Clamp startRow so a 3x3 window fits
      int r0 = Math.max(1, startRow);
      int rMax = height - 2;

      int lastRow = lastOccupiedRow();
      int maxSmallBits = small.bits.cardinality();

      for (int r = r0; r <= rMax; r++) {

        // Early exit: everything below is empty
        if (r - 1 > lastRow)
          break;

        for (int c = 1; c < width - 1; c++) {

          // Optional fast skip for saturated areas
          if (count3x3(r, c) > maxSmallBits)
            continue;

          int score = scorePlacement(small, r, c);
          if (score >= 0) {
            result.put(new Point(c, r), score);
          }
        }
      }

      return result;
    }

    public int lastOccupiedRow() {
      if (bits == null) {
        return -1;
      }
      int lastBit = bits.length() - 1;
      if (lastBit < 0)
        return -1;

      return lastBit / width;
    }

    public int firstOccupiedRow() {
      for (int r = 0; r < height; r++) {
        for (int c = 0; c < width; c++) {
          if (get(r, c)) { // get(r, c) checks the bit at row r, col c
            return r;
          }
        }
      }
      return height; // no bits set
    }

    public int count3x3(int centerR, int centerC) {
      int count = 0;

      for (int dr = -1; dr <= 1; dr++) {
        for (int dc = -1; dc <= 1; dc++) {
          int r = centerR + dr;
          int c = centerC + dc;

          if (r >= 0 && r < height && c >= 0 && c < width) {
            if (get(r, c))
              count++;
          }
        }
      }
      return count;
    }

    public int rowBitCount(int row) {
      int count = 0;
      for (int c = 0; c < width; c++) {
        if (get(row, c))
          count++;
      }
      return count;
    }

    public int firstViableRow(BitGrid small) {
      int limit = small.bits.cardinality();

      for (int r = 1; r < height - 1; r++) {
        int bandCount = rowBitCount(r - 1)
            + rowBitCount(r)
            + rowBitCount(r + 1);

        if (bandCount <= limit) {
          return r;
        }
      }

      return height; // no viable placement
    }
  }
}
