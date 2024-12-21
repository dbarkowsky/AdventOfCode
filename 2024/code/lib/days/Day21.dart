import 'dart:core';
import 'package:code/days/Day.dart';

// My good friend, Coordinate, back again.
typedef Coordinate = ({int x, int y});

class Day21 extends Day {
  Day21(super.fileName, super.useTestData);

  Map<String, Coordinate> numPad = {
    '7': (x: 0, y: 0),
    '8': (x: 0, y: 1),
    '9': (x: 0, y: 2),
    '4': (x: 1, y: 0),
    '5': (x: 1, y: 1),
    '6': (x: 1, y: 2),
    '1': (x: 2, y: 0),
    '2': (x: 2, y: 1),
    '3': (x: 2, y: 2),
    '0': (x: 3, y: 1),
    'A': (x: 3, y: 2),
    'C': (x: 1, y: 1),
  };

  Map<String, Coordinate> dirPad = {
    '^': (x: 0, y: 1),
    'A': (x: 0, y: 2),
    '<': (x: 1, y: 0),
    'v': (x: 1, y: 1),
    '>': (x: 1, y: 2),
    'C': (x: 1, y: 1),
  };
  // NOTE: If you're moving towards the "centre" (C), move horizontal first. If moving away from "centre", move vertically first.

  void part1() {
    int total = 0;
    String code = '379A'; // 24256
    // for (final code in input){
      // Get initial directions for numPad
      List<String> numPadDirections = convertToNext(code.split(""), numPad);
      // Convert for middle robot
      List<String> middleDirections = convertToNext(numPadDirections, dirPad);
      // Convert again for human (me)
      List<String> humanDirections = convertToNext(middleDirections, dirPad);
      // Get complexity score
      RegExp number = RegExp(r"[\d]+");
      int numericPart = int.parse(number.firstMatch(code)!.group(0)!);
      total += humanDirections.length * numericPart;
      /// <vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A
      /// v<<A>>^A<A>AvA<^AA>A<vAAA>^A
      /// <A^A>^^AvvvA
      /// 029A
      print(humanDirections.join(""));
      print(middleDirections.join(""));;
      print(numPadDirections.join(""));
      print(code);
      print(humanDirections.length * numericPart);
    // }
    print(total);
  }

  void part2() {
  }

  // FIXME: Could cache these results if we tracked depth of robots + pad
  List<String> convertToNext(List<String> input, Map<String, Coordinate> pad){
    List<String> moves = [];
    String current = 'A';
    for (final piece in input){
      // Get moves from current to this piece
      List<String> movesToPiece = getMovesBetween(current, piece, pad);
      // Add moves to list
      moves.addAll(movesToPiece);
      moves.add('A');
      // Update current
      current = piece;
    }
    return moves;
  }

  // FIXME: There's something here where we need to consider multiple different possible moves. 
  List<String> getMovesBetween(String start, String end, Map<String, Coordinate> pad){
    List<String> moves = [];
    int verticalDiff = pad[end]!.x - pad[start]!.x;
    int horizontalDiff = pad[end]!.y - pad[start]!.y;
    // If we stay on the same button, return empty moves.
    if (start == end) return moves;
    bool doHorizontalFirst = pad == numPad ? verticalDiff > 0 ? true : false : verticalDiff > 0 ? false : true;
    // Are we going away from centre or towards it?
    // This decision helps avoid the blank space on both pads
    if (doHorizontalFirst){
      // We are getting closer, do horizontal first
      if (horizontalDiff > 0){
        // We're going right
        moves.addAll(List.filled(horizontalDiff, '>'));
      } else if (horizontalDiff < 0){
        // We're going left
        moves.addAll(List.filled(horizontalDiff.abs(), '<'));
      }
      if (verticalDiff > 0){
        // We're going down
        moves.addAll(List.filled(verticalDiff, 'v'));
      } else if (verticalDiff < 0){
        // We're going up
        moves.addAll(List.filled(verticalDiff.abs(), '^'));
      }
    } else {
      // We are getting farther, do vertical first
      if (verticalDiff > 0){
        // We're going down
        moves.addAll(List.filled(verticalDiff, 'v'));
      } else if (verticalDiff < 0){
        // We're going up
        moves.addAll(List.filled(verticalDiff.abs(), '^'));
      }
      if (horizontalDiff > 0){
        // We're going right
        moves.addAll(List.filled(horizontalDiff, '>'));
      } else if (horizontalDiff < 0){
        // We're going left
        moves.addAll(List.filled(horizontalDiff.abs(), '<'));
      }
    }
    return moves;
  }

  int getDistance(Coordinate a, Coordinate b){
    return (a.x - b.x).abs() + (a.y - b.y).abs();
  }
}
