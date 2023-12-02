using Tools;
using System;
using System.Text.RegularExpressions;

namespace Solutions
{
  public class Day02
  {
    List<String> lines = new List<String>();


    public Day02(String fileName)
    {
      lines = FileReader.AsStringArray(fileName).ToList();
    }

    public int PartOne()
    {
	int total = 0;
	int maxRed = 0;
	int maxGreen = 0;
	int maxBlue = 0;

	// For each of the lines
		// Split string on ": "
		// Record number of this group from left side or use index from for each
		// Then split right half on "; " to separate pulls
		// For each pull,  split on ", " to separate colours
			// For each colour like "blue 12", use switch to compare number to max above
			// IF it's above, continue group loop, ELSE add index +1 to total 
      return -1;
    }

    public int PartTwo()
    {
     return -1;
    }
  }
}


