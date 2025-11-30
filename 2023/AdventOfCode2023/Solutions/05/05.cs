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

    // public ulong PartTwo()
    // {
    //   List<ulong> smallestLocations = new List<ulong>();
    //   Parallel.For(0, seedStrings.Count, i =>
    //   {
    //     Console.WriteLine($"thread: {i} start");
    //     if (i % 2 == 0)
    //     {
    //       Dictionary<string, Dictionary<string, ulong>> seedMapper = new Dictionary<string, Dictionary<string, ulong>>();
    //       ulong startingSeed = ulong.Parse(seedStrings[i]);
    //       ulong range = ulong.Parse(seedStrings[i + 1]);
    //       // Console.WriteLine($"{startingSeed} - {range}");

    //       for (ulong j = startingSeed; j < startingSeed + range; j++)
    //       {
    //         seedMapper.Add($"{j}", new Dictionary<string, ulong>{
    //         {"value", j},
    //         {"touched", 0}
    //       });
    //       }
    //       Console.WriteLine($"thread: {i} setup complete");

    //       ProcessMaps(seedMapper);

    //       Console.WriteLine($"thread: {i} process complete");

    //       smallestLocations.Add(GetMinValue(seedMapper));
    //     }
    //     Console.WriteLine($"thread: {i} end");
    //   });
    //   return smallestLocations.Min();
    // }

    // public ulong Part2v2()
    // {
    //   object threadLock = new();
    //   ulong smallestLocation = ulong.MaxValue;

    //   Parallel.ForEach(seedStrings.Chunk(2), seedAndRange =>
    //   {
    //     ulong tempSmallestLocation = ulong.MaxValue;
    //     ulong startingSeed = ulong.Parse(seedAndRange[0]);
    //     ulong seedRange = ulong.Parse(seedAndRange[1]);
    //     Console.WriteLine($"{startingSeed} - {seedRange}");
    //     for (ulong currentSeed = startingSeed; currentSeed < startingSeed + seedRange; currentSeed++)
    //     {
    //       ulong seed = currentSeed;
    //       foreach (List<ulong[]> map in mappings)
    //       {
    //         bool touched = false;
    //         foreach (ulong[] values in map)
    //         {
    //           ulong destinationStart = values[0];
    //           ulong sourceStart = values[1];
    //           ulong range = values[2];
    //           // What's the difference between the source and destination?
    //           ulong difference = destinationStart - sourceStart;
    //           // Console.WriteLine($"Values: {values[0]} {values[1]} {values[2]}");

    //           // Is the seed in the range of source + range? && hasn't already been touched this map
    //           if (seed >= sourceStart && seed < sourceStart + range && !touched)
    //           {
    //             // Then the number should change by the difference
    //             seed += difference;
    //             touched = true;
    //           }
    //           else
    //           {
    //             // Not in range, just leave the number intact
    //           }
    //           // Console.WriteLine($"Current: {key}, {dictionary[key]["value"]}");
    //         }

    //         // Reset all keys to "untouched"
    //         touched = false;
    //       }
    //     }
    //     lock (threadLock)
    //     {
    //       if (tempSmallestLocation < smallestLocation)
    //       {
    //         smallestLocation = tempSmallestLocation;
    //       }
    //     }
    //   });

    //   return smallestLocation;
    // }

    // public ulong Part2v3()
    // {
    //   ulong minimumFertilizer = ulong.MaxValue;
    //   ulong minimumSeeds = ulong.MaxValue;
    //   // Convert seed strings to numbers
    //   ulong[] seedData = seedStrings.Select(ulong.Parse).ToArray();
    //   // List<List<ulong[]>> mappedNumberGrid = new List<List<ulong[]>>();
    //   // int k = 0;
    //   // foreach (List<ulong[]> map in mappings)
    //   // {
    //   //   foreach (ulong[] row in map)
    //   //   {
    //   //     mappedNumberGrid.Add(new List<ulong>(row));
    //   //   }
    //   //   // Console.WriteLine($"{mappedNumberGrid[k][0]}, {mappedNumberGrid[k][1]}, {mappedNumberGrid[k][2]}");
    //   //   k++;
    //   // }


    //   for (int i = 0; i < seedData.Length; i += 2)
    //   {
    //     for (ulong j = 0; j < seedData[i + 1]; j++)
    //     {
    //       Tuple<ulong, ulong> currentResult = mappings.Aggregate(new Tuple<ulong, ulong>(seedData[i], minimumSeeds), (accTuple, mapData) =>
    //       {
    //         ulong currentPosition = accTuple.Item1;
    //         ulong minimumFertilizerAmount = accTuple.Item2;
    //         ulong[] foundRange = mapData.Find(range => currentPosition >= range[1] && currentPosition < range[1] + range[2]);
    //         if (foundRange != null)
    //         {
    //           return new Tuple<ulong, ulong>(foundRange[0] + currentPosition - foundRange[1], Math.Min(foundRange[2] + foundRange[1] - currentPosition, minimumFertilizerAmount));
    //         }
    //         else
    //         {
    //           ulong[] nextRange = mapData.Find(range => currentPosition < range[i]);
    //           if (nextRange != null)
    //           {
    //             return new Tuple<ulong, ulong>(currentPosition, Math.Min(nextRange[1] - currentPosition, minimumFertilizerAmount));
    //           }
    //           return new Tuple<ulong, ulong>(currentPosition, minimumFertilizerAmount);
    //         }
    //       });
    //       minimumFertilizer = Math.Min(currentResult.Item1, minimumFertilizer);
    //     }
    //   }
    //   return minimumFertilizer;
    // }

    public ulong Part2v4()
    {
      Dictionary<ulong, ulong> seedRangeMap = new Dictionary<ulong, ulong> { };
      // Examine seed ranges. Determine if there are overlaps and establish final ranges.
      for (int i = 0; i < seedStrings.Count; i += 2)
      {
        ulong start = ulong.Parse(seedStrings[i]);
        ulong length = ulong.Parse(seedStrings[i + 1]);
        ulong end = start + length - 1;
        // Console.WriteLine($"{start}, {length}");
        // Is there overlap with an existing value?
        if (seedRangeMap.Keys.Count > 0)
        {
          // Does our starting value already fall in a range?
          ulong[] existingStarts = seedRangeMap.Keys.ToArray();
          int currentIndex = 0;
          bool overlapFound = false;
          ulong newStart = 0;
          ulong newEnd = 0;
          // Keep checking while start is greater or equal to key
          while (currentIndex < existingStarts.Length && !overlapFound)
          {
            // Is the start between key and value?
            if (start >= existingStarts[currentIndex] && start <= seedRangeMap[existingStarts[currentIndex]])
            {
              overlapFound = true;
              // Then we see if it all fits in this range already
              // Can see just by comparing the end values
              // If new end is less than existing end, do nothing.
              // If new end is more, then we just expand this key.
              if (end > seedRangeMap[existingStarts[currentIndex]])
              {
                seedRangeMap[existingStarts[currentIndex]] = end;
              }
              newStart = existingStarts[currentIndex];
              newEnd = end > seedRangeMap[existingStarts[currentIndex]] ? end : seedRangeMap[existingStarts[currentIndex]];
              break;
            }
            // Is the end between key and value?
            else if (end >= existingStarts[currentIndex] && end <= seedRangeMap[existingStarts[currentIndex]])
            {
              overlapFound = true;
              if (start < existingStarts[currentIndex])
              {
                // Add with new start key and delete the old one.
                seedRangeMap.Add(start, seedRangeMap[existingStarts[currentIndex]]);
                seedRangeMap.Remove(existingStarts[currentIndex]);
              }
              newStart = start < existingStarts[currentIndex] ? start : existingStarts[currentIndex];
              newEnd = seedRangeMap[existingStarts[currentIndex]];
              break;
            }
            currentIndex++;
          }
          // Was any overlap found?
          if (overlapFound)
          {
            // Need to check if there are any other entries that got engulfed by the expanded entry.
            // Only entries between the currently updated entry
            for (int j = 0; j < existingStarts.Length; j++)
            {
              if (existingStarts[currentIndex] > start && seedRangeMap[existingStarts[currentIndex]] < end)
              {
                seedRangeMap.Remove(existingStarts[currentIndex]);
              }
            }
          }
          else
          {
            // No overlap, just add it to the map
            seedRangeMap.Add(start, end);
          }
        }
        else
        {
          // First entry, can't be overlap
          seedRangeMap.Add(start, start + length - 1);
        }
      }
      Console.WriteLine("Starting Ranges");
      seedRangeMap.OrderBy(pair => pair.Key).ToList().ForEach((e) =>
      {
        Console.WriteLine(e);
      });
      // For each set of ranges, apply and adjust ranges as needed
      Dictionary<ulong, ulong> currentMap = new Dictionary<ulong, ulong>(seedRangeMap);
      foreach (List<ulong[]> map in mappings)
      {
        // We change the current map with each iteration so it doesn't affect the changes in weird ways.
        Dictionary<ulong, ulong> updatedMap = new Dictionary<ulong, ulong>(currentMap);
        foreach (ulong[] values in map)
        {
          ulong destinationStart = values[0];
          ulong sourceStart = values[1];
          ulong range = values[2];
          ulong sourceEnd = sourceStart + range - 1;
          ulong destinationEnd = destinationStart + range - 1;
          ulong difference = destinationStart - sourceStart;

          // Console.WriteLine($"{destinationStart}, {sourceStart}, {range}");
          // Is there any overlap with existing ranges?
          // No point looking at ones if some part is not in existing range
          ulong[] validRangeKeys = currentMap.Keys.Where(key => IsBetween(key, sourceStart, sourceEnd) || IsBetween(currentMap[key], sourceStart, sourceEnd)).ToArray();
          // For every valid range in the map, adjust the keys and values based on incoming map
          foreach(ulong rangeStart in validRangeKeys)
          {
            ulong rangeEnd = currentMap[rangeStart];
            // Does some of the change happen before this range starts?
            bool someBefore = sourceStart < rangeStart && sourceEnd >= rangeStart && sourceEnd <= rangeEnd;
            // Does some of the change happen after the range ends?
            bool someAfter = sourceStart >= rangeStart && sourceStart <= sourceEnd && sourceEnd > rangeEnd;
            // Is the change fully encompassed within the existing range?
            bool changeInRange = sourceStart >= rangeStart && sourceEnd <= rangeEnd;
            // Is the existing range fully encompassed in the change?
            bool rangeInChange = rangeStart >= sourceStart && rangeEnd <= sourceEnd;

            // I messed this up originally. I had added values outside the original range. 
            // Instead, I needed to keep the values in the original ranges that weren't advanced.
            if (someBefore)
            {
              // Range that updates
              ulong inRangeStart = rangeStart;
              ulong inRangeEnd = sourceEnd;
              // Range that stays
              ulong outRangeStart = sourceEnd + 1;
              ulong outRangeEnd = rangeEnd;
              // Remove entry
              updatedMap.Remove(rangeStart);
              // Add changed entries
              updatedMap[inRangeStart + difference] = inRangeEnd + difference;
              // Add the unchanged entries
              updatedMap[outRangeStart] = outRangeEnd;
            } else if (someAfter)
            {
              // Identify what's in range and what's after.
              ulong inRangeStart = sourceStart;
              ulong inRangeEnd = rangeEnd;
              ulong outRangeStart = rangeStart;
              ulong outRangeEnd = sourceStart - 1;
              // Remove original
              updatedMap.Remove(rangeStart);
              // Add changed entries
              updatedMap[inRangeStart + difference] = inRangeEnd + difference;
              // Add the unchanged entries
              updatedMap[outRangeStart] = outRangeEnd;
            } else if (changeInRange)
            {
              // Identify 2-3 subranges: area before, area in, area after.
              ulong beforeRangeStart = rangeStart;
              ulong beforeRangeEnd = sourceStart - 1;
              ulong inRangeStart = sourceStart;
              ulong inRangeEnd = sourceEnd;
              ulong afterRangeStart = sourceEnd + 1;
              ulong afterRangeEnd = rangeEnd;
              // Remove original entry
              updatedMap.Remove(rangeStart);
              // Add entry that gets changed by difference
              updatedMap[inRangeStart + difference] = inRangeEnd + difference;
              // Carry over unaltered part of previous range
              updatedMap[beforeRangeStart] = beforeRangeEnd;
              updatedMap[afterRangeStart] = afterRangeEnd;
            } else if (rangeInChange)
            {
              // Remove original entry
              updatedMap.Remove(rangeStart);
              // Add changed entry
              updatedMap[rangeStart + difference] = rangeEnd + difference;
            }
          }
        }
        currentMap = new Dictionary<ulong, ulong>(updatedMap);
      }
      Console.WriteLine("Ending Ranges");
      currentMap.OrderBy(pair => pair.Key).ToList().ForEach((e) =>
      {
        Console.WriteLine(e);
      });
      return currentMap.OrderBy(pair => pair.Key).ToList().First().Key;
    }

    private static bool IsBetween(ulong value, ulong start, ulong end)
    {
      return value >= start && value <= end;
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
