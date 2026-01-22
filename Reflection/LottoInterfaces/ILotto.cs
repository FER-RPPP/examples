using System.Collections.Generic;

namespace LottoInterfaces;

public interface ILotto
{
  List<int> DrawNumbers(bool sort);
}
