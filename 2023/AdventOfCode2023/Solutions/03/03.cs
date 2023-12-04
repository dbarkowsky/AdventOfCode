using System.Text.RegularExpressions;
using Tools;

namespace Solutions
{
  public class Day03
  {
    List<string> lines = new List<string>();
    List<List<char>> grid = new List<List<char>>();

    public Day03(string fileName)
    {
      lines = FileReader.AsStringArray(fileName).ToList();
      foreach (string line in lines)
      {
        grid.Add(line.ToCharArray().ToList());
      }
    }

    public int PartOne()
    {
      int sum = 0;
      // For each line, find the numbers
      string pattern = @"[\d]+";
      int lineNumber = 0;
      foreach (string line in lines)
      {
        MatchCollection matches = Regex.Matches(line, pattern);
        // For each number...
        foreach (Match match in matches)
        {
          // Is it adjacent to a symbol?
          if (IsAdjacentToSymbol(match, lineNumber))
          {
            sum += int.Parse(match.Value);
          }
        }
        lineNumber++;
      }
      return sum;
    }

    // I could/should have done part 1 this same way, looking for the symbols, then returning adjacent numbers
    // I think this is probably faster, as there are fewer spaces to check with no 2D List needed.
    public int PartTwo()
    {
      int sum = 0;
      // For each line, find the numbers
      string pattern = @"\*";
      int lineNumber = 0;
      foreach (string line in lines)
      {
        MatchCollection matches = Regex.Matches(line, pattern);
        // For each number...
        foreach (Match match in matches)
        {
          // Get all adjacent numbers
          List<int> numbers = GetAdjacentNumbers(lineNumber, match.Index);
          // If there are more than two
          if (numbers.Count >= 2)
          {
            // Add their product to the sum
            int product = 1;
            foreach (int number in numbers)
            {
              product *= number;
            }
            sum += product;
          }
        }
        lineNumber++;
      }
      return sum;
    }

    private List<int> GetAdjacentNumbers(int x, int y)
    {
      List<int> numbers = new List<int>();
      string pattern = @"[\d]+";
      // Get all numbers from this row
      MatchCollection matches = Regex.Matches(lines[x], pattern);
      // For each number...
      foreach (Match match in matches)
      {
        if (IsThisAdjacent(match, x, x, y))
        {
          numbers.Add(int.Parse(match.Value));
        }
      }
      // Repeat for above and below lines if available
      if (x > 0)
      {
        // Get all numbers from this row
        matches = Regex.Matches(lines[x - 1], pattern);
        // For each number...
        foreach (Match match in matches)
        {
          if (IsThisAdjacent(match, x - 1, x, y))
          {
            numbers.Add(int.Parse(match.Value));
          }
        }
      }
      if (x < lines.Count - 1)
      {
        // Get all numbers from this row
        matches = Regex.Matches(lines[x + 1], pattern);
        // For each number...
        foreach (Match match in matches)
        {
          if (IsThisAdjacent(match, x + 1, x, y))
          {
            numbers.Add(int.Parse(match.Value));
          }
        }
      }
      return numbers;
    }

    private bool IsThisAdjacent(Match match, int lineNumber, int x, int y)
    {
      // Here, we just need to see if the absolute value of the differences between any number and the * are <1 for x and y
      // If so, it must be adjacent
      for (int i = 0; i < match.Length; i++)
      {
        if (Math.Abs(lineNumber - x) <= 1 && Math.Abs(match.Index + i - y) <= 1)
        {
          return true;
        }
      }
      // It was not adjacent
      return false;
    }

    private bool IsAdjacentToSymbol(Match match, int lineNumber)
    {
      // If there is a left, check directly left, then above and below
      if (match.Index > 0)
      {
        if (IsASymbol(match.Index - 1, lineNumber)) return true;
        // Check above
        if (lineNumber > 0 && IsASymbol(match.Index - 1, lineNumber - 1)) return true;
        // Check below
        if (lineNumber < grid.Count - 1 && IsASymbol(match.Index - 1, lineNumber + 1)) return true;
      }
      // If there is a right, check directly right, then above and below 
      if (match.Index + match.Length < grid[lineNumber].Count)
      {
        if (IsASymbol(match.Index + match.Length, lineNumber)) return true;
        // Check above
        if (lineNumber > 0 && IsASymbol(match.Index + match.Length, lineNumber - 1)) return true;
        // Check below
        if (lineNumber < grid.Count - 1 && IsASymbol(match.Index + match.Length, lineNumber + 1)) return true;
      }
      // Check directly above
      if (lineNumber > 0)
      {
        for (int i = 0; i < match.Value.Length; i++)
        {
          if (IsASymbol(match.Index + i, lineNumber - 1)) return true;
        }
      }
      // Check directly below
      if (lineNumber < grid.Count - 1)
      {
        for (int i = 0; i < match.Value.Length; i++)
        {
          if (IsASymbol(match.Index + i, lineNumber + 1)) return true;
        }
      }
      // None of the checks found symbols, return false
      return false;
    }

    private bool IsASymbol(int y, int x)
    {
      char character = grid[x][y];
      Regex pattern = new Regex(@"\.|\d");
      if (!pattern.IsMatch(character.ToString()))
      {
        return true;
      }
      return false;
    }
  }
}
