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
        sum += GetHash(code);
      }
      return sum;
    }

    public int PartTwo()
    {
      // Can definitely do this with a hash table or dictionary, each slot containing a list
      // Maybe even easier with an array of lists
      List<Lens>[] boxes = new List<Lens>[256];
      for (int i = 0; i < boxes.Length; i++)
      {
        boxes[i] = new List<Lens>();
      }
      // The modulus result is the hash. -> would be index in array
      // Insert values into the appropriate list where needed
      foreach (string code in strings)
      {
        // Is it an assignment or subtraction?
        bool isAssignment = code.Contains('=');
        string hashInput;
        if (isAssignment)
        {
          hashInput = code.Split("=").First();
        }
        else
        {
          hashInput = code.Split("-").First();
        }
        // Get hash
        int hash = GetHash(hashInput);
        if (isAssignment)
        {
          int codeValue = int.Parse(code.Split("=").Last());
          int index = boxes[hash].FindIndex((Lens item) => item.label == hashInput);
          // Returns -1 if not found
          if (index >= 0)
          {
            // Replace with new lens value
            boxes[hash][index].value = codeValue;
          }
          else
          {
            // Add to the end of the lens list
            boxes[hash].Add(new Lens(hashInput, codeValue));
          };
        }
        else
        {
          boxes[hash].RemoveAll((Lens item) => item.label == hashInput);
        }
      }
      // Calculate the focusing power
      int sum = 0;
      foreach ((List<Lens> lenses, int i) in boxes.WithIndex())
      {
        foreach ((Lens lens, int j) in lenses.WithIndex())
        {
          sum += (i + 1) * (j + 1) * lens.value;
        }
      }
      return sum;
    }

    class Lens
    {
      public string label;
      public int value;
      public Lens(string label, int value)
      {
        this.label = label;
        this.value = value;
      }
    }

    private int GetHash(string code)
    {
      int currentValue = 0;
      foreach (char character in code.ToCharArray())
      {
        currentValue += GetASCII(character);
        currentValue *= 17;
        currentValue %= 256;
      }
      return currentValue;
    }

    private int GetASCII(char character)
    {
      return (int)character;
    }
  }
}


