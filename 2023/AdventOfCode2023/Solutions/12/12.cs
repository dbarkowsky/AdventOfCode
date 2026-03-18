using Tools;
using System.Text.RegularExpressions;

namespace Solutions
{
  public class Day12
  {
    List<string> strings = new();
    List<string> records = new();
    List<int[]> damagedGroups = new();


    public Day12(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      foreach (string line in strings)
      {
        records.Add(line.Split(" ").First());
        damagedGroups.Add(line.Split(" ").Last().Split(",").Select(int.Parse).ToArray());
      }
    }

    public int PartOne()
    {
      return Solve(records, damagedGroups);
    }

    // This didn't work. The search space is huge, and it will run forever.
    public int PartTwo()
    {
      // Multiply lists times 5
      List<string> bigRecords = records.Select(record =>
      {
        string bigRecord = "";
        for (int i = 0; i < 5; i++)
        {
          bigRecord += record;
          if (i < 4)
          {
            bigRecord += "?";
          }
        }
        return bigRecord;
      }).ToList();

      List<int[]> bigDamagedGroups = damagedGroups.Select(group =>
      {
        int[] bigGroup = new int[group.Length * 5];
        for (int i = 0; i < bigGroup.Length; i++)
        {
          bigGroup[i] = group[i % group.Length];
        }
        return bigGroup;
      }).ToList();
      Console.WriteLine("Start Solving PArt 2");
      return Solve(bigRecords, bigDamagedGroups);
    }

    // This works for small sizes (Part 1), but it's just too brute-forcey to work for larger inputs (Part 2).
    private int Solve(List<string> records, List<int[]> damagedGroups)
    {
      int goodArrangements = 0;
      for (int i = 0; i < records.Count; i++)
      {
        // Console.WriteLine($"Record: {records[i]}");
        // How many unknown parts of a record?
        int numberUnknown = records[i].ToCharArray().Aggregate(0, (sum, character) =>
        {
          if (character == '?') sum++;
          return sum;
        });
        // Console.WriteLine($"Start {records[i]}, numberUnknown: {numberUnknown}");
        // TODO: This next part takes way too long.
        // Create list of possible arrangements
        Queue<string> possibleArrangements = new();
        possibleArrangements.Enqueue(".");
        possibleArrangements.Enqueue("#");
        for (int j = 0; j < Math.Pow(2, numberUnknown) - 2; j++)
        {
          string current = possibleArrangements.Dequeue();
          possibleArrangements.Enqueue(current + ".");
          possibleArrangements.Enqueue(current + "#");
        }
        // Console.WriteLine("Got possible arrangements");
        // For each possible arrangement, create the full arrangement combined with original record
        List<string> arrangements = new();
        foreach (string possibleArrangement in possibleArrangements)
        {
          // Console.WriteLine(possibleArrangement);
          char[] arrangementArray = possibleArrangement.ToCharArray();
          int currentPossibleIndex = 0;
          char[] tempRecord = records[i].ToCharArray();
          for (int j = 0; j < tempRecord.Length; j++)
          {
            if (tempRecord[j] == '?')
            {
              tempRecord[j] = arrangementArray[currentPossibleIndex];
              currentPossibleIndex++;
            }
          }
          // Console.WriteLine(new string(tempRecord));
          arrangements.Add(new string(tempRecord));
        }
        // Console.WriteLine("Got arrangements");
        // Create pattern in regex from damaged groups
        string pattern = "^[^#]*";
        foreach ((int numOfBroken, int index) in damagedGroups[i].WithIndex())
        {
          pattern += $"[#]{{{numOfBroken}}}[^#]+";
        }
        pattern = pattern.Substring(0, pattern.Length - 5);
        pattern += "[^#]*$";
        // Console.WriteLine(pattern);
        // Go through these possible arrangements and see if they match pattern
        foreach (string arrangement in arrangements)
        {
          if (Regex.IsMatch(arrangement, pattern))
          {
            // Console.WriteLine("Good: " + arrangement);
            goodArrangements++;
          }
        }
        // Console.WriteLine($"{records[i]}: {goodArrangements}");
      }
      return goodArrangements;
    }

    public long PartTwov2()
    {
      // Multiply lists times 5
      List<string> bigRecords = records.Select(record =>
      {
        string bigRecord = "";
        for (int i = 0; i < 5; i++)
        {
          bigRecord += record;
          if (i < 4)
          {
            bigRecord += "?";
          }
        }
        return bigRecord;
      }).ToList();

      List<int[]> bigDamagedGroups = damagedGroups.Select(group =>
      {
        int[] bigGroup = new int[group.Length * 5];
        for (int i = 0; i < bigGroup.Length; i++)
        {
          bigGroup[i] = group[i % group.Length];
        }
        return bigGroup;
      }).ToList();

      // Dictionary to use as cache, so we don't repeat calculations
      Dictionary<string, long> results = new Dictionary<string, long>();

      long count = 0;
      for (int i = 0; i < bigRecords.Count; i++)
      {
        count += CountValidSolutions(bigRecords[i], bigDamagedGroups[i].ToList(), results);
      }
      return count;
    }

    // Based on this write up here: https://advent-of-code.xavd.id/writeups/2023/day/12/
    // The caching is very necessary. 
    private long CountValidSolutions(string row, List<int> groups, Dictionary<string, long> results)
    {
      string cacheKey = row + "-" + string.Join(",", groups);
      // Cache out if we've done this before...
      if (results.ContainsKey(cacheKey)){
        return results[cacheKey];
      }
      // If the row has been trimmed down to nothing, 
      // we have no more spots to check.
      // So it's either good (1 - all groups satisfied) or bad (0 - still unsatisfied groups)
      if (string.IsNullOrEmpty(row))
      {
        return groups.Count == 0 ? 1 : 0;
      }

      // There are no more groups to check, but we still have part of a row.
      // If there are still broken springs, this is not good (0)
      // Otherwise, we can accept a row portion with known good springs, the . (return 1)
      if (groups.Count == 0)
      {
        return row.Contains('#') ? 0 : 1;
      }

      // What's our recursion path?
      // First, break up the record
      string firstChar = row[..1];
      string restOfRow = row[1..];

      // A . is a known not-spring. We can actually ignore this case
      // Recurse with the remainder of the row
      if (firstChar.Equals("."))
      {
        long result = CountValidSolutions(restOfRow, groups, results);
        results.Add(cacheKey, result);
        return result;
      }

      // Are we at the start of a group of broken springs?
      if (firstChar.Equals("#"))
      {
        int group = groups[0];
        if (
          // Does this group fit in the existing record?
          row.Length >= group &&
          // Are all the spring locations in this group space possibly a broken spring? (#)
          // Can't have breaks (.)
          !row[..group].Contains('.') &&
          // Does this spacing work? 
          // i.e. We're either at the end of the row (same length) OR
          // the next character isn't another broken spring (#) that would invalidate this group.
          (row.Length == group || !row[group].Equals('#'))
        )
        {
          // Had a problem here running out of the index space when we have a situation like # - 1
          // We'll just return a blank here in that case. 
          string nextRow = (group + 1 < row.Length) ? row[(group + 1)..] : "";
          // Otherwise, we recurse with the remainder of the row and the rest of the groups
          long result = CountValidSolutions(nextRow, groups.GetRange(1, groups.Count - 1), results);
          results.Add(cacheKey, result);
          return result;
        }
        // It's not a good match for this group
        return 0;
      }

      // What if the character is unknown? (?)
      if (firstChar.Equals("?"))
      {
        // We add possible solutions as if it were a '.' and as if it were a '#'
        long result = CountValidSolutions("#" + restOfRow, groups, results) + CountValidSolutions("." + restOfRow, groups, results);
        results.Add(cacheKey, result);
        return result;
      }

      // This should never actually get triggered
      return 0;
    }

  }
}


