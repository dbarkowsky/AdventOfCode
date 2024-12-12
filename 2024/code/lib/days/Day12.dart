import 'package:code/days/Day.dart';

class FarmNode {
  bool visited = false;
  int x;
  int y;
  String type;
  FarmNode(this.x, this.y, this.type);

  // Get the cardinal direction neighbours
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

  // Get its neighbours but only diagonally
  List<FarmNode> getDiagonalNeighbours(List<List<FarmNode>> farm) {
    List<(int, int)> offsets = [(1, 1), (-1, 1), (1, -1), (-1, -1)];
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
  (int, int) answer = (-1, -1);
  Day12(super.fileName, super.useTestData){
    answer = floodFarm();
  }

  // Builds input into grid of farm nodes
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

  // Getting total cost, which is area of section * perimeter of section
  void part1() {
    print(answer.$1);
  }

  // Need to count corners for sections instead to get sides
  // Cost is now area * sides
  void part2() {
    print(answer.$2);
  }

  // One functions to flood the entire farm and get part 1 and 2 answers
  // No repetition this way
  (int, int) floodFarm(){
    List<List<FarmNode>> farm = buildFarm();
    int totalPart1 = 0;
    int totalPart2 = 0;
    for (List<FarmNode> row in farm) {
      for (FarmNode plot in row) {
        // For every plot, if not visited...
        if (!plot.visited) {
          // Flood its section and get measurements back
          var (area, perimeter, sides) = floodFillSection(farm, plot);
          totalPart1 += area * perimeter;
          totalPart2 += area * sides;
        }
      }
    }
    return (totalPart1, totalPart2);
  }

  // Starts at a farm plot. Continues to traverse matching plots, accumulating area, perimeter, and sides.
  (int, int, int) floodFillSection(List<List<FarmNode>> farm, FarmNode start) {
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
    // Count the corners to determine how many edges this shape has
    List<FarmNode> diagonalNeighbours = start.getDiagonalNeighbours(farm);
    int corners = countCorners(start, neighbours, diagonalNeighbours);
    // Get results for flood filling neighbours with same types
    List<(int, int, int)> neighbourFloods = neighbours
        .where(
            (neighbour) => !neighbour.visited && neighbour.type == start.type)
        .map((neighbour) => floodFillSection(farm, neighbour))
        .toList();
    // Add results to this plot and return
    // For perimeter, that's the perimeter of neighbours of same type + this plot's perimeter
    // For area, that's 1 for this plot, plus the area returned by flooding neighbours of same type
    // For corners/sides, that's this plot's corners plus those of its neighbours
    int area = 1;
    for (final flood in neighbourFloods) {
      area += flood.$1;
      perimeter += flood.$2;
      corners += flood.$3;
    }
    return (area, perimeter, corners);
  }

  // Returns the number of corners that can be identified by a plot and its neighbours
  int countCorners(FarmNode plot, List<FarmNode> neighbours, List<FarmNode> diagonalNeighbours) {
    FarmNode north = neighbours.firstWhere(
        (neighbour) => neighbour.x == plot.x - 1 && neighbour.y == plot.y,
        orElse: () => FarmNode(-1, -1, "null"));
    FarmNode south = neighbours.firstWhere(
        (neighbour) => neighbour.x == plot.x + 1 && neighbour.y == plot.y,
        orElse: () => FarmNode(-1, -1, "null"));
    FarmNode east = neighbours.firstWhere(
        (neighbour) => neighbour.x == plot.x && neighbour.y == plot.y + 1,
        orElse: () => FarmNode(-1, -1, "null"));
    FarmNode west = neighbours.firstWhere(
        (neighbour) => neighbour.x == plot.x && neighbour.y == plot.y - 1,
        orElse: () => FarmNode(-1, -1, "null"));
    FarmNode northEast = diagonalNeighbours.firstWhere(
        (neighbour) => neighbour.x == plot.x - 1 && neighbour.y == plot.y + 1,
        orElse: () => FarmNode(-1, -1, "null"));
    FarmNode northWest = diagonalNeighbours.firstWhere(
        (neighbour) => neighbour.x == plot.x - 1 && neighbour.y == plot.y - 1,
        orElse: () => FarmNode(-1, -1, "null"));
    FarmNode southEast = diagonalNeighbours.firstWhere(
        (neighbour) => neighbour.x == plot.x + 1 && neighbour.y == plot.y + 1,
        orElse: () => FarmNode(-1, -1, "null"));
    FarmNode southWest = diagonalNeighbours.firstWhere(
        (neighbour) => neighbour.x == plot.x + 1 && neighbour.y == plot.y - 1,
        orElse: () => FarmNode(-1, -1, "null"));
    int count = 0;
    // These are the conditions for outward-facing corners
    count += north.type != plot.type && east.type != plot.type ? 1 : 0;
    count += north.type != plot.type && west.type != plot.type ? 1 : 0;
    count += south.type != plot.type && east.type != plot.type ? 1 : 0;
    count += south.type != plot.type && west.type != plot.type ? 1 : 0;
    // These are the conditions for inward-facing corners
    // e.g. CD has a corner that faces D
    //      CC
    count += north.type == plot.type && east.type == plot.type && northEast.type != plot.type ? 1 : 0;
    count += north.type == plot.type && west.type == plot.type && northWest.type != plot.type ? 1 : 0;
    count += south.type == plot.type && east.type == plot.type && southEast.type != plot.type ? 1 : 0;
    count += south.type == plot.type && west.type == plot.type && southWest.type != plot.type ? 1 : 0;
    return count;
  }

  void printFarm(List<List<FarmNode>> farm) {
    for (final row in farm) {
      print(row.join());
    }
    print("");
  }
}
