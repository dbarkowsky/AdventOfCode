import 'package:code/days/Day.dart';

class Day04 extends Day {
  Day04(super.fileName, super.useTestData);

  // Counting instances of word XMAS
  void part1() {
    int total = 0;
    // Horizontal lines
    total += countLines(input);
    // Horizontal lines reversed
    List<String> horizontalReversed =
        input.map((line) => line.split("").reversed.join()).toList();
    total += countLines(horizontalReversed);
    // Vertical lines
    List<List<String>> matrix = input.map((line) => line.split("")).toList();
    List<String> transposedInput = [];
    for (int i = 0; i < matrix[0].length; i++) {
      String vertical = "";
      for (int j = 0; j < matrix.length; j++) {
        vertical += matrix[j][i];
      }
      transposedInput.add(vertical);
    }
    total += countLines(transposedInput);
    // Vertical lines reversed
    List<String> verticalReversed =
        transposedInput.map((line) => line.split("").reversed.join()).toList();
    total += countLines(verticalReversed);
    // Diagonal lines
    // Input is always a square
    List<String> diagonalInput = getDiagonals(matrix);
    total += countLines(diagonalInput);
    // Diagonal lines reversed
    List<String> diagonalReversed =
        diagonalInput.map((line) => line.split("").reversed.join()).toList();
    total += countLines(diagonalReversed);
    // Now diagonals in the other direction
    // Achieved by flipping the board
    List<List<String>> flippedMatrix =
        matrix.map((line) => line.reversed.toList()).toList();
    List<String> diagonalFlippedInput = getDiagonals(flippedMatrix);
    total += countLines(diagonalFlippedInput);
    // Diagonal lines reversed
    List<String> diagonalFlippedReversed = diagonalFlippedInput
        .map((line) => line.split("").reversed.join())
        .toList();
    total += countLines(diagonalFlippedReversed);
    // Final count
    print(total);
  }

  // Counting instances of
  // M   M
  //   A
  // S   S
  void part2() {
    // Create matrix, iterate through each X and Y, checking XMAS pattern.
    // Do all XMAS orientations as you go.
    List<List<String>> matrix = input.map((line) => line.split("")).toList();
    int total = 0;
    // We can start at 1 and end 1 early because can't make X from edges
    for (int x = 1; x < matrix.length - 1; x++) {
      for (int y = 1; y < matrix[x].length - 1; y++) {
        // Only need to check A locations
        if (matrix[x][y] == "A") {
          if (isXmas(x, y, matrix)) {
            total++;
          }
        }
      }
    }
    print(total);
  }

  bool isXmas(int x, int y, List<List<String>> matrix) {
    // We are looking for this pattern in any orientation
    // M   M    a   b
    //   A    =   A
    // S   S    c   d
    try {
      String a = matrix[x - 1][y - 1];
      String b = matrix[x - 1][y + 1];
      String c = matrix[x + 1][y - 1];
      String d = matrix[x + 1][y + 1];

      // Combine to one string, see if they match expected patterns
      String combo = a + b + c + d;
      RegExp validOptions =
          RegExp(r"MMSS|SSMM|MSMS|SMSM"); // SMMS and MSSM are not valid
      return validOptions.hasMatch(combo);
    } catch (e) {
      return false;
    }
  }

  int countLines(List<String> entries) {
    int sum = 0;
    for (final line in entries) {
      sum += getXmasCount(line);
    }
    return sum;
  }

  List<String> getDiagonals(List<List<String>> matrix) {
    List<String> diagonalLines = [];
    // Add values to half way point
    for (int i = 0; i < matrix.length; i++) {
      int x = i;
      int y = 0;
      String diagonalLine = "";
      while (x >= 0) {
        diagonalLine += matrix[x][y];
        x--;
        y++;
      }
      diagonalLines.add(diagonalLine);
    }
    // Add values from half way to end
    for (int i = 1; i < matrix[0].length; i++) {
      int y = i;
      int x = matrix.length - 1;
      String diagonalLine = "";
      while (x >= 0 && y < matrix[0].length) {
        diagonalLine += matrix[x][y];
        x--;
        y++;
      }
      diagonalLines.add(diagonalLine);
    }
    return diagonalLines;
  }

  int getXmasCount(String line) {
    RegExp xmas = RegExp(r"XMAS");
    return xmas.allMatches(line).length;
  }
}
