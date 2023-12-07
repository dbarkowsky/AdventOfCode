using Tools;
using System.Collections.Generic;

namespace Solutions
{


  public static class IEnumerableExtensions
  {
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
       => self.Select((item, index) => (item, index));
  }

  public enum HandType
  {
    HIGH_CARD,
    PAIR,
    TWO_PAIR,
    THREE_KIND,
    FULL_HOUSE,
    FOUR_KIND,
    FIVE_KIND
  }

  public enum CardValue
  {
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX,
    SEVEN,
    EIGHT,
    NINE,
    TEN,
    JACK,
    QUEEN,
    KING,
    ACE
  }

  public class Hand
  {
    public int Rank { set; get; }
    public int Bid { set; get; }
    public string Cards { set; get; }
    public HandType Type { set; get; }

    public Hand(string cards, int bid)
    {
      Bid = bid;
      Cards = cards;
      Type = GetHandType(cards);
    }

    private HandType GetHandType(string cards)
    {
      Dictionary<string, int> cardCounter = new Dictionary<string, int>();
      foreach (char card in cards)
      {
        cardCounter.TryGetValue(card.ToString(), out int currentCount);
        cardCounter[card.ToString()] = currentCount + 1;
      }
      cardCounter = cardCounter.OrderByDescending(card => card.Value).ToDictionary(card => card.Key, card => card.Value);

      if (IsFiveKind()) return HandType.FIVE_KIND;
      else if (IsFourKind(cardCounter)) return HandType.FOUR_KIND;
      else if (IsFullHouse(cardCounter)) return HandType.FULL_HOUSE;
      else if (IsThreeKind(cardCounter)) return HandType.THREE_KIND;
      else if (IsTwoPair(cardCounter)) return HandType.TWO_PAIR;
      else if (IsPair(cardCounter)) return HandType.PAIR;
      else return HandType.HIGH_CARD;
    }

    private bool IsFiveKind()
    {
      return Cards.All(card => card == Cards[0]);
    }

    private bool IsFourKind(Dictionary<string, int> cards)
    {
      if (cards.First().Value == 4)
      {
        return true;
      }
      return false;
    }

    private bool IsFullHouse(Dictionary<string, int> cards)
    {
      List<int> values = new List<int>();
      foreach (KeyValuePair<string, int> card in cards)
      {
        values.Add(card.Value);
      }
      if (values.Contains(3) && values.Contains(2) && values.Count == 2)
      {
        return true;
      }
      return false;
    }

    private bool IsThreeKind(Dictionary<string, int> cards)
    {
      if (cards.First().Value == 3 && cards.Count == 3)
      {
        return true;
      }
      return false;
    }

    private bool IsTwoPair(Dictionary<string, int> cards)
    {
      List<int> values = new List<int>();
      foreach (KeyValuePair<string, int> card in cards)
      {
        values.Add(card.Value);
      }
      string[] keys = cards.Keys.ToArray();
      if (cards.Count == 3 && cards[keys[0]] == 2 && cards[keys[1]] == 2)
      {
        return true;
      }
      return false;
    }

    private bool IsPair(Dictionary<string, int> cards)
    {
      if (cards.First().Value == 2 && cards.Count == 4)
      {
        return true;
      }
      return false;
    }

    public override string ToString()
    {
      return $"Cards: {Cards}, Bid: {Bid}, Type: {Type.ToString()}";
    }

    public int CompareTo(Hand h2)
    {
      if (Type > h2.Type)
      {
        return 1; // Bigger than h2
      }
      else if (Type < h2.Type)
      {
        return -1; // Smaller than h2
      }
      else
      {
        // Same types, must compare by each card
        for (int i = 0; i < Cards.Length; i++)
        {
          if (CardToValue(Cards[i]) > CardToValue(h2.Cards[i]))
          {
            return 1;
          }
          else if (CardToValue(Cards[i]) < CardToValue(h2.Cards[i]))
          {
            return -1;
          }
        }
      }
      return 0; // Equal with h2
    }

    private CardValue CardToValue(char card)
    {
      switch (card)
      {
        case '2':
          return CardValue.TWO;
        case '3':
          return CardValue.THREE;
        case '4':
          return CardValue.FOUR;
        case '5':
          return CardValue.FIVE;
        case '6':
          return CardValue.SIX;
        case '7':
          return CardValue.SEVEN;
        case '8':
          return CardValue.EIGHT;
        case '9':
          return CardValue.NINE;
        case 'T':
          return CardValue.TEN;
        case 'J':
          return CardValue.JACK;
        case 'Q':
          return CardValue.QUEEN;
        case 'K':
          return CardValue.KING;
        case 'A':
          return CardValue.ACE;
        default:
          return CardValue.TWO;
      }
    }
  }

  public class Day07
  {
    List<string> strings = new List<string>();
    List<Hand> hands = new List<Hand>();

    public Day07(string fileName)
    {
      strings = FileReader.AsStringArray(fileName).ToList();
      foreach (string item in strings)
      {
        string hand = item.Split(" ")[0];
        int bid = int.Parse(item.Split(" ")[1]);
        hands.Add(new Hand(hand, bid));
        // Console.WriteLine(hands.Last().ToString());
      }
    }

    public int PartOne()
    {
      hands.Sort((h1, h2) => h1.CompareTo(h2));
      int total = 0;
      foreach ((Hand hand, int index) in hands.WithIndex())
      {
        int winnings = (index + 1) * hand.Bid;
        total += winnings;
        Console.WriteLine(hand.ToString());
      }
      return total;
    }

    public int PartTwo()
    {
      return -1;
    }
  }
}


