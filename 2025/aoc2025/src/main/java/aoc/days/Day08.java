package aoc.days;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Objects;
import java.util.PriorityQueue;
import java.util.Set;

public class Day08 {
  ArrayList<JunctionBox> boxes = new ArrayList<>();

  public Day08(ArrayList<String> input) {
    for (String line : input) {
      int x = Integer.parseInt(line.split(",")[0]);
      int y = Integer.parseInt(line.split(",")[1]);
      int z = Integer.parseInt(line.split(",")[2]);
      boxes.add(new JunctionBox(x, y, z));
    }
  }

  // Make X number of connections between boxes, starting with the shorest
  // connections
  // Multiply size of 3 biggests groups (sub-graphs)
  public void part1(int numOfConnections) {
    // First part of this is determining the distance between each pair of boxes
    // We'll add them to a priority queue, and if the queue is greater than the
    // threshold,
    // we'll de-queue one from the back of the queue
    PriorityQueue<JunctionBoxPair> allPairs = new PriorityQueue<>();
    // Looping like this instead means we shouldn't have to check for duplicates
    for (int i = 0; i < boxes.size(); i++) {
      for (int j = i + 1; j < boxes.size(); j++) {
        JunctionBox boxA = boxes.get(i);
        JunctionBox boxB = boxes.get(j);
        JunctionBoxPair pair = new JunctionBoxPair(boxA, boxB);
        allPairs.add(pair);
      }
    }
    // Let's just get the top 10 here instead.
    PriorityQueue<JunctionBoxPair> topXPairs = new PriorityQueue<>();
    for (int i = 0; i < numOfConnections; i++) {
      topXPairs.add(allPairs.poll());
    }
    // At this point, we should have a queue/list of the shortest connections
    // We can treat this like a collection of subgraphs.
    // If we start at one point and visit all connected boxes (nodes),
    // we can identify the size of each subgraph

    // Get a map of all the unique JunctionBoxes from the queue
    Map<String, JunctionBox> uniqueBoxes = new HashMap<>();
    while (topXPairs.size() > 0) {
      JunctionBoxPair pair = topXPairs.poll();
      // ALWAYS use new instances for the graph
      JunctionBox a = uniqueBoxes.computeIfAbsent(
          pair.a.getName(),
          k -> new JunctionBox(pair.a.x, pair.a.y, pair.a.z));

      JunctionBox b = uniqueBoxes.computeIfAbsent(
          pair.b.getName(),
          k -> new JunctionBox(pair.b.x, pair.b.y, pair.b.z));

      a.addConnection(b);
      b.addConnection(a);
    }

    // Track the visited Junction Boxes
    Set<String> visited = new HashSet<>();
    // Priority queue with decending priority. This really messed me up in part 1.
    PriorityQueue<Integer> subgraphSizes = new PriorityQueue<Integer>((a, b) -> b - a);
    // But traverse the graph, starting at each JunctionBox
    for (JunctionBox box : uniqueBoxes.values()) {
      // Start graph traversal
      subgraphSizes.add(findSizeOfSubgraph(box, visited));
    }
    // Now we should have a list of subgraph sizes. Just get top three
    ArrayList<Integer> top3 = new ArrayList<>();
    for (int i = 0; i < 3; i++) {
      top3.add(subgraphSizes.poll());
    }
    // Multiply them
    long product = top3.get(0) * top3.get(1) * top3.get(2);
    System.out.println(product); // Wrong at first attempt. Value 12256 too low
  }

  public void part2() {
  }

  private int findSizeOfSubgraph(JunctionBox startingBox, Set<String> visited) {
    // Don't bother if not visited
    if (!visited.contains(startingBox.getName())) {
      int subgraphSize = 1;
      // Mark as visited right away
      visited.add(startingBox.getName());
      // Now visit other neighbours
      for (JunctionBox neighbour : startingBox.connections.keySet()) {
        subgraphSize += findSizeOfSubgraph(neighbour, visited);
      }
      return subgraphSize;
    } else {
      return 0;
    }
  }

  // Because priority queue in java has no way to just kick off end element or set
  // max
  // Basically returning a fresh queue with correct number of elements
  private PriorityQueue<JunctionBoxPair> cutQueueToSize(PriorityQueue<JunctionBoxPair> queue, int size) {
    PriorityQueue<JunctionBoxPair> newQueue = new PriorityQueue<JunctionBoxPair>();
    for (int i = 0; i < size; i++) {
      newQueue.add(queue.poll());
    }

    queue.clear();
    return newQueue;
  }

  // Class to manage JunctionBox
  private class JunctionBox {
    public int x;
    public int y;
    public int z;
    public Map<JunctionBox, Double> connections;

    public JunctionBox(int x, int y, int z) {
      // These are coordinates in a 3D space
      this.x = x;
      this.y = y;
      this.z = z;
      connections = new HashMap<>();
    }

    public void addConnection(JunctionBox toBox) {
      // Don't add twice
      if (connections.containsKey(toBox))
        return;
      // Calculate distance to this box too
      connections.put(toBox, getDistanceToBox(toBox));
    }

    public void removeConnection(JunctionBox toBox) {
      connections.remove(toBox);
    }

    public double getDistanceToBox(JunctionBox toBox) {
      int[] fromVector = new int[] { this.x, this.y, this.z };
      int[] toVector = new int[] { toBox.x, toBox.y, toBox.z };
      // Find the square differential
      double squareDiff = 0.0;
      for (int i = 0; i < fromVector.length; i++) {
        squareDiff += (fromVector[i] - toVector[i]) * (fromVector[i] - toVector[i]);
      }
      return squareDiff;
    }

    public String getName() {
      return String.format("%d,%d,%d", this.x, this.y, this.z);
    }

    @Override
    public boolean equals(Object other) {
      if (this == other)
        return true;
      if (other == null || getClass() != other.getClass())
        return false;
      JunctionBox otherBox = (JunctionBox) other;
      return this.x == otherBox.x && this.y == otherBox.y && this.z == otherBox.z;
    }

    @Override
    public int hashCode() {
      return Objects.hash(this.x, this.y, this.z);
    }

    @Override
    public String toString() {
      return this.getName();
    }
  }

  private class JunctionBoxPair implements Comparable<JunctionBoxPair> {
    public JunctionBox a;
    public JunctionBox b;
    public double distance;

    public JunctionBoxPair(JunctionBox a, JunctionBox b) {
      this.a = a;
      this.b = b;
      this.distance = a.getDistanceToBox(b);
    }

    @Override
    public int compareTo(JunctionBoxPair other) {
      // Higher priorityValue means higher priority (min-heap behavior)
      return Double.compare(this.distance, other.distance);
    }

    @Override
    public boolean equals(Object o) {
      if (this == o)
        return true;
      if (!(o instanceof JunctionBoxPair))
        return false;
      JunctionBoxPair other = (JunctionBoxPair) o;
      if (this.a == other.a && this.b == other.b)
        return true;
      if (this.a == other.b && this.b == other.a)
        return true;
      return false;
    }

    @Override
    public int hashCode() {
      return a.hashCode() + b.hashCode();
    }
  }
}
