using System.Runtime.CompilerServices;
using Tools;

namespace Solutions
{
  public class Day22
  {
    HashSet<Cube> exampleWhenSettled = new HashSet<Cube>()
    {
      new(1,0,1), // A 
      new(1,1,1), // A 
      new(1,2,1), // A
      new(0,0,2), // B
      new(1,0,2), // B
      new(2,0,2), // B
      new(0,2,2), // C
      new(1,2,2), // C
      new(2,2,2), // C
      new(0,0,3), // D
      new(0,1,3), // D 
      new(0,2,3), // D
      new(2,0,3), // E
      new(2,1,3), // E 
      new(2,2,3), // E
      new(0,1,4), // F
      new(1,1,4), // F
      new(2,1,4), // F
      new(1,1,5), // G
      new(1,1,6), // G
    };
    private class Cube
    {
      public int x;
      public int y;
      public int z;
      public Cube(int x, int y, int z)
      {
        this.x = x;
        this.y = y;
        this.z = z;
      }



      public void Fall()
      {
        z--;
      }

      public Cube GetNext()
      {
        Cube next = new Cube(x, y, z);
        next.Fall();
        return next;
      }

      public override bool Equals(object obj)
      {
        if (obj == null) return false;
        Cube c = (obj as Cube)!;
        return c.x == x && c.y == y && c.z == z;
      }

      public override string ToString()
      {
        return x + "," + y + "," + z;
      }

      public override int GetHashCode()
      {
        return ToString().GetHashCode();
      }
    }

    private class Block
    {
      public HashSet<Cube> cubes = new HashSet<Cube>();
      public Block(string line)
      {
        // Build cubes based on this line input: 0,0,2~2,0,2
        string left = line.Split('~').First();
        string right = line.Split('~').Last();
        int[] leftValues = left.Split(',').Select(x => int.Parse(x)).ToArray();
        int[] rightValues = right.Split(',').Select(x => int.Parse(x)).ToArray();
        int X_POS = 0;
        int Y_POS = 1;
        int Z_POS = 2;
        for (int x = leftValues[X_POS]; x <= rightValues[X_POS]; x++)
        {
          for (int y = leftValues[Y_POS]; y <= rightValues[Y_POS]; y++)
          {
            for (int z = leftValues[Z_POS]; z <= rightValues[Z_POS]; z++)
            {
              cubes.Add(new Cube(x, y, z));
            }
          }
        }
      }

      public Block(Block b)
      {
        cubes = cubes.Union(b.cubes).ToHashSet();
      }

      public int GetLowestZ()
      {
        int lowest = int.MaxValue;
        foreach (Cube cube in cubes)
        {
          lowest = Math.Min(lowest, cube.z);
        }
        return lowest;
      }

      private HashSet<Cube> GetNextCubeSet()
      {
        HashSet<Cube> nextSet = new();
        foreach (Cube cube in cubes)
        {
          nextSet.Add(cube.GetNext());
        }
        return nextSet;
      }

      private void DropCubeSetBy(int distance, Block b)
      {
        foreach (Cube cube in b.cubes)
        {
          cube.z -= distance;
        }
      }

      public Block FallUntilStopped(HashSet<Cube> fallenCubes)
      {
        // Copy so we don't alter the original
        Block copy = new Block(this);
        // First, drop it to the rough location, one block above known highest fallen cube.
        // TODO: Track this as we drop instead of recalculating each time
        // int highestZInFallenCubes = 1;
        // foreach (Cube cube in fallenCubes)
        // {
        //   highestZInFallenCubes = Math.Max(highestZInFallenCubes, cube.z);
        // }
        // int distance = GetLowestZ() - highestZInFallenCubes - 1;
        // Console.WriteLine(highestZInFallenCubes);
        //           Console.WriteLine(distance);

        // DropCubeSetBy(distance, copy);
        // Then loop the slow fall until blocked
        bool blocked = false;
        while (!blocked)
        {
          // Console.WriteLine(copy.GetLowestZ());
          // Stop if lowest point is at 1
          if (copy.GetLowestZ() <= 1)
          {
            blocked = true;
            Console.WriteLine("hit bottom");
            continue;
          }
          // Stop if the the next point down would intersect with fallen blocks.
          HashSet<Cube> nextCubesPosition = copy.GetNextCubeSet();
          Console.WriteLine("next position");
          Console.WriteLine(string.Join("\n", nextCubesPosition));
          if (nextCubesPosition.Intersect(fallenCubes).Any())
          {
            blocked = true;
            Console.WriteLine("interecept");
            continue;
          }
          // Otherwise, fall once more
          foreach (Cube cube in copy.cubes)
          {
            cube.Fall();
          }
          Console.WriteLine("fall");
        }
        // Print locations of blocked fallen cubes
        Console.WriteLine("final coords");
        this.PrintCoords();
        return copy;
      }

      public void PrintCoords()
      {
        foreach (Cube cube in cubes)
        {
          Console.WriteLine(cube);
        }
      }
    }

    readonly List<string> strings = new List<string>();
    HashSet<Cube> fallenCubes = new HashSet<Cube>();
    List<Block> blocks = new List<Block>();
    public Day22(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      foreach (string s in strings)
      {
        blocks.Add(new Block(s));
      }
      // Sort these by z position. It's going to determine how we drop them.
      blocks.Sort((a, b) => a.GetLowestZ().CompareTo(b.GetLowestZ()));
      // TODO: Check that these are sorted!
    }

    // This feels a lot like how we build Tetris. Drop blocks, and they become part of fallen blocks once stopped.
    public int PartOne()
    {
      // First, drop all the blocks to their lowest point.
      // Start with the lowest block.
      // Blocks stop when reaching bottom or if they would intersect another block.
      foreach (Block block in blocks)
      {
        // Fall until stopped
        block.FallUntilStopped(fallenCubes);
        // Then save its cubes to the fallen cubes set
        foreach (Cube cube in block.cubes)
        {
          fallenCubes.Add(cube);
        }
      }
      // Console.WriteLine(fallenCubes);
      Console.WriteLine(fallenCubes.Count);
      bool matchesTest = fallenCubes.SetEquals(exampleWhenSettled);
      Console.WriteLine(matchesTest);
      // FIXME: This might not work... I need to be able to remove individual blocks, so lumping them all into fallenCubes doesn't work.
      return -1;
    }

    public int PartTwo()
    {
      return -1;
    }
  }
}


