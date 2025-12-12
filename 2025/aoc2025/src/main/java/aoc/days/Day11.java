package aoc.days;

import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Queue;
import java.util.Set;

public class Day11 {
  Map<String, ArrayList<String>> graph = new HashMap<>();

  public Day11(ArrayList<String> input) {
    // 100% is a directional graph, so let's make that
    // Will be an adjacency list
    for (String line : input) {
      String key = line.split(": ")[0];
      String[] values = line.split(": ")[1].split(" ");
      graph.put(key, new ArrayList<String>(Arrays.asList(values)));
    }
  }

  // Find all possible paths from `you` to `out`
  // This is basically a BFS. Works well enough for this small size
  public void part1() {
    System.out.println("Day 11, Part 1");
    long count = 0;
    String currentNode = "you";
    String target = "out";
    // We'll use a queue to run through all the options.
    Queue<String> queue = new ArrayDeque<>();
    queue.add(currentNode);

    while (!queue.isEmpty()) {
      currentNode = queue.poll();
      if (currentNode.equals(target)) {
        count++;
      } else {
        ArrayList<String> neighbours = graph.get(currentNode);
        for (String neighbour : neighbours) {
          // Add neighbour to the queue
          queue.add(neighbour);
        }
      }
    }
    System.out.println(count);
  }

  public void part2() {
    System.out.println("Day 11, Part 2");
    // Going to try to break this up and use DFS instead
    long a = countPaths("svr", "fft");
    long b = countPaths("fft", "dac");
    long c = countPaths("dac", "out");

    long d = countPaths("svr", "dac");
    long e = countPaths("dac", "fft");
    long f = countPaths("fft", "out");

    // The concept here is that we can take all the separate parts and get the final answer
    // This hopefully prevents any single part from exceeding memory, etc.
    long total = (a * b * c) + (d * e * f);

    System.out.println(total);
  }

  // Just a helper to start the DFS for a section
  private long countPaths(String start, String end) {
    return dfsCount(start, end, new HashSet<>(), new HashMap<>());
  }

  // Depth-first search to count number of ways from current to end.
  // The memoization was key here. It never finished without it.
  // The int vs long issue also messed me up here for a while. Another overflow.
  private long dfsCount(String current, String end, Set<String> visited, Map<String, Long> memo) {
    if (current.equals(end)) {
      return 1;
    }
    if (visited.contains(current))
      return 0;

    // If we've seen this one before, use known value.
    if (memo.containsKey(current))
      return memo.get(current);

    visited.add(current);

    long total = 0;
    // Shouldn't even need this check.. but just in case
    if (graph.containsKey(current)) {
      for (String next : graph.get(current)) {
        total += dfsCount(next, end, visited, memo);
      }
    }

    visited.remove(current);
    // Store known endings from this node
    // If we encounter it in the future, we don't have to recalculate.
    memo.put(current, total);
    return total;
  }

  // This didn't work using the same approach but storing the state.
  // Runs out of memory even if I go the other way from end to start
  // and not tracking the path in full.
  public void part2v1() {
    System.out.println("Day 11, Part 2");
    long count = 0;
    State currentState = new State("svr");
    // We'll use a queue to run through all the options.
    Queue<State> queue = new ArrayDeque<>();
    queue.add(currentState);

    while (!queue.isEmpty()) {
      currentState = queue.poll();
      if (currentState.isAtEnd() && currentState.containsDevices()) {
        count++;
      } else if (!currentState.isAtEnd()) {
        ArrayList<String> neighbours = graph.get(currentState.getLast());
        for (String neighbour : neighbours) {
          if (!currentState.path.contains(neighbour)) {
            State neighbourState = new State(currentState.path, neighbour);
            queue.add(neighbourState);
          }
        }
      }
      // At end but doesn't contain devices? We let this path die.
    }
    System.out.println(count);
  }

  private class State {
    ArrayList<String> path = new ArrayList<>();
    boolean fft = false;
    boolean dac = false;

    public State(String initialNode) {
      path.add(initialNode);
    }

    public State(ArrayList<String> existingPath, String newNode) {
      this.path = new ArrayList<>(existingPath);
      this.path.add(newNode);
    }

    public String getLast() {
      return path.get(path.size() - 1);
    }

    public boolean isAtEnd() {
      return path.get(path.size() - 1).equals("out");
    }

    public boolean containsDevices() {
      return path.contains("fft") && path.contains("dac");
    }
  }
}
