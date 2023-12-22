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
      long startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
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
        case 14:
          Day14 day14 = new Day14("Day14.txt");
          Console.WriteLine($"Part 1: {day14.PartOne()}");
          Console.WriteLine($"Part 2: {day14.PartTwo()}");
          break;
        case 15:
          Day15 day15 = new Day15("Day15.txt");
          Console.WriteLine($"Part 1: {day15.PartOne()}");
          Console.WriteLine($"Part 2: {day15.PartTwo()}");
          break;
        case 16:
          Day16 day16 = new Day16("Day16.txt");
          Console.WriteLine($"Part 1: {day16.PartOne()}");
          Console.WriteLine($"Part 2: {day16.PartTwo()}");
          break;
        case 17:
          Day17 day17 = new Day17("Day17.txt");
          Console.WriteLine($"Part 1: {day17.PartOne()}");
          Console.WriteLine($"Part 2: {day17.PartTwo()}");
          break;
        case 18:
          Day18 day18 = new Day18("Day18.txt");
          Console.WriteLine($"Part 1: {day18.PartOne()}");
          Console.WriteLine($"Part 2: {day18.PartTwo()}");
          break;
        case 19:
          Day19 day19 = new Day19("Day19.txt");
          Console.WriteLine($"Part 1: {day19.PartOne()}");
          Console.WriteLine($"Part 2: {day19.PartTwo()}");
          break;
        case 20:
          Day20 day20 = new Day20("Day20.txt");
          Console.WriteLine($"Part 1: {day20.PartOne()}");
          Console.WriteLine($"Part 2: {day20.PartTwo()}");
          break;
        case 21:
          Day21 day21 = new Day21("Day21.txt");
          Console.WriteLine($"Part 1: {day21.PartOne()}");
          Console.WriteLine($"Part 2: {day21.PartTwo()}");
          break;
        case 22:
          Day22 day22 = new Day22("Day22.txt");
          Console.WriteLine($"Part 1: {day22.PartOne()}");
          Console.WriteLine($"Part 2: {day22.PartTwo()}");
          break;
        case 23:
          Day23 day23 = new Day23("Day23.txt");
          Console.WriteLine($"Part 1: {day23.PartOne()}");
          Console.WriteLine($"Part 2: {day23.PartTwo()}");
          break;
        case 24:
          Day24 day24 = new Day24("Day24.txt");
          Console.WriteLine($"Part 1: {day24.PartOne()}");
          Console.WriteLine($"Part 2: {day24.PartTwo()}");
          break;
        case 25:
          Day25 day25 = new Day25("Day25.txt");
          Console.WriteLine($"Part 1: {day25.PartOne()}");
          Console.WriteLine($"Part 2: {day25.PartTwo()}");
          break;
        default:
          Console.WriteLine("Day not yet created/solved.");
          break;
      }
      long endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
      Console.WriteLine($"Time elapsed (ms): {endTime - startTime}");
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
      System.Environment.Exit(0);
    }
  }
}

