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
      public long px, py, pz;
      public long vx, vy, vz;

      public Hailstone(string line)
      {
        string[] positionParts = line.Split(" @ ")[0].Split(", ");
        px = long.Parse(positionParts[0].Trim());
        py = long.Parse(positionParts[1].Trim());
        pz = long.Parse(positionParts[2].Trim());
        start = new Vector3(px, py, pz);
        string[] velocityParts = line.Split(" @ ")[1].Split(", ");
        vx = long.Parse(velocityParts[0].Trim());
        vy = long.Parse(velocityParts[1].Trim());
        vz = long.Parse(velocityParts[2].Trim());
        velocity = new Vector3(vx, vy, vz);
      }

      public override string ToString()
      {
        return start.ToString() + " @ " + velocity.ToString();
      }
    }
    List<string> strings = new List<string>();
    List<Hailstone> hailstones = new List<Hailstone>();

    public Day24(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      foreach (string line in strings)
      {
        hailstones.Add(new Hailstone(line));
      }
    }

    // Two lines collide if we use this formula: mx + b = y, except we want the same y value for both lines,
    // so we can actually say mx + b = mx + b, where each side is from a different Vector
    // so x will be the same between them too. We solve for X. If it isn't the same, they don't intersect.
    // Test bounds: X: 7, Y: 27
    // Real bounds: X: 200000000000000, Y: 400000000000000
    // public long PartOne(long lowerBound = 7, long upperBound = 27)
    public long PartOne(long lowerBound = 200000000000000, long upperBound = 400000000000000)
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

    // Really struggled with this. I had the thought that we could just find points and
    // find a line that would hit all those points, but the range is too big.
    // Instead, used this example to try and get this done:
    // https://old.reddit.com/r/adventofcode/comments/18pnycy/2023_day_24_solutions/keqf8uq/
    // But even that didn't get the correct answer. It was off by hundreds due to floating point percision issues
    // And I had integer division issues that resulted in things like 7 / 3 == 2.
    // And I had issues with overflow, even with the long number type. Had to use BigInteger
    public long PartTwo()
    {
      // This first part is about finding a relative velocity that we can hit all stones with
      HashSet<long>? xSet = null;
      HashSet<long>? ySet = null;
      HashSet<long>? zSet = null;

      for (int i = 0; i < hailstones.Count - 1; i++)
      {
        for (int j = i + 1; j < hailstones.Count; j++)
        {
          Hailstone currentA = hailstones[i];
          Hailstone currentB = hailstones[j];

          // For every pair, we'll see if they have an identical velocity in one of the dimensions.
          // If so, we track that. We'll only keep the velocities that work for every hailstone pair.
          
          // X
          if (currentA.vx == currentB.vx && currentA.vx != 0){
            HashSet<long> tempSet = new();
            long difference = currentB.px - currentA.px;
            for (long v = -1000; v < 1000; v++){
              if (v == currentA.vx) continue;
              if (difference % (v - currentA.vx) == 0){
                tempSet.Add(v);
              }
            }
            if (xSet == null) xSet = tempSet;
            else xSet.IntersectWith(tempSet);
          }
          // Y
          if (currentA.vy == currentB.vy && currentA.vy != 0){
            HashSet<long> tempSet = new();
            long difference = currentB.py - currentA.py;
            for (long v = -1000; v < 1000; v++){
              if (v == currentA.vy) continue;
              if (difference % (v - currentA.vy) == 0){
                tempSet.Add(v);
              }
            }
            if (ySet == null) ySet = tempSet;
            else ySet.IntersectWith(tempSet);
          }
          // Z
          if (currentA.vz == currentB.vz && currentA.vz != 0){
            HashSet<long> tempSet = new();
            long difference = currentB.pz - currentA.pz;
            for (long v = -1000; v < 1000; v++){
              if (v == currentA.vz) continue;
              if (difference % (v - currentA.vz) == 0){
                tempSet.Add(v);
              }
            }
            if (zSet == null) zSet = tempSet;
            else zSet.IntersectWith(tempSet);
          }
        }
      }

      // Print our sets to see the results for debugging
      // Console.WriteLine(string.Join(", ", xSet!));
      // Console.WriteLine(string.Join(", ", ySet!));
      // Console.WriteLine(string.Join(", ", zSet!));
      // {-63} {-263} {195}

      // Now we know the x,y,z velocity that our collision rock needs to have. 
      // We just need a place to start when we launch it.
      // For the real input, there's only one correct relative velocity for each axis
      // For the test input, there are many that get flagged. The site-provided one is in there, but I don't think it's the only valid option.
      long relativeVelocityX = xSet!.First();
      long relativeVelocityY = ySet!.First();
      long relativeVelocityZ = zSet!.First();

      // We only need to line up two hailstones. If those work, we also hit all the others.
      Hailstone hailstoneA = hailstones[0];
      Hailstone hailstoneB = hailstones[1];
      
      // Below here, slopes didn't work. This is where I deviated from the reddit example above.
      // Cramer's Rule was the recommendation for this part, which was something new to me.
      // This video was helpful: https://www.youtube.com/watch?v=RdLo-9jh2EM
      // Thankfully, our problem is just a 2x2 matrix.

      // We still need the adjusted velocities by base-lining the stones' velocities based on the relative measurement.
      long relDirAX = hailstoneA.vx - relativeVelocityX;
      long relDirAY = hailstoneA.vy - relativeVelocityY;
      long relDirBX = hailstoneB.vx - relativeVelocityX;
      long relDirBY = hailstoneB.vy - relativeVelocityY;

      // I was really stuck here. I kept getting a division by 0 error. That was an integer division problem at first.
      // Plus the overflow problem that required BigInteger
      // This part confused me, but if you use the video as an example, this is the part after the = in the example section.
      // We're just using the relative velocities of the hailstones to fill the unknown gaps, but the are our "x"
      BigInteger constantA = (BigInteger)relDirAX * hailstoneA.py - (BigInteger)relDirAY * hailstoneA.px;
      BigInteger constantB = (BigInteger)relDirBX * hailstoneB.py - (BigInteger)relDirBY * hailstoneB.px;
      // So matrix is something like | relDirAx  relDirAY | I think. Cross multiply and subtract to get determinant.
      //                             | relDirBX  relDirBY |
      BigInteger determinant = (BigInteger)relDirAX * relDirBY - (BigInteger)relDirAY * relDirBX;

      // Then this is the part that goes on top. We're replacing part of the matrix with our constants.
      // The division could be its own step, but it was done as one here.
      // It's still the cross-multiply and subtract pattern.
      long xPosition = (long)((constantA * relDirBX - constantB * relDirAX) / determinant);
      long yPosition = (long)((constantA * relDirBY - constantB * relDirAY) / determinant);
      // So we only needed the two dimensions. At this point, there's only one possible Z it could be,
      // and we calculate that based on the time that was needed for the X axis to align.
      long time = (xPosition - hailstoneA.px) / relDirAX;
      long zPosition = hailstoneA.pz + (hailstoneA.vz - relativeVelocityZ) * time;

      // Console.WriteLine(string.Join(", ", xPosition, yPosition, zPosition));
      return xPosition + yPosition + zPosition;
    }

    private (bool, double, double) DoTheyIntersect2D(Hailstone a, Hailstone b, long lowerBound, long upperBound, bool futureOnly = true)
    {
      double sharedX, sharedY;

      // So this part doesn't actually matter for my input, but it's not an impossible case.
      // Needed in case there's a 0 X velocity on one or more lines
      // Otherwise we would divide by 0 in the else case when finding the slope.
      if (a.velocity.X == 0 || b.velocity.X == 0)
      {
        // Handle vertical lines (vx == 0): x is constant, can't use y = mx + b form
        if (a.velocity.X == 0 && b.velocity.X == 0) return (false, 0, 0); // both vertical, parallel
        // Determine the vertical, non-vertical ones
        Hailstone vertical = a.velocity.X == 0 ? a : b;
        Hailstone nonVertical = a.velocity.X == 0 ? b : a;
        sharedX = vertical.start.X;
        double slopeNV = nonVertical.velocity.Y / nonVertical.velocity.X;
        double yIntNV = nonVertical.start.Y - (slopeNV * nonVertical.start.X);
        sharedY = (slopeNV * sharedX) + yIntNV;
      }
      else
      {
        // Determine slopes (dy/dx = vy/vx)
        double slopeA = a.velocity.Y / a.velocity.X;
        double slopeB = b.velocity.Y / b.velocity.X;
        // If the same, these are parallel
        if (slopeA == slopeB) return (false, 0, 0);
        // Determine the y intercept
        double yInterceptA = a.start.Y - (slopeA * a.start.X);
        double yInterceptB = b.start.Y - (slopeB * b.start.X);
        // Solve to find collision point
        sharedX = (yInterceptB - yInterceptA) / (slopeA - slopeB);
        sharedY = (slopeA * sharedX) + yInterceptA;
      }
      // Is this in the collision bounds?
      if (sharedX < lowerBound) return (false, 0, 0);
      if (sharedY < lowerBound) return (false, 0, 0);
      if (sharedX > upperBound) return (false, 0, 0);
      if (sharedY > upperBound) return (false, 0, 0);
      // Is this forward in time only? This part actually does matter.
      if (futureOnly)
      {
        // If it's the future, then the values must be bigger if positive and smaller if negative....
        // a.X
        if (a.velocity.X != 0)
        {
          if (a.velocity.X > 0)
          {
            if (sharedX < a.start.X) return (false, sharedX, sharedY);
          }
          else
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
          }
          else
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
          }
          else
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
          }
          else
          {
            if (sharedY > b.start.Y) return (false, sharedX, sharedY);
          }
        }
      }

      return (true, sharedX, sharedY);
    }
  }
}


