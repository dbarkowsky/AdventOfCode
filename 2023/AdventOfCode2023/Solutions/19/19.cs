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


    public Day19(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      foreach (string input in strings)
      {
        Console.WriteLine(input);
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
            Console.WriteLine($"partValue: {condition["partLetter"]} {partValue}");
            if (IsTrue(partValue, condition))
            {
              Console.WriteLine("istrue");

              current = condition["outcome"];
              Console.WriteLine($"current: {current}");
              break;
            }
          }
          if (current == originalCurrent)
          {
            Console.WriteLine($"at default outcome: {instructions.defaultOutcome}");
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

    public int PartTwo()
    {
      return -1;
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
        return comparedValue < int.Parse(option["value"]) ? true : false;
      }
      else
      {
        return comparedValue > int.Parse(option["value"]) ? true : false;
      }
    }
  }
}


