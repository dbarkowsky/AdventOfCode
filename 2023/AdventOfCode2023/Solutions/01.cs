using Tools;

namespace Solutions
{
  public class Day01
  {
    public static void PartOne(string fileName)
    {
      string[] strings = FileReader.AsStringArray(fileName);
      foreach (string line in strings)
      {
        Console.WriteLine(line);
      }
      Console.WriteLine(strings.Length);
    }

  }
}


