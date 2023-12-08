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

    public long PartTwo()
    {
      int stepCount = 0;
      int index = 0;
      List<string> currentNodes = nodes.Keys.Where(key => key.Last() == 'A').ToList();
      List<int> countsAtZ = new List<int>();
      while (true)
      {
        if (currentNodes.Count == 0)
        {
          foreach (int value in countsAtZ)
          {
            Console.WriteLine(value);
          }
          return lcm_of_array_elements(countsAtZ);
        }
        Parallel.For(0, currentNodes.Count, i =>
        {
          currentNodes[i] = FollowNode(currentNodes[i], directions[index]);
        });

        for (int i = currentNodes.Count - 1; i >= 0; i--)
        {
          if (currentNodes[i].Last() == 'Z')
          {
            countsAtZ.Add(stepCount);
            currentNodes.RemoveAt(i);
          }
        }

        stepCount++;
        index = (index + 1) % directions.Count;
      }
    }

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

    private int GetGCD(int n1, int n2)
    {
      if (n2 == 0)
      {
        return n1;
      }
      else
      {
        return GetGCD(n2, n1 % n2);
      }
    }

    private int GetLCM(List<int> numbers)
    {
      return numbers.Aggregate((S, val) => S * val / GetGCD(S, val));
    }

    public long lcm_of_array_elements(List<int> element_array)
    {
      long lcm_of_array_elements = 1;
      int divisor = 2;

      while (true)
      {

        int counter = 0;
        bool divisible = false;
        for (int i = 0; i < element_array.Count; i++)
        {

          // lcm_of_array_elements (n1, n2, ... 0) = 0.
          // For negative number we convert into
          // positive and calculate lcm_of_array_elements.
          if (element_array[i] == 0)
          {
            return 0;
          }
          else if (element_array[i] < 0)
          {
            element_array[i] = element_array[i] * (-1);
          }
          if (element_array[i] == 1)
          {
            counter++;
          }

          // Divide element_array by devisor if complete
          // division i.e. without remainder then replace
          // number with quotient; used for find next factor
          if (element_array[i] % divisor == 0)
          {
            divisible = true;
            element_array[i] = element_array[i] / divisor;
          }
        }

        // If divisor able to completely divide any number
        // from array multiply with lcm_of_array_elements
        // and store into lcm_of_array_elements and continue
        // to same divisor for next factor finding.
        // else increment divisor
        if (divisible)
        {
          lcm_of_array_elements = lcm_of_array_elements * divisor;
        }
        else
        {
          divisor++;
        }

        // Check if all element_array is 1 indicate 
        // we found all factors and terminate while loop.
        if (counter == element_array.Count)
        {
          return lcm_of_array_elements;
        }
      }
    }
  }
}


