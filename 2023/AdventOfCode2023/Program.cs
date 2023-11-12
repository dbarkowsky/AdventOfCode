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
          Day01.PartOne("test.txt");
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

