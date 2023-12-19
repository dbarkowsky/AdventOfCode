using System.Text.RegularExpressions;
using Tools;

namespace Solutions
{
  public class Day18
  {
    List<string> strings = new List<string>();
    List<(string direction, int distance, string colour)> instructions;

    public Day18(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      instructions = ParseInput(strings);
    }

    public double PartOne()
    {
      int currentX = 0;
      int currentY = 0;
      // Set to store all locations visited
      HashSet<(int x, int y)> path = new();
      // Need to track this separately. 
      // Shoelace doesn't consider entire coordinate to be within area
      // Only half for the lines, but a full unit for the corners
      HashSet<(int x, int y)> corners = new()
      {
          // Add starting location
          (currentX, currentY)
      };

      int area = 0;

      // Loop through all instructions, adding coordinates to set
      foreach ((string direction, int distance, string colour) instruction in instructions)
      {
        // Log corner
        path.Add((currentX, currentY));
        // Remember original x and y
        (int x, int y) original = (currentX, currentY);
        for (int i = 0; i < instruction.distance; i++)
        {
          switch (instruction.direction)
          {
            case "R":
              currentY++;
              break;
            case "L":
              currentY--;
              break;
            case "U":
              currentX--;
              break;
            case "D":
              currentX++;
              break;
          }
          path.Add((currentX, currentY));
        }
        // Add to area
        // area += Math.Abs((original.x * currentY) - (currentX * original.y));
      }
      Console.WriteLine(path.Count);
      Console.WriteLine(Math.Abs(area));
      // Need to add the perimeter to the area
      // area = area / 2 + lines.Count / 2;
      // return area;
      double sum1 = 0;
      double sum2 = 0;
      List<(int x, int y)> listOfHash = path.ToList();
      for (int i = 0; i < path.Count - 1; i++)
      {
        sum1 += listOfHash[i].x * listOfHash[i + 1].y;
        sum2 += listOfHash[i].y * listOfHash[i + 1].x;
      }
      double darea = 0.5 * Math.Abs(sum1 - sum2);
      return darea;
    }

    public int PartTwo()
    {
      return -1;
    }

    private List<(string direction, int distance, string colour)> ParseInput(List<string> strings)
    {
      List<(string direction, int distance, string colour)> instructions = new();
      string pattern = @"^(.) ([\d]+) \((#[0-9a-f]{6})\)";
      foreach (string line in strings)
      {
        Match match = Regex.Match(line, pattern);
        instructions.Add((match.Groups[1].Value, int.Parse(match.Groups[2].Value), match.Groups[3].Value));
      }
      return instructions;
    }
  }
}


