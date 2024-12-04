import 'package:code/days/Day.dart';

class Day04 extends Day {
  Day04(super.fileName, super.useTestData);

  void part1() {
    int total = 0;
    // Horizontal lines
    total += countLines(input);
    // Horizontal lines reversed
    List<String> horizontalReversed = input.map((line) => line.split("").reversed.join()).toList();
    total += countLines(horizontalReversed);
    // Vertical lines
    List<List<String>> matrix = input.map((line) => line.split("")).toList();
    List<String> transposedInput = [];
    for (int i = 0; i < matrix[0].length; i++){
      String vertical = "";
      for (int j = 0; j < matrix.length; j++){
        vertical += matrix[j][i];
      }
      transposedInput.add(vertical);
    }
    total += countLines(transposedInput);
    // Vertical lines reversed 
    List<String> verticalReversed = transposedInput.map((line) => line.split("").reversed.join()).toList();
    total += countLines(verticalReversed);
    // Diagonal lines
    // Input is always a square
    List<String> diagonalInput = [];
    // Add values to half way point
    for (int i = 0; i < matrix.length; i++){
      int x = i;
      int y = 0;
      String diagonalLine = "";
      while (x >= 0){
        diagonalLine += matrix[x][y];
        x--;
        y++;
      }
      diagonalInput.add(diagonalLine);
    }
    // Add values from half way to end
    for (int i = 1; i < matrix[0].length; i++){
      int y = i;
      int x = matrix.length - 1;
      String diagonalLine = "";
      while (x >= 0 && y < matrix[0].length){
        diagonalLine += matrix[x][y];
        x--;
        y++;
      }
      diagonalInput.add(diagonalLine);
    }
    total += countLines(diagonalInput);
    // Diagonal lines reversed
    List<String> diagonalReversed = diagonalInput.map((line) => line.split("").reversed.join()).toList();
    total += countLines(diagonalReversed);
    // Now diagonals in the other direction
    // Achieved by flipping the board
        List<List<String>> flippedMatrix = matrix.map((line) => line.reversed.toList()).toList();
List<String> diagonalFlippedInput = [];
    // Add values to half way point
    for (int i = 0; i < flippedMatrix.length; i++){
      int x = i;
      int y = 0;
      String diagonalLine = "";
      while (x >= 0){
        diagonalLine += flippedMatrix[x][y];
        x--;
        y++;
      }
      diagonalFlippedInput.add(diagonalLine);
    }
    // Add values from half way to end
    for (int i = 1; i < flippedMatrix[0].length; i++){
      int y = i;
      int x = flippedMatrix.length - 1;
      String diagonalLine = "";
      while (x >= 0 && y < flippedMatrix[0].length){
        diagonalLine += flippedMatrix[x][y];
        x--;
        y++;
      }
      diagonalFlippedInput.add(diagonalLine);
    }
    total += countLines(diagonalFlippedInput);
    // Diagonal lines reversed
    List<String> diagonalFlippedReversed = diagonalFlippedInput.map((line) => line.split("").reversed.join()).toList();
    total += countLines(diagonalFlippedReversed);
    // Final count
    print(total);
  }

  void part2() {
   
  }

  int countLines(List<String> entries){
    int sum = 0;
    for (final line in entries){
      sum += getXmasCount(line);
    }
    return sum;
  }

  bool isXmas(String first, String second, String third, String fourth){
    return first == "X" && second == "M" && third == "A" && fourth == "S";
  }

  int getXmasCount(String line){
    RegExp xmas = RegExp(r"XMAS");
    return xmas.allMatches(line).length;
  }
}
