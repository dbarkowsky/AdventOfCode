import 'dart:math';

import 'package:code/days/Day.dart';
import 'package:code/days/Day13.dart';

enum Type { ROBOT, BOX, WALL }

class WideItem {
  List<Coordinate> occupies;
  Type type;
  WideItem(this.occupies, this.type);

  @override
  bool operator ==(Object other) {
    WideItem b = other as WideItem;
    return b.occupies.every((coord) => occupies.contains(coord)) &&
        b.type == type;
  }

  @override
  String toString() {
    return '(${occupies.first.x},${occupies.first.y}):(${occupies.last.x},${occupies.last.y})';
  }
}

class Coordinate {
  int x;
  int y;
  Coordinate(this.x, this.y);

  @override
  bool operator ==(Object other) {
    Coordinate b = other as Coordinate;
    return x == b.x && y == b.y;
  }

  @override
  String toString() {
    return '$x, $y';
  }

  @override
  int get hashCode => toString().hashCode;
}

class Day15 extends Day {
  Day15(super.fileName, super.useTestData);

  (Map<Coordinate, Type>, Coordinate, List<String>) buildItems() {
    List<String> instructions = [];
    Coordinate robot = Coordinate(-1, -1);
    Map<Coordinate, Type> items = {};
    int x = 0;
    while (input[x] != "") {
      List<String> rowItems = input[x].split("");
      for (int y = 0; y < rowItems.length; y++) {
        switch (rowItems[y]) {
          case "@":
            items[Coordinate(x, y)] = Type.ROBOT;
            robot = Coordinate(x, y);
            break;
          case "#":
            items[Coordinate(x, y)] = Type.WALL;
            break;
          case "O":
            items[Coordinate(x, y)] = Type.BOX;
            break;
        }
      }
      x++;
    }
    x++; // Skip blank line
    while (x < input.length) {
      instructions.addAll(input[x].split(""));
      x++;
    }
    return (items, robot, instructions);
  }

  void part1() {
    var (items, robot, instructions) = buildItems();
    for (final instruction in instructions) {
      Coordinate hopefulposition = getNewPosition(instruction, robot);
      // Is there anything here?
      if (items.containsKey(hopefulposition)) {
        if (items[hopefulposition] == Type.WALL) {
          // Do nothing. We're stuck against a wall
        } else {
          // Must be a box
          Coordinate currentPosition =
              getNewPosition(instruction, hopefulposition);
          bool openSpace = true;
          while (items.containsKey(currentPosition)) {
            if (items[currentPosition] == Type.WALL) {
              // There are no blank spaces before the wall. We do nothing, so break the loop
              openSpace = false;
              break;
            }
            currentPosition = getNewPosition(instruction, currentPosition);
          }
          // If we eventually hit an open space (current position)
          if (openSpace) {
            // Move the item in the hopeful position here
            moveBox(hopefulposition, currentPosition, items);
            // Then move the robot into its new space
            robot = moveRobot(hopefulposition, items, robot);
          }
        }
      } else {
        // Nothing in the way. Just move robot
        robot = moveRobot(hopefulposition, items, robot);
      }
    }

    // Calculate score
    int score = 0;
    for (final item in items.entries) {
      if (item.value == Type.BOX) {
        score += 100 * item.key.x + item.key.y;
      }
    }
    print(score);
  }

  void part2() {
    // Turn old items into wide items
    var (oldItems, robot, instructions) = buildItems();
    List<WideItem> items = [];
    for (final item in oldItems.entries) {
      if (item.value != Type.ROBOT) {
        List<Coordinate> coordinates = [
          Coordinate(item.key.x, item.key.y * 2),
          Coordinate(item.key.x, (item.key.y * 2) + 1)
        ];
        items.add(WideItem(coordinates, item.value));
      }
    }
    robot = Coordinate(robot.x, robot.y * 2);
    int i = 0;
    // Actually solve problem
    for (final instruction in instructions) {
      List<Coordinate> hopefulPositions =
          getNextPositions(instruction, [robot], filter: true);
      if (canMove(items, [robot], hopefulPositions, instruction)) {
        // Just move the robot. Everything else should be okay.
        robot =
            hopefulPositions.first; // There should only be one from the robot
      }
      int printnum = 650;
      // if (i > printnum && i <= printnum + 50){
      // print(instruction);
      // printGrid(items, robot);}
      i++;
      
    }
    printGrid(items, robot);
    // Calculate score
    int score = 0;
    for (final item in items) {
      if (item.type == Type.BOX) {
        print(item);
        score += 100 * item.occupies.first.x + item.occupies.first.y;
      }
    }
    print(score);
  }

  bool canMove(List<WideItem> items, List<Coordinate> originalPositions,
      List<Coordinate> hopefulPositions, String instruction) {
    List<WideItem> itemsInHopefulPositions = items
        .where((item) => item.occupies
            .toSet()
            .intersection(hopefulPositions.toSet())
            .isNotEmpty)
        .toList();
    // If no intersection, we can say it's a safe move
    if (itemsInHopefulPositions.isEmpty) {
      // Don't update here, update where called
      return true;
    }
    // Are any of these items walls? Then we can't move
    if (itemsInHopefulPositions.any((item) => item.type == Type.WALL)) {
      return false;
    }
    // Then they must be boxes
    // For each box that's in the way, we need to check if those can move
    // They all need to be able to move for this to continue
    bool allCanMove = itemsInHopefulPositions.every((item) => canMove(
        items,
        item.occupies,
        getNextPositions(instruction, item.occupies, filter: true),
        instruction));
    if (allCanMove) {
      // Update their positions, then pass the true upwards
      for (final hopefulItem in itemsInHopefulPositions) {
        items.removeWhere((item) => item == hopefulItem);
        items.add(WideItem(getNextPositions(instruction, hopefulItem.occupies),
            hopefulItem.type));
      }
      return true;
    }
    return false;
  }

  Coordinate moveRobot(
      Coordinate newPosition, Map<Coordinate, Type> items, Coordinate robot) {
    // Remove the old entry from the map and add new one
    items.remove(robot);
    items[newPosition] = Type.ROBOT;
    // Update reference
    return newPosition;
  }

  void moveBox(Coordinate from, Coordinate to, Map<Coordinate, Type> items) {
    items[to] = Type.BOX;
    items.remove(from);
  }

  Coordinate getNewPosition(String instruction, Coordinate current) {
    switch (instruction) {
      case "<":
        return Coordinate(current.x, current.y - 1);
      case ">":
        return Coordinate(current.x, current.y + 1);
      case "^":
        return Coordinate(current.x - 1, current.y);
      case "v":
        return Coordinate(current.x + 1, current.y);
      default:
        return current;
    }
  }

  List<Coordinate> getNextPositions(
      String instruction, List<Coordinate> currents,
      {bool filter = false}) {
    List<Coordinate> coords = [];
    switch (instruction) {
      case "<":
        coords =
            currents.map((coord) => Coordinate(coord.x, coord.y - 1)).toList();
        break;
      case ">":
        coords =
            currents.map((coord) => Coordinate(coord.x, coord.y + 1)).toList();
        break;
      case "^":
        coords =
            currents.map((coord) => Coordinate(coord.x - 1, coord.y)).toList();
        break;
      case "v":
        coords =
            currents.map((coord) => Coordinate(coord.x + 1, coord.y)).toList();
        break;
      default:
    }
    if (filter) {
      return coords.where((coord) => !currents.contains(coord)).toList();
    }
    return coords;
  }

  void printGrid(List<WideItem> items, Coordinate robot){
    // 100 x 100 assumed
    int size = 20;
    for (int x = 0; x < size / 2; x++){
      List<String> row = [];
      for (int y = 0; y < size; y++){
          if (items.contains(WideItem([Coordinate(x, y)], Type.BOX))){
            row.add("O");
          } else if (items.contains(WideItem([Coordinate(x, y)], Type.WALL))){
            row.add("#");
          } else if (robot.x == x && robot.y == y){
            row.add("@");
          } else {
            row.add(".");
          }
      }
      print(row.join(""));
    }
  }
}
