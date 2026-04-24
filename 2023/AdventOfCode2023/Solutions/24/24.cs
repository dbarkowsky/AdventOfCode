using System.Drawing;
using System.Numerics;
using Tools;

namespace Solutions
{
  public class Day24
  {

    private class Hailstone
    {
      public Vector3 start;
      public Vector3 velocity;

      public Hailstone(string line)
      {
        string[] positionParts = line.Split(" @ ")[0].Split(", ");
        start = new Vector3(long.Parse(positionParts[0]), long.Parse(positionParts[1]), long.Parse(positionParts[2]));
        string[] velocityParts = line.Split(" @ ")[1].Split(", ");
        velocity = new Vector3(long.Parse(velocityParts[0]), long.Parse(velocityParts[1]), long.Parse(velocityParts[2]));
      }
    }
    List<string> strings = new List<string>();
    List<Hailstone> hailstones= new List<Hailstone>();

    public Day24(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      foreach(string line in strings)
      {
        hailstones.Add(new Hailstone(line));
      }
    }

    // Two lines collide if we use this formula: mx + b = y, except we want the same y value for both lines,
    // so we can actually say mx + b = mx + b, where each side is from a different Vector
    // so x will be the same between them too. We solve for X. If it isn't the same, they don't intersect.
    // Test bounds: X: 7, Y: 27
    // Real bounds: X: 200000000000000, Y: 400000000000000
    public long PartOne(long lowerBound = 7, long upperBound = 27)
    // public long PartOne(long lowerBound = 200000000000000, long upperBound = 400000000000000)
    {
      long count = 0;
      // Going to have to loop through each pair of hailstones
      for (int i = 0; i < hailstones.Count - 1; i++)
      {
        for (int j = i + 1; j < hailstones.Count; j++)
        {
          Hailstone a = hailstones[i];
          Hailstone b = hailstones[j];
          // Do they intersect?
          (bool intersects, double x, double y) = DoTheyIntersect2D(a, b, lowerBound, upperBound);
          if (intersects)
          {
            count++;
          }
        }
      }
      return count;
    }

    public long PartTwo()
    {
      return -1;
    }

    private (bool, double, double) DoTheyIntersect2D(Hailstone a, Hailstone b, long lowerBound, long upperBound, bool futureOnly = true)
    {
      // Determine slopes
      double slopeA = a.velocity.X / a.velocity.Y;
      double slopeB = b.velocity.X / b.velocity.Y;
      // If the same, these are parallel
      if (slopeA == slopeB) return (false, 0, 0);
      // Determine the y intercept (b)
      double yInterceptA = a.start.Y - (slopeA * a.start.X);
      double yInterceptB = b.start.Y - (slopeB * b.start.X);
      // Solve to find collision point
      double sharedX = (yInterceptB - yInterceptA) / (slopeA - slopeB);
      double sharedY = (slopeA * sharedX) + yInterceptA;
      // Is this in the collision bounds?
      if (sharedX < lowerBound) return (false, 0, 0);
      if (sharedY < lowerBound) return (false, 0, 0);
      if (sharedX > upperBound) return (false, 0, 0);
      if (sharedY > upperBound) return (false, 0, 0);
      // Is this forward in time only?
      if (futureOnly)
      {
        // If it's the future, then the values must be bigger if positive and smaller if negative....
        // a.X
        if (a.velocity.X != 0)
        {
          if (a.velocity.X > 0)
          {
            if (sharedX < a.start.X) return (false, sharedX, sharedY);
          } else
          {
            if (sharedX > a.start.X) return (false, sharedX, sharedY);
          }
        }
        // a.Y
        if (a.velocity.Y != 0)
        {
          if (a.velocity.Y > 0)
          {
            if (sharedY < a.start.Y) return (false, sharedX, sharedY);
          } else
          {
            if (sharedY > a.start.Y) return (false, sharedX, sharedY);
          }
        }
        // b.X
        if (b.velocity.X != 0)
        {
          if (b.velocity.X > 0)
          {
            if (sharedX < b.start.X) return (false, sharedX, sharedY);
          } else
          {
            if (sharedX > b.start.X) return (false, sharedX, sharedY);
          }
        }
        // b.Y
        if (b.velocity.Y != 0)
        {
          if (b.velocity.Y > 0)
          {
            if (sharedY < b.start.Y) return (false, sharedX, sharedY);
          } else
          {
            if (sharedY > b.start.Y) return (false, sharedX, sharedY);
          }
        }
      }

      return (true, sharedX, sharedY);
    }


    // Thought I would need these, but hasn't come up yet...
    private long GetManhattanDistance2D(Vector3 a, Vector3 b)
    {
      return (long)(Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y));
    }

    private long GetManhattanDistance3D(Vector3 a, Vector3 b)
    {
      return (long)(Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z));
    }
  }
}


