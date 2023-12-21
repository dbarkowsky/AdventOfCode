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
      List<(int x, int y)> pathList = FollowPath(instructions);
      return GetArea(pathList);
    }

    public long PartTwo()
    {
      // Convert hex to new instructions
      List<(string direction, int distance, string colour)> instructionsPart2 = instructions.Select(instruction =>
      {
        // This is a cool way to do a switch on a string (returned by the substring)
        string direction = instruction.colour.Substring(6, 1) switch
        {
          "0" => "R",
          "1" => "D",
          "2" => "L",
          "3" => "U",
          _ => "." // Should never reach that
        };
        int distance = Convert.ToInt32($"{instruction.colour.Substring(1, 5)}", 16);
        return (direction, distance, instruction.colour);
      }).ToList();
      List<(int x, int y)> pathList = FollowPath(instructionsPart2);
      return GetArea(pathList);
    }

    private List<(int x, int y)> FollowPath(List<(string direction, int distance, string colour)> instructions)
    {
      int currentX = 0;
      int currentY = 0;
      // Set to store all locations visited
      HashSet<(int x, int y)> path = new();

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
      return path.ToList();
    }

    private long GetArea(List<(int x, int y)> path)
    {
      // shoelace algorithm
      // I was doing something wrong here for a long time. Was always like 2 off
      // This for-loop solution from Ryan Heath
      long area = 0;
      for (int i = 0; i < path.Count; i++)
      {
        int nextI = (i + 1) % path.Count;
        int prevI = i - 1 < 0 ? path.Count - 1 : i - 1;
        area += path[i].y * (path[nextI].x - path[prevI].x);
      }

      area = Math.Abs(area) / 2;
      // Need to add the perimeter to the area
      area += path.Count / 2 + 1;
      return area;
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


