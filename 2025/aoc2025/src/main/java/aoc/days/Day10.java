package aoc.days;

import java.util.ArrayList;
import java.util.Collections;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class Day10 {
  ArrayList<Machine> machines = new ArrayList<>();

  public Day10(ArrayList<String> input) {
    Pattern lightPattern = Pattern.compile("\\[(.+)\\]");
    Pattern buttonPattern = Pattern.compile("\\(([\\d,]+)\\)");
    Pattern joltagePattern = Pattern.compile("\\{(.+)\\}");
    for (String line : input) {
      // Lights
      Matcher lightMatcher = lightPattern.matcher(line);
      ArrayList<String> lights = new ArrayList<>();
      if (lightMatcher.find()) {
        String inside = lightMatcher.group(1); // the bracket content
        Collections.addAll(lights, inside.split(""));
      }
      // Buttons
      Matcher buttonMatcher = buttonPattern.matcher(line);
      ArrayList<ArrayList<Integer>> buttons = new ArrayList<>();
      buttonMatcher.results().forEach(r -> {
        ArrayList<Integer> buttonGroup = new ArrayList<>();
        String[] buttonStrings = r.group(1).split(",");
        for (String s : buttonStrings) {
          buttonGroup.add(Integer.parseInt(s));
        }
        buttons.add(buttonGroup);
      });
      // Joltages
      Matcher joltageMatcher = joltagePattern.matcher(line);
      ArrayList<Integer> joltages = new ArrayList<>();
      if (joltageMatcher.find()) {
        String inside = joltageMatcher.group(1); // the bracket content
        for (String s : inside.split(",")) {
          joltages.add(Integer.parseInt(s));
        }
      }
      // Then save them all together
      machines.add(new Machine(lights, buttons, joltages));
    }
  }

  public void part1() {
    System.out.println("Day 10, Part 1");

    System.out.println();
  }

  public void part2() {
    System.out.println("Day 10, Part 2");

    System.out.println();
  }

  private class Machine {
    ArrayList<String> lights = new ArrayList<>();
    ArrayList<ArrayList<Integer>> buttons = new ArrayList<>();
    ArrayList<Integer> joltages = new ArrayList<>();

    public Machine(ArrayList<String> lights, ArrayList<ArrayList<Integer>> buttons, ArrayList<Integer> joltages) {
      this.lights = lights;
      this.buttons = buttons;
      this.joltages = joltages;
    }
  }
}
