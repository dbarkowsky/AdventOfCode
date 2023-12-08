namespace Tools
{
  // From a Stack Overflow post...
  // Allows the used of the .WithIndex() method on IEnumerables
  public static class IEnumerableExtensions
  {
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
       => self.Select((item, index) => (item, index));
  }
}
