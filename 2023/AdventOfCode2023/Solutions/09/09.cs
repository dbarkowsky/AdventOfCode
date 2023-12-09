using Tools;

namespace Solutions
{
  public class Day09
  {
    List<string> strings = new List<string>();

    public Day09(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
    }

    public int PartOne()
    {
      int sum = 0;
      foreach (string row in strings)
      {
        List<int> sequence = row.Split(" ").Select(int.Parse).ToList();
        int nextNumber = getNextNumber(sequence);
        sum += nextNumber;
      }
      return sum;
    }

    public int PartTwo()
    {
      int sum = 0;
      foreach (string row in strings)
      {
        List<int> sequence = row.Split(" ").Select(int.Parse).ToList();
        int nextNumber = getPreviousNumber(sequence);
        sum += nextNumber;
      }
      return sum;
    }

    // Gets the next number in a sequence
    private int getNextNumber(List<int> sequence)
    {
      List<int> differences = new List<int>();
      for (int i = 1; i < sequence.Count; i++)
      {
        differences.Add(sequence[i] - sequence[i - 1]);
      }

      if (differences.All(number => number == 0))
      {
        return sequence[0]; // All numbers are the same, return any number
      }
      else
      {
        int jump = getNextNumber(differences);
        return sequence.Last() + jump;
      }
    }

    // Gets the previous number in a sequence
    private int getPreviousNumber(List<int> sequence)
    {
      List<int> differences = new List<int>();
      for (int i = 1; i < sequence.Count; i++)
      {
        differences.Add(sequence[i] - sequence[i - 1]);
      }

      if (differences.All(number => number == 0))
      {
        return sequence[0]; // All numbers are the same, return any number
      }
      else
      {
        int jump = getPreviousNumber(differences);
        return sequence.First() - jump;
      }
    }
  }
}


