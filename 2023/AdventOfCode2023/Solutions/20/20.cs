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
      public Pulse pulseLastReceived;
      public Node(string[] d, Type t)
      {
        destinations = d.ToList();
        type = t;
        isOn = false;
        pulseLastReceived = Pulse.LOW;
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
        (int newLows, int newHighs, bool rxReceivedLowPulse) counts = PressButton();
        lows += counts.newLows;
        highs += counts.newHighs;
      }
      return lows * highs;
    }

    public long PartTwo()
    {
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

      while (!solutionFound){
        (int newLows, int newHighs, bool rxReceivedLowPulse) counts = PressButton();
        buttonPresses++;
        // Check if any of the sources are HIGH. Log their value, keeping the smallest one.
        foreach (string source in nodes[rxSource].sources.Keys){
          // Console.WriteLine($"source: {source}");
          if (nodes[rxSource].sources[source] == Pulse.HIGH){
            sourceTracker[source] = Math.Min(buttonPresses, sourceTracker[source]);
            Console.WriteLine($"new min for {source}: {sourceTracker[source]}");
          }
        }
        solutionFound = sourceTracker.Values.All(value => value != long.MaxValue);
      }
      Console.WriteLine($"all hits {buttonPresses}");
      // TODO: Find LCM of minimum button presses
      return buttonPresses;
    }

    private (int lows, int highs, bool rx) PressButton()
    {
      bool rxReceivedLowPulse = false;
      int lows = 0;
      int highs = 0;
      Queue<(string key, Pulse pulse, string from)> pulseQueue = new();
      pulseQueue.Enqueue(("broadcaster", Pulse.LOW, "button"));

      while (pulseQueue.Count > 0)
      {
        // Get first action in queue
        (string key, Pulse pulse, string from) current = pulseQueue.Dequeue();
        // Record received pulse
        nodes[current.key].pulseLastReceived = current.pulse;
        // Increment counters
        if (current.pulse == Pulse.HIGH) highs++;
        else lows++;
        // Hit it with required pulse and get next for queue
        List<(string node, Pulse pulse, string from)> nextNodes = nodes[current.key].GetNext(current.from, current.pulse);
        List<(string node, Pulse pulse, string from)> refinedNodes = new();
        foreach ((string node, Pulse pulse, string from) nextNode in nextNodes)
        {
          // Console.WriteLine($"{current.key} -{nextNode.pulse}-> {nextNode.node}");
          refinedNodes.Add((nextNode.node, nextNode.pulse, current.key));
          // Check if it's the rx low pulse
          if (nextNode.pulse == Pulse.LOW && nextNode.node == "rx")
          {
            rxReceivedLowPulse = true;
          }
        }
        refinedNodes.ForEach(pulseQueue.Enqueue);
      }
      return (lows, highs, rxReceivedLowPulse);
    }
  }
}


