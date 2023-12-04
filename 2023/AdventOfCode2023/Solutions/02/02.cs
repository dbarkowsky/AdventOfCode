using Tools;
using System.Text.RegularExpressions;

namespace Solutions
{
  public class Day02
  {
    List<string> lines = new List<string>();

    public Day02(string fileName)
    {
      lines = FileReader.AsStringArray(fileName).ToList();
    }

    public int PartOne()
    {
      int total = 0;
      int maxRed = 12;
      int maxGreen = 13;
      int maxBlue = 14;
      string pattern = @"([\d]+) (red|green|blue)";
      int currentGame = 1;

      foreach (string line in lines)
      {
        bool possible = true;
        MatchCollection matchingPairs = Regex.Matches(line, pattern);
        foreach (Match match in matchingPairs)
        {
          int number = int.Parse(match.Groups[1].Value);
          string colour = match.Groups[2].Value;

          switch (colour)
          {
            case "red":
              if (number > maxRed) possible = false;
              break;
            case "green":
              if (number > maxGreen) possible = false;
              break;
            case "blue":
              if (number > maxBlue) possible = false;
              break;
            default:
              break;
          }
        }
        if (possible)
        {
          total += currentGame;
        }
        currentGame++;
      }
      return total;
    }

    public int PartTwo()
    {
      int sum = 0;
      string pattern = @"([\d]+) (red|green|blue)";

      foreach (string line in lines)
      {
        // We assume 1 so that it's easily multiplied if non-existent. 0 would be bad.
        int minRed = 1;
        int minGreen = 1;
        int minBlue = 1;
        MatchCollection matchingPairs = Regex.Matches(line, pattern);
        foreach (Match match in matchingPairs)
        {
          int number = int.Parse(match.Groups[1].Value);
          string colour = match.Groups[2].Value;

          switch (colour)
          {
            case "red":
              if (number > minRed) minRed = number;
              break;
            case "green":
              if (number > minGreen) minGreen = number;
              break;
            case "blue":
              if (number > minBlue) minBlue = number;
              break;
            default:
              break;
          }
        }
        int power = minRed * minGreen * minBlue;
        sum += power;
      }
      return sum;
    }
  }
}
