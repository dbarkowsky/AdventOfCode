package aoc.days;

import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Queue;
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

  // This solution isn't very elegant.
  // We are basically trying all possible combos
  // There's probably a way to optimize this, but more likely
  // there's just a better way to do it.
  public void part1() {
    System.out.println("Day 10, Part 1");
    ArrayList<Integer> fewestPresses = new ArrayList<>();
    for (Machine m : machines) {
      // Make a queue.
      // Saves current state of lights, buttons pressed, etc.
      Queue<State> stateQueue = new ArrayDeque<>();
      // Add each button press as the first step.
      // Each one is the start of a new timeline...
      for (ArrayList<Integer> button : m.buttons) {
        State s = new State(m.lights);
        s.pressButton(button);
        stateQueue.add(s);
      }

      // Run Queue loop
      // We'll run this until we break....
      while (!stateQueue.isEmpty()) {
        State current = stateQueue.poll();
        // Did this state already turn off all the lights?
        if (current.allLightsAreOff()) {
          // We've done what we set out to do
          // Add to list for total and move on
          fewestPresses.add(current.steps);
          break;
        } else {
          // Split the timeline again!
          for (ArrayList<Integer> button : m.buttons) {
            State s = new State(current);
            s.pressButton(button);
            stateQueue.add(s);
          }
        }
      }
    }
    
    // We've evaluated all machines. Get the total
    long sum = 0;
    for (Integer n : fewestPresses){
      sum += n;
    }

    System.out.println(sum);
  }

  public void part2() {
    System.out.println("Day 10, Part 2");
    // I think we can do something similar here.
    // Start with the joltage amounts and count down. 
    // Split timelines around the queue, and stop a timeline if anything goes below 0
    // NOTE: Online discorse is to use Z3, but I'd like to avoid an external library.
    System.out.println();
  }

  private class State {
    int steps = 0;
    ArrayList<ArrayList<Integer>> buttonsPressed = new ArrayList<>();
    ArrayList<String> lights;

    public State(ArrayList<String> lights) {
      this.lights = new ArrayList<>(lights);
    }

    public State(State original) {
      this.steps = original.steps;
      this.buttonsPressed = new ArrayList<>(original.buttonsPressed);
      this.lights = new ArrayList<>(original.lights);
    }

    public void incrementStep() {
      this.steps++;
    }

    public void pressButton(ArrayList<Integer> button) {
      for (int i = 0; i < lights.size(); i++){
        if (button.contains(i)){
          if (lights.get(i).equals( "#")){
            lights.set(i, ".");
          } else {
            lights.set(i, "#");
          }
        }
      }
      buttonsPressed.add(button);
      this.incrementStep();
    }

    public boolean allLightsAreOff() {
      for (String light : lights) {
        if (light.equals("#"))
          return false;
      }
      return true;
    }
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
