using LottoInterfaces;
using System;
using System.Collections.Generic;

namespace LottoImplementation;

public class Lotto : ILotto
{
  private int ballsToBeDrawn, numberOfBalls;
  public string? SomeProperty { get; set; }
  public Lotto(int ballsToBeDrawn, int numberOfBalls)
  {
    this.ballsToBeDrawn = ballsToBeDrawn;
    this.numberOfBalls = numberOfBalls;
  }

  public List<int> DrawNumbers(bool sort)
  {
    List<int> list = new List<int>();
    Random r = new Random(DateTime.Now.Millisecond);
    int counter = ballsToBeDrawn;
    while (counter > 0)
    {
      int x = r.Next(1, numberOfBalls + 1);
      if (!list.Contains(x))
      {
        list.Add(x);
        counter--;
      }
    }
    if (sort)
    {
      list.Sort();
    }
    return list;
  }
}
