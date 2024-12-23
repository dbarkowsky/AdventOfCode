import 'package:code/days/Day.dart';

class Day23 extends Day {
  Map<String, List<String>> graph = {};

  // Build graph here as adjacency list
  Day23(super.fileName, super.useTestData) {
    for (final line in input) {
      var [a, b] = line.split("-");
      graph.update(a, (value) => [...value, b], ifAbsent: () => [b]);
      graph.update(b, (value) => [...value, a], ifAbsent: () => [a]);
    }
  }

  // How many 3-node cycles are there that have a node starting with t?
  void part1() {
    // For each node, check its connections, see if any of its connections connect back to the original node
    Set<String> cycles = {};
    for (final entry in graph.entries) {
      // Check its connections
      for (final connection in entry.value) {
        // And its connections
        List<String> connectionsConnections = graph[connection]!;
        for (final deepConnection in connectionsConnections) {
          if (graph[deepConnection]!.contains(entry.key)) {
            // We have a 3-node cycle
            List<String> cycle = [entry.key, connection, deepConnection];
            cycle.sort();
            if (cycle.any((el) => el.startsWith("t"))) {
              cycles.add(cycle.join("-"));
            }
          }
        }
      }
    }
    print(cycles.length);
  }

  // What's the biggest clique (fully-interconnected nodes) in the graph?
  void part2() {
    // Find all the possible cliques and sort by length
    List<List<String>> cliques = findCliques();
    cliques.sort((a, b) => b.length - a.length);
    // There is only one of maximum length, so no need to compare alphabetical sort and length
    List<String> codeList = cliques.first;
    codeList.sort();
    print(codeList.join(","));
  }

  // Parent function to hold list of cliques
  List<List<String>> findCliques() {
    List<String> nodes = graph.keys.toList();
    List<List<String>> cliques = [];
    bronKerbosch([], nodes, [], cliques);
    return cliques;
  }

  // https://en.wikipedia.org/wiki/Bron%E2%80%93Kerbosch_algorithm
  // Recursive algorithm that uses lists of potential nodes and excluded nodes to build cliques
  void bronKerbosch(List<String> currentClique, List<String> potentialNodes,
      List<String> excludedNodes, List<List<String>> cliques) {
    if (potentialNodes.isEmpty && excludedNodes.isEmpty) {
      // There are no more options, so this is a fully realized clique
      cliques.add([...currentClique]);
      return;
    }
    // For each potential node
    for (final node in potentialNodes) {
      // Get its edges and recurse
      List<String> edges = graph[node]!;
      // Current clique grows by adding this node
      // Potential nodes are only the intersection of the original potential nodes and this node's edges
      // Excluded nodes are the intersection of previously excluded nodes and this node's edges
      bronKerbosch(
          [...currentClique, node],
          intersection(potentialNodes, edges),
          intersection(excludedNodes, edges),
          cliques);

      // Node is removed from potential nodes but added to excluded nodes
      // This affects subsequent nodes in this loop
      potentialNodes = potentialNodes.where((el) => el != node).toList();
      excludedNodes.add(node);
    }
  }

  List<String> intersection(List<String> a, List<String> b) {
    return a.where((n) => b.contains(n)).toList();
  }
}
