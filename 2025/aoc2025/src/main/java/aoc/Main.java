package aoc;

import aoc.utils.FileReader;
import aoc.days.*;

public class Main {
  public static void main(String[] args) {
    // Check arguments. Should be at least 1 argument: the day number
    if (args.length < 1) {
      System.out.println("Please provide the day number as an argument.");
      return;
    }
    // print the working path
    System.out.println("Working Directory = " + System.getProperty("user.dir"));
    // Deal with args
    int dayNum = Integer.parseInt(args[0]);
    boolean test = args.length > 1 && args[1].equals("test");
    // Load file
    String filePath =  String.format(test ? "input/%02d_test.txt" : "input/%02d.txt", dayNum);
    FileReader fileReader = new FileReader(filePath);
    // Start timer
    long startingMillis = System.currentTimeMillis();
    // Call correct day
    switch (dayNum) {
      case 1:
        Day01 day = new Day01(fileReader.lines);
        day.Part1();
        day.Part2();
        break;
      // Add more cases for other days
      default:
        System.out.println("Invalid day number.");
    }
    // Print time
    System.out.printf("Time taken: %dms", System.currentTimeMillis() - startingMillis);
  }
}
