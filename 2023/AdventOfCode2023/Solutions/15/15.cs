using Tools;

namespace Solutions
{
  public class Day15
  {
    List<string> strings = new List<string>();


    public Day15(string fileName)
    {
      strings = FileReader.AsStringArray(fileName)[0].Split(",").ToList();
    }

    public int PartOne()
    {
      int sum = 0;
      foreach (string code in strings)
      {
        int currentValue = 0;
        foreach (char character in code.ToCharArray())
        {
          currentValue += GetASCII(character);
          currentValue *= 17;
          currentValue %= 256;
        }
        sum += currentValue;
      }
      return sum;
    }

    public int PartTwo()
    {
      // Can definitely do this with a hash table or dictionary, each slot containing a list
      // Maybe even easier with an array of lists
      // The modulus result is the hash. -> would be index in array
      // Insert values into the appropriate list where needed
      return -1;
    }

    private int GetASCII(char character)
    {
      return (int)character;
    }
  }
}


