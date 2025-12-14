package aoc.days;

import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Queue;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class Day10 {
  ArrayList<Machine> machines = new ArrayList<>();

  public Day10(ArrayList<String> input) {
    Pattern lightPattern = Pattern.compile("\\[(.+)\\]");
    Pattern buttonPattern = Pattern.compile("\\(([\\d,]+)\\)");
    Pattern joltagePattern = Pattern.compile("\\{(.+)\\}");
    for (String line : input) {
      // Lights
      Matcher lightMatcher = lightPattern.matcher(line);
      ArrayList<String> lights = new ArrayList<>();
      if (lightMatcher.find()) {
        String inside = lightMatcher.group(1); // the bracket content
        Collections.addAll(lights, inside.split(""));
      }
      // Buttons
      Matcher buttonMatcher = buttonPattern.matcher(line);
      ArrayList<ArrayList<Integer>> buttons = new ArrayList<>();
      buttonMatcher.results().forEach(r -> {
        ArrayList<Integer> buttonGroup = new ArrayList<>();
        String[] buttonStrings = r.group(1).split(",");
        for (String s : buttonStrings) {
          buttonGroup.add(Integer.parseInt(s));
        }
        buttons.add(buttonGroup);
      });
      // Joltages
      Matcher joltageMatcher = joltagePattern.matcher(line);
      ArrayList<Integer> joltages = new ArrayList<>();
      if (joltageMatcher.find()) {
        String inside = joltageMatcher.group(1); // the bracket content
        for (String s : inside.split(",")) {
          joltages.add(Integer.parseInt(s));
        }
      }
      // Then save them all together
      machines.add(new Machine(lights, buttons, joltages));
    }
  }

  // This solution isn't very elegant.
  // We are basically trying all possible combos
  // There's probably a way to optimize this, but more likely
  // there's just a better way to do it.
  public void part1() {
    System.out.println("Day 10, Part 1");
    ArrayList<Integer> fewestPresses = new ArrayList<>();
    for (Machine m : machines) {
      // Make a queue.
      // Saves current state of lights, buttons pressed, etc.
      Queue<State> stateQueue = new ArrayDeque<>();
      // Add each button press as the first step.
      // Each one is the start of a new timeline...
      for (ArrayList<Integer> button : m.buttons) {
        State s = new State(m.lights, new ArrayList<>());
        s.pressButton(button);
        stateQueue.add(s);
      }

      // Run Queue loop
      // We'll run this until we break....
      while (!stateQueue.isEmpty()) {
        State current = stateQueue.poll();
        // Did this state already turn off all the lights?
        if (current.allLightsAreOff()) {
          // We've done what we set out to do
          // Add to list for total and move on
          fewestPresses.add(current.steps);
          break;
        } else {
          // Split the timeline again!
          for (ArrayList<Integer> button : m.buttons) {
            State s = new State(current);
            s.pressButton(button);
            stateQueue.add(s);
          }
        }
      }
    }

    // We've evaluated all machines. Get the total
    long sum = 0;
    for (Integer n : fewestPresses) {
      sum += n;
    }

    System.out.println(sum);
  }

  // NOTE: Online discorse is to use Z3,
  // but I didn't want to use an external library.
  // This level of algebra was way over my head though.
  // Had to turn to GPT for help on this one.
  public void part2() {
    System.out.println("Day 10, Part 2");
    long sum = 0;
    for (Machine m : machines) {
      ArrayList<Integer> currentJoltage = new ArrayList<>(m.joltages);
      // Using a matrix. Each column is a button, rows are voltages affected
      // One extra row for Reduced Row Echelon Form (RREF)
      double[][] matrix = new double[currentJoltage.size()][m.buttons.size() + 1];
      for (int col = 0; col < m.buttons.size(); col++) {
        for (int row = 0; row < currentJoltage.size(); row++) {
          matrix[row][col] = m.buttons.get(col).contains(row) ? 1 : 0;
        }
      }
      // For each row in the last collumn, add the desired end value
      int col = m.buttons.size();
      for (int row = 0; row < currentJoltage.size(); row++) {
        matrix[row][col] = m.joltages.get(row);
      }
      // Going to use something called Gaussian Elimination
      // First, populate the matrix with the RREF value
      rref(matrix);
      // Then establish a LinearSystem for calculations
      LinearSystem sys = new LinearSystem(matrix, m.buttons.size());
      // We'll store the best ongoing solution here, gets updated later
      BestSolution best = new BestSolution();

      // Which variables are "free", i.e. the ones we try values on?
      List<Integer> freeVars = sys.freeVariables();
      if (freeVars.isEmpty()) {
        double[] sol = solveFromFreeVars(sys, Map.of());
        if (isValid(sol)) {
          best.best = sol;
          best.bestSum = Arrays.stream(sol).sum();
        }
      } else {
        // We never need to try more pushes than the max joltage
        int max = Collections.max(m.joltages); // safe upper bound
        enumerateFreeVars(sys, freeVars, 0,
            new double[freeVars.size()],
            max,
            best);
      }
      sum += best.bestSum;
    }
    System.out.println(sum);
  }

  // Could have just done a double for this,
  // but passing by reference was nice.
  private class BestSolution {
    double bestSum = Double.POSITIVE_INFINITY;
    double[] best = null;
  }

  private void enumerateFreeVars(
      LinearSystem sys,
      List<Integer> freeVars,
      int idx,
      double[] values,
      int max,
      BestSolution best) {
    // EARLY PRUNE: partial sum of free vars already too big
    double partial = 0;
    for (int i = 0; i < idx; i++)
      partial += values[i];

    if (partial >= best.bestSum)
      return;

    // We've checked all the free variables.
    // Time to pick the best and leave
    if (idx == freeVars.size()) {
      Map<Integer, Double> free = new HashMap<>();
      for (int i = 0; i < freeVars.size(); i++)
        free.put(freeVars.get(i), values[i]);

      double[] sol = solveFromFreeVars(sys, free);
      if (!isValid(sol))
        return;

      double sum = 0;
      for (double v : sol)
        sum += v;

      if (sum < best.bestSum) {
        best.bestSum = sum;
        best.best = sol.clone();
      }
      return;
    }

    // Otherwise, we continue deeper, adjusting the starting index and values
    for (int v = 0; v <= max; v++) {
      values[idx] = v;
      enumerateFreeVars(sys, freeVars, idx + 1, values, max, best);
    }
  }

  // Determines a set of button presses that could result in the correct joltage
  // based on which variables are "free"
  private double[] solveFromFreeVars(
      LinearSystem sys,
      Map<Integer, Double> free) {
    double[] x = new double[sys.vars];

    // assign free variables
    for (var e : free.entrySet())
      x[e.getKey()] = e.getValue();

    // compute dependent variables
    for (int v = 0; v < sys.vars; v++) {
      int row = sys.pivotRow[v];
      if (row == -1)
        continue;

      double value = sys.rref[row][sys.vars];
      for (int j = 0; j < sys.vars; j++)
        if (j != v)
          value -= sys.rref[row][j] * x[j];

      x[v] = value;
    }

    return x;
  }

  // Class to help establish which variables in the equation are considered "free
  // variables", which are the ones we can iterate and determine other variables
  // from
  private class LinearSystem {
    int vars;
    int[] pivotRow; // -1 means free variable
    double[][] rref;

    LinearSystem(double[][] rref, int vars) {
      this.rref = rref;
      this.vars = vars;
      pivotRow = new int[vars];
      Arrays.fill(pivotRow, -1);

      for (int i = 0; i < rref.length; i++) {
        for (int j = 0; j < vars; j++) {
          if (Math.abs(rref[i][j] - 1.0) < 1e-9) {
            pivotRow[j] = i;
            break;
          }
        }
      }
    }

    List<Integer> freeVariables() {
      List<Integer> list = new ArrayList<>();
      for (int i = 0; i < vars; i++)
        if (pivotRow[i] == -1)
          list.add(i);
      return list;
    }
  }

  // http://linear.ups.edu/html/section-RREF.html
  // Reduces the matrix that represents buttons/joltages
  // It's a little over my head.
  private void rref(double[][] m) {
    int rows = m.length;
    int cols = m[0].length;

    int lead = 0;

    for (int r = 0; r < rows; r++) {
      if (lead >= cols)
        return;

      int i = r;
      while (Math.abs(m[i][lead]) < 1e-9) {
        i++;
        if (i == rows) {
          i = r;
          lead++;
          if (lead == cols)
            return;
        }
      }

      // Swap rows i and r
      double[] temp = m[r];
      m[r] = m[i];
      m[i] = temp;

      // Normalize row r
      double lv = m[r][lead];
      for (int j = 0; j < cols; j++)
        m[r][j] /= lv;

      // Eliminate other rows
      for (int k = 0; k < rows; k++) {
        if (k != r) {
          double factor = m[k][lead];
          for (int j = 0; j < cols; j++)
            m[k][j] -= factor * m[r][j];
        }
      }

      lead++;
    }
  }

  // Checks if all values are within max bounds
  private boolean isValid(double[] x) {
    for (double v : x) {
      if (v < -1e-9)
        return false;
      if (Math.abs(v - Math.round(v)) > 1e-9)
        return false;
    }
    return true;
  }

  // This did not work. Maybe it would eventually,
  // but I've once again ran out of RAM.
  public void part2v1() {
    System.out.println("Day 10, Part 2");
    // I think we can do something similar here.
    // Start with the joltage amounts and count down.
    // Split timelines around the queue, and stop a timeline if anything goes < 0
    ArrayList<Integer> fewestPresses = new ArrayList<>();
    for (Machine m : machines) {
      Queue<State> stateQueue = new ArrayDeque<>();
      for (ArrayList<Integer> button : m.buttons) {
        State s = new State(new ArrayList<>(), m.joltages);
        s.pressButtonForJoltage(button);
        stateQueue.add(s);
      }

      // Run Queue loop
      // We'll run this until we break....
      while (!stateQueue.isEmpty()) {
        State current = stateQueue.poll();
        if (current.joltageIsZero()) {
          fewestPresses.add(current.steps);
          break;
        } else {
          for (ArrayList<Integer> button : m.buttons) {
            State s = new State(current);
            s.pressButtonForJoltage(button);
            stateQueue.add(s);
          }
        }
      }
    }

    // We've evaluated all machines. Get the total
    long sum = 0;
    for (Integer n : fewestPresses) {
      sum += n;
    }

    System.out.println(sum);
  }

  private class State {
    int steps = 0;
    ArrayList<ArrayList<Integer>> buttonsPressed = new ArrayList<>();
    ArrayList<String> lights;
    ArrayList<Integer> joltages = new ArrayList<>();

    public State(ArrayList<String> lights, ArrayList<Integer> joltages) {
      this.lights = new ArrayList<>(lights);
      this.joltages = new ArrayList<>(joltages);
    }

    public State(State original) {
      this.steps = original.steps;
      this.buttonsPressed = new ArrayList<>(original.buttonsPressed);
      this.lights = new ArrayList<>(original.lights);
      this.joltages = new ArrayList<>(original.joltages);
    }

    public void incrementStep() {
      this.steps++;
    }

    public void pressButton(ArrayList<Integer> button) {
      for (int i = 0; i < lights.size(); i++) {
        if (button.contains(i)) {
          if (lights.get(i).equals("#")) {
            lights.set(i, ".");
          } else {
            lights.set(i, "#");
          }
        }
      }
      buttonsPressed.add(button);
      this.incrementStep();
    }

    public void pressButtonForJoltage(ArrayList<Integer> button) {
      for (int i = 0; i < button.size(); i++) {
        int joltageIndex = button.get(i);
        joltages.set(joltageIndex, joltages.get(joltageIndex) - 1);
      }
      buttonsPressed.add(button);
      this.incrementStep();
    }

    public boolean joltageIsZero() {
      for (int jolt : joltages) {
        if (jolt != 0)
          return false;
      }
      return true;
    }

    public boolean allLightsAreOff() {
      for (String light : lights) {
        if (light.equals("#"))
          return false;
      }
      return true;
    }
  }

  private class Machine {
    ArrayList<String> lights = new ArrayList<>();
    ArrayList<ArrayList<Integer>> buttons = new ArrayList<>();
    ArrayList<Integer> joltages = new ArrayList<>();

    public Machine(ArrayList<String> lights, ArrayList<ArrayList<Integer>> buttons, ArrayList<Integer> joltages) {
      this.lights = lights;
      this.buttons = buttons;
      this.joltages = joltages;
    }
  }
}
