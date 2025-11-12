package aoc.utils;

import java.io.File;
import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.Scanner;

public class FileReader {
  public ArrayList<String> lines = new ArrayList<>();
  public FileReader(String path) {
    try {
      Scanner scanner = new Scanner(new File(path));
      while (scanner.hasNextLine()) {
        lines.add(scanner.nextLine());
      }
      scanner.close();
    } catch (FileNotFoundException e) {
      e.printStackTrace();
    }
  }
}
