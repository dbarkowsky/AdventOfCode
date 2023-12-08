using Tools;

namespace Solutions
{
  public class Day08
  {
    List<string> strings = new List<string>();
    Dictionary<string, (string, string)> nodes = new Dictionary<string, (string, string)>();
    List<char> directions = new List<char>();


    public Day08(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      directions = strings[0].ToList();

      // Nodes start on line 2
      for (int i = 2; i < strings.Count; i++)
      {
        // Getting key and values
        string key = strings[i].Split(" = ")[0];
        string[] values = strings[i].Split(" = ")[1].Substring(1, 8).Split(", ");
        // Inserting into nodes list
        nodes[key] = (values.First(), values.Last());
      }
    }

    // How many steps to get to ZZZ
    public int PartOne()
    {
      int stepCount = 0;
      int index = 0;
      string currentNode = "AAA";
      while (true)
      {
        if (currentNode == "ZZZ")
        {
          return stepCount;
        }
        currentNode = FollowNode(currentNode, directions[index]);
        stepCount++;
        index = (index + 1) % directions.Count;
      }
    }

    // All the nodes need to end in Z to finish
    // Numbers too big to finish with brute force
    // Would never have guessed LCM if I hadn't read about it
    public ulong PartTwo()
    {
      ulong stepCount = 0;
      int index = 0;
      List<string> currentNodes = nodes.Keys.Where(key => key.Last() == 'A').ToList();
      List<ulong> countsAtZ = new List<ulong>();
      while (true)
      {
        if (currentNodes.Count == 0)
        {
          return LCM(countsAtZ);
        }
        Parallel.For(0, currentNodes.Count, i =>
        {
          currentNodes[i] = FollowNode(currentNodes[i], directions[index]);
        });
        stepCount++;

        for (int i = currentNodes.Count - 1; i >= 0; i--)
        {
          if (currentNodes[i].Last() == 'Z')
          {
            countsAtZ.Add(stepCount);
            currentNodes.RemoveAt(i);
          }
        }

        index = (index + 1) % directions.Count;
      }
    }

    // Gets the next node based on directions
    private string FollowNode(string key, char direction)
    {
      (string left, string right) = nodes[key];
      if (direction == 'L')
      {
        return left;
      }
      else
      {
        return right;
      }
    }

    /** Some generic LCM code **/
    private ulong LCM(List<ulong> numbers)
    {
      return numbers.Aggregate((x, y) => x * y / GCD(x, y));
    }

    private ulong GCD(ulong a, ulong b)
    {
      if (b == 0)
        return a;
      return GCD(b, a % b);
    }
  }
}


