import 'dart:collection';
import 'dart:math';
import 'package:collection/collection.dart';
import 'package:code/days/Day.dart';

class MazeState {
  (int, int) location;
  (int, int)? from;
  List<(int, int)> visited;
  int score = 0;

  MazeState(this.location, this.visited, this.score, this.from);
}

class Day16 extends Day {
  (int, int) start = (0, 0);
  (int, int) end = (0, 0);
  List<List<String>> maze = [];
  Day16(super.fileName, super.useTestData) {
    for (int x = 0; x < input.length; x++) {
      List<String> newRow = input[x].split("");
      maze.add(newRow);
      int endIndex = newRow.indexOf("E");
      int startIndex = newRow.indexOf("S");
      if (endIndex != -1) end = (x, endIndex);
      if (startIndex != -1) start = (x, startIndex);
    }
  }

  void part1() {
    PriorityQueue<MazeState> queue =
        PriorityQueue<MazeState>((a, b) => a.score.compareTo(b.score));
    Map<(int, int, int?, int?), int> scoreKeeper = {};
    queue.add(MazeState(start, [], 0, (start.$1, start.$2 - 1)));
    bool endFound = false;
    while (queue.isNotEmpty) {
      MazeState current = queue.removeFirst();
      (int, int, int?, int?) key = (current.location.$1, current.location.$2, current.from?.$1, current.from?.$2);
      if (scoreKeeper.containsKey(key) && scoreKeeper[key]! <= current.score){
        continue;
      } else {
        scoreKeeper[key] = current.score;
      }
      // Is this the end?
      if (current.location.$1 == end.$1 && current.location.$2 == end.$2){
        endFound = true;
        // print(current.score);
        // printGrid(current.visited);
        // break;
      }
      List<(int, int)> neighbours = getNeighbours(current.location);
      // For each one, make a maze state, but then check if we've been here before and if it was a better score.
      // Will have to keep a map cache probably.
      // If this score isn't better, don't add to queue. If it is better, update cache and add to queue
      List<MazeState> possibleStates = neighbours.where((n) => !current.visited.contains(n) && current.location != current.from).map(
          (n) => MazeState(
              n,
              [...current.visited, current.location],
              (current.score +
                  getAdditionalScore(current.from, current.location, n)),
              current.location)).toList();

      queue.addAll(possibleStates);
    }
    if (!endFound) {
      print("Warning: end not found");
    }
    List<int> possibleScores = getNeighbours(end).where((n) => scoreKeeper.containsKey((end.$1, end.$2, n.$1, n.$2))).map((n) => scoreKeeper[(end.$1, end.$2, n.$1, n.$2)]!).toList();
    print(min(possibleScores.first, possibleScores.last));
  }

  void part2() {}

  void printGrid(List<(int, int)> visited){
    for(int x = 0; x < maze.length; x++){
      List<String> row = [];
      for(int y = 0; y < maze[x].length; y++){
        if (visited.contains((x, y))){
          row.add("X");
        } else {
          row.add(maze[x][y]);
        }
      }
      print(row.join(""));
    }
  }

  int getAdditionalScore(
      (int, int)? from, (int, int) current, (int, int) next) {
    // No from? Must be the first one.
    if (from == null) return 1;
    // Did it turn? Not if the difference is the same between from/current and current/next
    (int, int) difference1 = (from.$1 - current.$1, from.$2 - current.$2);
    (int, int) difference2 = (current.$1 - next.$1, current.$2 - next.$2);
    if (difference1.$1 == difference2.$1 && difference1.$2 == difference2.$2) return 1; 
    // Turned
    return 1001;
  }

  List<(int, int)> getNeighbours((int, int) origin) {
    List<(int, int)> offsets = [(-1, 0), (1, 0), (0, -1), (0, 1)];
    return offsets
        .map((offset) => (origin.$1 + offset.$1, origin.$2 + offset.$2))
        .where((coord) => maze[coord.$1][coord.$2] != "#")
        .toList();
  }
}
