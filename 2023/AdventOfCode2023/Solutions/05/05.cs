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
      Parallel.For(0, seedStrings.Count, i =>
      {
        Console.WriteLine($"thread: {i} start");
        if (i % 2 == 0)
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
          Console.WriteLine($"thread: {i} setup complete");

          ProcessMaps(seedMapper);

          Console.WriteLine($"thread: {i} process complete");

          smallestLocations.Add(GetMinValue(seedMapper));
        }
        Console.WriteLine($"thread: {i} end");
      });
      return smallestLocations.Min();
    }

    public ulong Part2v2()
    {
      object threadLock = new();
      ulong smallestLocation = ulong.MaxValue;

      Parallel.ForEach(seedStrings.Chunk(2), seedAndRange =>
      {
        ulong tempSmallestLocation = ulong.MaxValue;
        ulong startingSeed = ulong.Parse(seedAndRange[0]);
        ulong seedRange = ulong.Parse(seedAndRange[1]);
        Console.WriteLine($"{startingSeed} - {seedRange}");
        for (ulong currentSeed = startingSeed; currentSeed < startingSeed + seedRange; currentSeed++)
        {
          ulong seed = currentSeed;
          foreach (List<ulong[]> map in mappings)
          {
            bool touched = false;
            foreach (ulong[] values in map)
            {
              ulong destinationStart = values[0];
              ulong sourceStart = values[1];
              ulong range = values[2];
              // What's the difference between the source and destination?
              ulong difference = destinationStart - sourceStart;
              // Console.WriteLine($"Values: {values[0]} {values[1]} {values[2]}");

              // Is the seed in the range of source + range? && hasn't already been touched this map
              if (seed >= sourceStart && seed < sourceStart + range && !touched)
              {
                // Then the number should change by the difference
                seed += difference;
                touched = true;
              }
              else
              {
                // Not in range, just leave the number intact
              }
              // Console.WriteLine($"Current: {key}, {dictionary[key]["value"]}");
            }

            // Reset all keys to "untouched"
            touched = false;
          }
        }
        lock (threadLock)
        {
          if (tempSmallestLocation < smallestLocation)
          {
            smallestLocation = tempSmallestLocation;
          }
        }
      });

      return smallestLocation;
    }

    public ulong Part2v3()
    {
      ulong minimumFertilizer = ulong.MaxValue;
      ulong minimumSeeds = ulong.MaxValue;
      // Convert seed strings to numbers
      ulong[] seedData = seedStrings.Select(ulong.Parse).ToArray();
      // List<List<ulong[]>> mappedNumberGrid = new List<List<ulong[]>>();
      // int k = 0;
      // foreach (List<ulong[]> map in mappings)
      // {
      //   foreach (ulong[] row in map)
      //   {
      //     mappedNumberGrid.Add(new List<ulong>(row));
      //   }
      //   // Console.WriteLine($"{mappedNumberGrid[k][0]}, {mappedNumberGrid[k][1]}, {mappedNumberGrid[k][2]}");
      //   k++;
      // }


      for (int i = 0; i < seedData.Length; i += 2)
      {
        for (ulong j = 0; j < seedData[i + 1]; j++)
        {
          Tuple<ulong, ulong> currentResult = mappings.Aggregate(new Tuple<ulong, ulong>(seedData[i], minimumSeeds), (accTuple, mapData) =>
          {
            ulong currentPosition = accTuple.Item1;
            ulong minimumFertilizerAmount = accTuple.Item2;
            ulong[] foundRange = mapData.Find(range => currentPosition >= range[1] && currentPosition < range[1] + range[2]);
            if (foundRange != null)
            {
              return new Tuple<ulong, ulong>(foundRange[0] + currentPosition - foundRange[1], Math.Min(foundRange[2] + foundRange[1] - currentPosition, minimumFertilizerAmount));
            }
            else
            {
              ulong[] nextRange = mapData.Find(range => currentPosition < range[i]);
              if (nextRange != null)
              {
                return new Tuple<ulong, ulong>(currentPosition, Math.Min(nextRange[1] - currentPosition, minimumFertilizerAmount));
              }
              return new Tuple<ulong, ulong>(currentPosition, minimumFertilizerAmount);
            }
          });
          minimumFertilizer = Math.Min(currentResult.Item1, minimumFertilizer);
        }
      }
      return minimumFertilizer;
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

          // For each of the seeds
          Parallel.ForEach(seeds.Keys, key =>
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
          });
        }

        // Reset all keys to "untouched"
        foreach (string key in seeds.Keys)
        {
          seeds[key]["touched"] = 0;
        }
      }
    }

    private ulong GetMinValue(Dictionary<string, Dictionary<string, ulong>> seeds)
    {
      ulong min = ulong.MaxValue;
      foreach (string key in seeds.Keys)
      {
        if (seeds[key]["value"] < min)
        {
          min = seeds[key]["value"];
        }
      }
      return min;
    }
  }
}
