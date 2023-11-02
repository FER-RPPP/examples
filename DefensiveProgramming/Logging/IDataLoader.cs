using System;
using System.Collections.Generic;

namespace Logging;

public interface IDataLoader
{
  List<(string, DateTime)> LoadData(string filename);
}
