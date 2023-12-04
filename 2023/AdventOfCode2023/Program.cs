using System;
using Solutions;
class AdventOfCode2023
{
  public static void Main(string[] args)
  {
    if (args.Length != 1)
    {
      Console.WriteLine("Command needs exactly one argument specifying the day. e.g. dotnet run 1");
      System.Environment.Exit(0);
    }
    int day = int.Parse(args[0]);

    if (day > 25 || day < 1)
    {
      Console.WriteLine("Day must be between 1 and 25.");
      System.Environment.Exit(0);
    }

    Console.WriteLine("Running day " + day);
    try
    {
      switch (day)
      {
        case 1:
          Day01 day01 = new Day01("Day01.txt");
          Console.WriteLine($"Part 1: {day01.PartOne()}");
          Console.WriteLine($"Part 2: {day01.PartTwo()}");
          break;
        case 2:
          Day02 day02 = new Day02("Day02.txt");
          Console.WriteLine($"Part 1: {day02.PartOne()}");
          Console.WriteLine($"Part 2: {day02.PartTwo()}");
          break;
        default:
          Console.WriteLine("Day not yet created/solved.");
          break;
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
      System.Environment.Exit(0);
    }
  }
}

