using System.Dynamic;
using System.Text.RegularExpressions;
using Tools;

namespace Solutions
{
  public class Day05
  {
    List<string> lines = new List<string>();
    Dictionary<string, Dictionary<string, ulong>> dictionary = new Dictionary<string, Dictionary<string, ulong>>();
    List<List<ulong[]>> mappings = new List<List<ulong[]>>();

    public Day05(string fileName)
    {
      lines = FileReader.AsStringArray(fileName).ToList();
      // Get seeds from line 1, insert into tuples
      string[] seedStrings = lines[0].Split(": ")[1].Split(" ");
      foreach (string seedString in seedStrings)
      {
        ulong seedNum = ulong.Parse(seedString);
        dictionary.Add(seedString, new Dictionary<string, ulong>{
          {"value", seedNum},
          {"touched", 0}
        });
      }
      // Remove first line
      lines.RemoveAt(0);
      // Get all other three-number lines and add to list of mappings
      string pattern = @"\d+ \d+ \d+";
      List<ulong[]> currentMapGroup = new List<ulong[]>();
      foreach (string line in lines)
      {
        if (Regex.IsMatch(line, pattern))
        {
          // Add [] to currentGroup list
          string[] nums = line.Split(" ");
          ulong[] thisGroup = new ulong[] { ulong.Parse(nums[0]), ulong.Parse(nums[1]), ulong.Parse(nums[2]) };
          currentMapGroup.Add(thisGroup);
        }
        else if (line == "")
        {
          // Add currentGroup to mappings, reset currentGroup
          mappings.Add(currentMapGroup);
          currentMapGroup = new List<ulong[]>();
        }
      }
    }

    public UInt64 PartOne()
    {
      // Go through each group of mapping numbers
      foreach (List<ulong[]> map in mappings)
      {
        foreach (ulong[] values in map)
        {
          ulong destinationStart = values[0];
          ulong sourceStart = values[1];
          ulong range = values[2];
          // What's the difference between the source and destination?
          ulong difference = destinationStart - sourceStart;
          // Console.WriteLine($"Values: {values[0]} {values[1]} {values[2]}");

          // For each of the seeds
          foreach (string key in dictionary.Keys)
          {
            // Is the seed in the range of source + range? && hasn't already been touched this map
            if (dictionary[key]["value"] >= sourceStart && dictionary[key]["value"] < sourceStart + range && dictionary[key]["touched"] == 0)
            {
              // Then the number should change by the difference
              dictionary[key]["value"] += difference;
              dictionary[key]["touched"] = 1;
            }
            else
            {
              // Not in range, just leave the number intact
            }
            // Console.WriteLine($"Current: {key}, {dictionary[key]["value"]}");
          }
        }

        // Reset all keys to "untouched"
        foreach (string key in dictionary.Keys)
        {
          dictionary[key]["touched"] = 0;
        }
      }

      return GetMinValue();
    }

    public UInt64 PartTwo()
    {
      return 0;
    }

    private UInt64 GetMinValue()
    {
      UInt64 min = UInt64.MaxValue;
      foreach (string key in dictionary.Keys)
      {
        Console.WriteLine(dictionary[key]["value"]);

        if (dictionary[key]["value"] < min)
        {
          min = dictionary[key]["value"];
        }
      }
      return min;
    }
  }
}
