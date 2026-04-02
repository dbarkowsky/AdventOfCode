using System;
using Tools;

namespace Solutions
{
  public class Day20
  {
    public enum Type
    {
      FLIPFLOP,
      BROADCAST,
      CONJUNCTION,
      TERMINAL
    }
    public enum Pulse
    {
      HIGH,
      LOW
    }
    public class Node
    {
      public List<string> destinations;
      public Dictionary<string, Pulse> sources = new();
      public Type type;
      public bool isOn;
      public Node(string[] d, Type t)
      {
        destinations = d.ToList();
        type = t;
        isOn = false;
      }

      public void Reset()
      {
        isOn = false;
        foreach (string key in sources.Keys.ToList())
        {
          sources[key] = Pulse.LOW;
        }
      }

      public List<(string node, Pulse pulse, string from)> GetNext(string from, Pulse p)
      {
        List<(string node, Pulse pulse, string from)> nextPulses = new();
        Pulse toSend;

        switch (type)
        {
          case Type.BROADCAST:
            // For each destination, queue up a new pulse to send
            foreach (string nextNode in destinations)
            {
              nextPulses.Add((nextNode, Pulse.LOW, from));
            }
            return nextPulses;
          case Type.FLIPFLOP:
            if (p == Pulse.LOW)
            {
              // Which pulse to send?
              if (isOn)
              {
                toSend = Pulse.LOW;
              }
              else
              {
                toSend = Pulse.HIGH;
              }
              // Toggle on status
              isOn = !isOn;
              // For each destination, queue up a new pulse to send
              foreach (string nextNode in destinations)
              {
                nextPulses.Add((nextNode, toSend, from));
              }
              return nextPulses;
            }
            return nextPulses;
          case Type.CONJUNCTION:
            // Set source pulse memory
            sources[from] = p;
            // If all HIGH, send low, else send high
            if (sources.Values.All(value => value == Pulse.HIGH))
            {
              toSend = Pulse.LOW;
            }
            else
            {
              toSend = Pulse.HIGH;
            }
            // For each destination, queue up a new pulse to send
            foreach (string nextNode in destinations)
            {
              nextPulses.Add((nextNode, toSend, from));
            }
            return nextPulses;
        }
        return nextPulses;
      }
    }
    List<string> strings = new List<string>();
    Dictionary<string, Node> nodes = new();

    public Day20(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      // Add terminal nodes here
      // nodes["output"] = new Node(new string[0], Type.TERMINAL); // for example
      nodes["rx"] = new Node(new string[0], Type.TERMINAL); // Output my input required

      // Add source nodes
      foreach (string row in strings)
      {
        string left = row.Split(" -> ").First();
        string right = row.Split(" -> ").Last();
        string[] destinations = right.Split(", ");
        if (left.First() == '%')
        {
          string name = left.Substring(1);
          nodes[name] = new Node(destinations, Type.FLIPFLOP);
        }
        else if (left.First() == '&')
        {
          string name = left.Substring(1);
          nodes[name] = new Node(destinations, Type.CONJUNCTION);
        }
        else
        {
          nodes[left] = new Node(destinations, Type.BROADCAST);
        }
      }

      // Add sources to conjunctions
      foreach (string key in nodes.Keys)
      {
        // Check destinations for conjuction types
        foreach (string destKey in nodes[key].destinations)
        {
          if (nodes[destKey].type == Type.CONJUNCTION || nodes[destKey].type == Type.TERMINAL)
          {
            // Add the key to the node's sources
            nodes[destKey].sources[key] = Pulse.LOW;
          }
        }
      }
    }

    public long PartOne()
    {
      int buttonPresses = 1000;
      int lows = 0;
      int highs = 0;

      for (int i = 1; i <= buttonPresses; i++)
      {
        var counts = PressButton();
        lows += counts.lows;
        highs += counts.highs;
      }
      return lows * highs;
    }

    private void Reset()
    {
      foreach (Node node in nodes.Values)
      {
        node.Reset();
      }
    }

    public long PartTwo()
    {
      // This screwed me up for a long time. Part 1 left the nodes in a altered state.
      // So running part 2 already had a number of button presses completed...
      Reset();
      // rx will only get a low pulse when its source (conjunctor) remembers all high pulses from its sources
      // What is the rx source?
      string rxSource = nodes.Keys.Where(key => nodes[key].destinations.Contains("rx")).ToList().First();
      // What are its sources?
      List<string> sources = nodes.Keys.Where(key => nodes[key].destinations.Contains(rxSource)).ToList();

      // Make a dict to track the lowest number of button presses to for these sources to send HIGH pulses
      Dictionary<string, long> sourceTracker = new();
      foreach (string source in sources){
        sourceTracker[source] = long.MaxValue;
      }
      
      long buttonPresses = 0;
      bool solutionFound = false;

      // This set tracks which nodes we want to watch for HIGH pulses.
      HashSet<string> watchSet = new(sources);

      while (!solutionFound){
        HashSet<string> firedHigh = PressButton(watchSet).firedHigh;
        buttonPresses++;
        foreach (string source in sources){
          if (firedHigh.Contains(source) && sourceTracker[source] == long.MaxValue){
            sourceTracker[source] = buttonPresses;
          }
        }
        solutionFound = sourceTracker.Values.All(value => value != long.MaxValue);
      }
      return sourceTracker.Values.Aggregate(Lcm);
    }

    // quick functions for finding the lowest common multiple
    private static long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);
    private static long Lcm(long a, long b) => a / Gcd(a, b) * b;

    private (int lows, int highs, HashSet<string> firedHigh) PressButton(HashSet<string>? watch = null)
    {
      HashSet<string> firedHigh = new();
      int lows = 0;
      int highs = 0;
      Queue<(string key, Pulse pulse, string from)> pulseQueue = new();
      pulseQueue.Enqueue(("broadcaster", Pulse.LOW, "button"));

      while (pulseQueue.Count > 0)
      {
        // Get first action in queue
        (string key, Pulse pulse, string from) current = pulseQueue.Dequeue();
        // Increment counters
        if (current.pulse == Pulse.HIGH) highs++;
        else lows++;
        // Hit it with required pulse and get next for queue
        List<(string node, Pulse pulse, string from)> nextNodes = nodes[current.key].GetNext(current.from, current.pulse);
        List<(string node, Pulse pulse, string from)> refinedNodes = new();
        foreach ((string node, Pulse pulse, string from) nextNode in nextNodes)
        {
          if (nextNode.pulse == Pulse.HIGH && watch != null && watch.Contains(current.key))
          {
            firedHigh.Add(current.key);
          }
          refinedNodes.Add((nextNode.node, nextNode.pulse, current.key));
        }
        refinedNodes.ForEach(pulseQueue.Enqueue);
      }
      return (lows, highs, firedHigh);
    }
  }
}


