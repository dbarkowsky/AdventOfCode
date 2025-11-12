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
    int day = Integer.parseInt(args[0]);
    boolean test = args.length > 1 && args[1].equals("test");
    switch (day) {
      case 1:
      // print the working path
      System.out.println("Working Directory = " + System.getProperty("user.dir"));
        String filePath = test ? "input/01_test.txt" : "input/01.txt";
        FileReader fileReader = new FileReader(filePath);
        Day01.Part1(fileReader.lines);
        break;
      // Add more cases for other days
      default:
        System.out.println("Invalid day number.");
    }
  }
}
