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
      int goodArrangements = 0;
      for (int i = 0; i < records.Count; i++)
      {
        Console.WriteLine($"Record: {records[i]}");
        // How many unknown parts of a record?
        int numberUnknown = records[i].ToCharArray().Aggregate(0, (sum, character) =>
        {
          if (character == '?') sum++;
          return sum;
        });
        // Create list of possible arrangements
        Queue<string> possibleArrangements = new();
        possibleArrangements.Enqueue(".");
        possibleArrangements.Enqueue("#");
        // Console.WriteLine(numberUnknown);
        for (int j = 0; j < Math.Pow(2, numberUnknown) - 2; j++)
        {
          string current = possibleArrangements.Dequeue();
          possibleArrangements.Enqueue(current + ".");
          possibleArrangements.Enqueue(current + "#");
        }
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
        // Create pattern in regex from damaged groups
        string pattern = "^[^#]*";
        foreach ((int numOfBroken, int index) in damagedGroups[i].WithIndex())
        {
          pattern += $"[#]{{{numOfBroken}}}[^#]+";
        }
        pattern = pattern.Substring(0, pattern.Length - 5);
        pattern += "[^#]*$";
        Console.WriteLine(pattern);
        // Go through these possible arrangements and see if they match pattern
        foreach (string arrangement in arrangements)
        {
          if (Regex.IsMatch(arrangement, pattern))
          {
            Console.WriteLine("Good: " + arrangement);
            goodArrangements++;
          }
          else
          {
            // Console.WriteLine("Bad: " + arrangement);

          }
        }
        Console.WriteLine(goodArrangements);
      }
      return goodArrangements;
    }

    public int PartTwo()
    {
      return -1;
    }

  }
}


