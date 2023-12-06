using Tools;

namespace Solutions
{
  public class Day06
  {
    List<string> strings = new List<string>();
    List<ulong> times = new List<ulong>();
    List<ulong> distances = new List<ulong>();


    public Day06(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      times = strings[0].Split(": ", StringSplitOptions.TrimEntries)[1].Split(" ", StringSplitOptions.TrimEntries).Where(x => x != string.Empty).Select(ulong.Parse).ToList();
      distances = strings[1].Split(": ", StringSplitOptions.TrimEntries)[1].Split(" ", StringSplitOptions.TrimEntries).Where(x => x != string.Empty).Select(ulong.Parse).ToList();
    }

    public int PartOne()
    {
      List<int> waysToWin = new List<int>();
      int trackingIndex = 0;
      // Check each time
      foreach (ulong time in times)
      {
        int wins = 0;
        // Count up the seconds to see which ones win
        for (ulong seconds = 0; seconds < time; seconds++)
        {
          ulong distance = CalculateDistance(seconds, time);
          if (distance > distances[trackingIndex])
          {
            wins++;
          }
        }
        waysToWin.Add(wins);
        trackingIndex++;
      }
      return waysToWin.Aggregate(1, (acc, val) => acc * val);
    }

    // Had to convert everything to ulong for Part 2...
    public ulong PartTwo()
    {
      string timeString = "";
      string distanceString = "";
      for (int i = 0; i < times.Count; i++)
      {
        timeString += $"{times[i]}";
        distanceString += $"{distances[i]}";
      }
      ulong time = ulong.Parse(timeString);
      ulong goal = ulong.Parse(distanceString);

      ulong wins = 0;
      for (ulong seconds = 0; seconds < time; seconds++)
      {
        ulong distance = CalculateDistance(seconds, time);
        if (distance > goal)
        {
          wins++;
        }
      }
      return wins;
    }

    private ulong CalculateDistance(ulong millisecondsForButton, ulong raceTime)
    {
      ulong remainingTime = raceTime - millisecondsForButton;
      ulong millimetersPerMillisecond = millisecondsForButton;
      ulong distanceCovered = millimetersPerMillisecond * remainingTime;
      return distanceCovered;
    }
  }
}


