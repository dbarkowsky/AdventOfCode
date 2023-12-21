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

      int area = 0;

      // Loop through all instructions, adding coordinates to set
      foreach ((string direction, int distance, string colour) instruction in instructions)
      {
        // Log corner
        path.Add((currentX, currentY));

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
      }

      // shoelace algorithm
      // I was doing something wrong here for a long time. Was always like 2 off
      // This for-loop solution from Ryan Heath
      List<(int x, int y)> pathList = path.ToList();
      for (int i = 0; i < pathList.Count; i++)
      {
        int nextI = (i + 1) % pathList.Count;
        int prevI = i - 1 < 0 ? pathList.Count - 1 : i - 1;
        area += pathList[i].y * (pathList[nextI].x - pathList[prevI].x);
      }

      area = Math.Abs(area) / 2;
      // Need to add the perimeter to the area
      area += pathList.Count / 2 + 1;

      return area;
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


