package aoc.days;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.Objects;
import java.util.Set;

public class Day04 {

  private Set<PaperRoll> paperRolls;

  public Day04(ArrayList<String> input) {
    this.paperRolls = new HashSet<Day04.PaperRoll>();
    // For this, top right is 0,0
    // Next row would be 1,0.
    // Not truly x,y like a graph, but easy to remember.
    for (int x = 0; x < input.size(); x++) {
      String[] row = input.get(x).split("");
      for (int y = 0; y < row.length; y++) {
        if (row[y].equals("@")) {
          // Add the paper's coordinates to the set
          paperRolls.add(new PaperRoll(x, y));
        }
      }
    }
  }

  public void part1() {
    System.out.println("Day 04, Part 1");
    // Cycle through each entry in the paper roll set
    // Check the surrounding area and add it to count if
    // surrounded by less than 4 other paper rolls.
    int accessiblePaperRollCount = 0;
    final int THRESHOLD = 4;
    for (PaperRoll roll : paperRolls) {
      if (!hasXPaperRollsAdjacent(roll, THRESHOLD, paperRolls)) {
        accessiblePaperRollCount++;
      }
    }
    System.out.println(accessiblePaperRollCount);
  }

  public void part2() {
    System.out.println("Day 04, Part 2");
    // Similar approach, but we need to remove the rolls and run again.
    // Repeat until all removable rolls have been taken.
    int accessiblePaperRollCount = 0;
    final int THRESHOLD = 4;
    int rollsTakenThisRound;
    do {
      // Reset rolls
      rollsTakenThisRound = 0;
      Set<PaperRoll> rollsToRemove = new HashSet<Day04.PaperRoll>();
      for (PaperRoll roll : paperRolls) {
        if (!hasXPaperRollsAdjacent(roll, THRESHOLD, paperRolls)) {
          accessiblePaperRollCount++;
          rollsTakenThisRound++;
          rollsToRemove.add(roll);
        }
      }
      // Notably, it seems we only remove them at the end like a wave.
      // This seems wrong to me, but it's easier to deal with.
      for (PaperRoll roll: rollsToRemove){
        paperRolls.remove(roll);
      }
    } while (rollsTakenThisRound > 0);
    System.out.println(accessiblePaperRollCount);
  }

  private boolean hasXPaperRollsAdjacent(PaperRoll roll, int threshold, Set<PaperRoll> rolls) {
    ArrayList<PaperRoll> rollsToCheck = new ArrayList<>();
    rollsToCheck.add(new PaperRoll(roll.x - 1, roll.y - 1)); // Top left
    rollsToCheck.add(new PaperRoll(roll.x - 1, roll.y)); // Top
    rollsToCheck.add(new PaperRoll(roll.x - 1, roll.y + 1)); // Top right
    rollsToCheck.add(new PaperRoll(roll.x, roll.y - 1)); // Left
    rollsToCheck.add(new PaperRoll(roll.x, roll.y + 1)); // Right
    rollsToCheck.add(new PaperRoll(roll.x + 1, roll.y - 1)); // Bottom left
    rollsToCheck.add(new PaperRoll(roll.x + 1, roll.y)); // Bottom
    rollsToCheck.add(new PaperRoll(roll.x + 1, roll.y + 1)); // Bottom right

    int rollCount = 0;
    for (PaperRoll r : rollsToCheck) {
      if (rolls.contains(r))
        rollCount++;
    }

    return rollCount >= threshold;
  }

  private class PaperRoll {
    public int x;
    public int y;

    public PaperRoll(int x, int y) {
      this.x = x;
      this.y = y;
    }

    @Override
    public boolean equals(Object other) {
      if (this == other)
        return true;
      if (other == null || getClass() != other.getClass())
        return false;
      PaperRoll otherPaperRoll = (PaperRoll) other;
      return this.x == otherPaperRoll.x && this.y == otherPaperRoll.y;
    }

    @Override
    public int hashCode() {
      return Objects.hash(this.x, this.y);
    }
  }
}
