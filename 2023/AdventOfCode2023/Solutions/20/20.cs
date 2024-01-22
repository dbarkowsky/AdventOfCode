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
                // Console.WriteLine("high pulse to send");
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
            // Console.WriteLine($"{from}: {sources[from]}");
            foreach (string key in sources.Keys){
              // Console.WriteLine($"{key}: {sources[key]}");
            }
            // If all HIGH, send low, else send high
            if (sources.Values.All(value => value == Pulse.HIGH))
            {
              toSend = Pulse.LOW;
              // Console.WriteLine("all are high");
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
      nodes["output"] = new Node(new string[0], Type.TERMINAL); // for example
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
          if (nodes[destKey].type == Type.CONJUNCTION)
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

      for (int i = 0; i < buttonPresses; i++)
      {
        (int newLows, int newHighs) counts = PressButton();
        lows += counts.newLows;
        highs += counts.newHighs;
      }
      Console.WriteLine($"lows: {lows}");
      Console.WriteLine($"highs: {highs}");

      return lows * highs;
    }

    public int PartTwo()
    {
      return -1;
    }

    private (int lows, int highs) PressButton()
    {
      int lows = 0;
      int highs = 0;
      Queue<(string key, Pulse pulse, string from)> pulseQueue = new();
      pulseQueue.Enqueue(("broadcaster", Pulse.LOW, "button"));

      while (pulseQueue.Count > 0)
      {
        // Get first action in queue
        (string key, Pulse pulse, string from) current = pulseQueue.Dequeue();
        if (current.pulse == Pulse.HIGH) highs++;
        else lows++;
        // Hit it with required pulse and get next for queue
        List<(string node, Pulse pulse, string from)> nextNodes = nodes[current.key].GetNext(current.from, current.pulse);
        List<(string node, Pulse pulse, string from)> refinedNodes = new();
        foreach((string node, Pulse pulse, string from) blah in nextNodes){
          Console.WriteLine($"{current.key} -{blah.pulse}-> {blah.node}");
          refinedNodes.Add((blah.node, blah.pulse, current.key));
        }
        refinedNodes.ForEach(pulseQueue.Enqueue);

        // nextNodes.ForEach(pulseQueue.Enqueue);
        foreach (string nextKey in nodes[current.key].destinations)
        {
          // Console.WriteLine($"{current.key} -{current.pulse}-> {nextKey}");
          
          // Console.WriteLine($"{current.key} queues {String.Join(",", nextNodes.Select(next => $"{next.node}"))}");
          
        }
      }
      return (lows, highs);
    }
  }
}


