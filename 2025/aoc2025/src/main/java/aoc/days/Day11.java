package aoc.days;

import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;
import java.util.Queue;

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
