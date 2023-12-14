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
        case 3:
          Day03 day03 = new Day03("Day03.txt");
          Console.WriteLine($"Part 1: {day03.PartOne()}");
          Console.WriteLine($"Part 2: {day03.PartTwo()}");
          break;
        case 4:
          Day04 day04 = new Day04("Day04.txt");
          Console.WriteLine($"Part 1: {day04.PartOne()}");
          Console.WriteLine($"Part 2: {day04.PartTwo()}");
          break;
        case 5:
          Day05 day05 = new Day05("Day05.txt");
          Console.WriteLine($"Part 1: {day05.PartOne()}");
          Console.WriteLine($"Part 2: {day05.Part2v3()}");
          break;
        case 6:
          Day06 day06 = new Day06("Day06.txt");
          Console.WriteLine($"Part 1: {day06.PartOne()}");
          Console.WriteLine($"Part 2: {day06.PartTwo()}");
          break;
        case 7:
          Day07 day07 = new Day07("Day07.txt");
          Console.WriteLine($"Part 1: {day07.PartOne()}");
          Console.WriteLine($"Part 2: {day07.PartTwo()}");
          break;
        case 8:
          Day08 day08 = new Day08("Day08.txt");
          Console.WriteLine($"Part 1: {day08.PartOne()}");
          Console.WriteLine($"Part 2: {day08.PartTwo()}");
          break;
        case 9:
          Day09 day09 = new Day09("Day09.txt");
          Console.WriteLine($"Part 1: {day09.PartOne()}");
          Console.WriteLine($"Part 2: {day09.PartTwo()}");
          break;
        case 10:
          Day10 day10 = new Day10("Day10.txt");
          Console.WriteLine($"Part 1: {day10.PartOne()}");
          Console.WriteLine($"Part 2: {day10.PartTwov2()}");
          break;
        case 11:
          Day11 day11 = new Day11("Day11.txt");
          Console.WriteLine($"Part 1: {day11.PartOne()}");
          Console.WriteLine($"Part 2: {day11.PartTwo()}");
          break;
        case 12:
          Day12 day12 = new Day12("Day12.txt");
          Console.WriteLine($"Part 1: {day12.PartOne()}");
          Console.WriteLine($"Part 2: {day12.PartTwo()}");
          break;
        case 13:
          Day13 day13 = new Day13("Day13.txt");
          Console.WriteLine($"Part 1: {day13.PartOne()}");
          Console.WriteLine($"Part 2: {day13.PartTwo()}");
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

