using Tools;

namespace Solutions
{
  public class Day04
  {
    List<string> lines = new List<string>();
    List<List<string>> winners = new List<List<string>>();
    List<List<string>> myNumbers = new List<List<string>>();
    // Global sum for counting total cards
    int cards = 0;


    public Day04(string fileName)
    {
      lines = FileReader.AsStringArray(fileName).ToList();
      foreach (string line in lines)
      {
        string numbers = line.Split(": ")[1];
        winners.Add(numbers.Split(" | ")[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList());
        myNumbers.Add(numbers.Split(" | ")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList());
      }
    }

    public int PartOne()
    {
      int sum = 0;
      // For each Card
      for (int i = 0; i < winners.Count; i++)
      {
        int points = 0;
        // For each of my numbers, check if the winners contains it
        foreach (string number in myNumbers[i])
        {
          if (points == 0 && winners[i].Contains(number))
          {
            points = 1;
          }
          else if (winners[i].Contains(number))
          {
            points *= 2;
          }
        }
        sum += points;
      }
      return sum;
    }

    // Just need to count cards here
    // Easiest just to let it run and increment global counter
    // Maybe not the fastest though
    public int PartTwo()
    {
      // For each Card
      for (int i = 0; i < winners.Count; i++)
      {
        ProcessCard(i);
      }
      return cards;
    }

    // Recursive function, finds number of matches, then runs this same function on those cards
    private void ProcessCard(int index)
    {
      cards++;
      int matches = CountMatches(index);

      for (int i = 1; i <= matches; i++)
      {
        if (index + i < winners.Count)
        {
          ProcessCard(index + i);
        }
      }
    }

    private int CountMatches(int index)
    {
      int count = 0;
      // For each of my numbers, check if the winners contains it
      foreach (string number in myNumbers[index])
      {
        if (winners[index].Contains(number))
        {
          count++;
        }
      }

      return count;
    }
  }
}
