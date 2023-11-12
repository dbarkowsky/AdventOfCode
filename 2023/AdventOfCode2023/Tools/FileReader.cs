using System;
using System.IO;
namespace Tools
{
  public static class FileReader
  {
    public static string[] AsStringArray(string fileName)
    {
      string filePath = "Input/" + fileName;
      if (File.Exists(filePath))
      {
        return File.ReadAllLines(filePath);
      }
      else
      {
        throw new FileNotFoundException("FileReader: Input file does not exist.");
      }
    }
  }
}
