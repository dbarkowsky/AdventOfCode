import 'package:code/days/Day.dart';

class Day23 extends Day {
  Day23(super.fileName, super.useTestData);

  void part1() {
    Map<String, List<String>> graph = {};
    Map<String, bool> visited = {};
    for (final line in input) {
      var [a, b] = line.split("-");
      graph.update(a, (value) => [...value, b], ifAbsent: () => [b]);
      graph.update(b, (value) => [...value, a], ifAbsent: () => [a]);
      visited[a] = false;
      visited[b] = false;
    }
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
            if (cycle.any((el) => el.startsWith("t"))){
            cycles.add(cycle.join("-"));}
          }
        }
      }
    }
    print(cycles.length);
  }

  void part2() {}
}
