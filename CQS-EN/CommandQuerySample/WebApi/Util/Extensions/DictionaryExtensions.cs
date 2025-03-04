﻿namespace WebApi.Util.Extensions;

public static class DictionaryExtensions
{
  public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) 
    where TValue : new() 
    where TKey : notnull
  {
    if (!dict.ContainsKey(key))
    {
      var item = new TValue();
      dict[key] = item;
      return item;
    }
    else
    {
      return dict[key];
    }	
  }  
}
