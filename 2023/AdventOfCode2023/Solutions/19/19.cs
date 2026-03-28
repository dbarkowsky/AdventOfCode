using System.Runtime.CompilerServices;
using Tools;

namespace Solutions
{
  public class Day19
  {
    class Instructions
    {
      public List<Dictionary<string, string>> conditions = new List<Dictionary<string, string>>();
      public string defaultOutcome;
      public Instructions(string input)
      {
        List<string> instructions = input.Split(',').ToList();
        this.defaultOutcome = instructions.Last();
        instructions.RemoveAt(instructions.Count - 1); // Remove the last bit that was the default
        foreach (string instruction in instructions)
        {
          Dictionary<string, string> option = new Dictionary<string, string>();
          option["outcome"] = instruction.Split(':').Last();
          string condition = instruction.Split(':').First();
          string greaterOrLess = condition.Contains('>') ? ">" : "<";

          option["operator"] = greaterOrLess;
          option["partLetter"] = condition.Split(greaterOrLess).First();
          option["value"] = condition.Split(greaterOrLess).Last();

          conditions.Add(option);
        }
      }
    }

    List<Dictionary<string, int>> parts = new List<Dictionary<string, int>>();
    Dictionary<string, Instructions> workflows = new Dictionary<string, Instructions>();
    List<string> strings = new List<string>();
    Dictionary<string, long> mins = new Dictionary<string, long>();
    Dictionary<string, long> maxes = new Dictionary<string, long>();
    List<Dictionary<string, (int, int)>> listOfAcceptedRanges = new List<Dictionary<string, (int, int)>>();




    public Day19(string fileName)
    {
      // Add base values to dictionaries
      mins.Add("x", long.MaxValue);
      mins.Add("m", long.MaxValue);
      mins.Add("a", long.MaxValue);
      mins.Add("s", long.MaxValue);
      maxes.Add("x", 0);
      maxes.Add("m", 0);
      maxes.Add("a", 0);
      maxes.Add("s", 0);
      // Parse strings
      strings = FileReader.AsStringArray(fileName).ToList();
      foreach (string input in strings)
      {
        // Console.WriteLine(input);
        if (input != "")
        {
          // Is this an workflow or a part?
          // Parts start with {
          if (input.First() == '{')
          {
            // Break the parts into a Dict
            Dictionary<string, int> newPartDict = new Dictionary<string, int>();
            // REmove {}
            string newPart = input.Remove(0, 1);
            newPart = newPart.Remove(newPart.Length - 1, 1);
            string[] partComponents = newPart.Split(",");
            foreach (string component in partComponents)
            {
              newPartDict[component.Split("=").First()] = int.Parse(component.Split("=").Last());
            }
            // Add Dict to list
            parts.Add(newPartDict);
          }
          else
          {
            // If it's not the blank row or a part, it's a workflow
            // Get workflow name
            string newWorkflow = input.Remove(input.Length - 1, 1); // Remove }
            string workflowName = newWorkflow.Split("{").First();
            string workflowBody = newWorkflow.Split("{").Last();
            workflows[workflowName] = new Instructions(workflowBody);
            // Adjust min and max dicts

          }
        }
      }
    }

    public long PartOne()
    {
      long totalSum = 0;
      foreach (Dictionary<string, int> part in parts)
      {
        string current = "in";
        // Starting at workflow "in"
        while (current != "A" && current != "R")
        {
          string originalCurrent = current;
          Instructions instructions = workflows[current];
          // For each condition in the instructions
          foreach (Dictionary<string, string> condition in instructions.conditions)
          {
            //// Check the condition
            //// If a condition is true, go to the outcome
            int partValue = part[condition["partLetter"]];
            // Console.WriteLine($"partValue: {condition["partLetter"]} {partValue}");
            if (IsTrue(partValue, condition))
            {
              // Console.WriteLine("istrue");

              current = condition["outcome"];
              // Console.WriteLine($"current: {current}");
              break;
            }
          }
          if (current == originalCurrent)
          {
            // Console.WriteLine($"at default outcome: {instructions.defaultOutcome}");
            // Else no conditions were true, 
            //// Go to default outcome
            current = instructions.defaultOutcome;
          }
        }

        // If A, add to part sum
        if (current == "A")
        {
          // Get part total 

          totalSum += GetPartSum(part);
        }
      }
      return totalSum;
    }

    // Finding out how many possible combinations of the xmas ranges make it to the Accepted node
    public long PartTwo()
    {
      // The input is already a graph of how nodes connect

      // Then, DFS down the entirety of the graph, staring from the imaginary "in" node.
      // For each node of the graph, we have an input set of ranges, one for each in 'xmas'
      Dictionary<string, (int, int)> startingRanges = new Dictionary<string, (int, int)>
      {
        { "x", (1, 4000) },
        { "m", (1, 4000) },
        { "a", (1, 4000) },
        { "s", (1, 4000) }
      };

      string startingNode = "in";
      FindCombos(startingNode, startingRanges);
      // At this point, we should have the list of ranges that made it to the A nodes filled.
      // We need to calculate how many potential combinations there are for each surviving range.

      int entryNum = 1;
      long sum = 0;

      foreach (Dictionary<string, (int, int)> entry in listOfAcceptedRanges)
      {
        long partValue = 1;
        // Console.WriteLine("Entry Number: " + entryNum);
        // Number of combinations from each set of ranges is just range of each letter multiplied together
        foreach (KeyValuePair<string, (int, int)> kvp in entry)
        {
          // Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
          if (kvp.Value.Item2 <= kvp.Value.Item1) continue;
          int sizeOfRange = kvp.Value.Item2 - kvp.Value.Item1 + 1;
          partValue *= sizeOfRange;
        }
        entryNum++;
        sum += partValue;
      }
      return sum;
    }

    // Recursive function to narrow down all ranges while traversing to the A node
    private void FindCombos(string key, Dictionary<string, (int, int)> ranges)
    {
      // If the key is one of the terminal nodes (A or R), we can stop recursing.
      if (key == "A")
      {
        listOfAcceptedRanges.Add(ranges);
        return;
      }
      if (key == "R") return;

      Instructions nodeInstructions = workflows[key];
      List<Dictionary<string, string>> conditions = nodeInstructions.conditions;
      // Check every condition in that node. Modify the original range and send recurse to the resulting node.
      for (int i = 0; i < conditions.Count; i++)
      {
        Dictionary<string, string> condition = conditions[i];
        // There will be two recurses from each condition: one for pass and one for fail. We travel both regardless.
        Dictionary<string, (int, int)> modifiedRange = GetModifiedRange(i, conditions, ranges);
        // Send it to the next outcome
        string trueOutcome = condition["outcome"];
        FindCombos(trueOutcome, modifiedRange);
      }
      // Also send the ranges down to the default node
      Dictionary<string, (int, int)> modifiedRanges = new Dictionary<string, (int, int)>(ranges);
      foreach (var condition in conditions)
      {
        // Apply the previous ones in reverse
        ApplyConditionToRange(condition, modifiedRanges, true);
      }
      FindCombos(nodeInstructions.defaultOutcome, modifiedRanges);
    }

    private Dictionary<string, (int, int)> GetModifiedRange(int currentConditionIndex, List<Dictionary<string, string>> conditions, Dictionary<string, (int, int)> previousRanges)
    {
      Dictionary<string, (int, int)> modifiedRanges = new Dictionary<string, (int, int)>(previousRanges);
      // Identify which conditions need to be applied in reverse. These are the ones we've already gone past, but now need the other half of the slice applied.
      List<Dictionary<string, string>> previousConditions = conditions.GetRange(0, currentConditionIndex); // Assuming exclusive of end
      foreach (var condition in previousConditions)
      {
        // Apply the previous ones in reverse
        ApplyConditionToRange(condition, modifiedRanges, true);
      }
      // Then apply the current one in the regular direction
      Dictionary<string, string> currentCondition = conditions[currentConditionIndex];
      ApplyConditionToRange(currentCondition, modifiedRanges);
      return modifiedRanges;
    }

    private void ApplyConditionToRange(Dictionary<string, string> condition, Dictionary<string, (int, int)> range, bool reverse = false)
    {
      string letter = condition["partLetter"];
      string greaterOrLess = condition["operator"];
      int value = int.Parse(condition["value"]);
      // Reverse for getting the remainder after the range restriction. Used for cascading range restrictions in part 2
      if (reverse)
      {
        // Tricky to wrap head around...but if the original was x>5, then here we need to assume the opposite (1-5)
        if (greaterOrLess == ">")
        {
          range[letter] = (range[letter].Item1, Math.Min(range[letter].Item2, value));
        }
        else
        {
          range[letter] = (Math.Max(range[letter].Item1, value), range[letter].Item2);
        }
      }
      else
      {
        // How we modify the ranges depends on the operator, but it will always split it into two new ranges
        if (greaterOrLess == ">")
        {
          range[letter] = (Math.Max(range[letter].Item1, value + 1), range[letter].Item2);
        }
        else
        {
          range[letter] = (range[letter].Item1, Math.Min(range[letter].Item2, value - 1));
        }
      }
    }

    public long GetPartSum(Dictionary<string, int> part)
    {
      long sum = 0;
      sum += part["x"];
      sum += part["m"];
      sum += part["a"];
      sum += part["s"];
      return sum;
    }

    public bool IsTrue(int comparedValue, Dictionary<string, string> option)
    {
      if (option["operator"] == "<")
      {
        return comparedValue < int.Parse(option["value"]);
      }
      else
      {
        return comparedValue > int.Parse(option["value"]);
      }
    }
  }
}


