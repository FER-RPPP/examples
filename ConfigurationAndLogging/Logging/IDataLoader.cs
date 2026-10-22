using System;
using System.Collections.Generic;

namespace Logging;

public interface IDataLoader
{
  List<(string, DateOnly)> LoadData(string filename);
}
