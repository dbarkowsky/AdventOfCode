using Tools;

namespace Solutions
{
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
    JOKER,
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

  // A class to handle all the checks needed for a hand of cards
  public class Hand
  {
    public int Bid { set; get; }
    public string Cards { set; get; }
    public HandType Type { set; get; }

    public Hand(string cards, int bid)
    {
      Bid = bid;
      Cards = cards;
      Type = GetHandType(cards);
    }

    // A constructor to reconstruct existing hands...
    public Hand(Hand original, bool part2 = false)
    {
      Bid = original.Bid;
      Cards = original.Cards;
      Type = GetHandType(original.Cards, part2);
    }

    // Returns the HandType of a given Hand. Answer depends on whether Js are Jokers or Jacks.
    private HandType GetHandType(string cards, bool part2 = false)
    {
      // Put cards into a dictionary, counting number of occurances
      Dictionary<string, int> cardCounter = new Dictionary<string, int>();
      foreach (char card in cards)
      {
        cardCounter.TryGetValue(card.ToString(), out int currentCount);
        cardCounter[card.ToString()] = currentCount + 1;
      }
      // Sorting the dictionary
      cardCounter = cardCounter.OrderByDescending(card => card.Value).ToDictionary(card => card.Key, card => card.Value);

      // If part 2, need to reevaluate the Jokers
      // Also only do this if there are any jokers
      if (part2 && cardCounter.TryGetValue("J", out int numberOfJokers))
      {
        switch (numberOfJokers)
        {
          case 5:
          case 4:
            // 5 or 4 is a FIVE_KIND
            return HandType.FIVE_KIND;
          case 3:
            // If other cards all match, Five Kind
            if (cardCounter.Count == 2) return HandType.FIVE_KIND;
            // Otherwise, Four Kind
            return HandType.FOUR_KIND;
          case 2:
            // If other cards all match, Five Kind
            if (cardCounter.Count == 2) return HandType.FIVE_KIND;
            // If only two match, Four Kind
            if (cardCounter.Count == 3) return HandType.FOUR_KIND;
            // Otherwise, Three Kind
            return HandType.THREE_KIND;
          case 1:
            // If other cards all match, Five Kind
            if (cardCounter.Count == 2) return HandType.FIVE_KIND;
            // If 3 match, Four Kind
            if (cardCounter.Count == 3 && cardCounter.First().Value == 3) return HandType.FOUR_KIND;
            // If two match, and another two match, like AABBJ, then Full House
            if (cardCounter.Count == 3 && cardCounter.First().Value == 2) return HandType.FULL_HOUSE;
            // If two match, like AABCJ, then Three Kind
            if (cardCounter.Count == 4 && cardCounter.First().Value == 2) return HandType.THREE_KIND;
            // None match, it's a PAIR
            return HandType.PAIR;
          default:
            return HandType.PAIR;
        }
      }
      else
      {
        if (IsFiveKind()) return HandType.FIVE_KIND;
        else if (IsFourKind(cardCounter)) return HandType.FOUR_KIND;
        else if (IsFullHouse(cardCounter)) return HandType.FULL_HOUSE;
        else if (IsThreeKind(cardCounter)) return HandType.THREE_KIND;
        else if (IsTwoPair(cardCounter)) return HandType.TWO_PAIR;
        else if (IsPair(cardCounter)) return HandType.PAIR;
        else return HandType.HIGH_CARD;
      }
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

    public int CompareTo(Hand h2, bool part2 = false)
    {
      // First compare if types are different
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
          CardValue thisCardValue = CardToValue(Cards[i], part2);
          CardValue otherCardValue = CardToValue(h2.Cards[i], part2);
          if (thisCardValue > otherCardValue)
          {
            return 1;
          }
          else if (thisCardValue < otherCardValue)
          {
            return -1;
          }
        }
      }
      return 0; // Equal with h2
    }

    private CardValue CardToValue(char card, bool part2 = false)
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
          if (part2) return CardValue.JOKER;
          else return CardValue.JACK;
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
        // Console.WriteLine(hand.ToString());
      }
      return total;
    }

    public int PartTwo()
    {
      // Rework what the hands are
      for (int i = 0; i < hands.Count; i++)
      {
        hands[i] = new Hand(hands[i], true);
      }
      hands.Sort((h1, h2) => h1.CompareTo(h2, true));
      int total = 0;
      foreach ((Hand hand, int index) in hands.WithIndex())
      {
        int winnings = (index + 1) * hand.Bid;
        total += winnings;
        // Console.WriteLine(hand.ToString() + $", Rank: {index + 1}");
      }
      return total;
    }
  }
}


