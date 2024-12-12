import 'package:code/days/Day.dart';

class FarmNode {
  bool visited = false;
  int x;
  int y;
  String type;
  FarmNode(this.x, this.y, this.type);

  List<FarmNode> getNeighbours(List<List<FarmNode>> farm) {
    List<(int, int)> offsets = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    List<(int, int)> nodeCoords = offsets
        .map(((offset) => (offset.$1 + x, offset.$2 + y)))
        .where((coord) =>
            coord.$1 >= 0 &&
            coord.$2 >= 0 &&
            coord.$1 < farm.length &&
            coord.$2 < farm.first.length)
        .toList();
    return nodeCoords.map((coords) => farm[coords.$1][coords.$2]).toList();
  }

  @override
  String toString() {
    return visited ? type.toLowerCase() : type;
  }

  void printCoordinates() {
    print('$x,$y');
  }
}

class Day12 extends Day {
  Day12(super.fileName, super.useTestData);

  List<List<FarmNode>> buildFarm() {
    List<List<FarmNode>> farm = [];
    for (int x = 0; x < input.length; x++) {
      List<FarmNode> farmRow = [];
      List<String> row = input[x].split("");
      for (int y = 0; y < row.length; y++) {
        farmRow.add(FarmNode(x, y, row[y]));
      }
      farm.add(farmRow);
    }
    return farm;
  }

  void part1() {
    List<List<FarmNode>> farm = buildFarm();
    int total = 0;
    for (List<FarmNode> row in farm) {
      for (FarmNode plot in row) {
        if (!plot.visited) {
          var (area, perimeter) = floodFillSection(farm, plot);
          int cost = area * perimeter;
          total += cost;
        }
      }
    }
    print(total);
  }

  // Need to count corners for sections instead
  void part2() {}

  (int, int) floodFillSection(List<List<FarmNode>> farm, FarmNode start) {
    // Mark this plot as visited
    start.visited = true;
    // Get all its neighbours
    List<FarmNode> neighbours = start.getNeighbours(farm);
    // Add fence lengths for edges of farm (i.e. 4 - length of neighbours)
    int perimeter = 4 - neighbours.length;
    // Add fence lengths for neighbours with different types
    for (final neighbour in neighbours) {
      if (neighbour.type != start.type) perimeter++;
    }
    // Get results for flood filling neighbours with same types
    List<(int, int)> neighbourFloods = neighbours
        .where(
            (neighbour) => !neighbour.visited && neighbour.type == start.type)
        .map((neighbour) => floodFillSection(farm, neighbour))
        .toList();
    // Add results to this plot and return
    // For perimeter, that's the perimeter of neighbours of same type + this plot's perimeter
    // For area, that's 1 for this plot, plus the area returned by flooding neighbours of same type
    int area = 1;
    for (final flood in neighbourFloods) {
      area += flood.$1;
      perimeter += flood.$2;
    }
    return (area, perimeter);
  }

  void printFarm(List<List<FarmNode>> farm) {
    for (final row in farm) {
      print(row.join());
    }
    print("");
  }
}
