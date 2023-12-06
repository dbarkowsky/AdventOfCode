using System.Dynamic;
using System.Text.RegularExpressions;
using Tools;

namespace Solutions
{
  public class Day05
  {
    List<string> lines = new List<string>();
    List<string> seedStrings;
    List<List<ulong[]>> mappings = new List<List<ulong[]>>();

    public Day05(string fileName)
    {
      lines = FileReader.AsStringArray(fileName).ToList();
      // Get seeds from line 1, insert into List
      seedStrings = new List<string>(lines[0].Split(": ")[1].Split(" "));

      // Remove first line
      lines.RemoveAt(0);
      // Get all other three-number lines and add to list of mappings
      string pattern = @"\d+ \d+ \d+";
      List<ulong[]> currentMapGroup = new List<ulong[]>();
      int lineNumber = 1;
      foreach (string line in lines)
      {
        // Console.WriteLine($"Line: {line}");
        if (Regex.IsMatch(line, pattern))
        {
          // Add [] to currentGroup list
          string[] nums = line.Split(" ");
          ulong[] thisGroup = new ulong[] { ulong.Parse(nums[0]), ulong.Parse(nums[1]), ulong.Parse(nums[2]) };
          currentMapGroup.Add(thisGroup);
        }
        if (line == "" || lineNumber == lines.Count)
        {
          // Add currentGroup to mappings, reset currentGroup
          mappings.Add(currentMapGroup);
          currentMapGroup = new List<ulong[]>();
        }
        lineNumber++;
      }
    }

    public ulong PartOne()
    {
      Dictionary<string, Dictionary<string, ulong>> seedMapper = new Dictionary<string, Dictionary<string, ulong>>();
      foreach (string seedString in seedStrings)
      {
        ulong seedNum = ulong.Parse(seedString);
        seedMapper.Add(seedString, new Dictionary<string, ulong>{
          {"value", seedNum},
          {"touched", 0}
        });
      }
      ProcessMaps(seedMapper);

      return GetMinValue(seedMapper);
    }

    public ulong PartTwo()
    {
      List<ulong> smallestLocations = new List<ulong>();
      for (int i = 0; i < seedStrings.Count; i += 2)
      {
        Dictionary<string, Dictionary<string, ulong>> seedMapper = new Dictionary<string, Dictionary<string, ulong>>();
        ulong startingSeed = ulong.Parse(seedStrings[i]);
        ulong range = ulong.Parse(seedStrings[i + 1]);
        // Console.WriteLine($"{startingSeed} - {range}");

        for (ulong j = startingSeed; j < startingSeed + range; j++)
        {
          seedMapper.Add($"{j}", new Dictionary<string, ulong>{
            {"value", j},
            {"touched", 0}
          });
        }
        ProcessMaps(seedMapper);
        // foreach (string key in seedMapper.Keys)
        // {
        //   Console.WriteLine($"{key}: {seedMapper[key]["value"]}");
        // }
        smallestLocations.Add(GetMinValue(seedMapper));
      }
      // foreach (ulong location in smallestLocations)
      // {
      //   Console.WriteLine(location);
      // }
      return smallestLocations.Min();
    }

    private void ProcessMaps(Dictionary<string, Dictionary<string, ulong>> seeds)
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
          foreach (string key in seeds.Keys)
          {
            // Is the seed in the range of source + range? && hasn't already been touched this map
            if (seeds[key]["value"] >= sourceStart && seeds[key]["value"] < sourceStart + range && seeds[key]["touched"] == 0)
            {
              // Then the number should change by the difference
              seeds[key]["value"] += difference;
              seeds[key]["touched"] = 1;
            }
            else
            {
              // Not in range, just leave the number intact
            }
            // Console.WriteLine($"Current: {key}, {dictionary[key]["value"]}");
          }
        }

        // Reset all keys to "untouched"
        foreach (string key in seeds.Keys)
        {
          seeds[key]["touched"] = 0;
          // Console.WriteLine($"Current: {key}, {dictionary[key]["value"]}");
        }
      }
    }

    private ulong GetMinValue(Dictionary<string, Dictionary<string, ulong>> seeds)
    {
      ulong min = ulong.MaxValue;
      foreach (string key in seeds.Keys)
      {
        // Console.WriteLine(dictionary[key]["value"]);

        if (seeds[key]["value"] < min)
        {
          min = seeds[key]["value"];
        }
      }
      return min;
    }
  }
}
