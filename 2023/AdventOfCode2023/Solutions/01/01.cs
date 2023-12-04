using Tools;
using System;
using System.Text.RegularExpressions;

namespace Solutions
{
  public class Day01
  {
    List<string> strings = new List<string>();


    public Day01(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
    }

    public int PartOne()
    {
      int partOneTotal = 0;
      string pattern = @"\d";
      foreach (string line in strings)
      {
        MatchCollection matches = Regex.Matches(line, pattern);
        string firstNum = matches[0].Value;
        string lastNum = matches[matches.Count - 1].Value;
        int combinedValue = int.Parse($"{firstNum}{lastNum}");
        partOneTotal += combinedValue;
      }
      return partOneTotal;
    }

    public int PartTwo()
    {
      int partTwoTotal = 0;
      string pattern = @"\d|one|two|three|four|five|six|seven|eight|nine";
      foreach (string line in strings)
      {
        string firstNum = Regex.Match(line, pattern).Value;
        string lastNum = Regex.Match(line, pattern, RegexOptions.RightToLeft).Value;
        int combinedValue = int.Parse($"{stringToNumber(firstNum)}{stringToNumber(lastNum)}");
        partTwoTotal += combinedValue;
      }
      return partTwoTotal;
    }

    private int stringToNumber(string number)
    {
      switch (number)
      {
        case "zero":
        case "0":
          return 0;
        case "one":
        case "1":
          return 1;
        case "two":
        case "2":
          return 2;
        case "three":
        case "3":
          return 3;
        case "four":
        case "4":
          return 4;
        case "five":
        case "5":
          return 5;
        case "six":
        case "6":
          return 6;
        case "seven":
        case "7":
          return 7;
        case "eight":
        case "8":
          return 8;
        case "nine":
        case "9":
          return 9;
        default:
          return -1;
      }
    }

  }
}


