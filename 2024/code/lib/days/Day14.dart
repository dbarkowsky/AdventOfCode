import 'package:code/days/Day.dart';

class Robot {
  int moveX = 0;
  int moveY = 0;
  int posX = 0;
  int posY = 0;

  Robot(this.moveX, this.moveY, this.posX, this.posY);

  // Robots handle their own movement. Just need to know max of floor space
  void move(int maxY, int maxX) {
    posX += moveX;
    posY += moveY;

    if (posX < 0) {
      posX = maxX + posX + 1;
    } else if (posX > maxX) {
      posX = posX - maxX - 1;
    }
    if (posY < 0) {
      posY = maxY + posY + 1;
    } else if (posY > maxY) {
      posY = posY - maxY - 1;
    }
  }

  @override
  String toString() {
    return 'p=$posX,$posY v=$moveX,$moveY';
  }

  // So we can use the .contains function on Lists
  @override
  bool operator ==(Object other) {
    Robot b = other as Robot;
    return b.posX == posX && b.posY == posY;
  }
}

class Day14 extends Day {
  int width = 0;
  int height = 0;
  Day14(super.fileName, super.useTestData) {
    // These values are hardcoded based on instructions.
    width = useTestData ? 11 : 101;
    height = useTestData ? 7 : 103;
  }

  // Builds the list of Robots from input
  // I reversed the X and Y dimensions from the instructions intentionally.
  List<Robot> buildRobots() {
    List<Robot> robots = [];
    // Match any positive or negative number
    RegExp number = RegExp(r"[-]?[\d]+");
    for (final line in input) {
      List<RegExpMatch> matches = number.allMatches(line).toList();
      int moveX = int.parse(matches[3].group(0)!);
      int moveY = int.parse(matches[2].group(0)!);
      int posX = int.parse(matches[1].group(0)!);
      int posY = int.parse(matches[0].group(0)!);
      robots.add(Robot(moveX, moveY, posX, posY));
    }
    return robots;
  }

  // Find out which quadrant robots end up in and then get product
  void part1() async {
    List<Robot> robots = buildRobots();
    // Robots move for 100 seconds
    for (int i = 0; i < 100; i++) {
      for (Robot robot in robots) {
        robot.move(width - 1, height - 1);
      }
    }

    // Divide into quadrants
    // Just need to know inside corners coordinates
    int topX = (height ~/ 2) - 1;
    int bottomX = topX + 2;
    int leftY = (width ~/ 2) - 1;
    int rightY = leftY + 2;

    // Put each robot in a quadrant bucket.
    // Numbered like  1 2
    //                3 4
    Map<int, List<Robot>> quadrantMap = {1: [], 2: [], 3: [], 4: []};
    for (Robot robot in robots) {
      if (robot.posX <= topX) {
        if (robot.posY <= leftY) {
          quadrantMap[1]!.add(robot);
        } else if (robot.posY >= rightY) {
          quadrantMap[2]!.add(robot);
        }
      } else if (robot.posX >= bottomX) {
        if (robot.posY <= leftY) {
          quadrantMap[3]!.add(robot);
        } else if (robot.posY >= rightY) {
          quadrantMap[4]!.add(robot);
        }
      }
    }

    // Output is the product of all quadrant occupancies
    int product = 1;
    for (final robotList in quadrantMap.values) {
      product *= robotList.length;
    }
    print(product);
  }

  // Part 2 wants you to find the image of a Christmas tree, but its shape wasn't defined.
  // Wasn't sure what to look for, so instead looked for iterations where robots were clumped up
  // Prints those clumped instances for manual review to find tree.
  void part2() {
    print('Uncomment lines in part 2 to print out seconds and pictures');
    List<Robot> robots = buildRobots();
    // Divide into quadrants
    // Just need to know inside corners coordinates
    int topX = (height ~/ 2) - 1;
    int bottomX = topX + 2;
    int leftY = (width ~/ 2) - 1;
    int rightY = leftY + 2;
    // Just lucky that my answer was below 10000
    // This time, make quadrant map for each second/loop
    // Check if they are clustered at end of iteration
    for (int i = 0; i < 10000; i++) {
      Map<int, List<Robot>> quadrantMap = {1: [], 2: [], 3: [], 4: []};
      for (Robot robot in robots) {
        robot.move(width - 1, height - 1);
        if (robot.posX <= topX) {
          if (robot.posY <= leftY) {
            quadrantMap[1]!.add(robot);
          } else if (robot.posY >= rightY) {
            quadrantMap[2]!.add(robot);
          }
        } else if (robot.posX >= bottomX) {
          if (robot.posY <= leftY) {
            quadrantMap[3]!.add(robot);
          } else if (robot.posY >= rightY) {
            quadrantMap[4]!.add(robot);
          }
        }
      }
      // If any quadrant exceeds the ratio, print it for viewing
      double minClusterRatio = 0.34;
      if (quadrantMap[1]!.length / robots.length > minClusterRatio ||
          quadrantMap[2]!.length / robots.length > minClusterRatio ||
          quadrantMap[3]!.length / robots.length > minClusterRatio ||
          quadrantMap[4]!.length / robots.length > minClusterRatio) {
        // Uncomment to manually find image
        // print(i);
        // printGrid(robots);
      }
    }
  }

  // Prints a grid where the robots are.
  void printGrid(List<Robot> robots) {
    for (int x = 0; x < width; x++) {
      List<String> row = [];
      for (int y = 0; y < height; y++) {
        Robot tempBot = Robot(0, 0, x, y);
        if (robots.contains(tempBot)) {
          row.add('X');
        } else {
          row.add('.');
        }
      }
      print(row.join(""));
    }
    print('');
  }
}
