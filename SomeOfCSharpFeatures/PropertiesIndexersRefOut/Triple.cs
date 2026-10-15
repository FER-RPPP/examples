namespace PropertiesIndexersRefOut;

public class Triple
{
  private int first;
  public int First
  {
    get
    {
      return first;
    }
    set
    {
      first = value;
    }
  }

  public int Second { get; set; }

  public int Third => First + Second;
  public override string ToString()
  {
    return $"({First}, {Second}, {Third})";
  }

  #region Indexer
  public int this[string s, int position]  
  {      
    get
    {        
      if (position <= 0)
      {
        throw new ArgumentException("Position must be a positive integer");
      }
      int num = s switch
      {
        "A" => this.First,
        "B" => this.Second,
        "C" => this.Third,
        _ => throw new ArgumentException("Invalid position in triple (should be A, B, or C)")
      };
      // position is 1-based from the left (1 = most significant digit).
      string digits = Math.Abs(num).ToString();
      if (position > digits.Length)
      {
        return 0;
      }
      return digits[position - 1] - '0';
    }
    set
    {
      // value should be a digit (0-9), and the position should be a positive integer
      // Positions are 1-based from the left (1 = most significant digit).
      // Example: for number 123, position 5 and value 9 -> new number 12309 (zeros appended on the right to reach position)
      if (position <= 0)
      {
        throw new ArgumentException("Position must be a positive integer");
      }
      if (value < 0 || value > 9)
      {
        throw new ArgumentException("Value must be a digit (0-9)");
      }
      // Only A and B can be set; C is derived (First + Second)
      switch (s)
      {
        case "A":
          this.First = SetDigit(this.First, position, value);
          break;
        case "B":
          this.Second = SetDigit(this.Second, position, value);
          break;
        case "C":
          throw new ArgumentException("Cannot set C because it is a derived value (First + Second)");
        default:
          throw new ArgumentException("Invalid position in triple (should be A or B)");
      }
    }
  }
  #endregion

  private static int SetDigit(int num, int position, int digit)
  {
    // position is 1-based from the left (1 = most significant digit).
    string sign = num < 0 ? "-" : "";
    string digits = Math.Abs(num).ToString();
    if (position > digits.Length)
    {
      digits = digits + new string('0', position - digits.Length);
    }
    char[] chars = digits.ToCharArray();
    chars[position - 1] = (char)('0' + digit);
    string result = new string(chars);
    return int.Parse(sign + result);
  }


  #region Operator overloading
  public static Triple operator +(Triple x, Triple y)
  {    
    return new Triple
    {
      First = x.First + y.First,
      Second = x.Second + y.Second
    };
  }
  #endregion




}
