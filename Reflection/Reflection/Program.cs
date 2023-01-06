using LottoInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Reflection
{
  class Program
  {
    static void Main(string[] args)
    {
      string assemblyLocation = @"..\\..\\..\\..\\LottoImplementation\bin\Debug\netstandard2.0\LottoImplementation.dll";
      Console.WriteLine("Loading dll from file: " + Path.GetFullPath(assemblyLocation));
      AssemblyLoadContext loadctx = AssemblyLoadContext.Default;
      Assembly asm = loadctx.LoadFromAssemblyPath(Path.GetFullPath(assemblyLocation));
      Type type = asm.GetType("LottoImplementation.Lotto");
      PrintData(type);

      Console.WriteLine();
      Console.WriteLine("Creating an instance...");
      object obj = Activator.CreateInstance(type, 7, 39);
      MethodInfo info = type.GetMethod("DrawNumbers");
      object result = info.Invoke(obj, new object[] { false });
      string print = string.Join(", ", (List<int>)result);
      Console.WriteLine(print);

      //izvlačimo opet, ali ovaj put želimo sortirano
      result = info.Invoke(obj, new object[] { true });
      print = string.Join(", ", (List<int>)result);
      Console.WriteLine(print);

      //znamo da razred koji smo dinamički instancirali implementira ILoto
      ILotto loto = (ILotto)obj;
      List<int> numbers = loto.DrawNumbers(true);
      print = string.Join(", ", numbers);
      Console.WriteLine(print);
    }

    private static void PrintData(Type t)
    {
      ListVariousStats(t);
      ListFields(t);
      ListProps(t);
      ListMethods(t);
      ListInterfaces(t);
    }

    static void ListVariousStats(Type t)
    {
      Console.WriteLine("***** Various Statistics *****");
      Console.WriteLine("Base class is: {0}", t.BaseType);
      Console.WriteLine("Is type abstract? {0}", t.IsAbstract);
      Console.WriteLine("Is type sealed? {0}", t.IsSealed);
      Console.WriteLine("Is type generic? {0}", t.IsGenericTypeDefinition);
      Console.WriteLine("Is type a class type? {0}", t.IsClass);
      Console.WriteLine();
    }

    static void ListInterfaces(Type t)
    {
      Console.WriteLine("***** Interfaces *****");
      foreach (Type i in t.GetInterfaces())
        Console.WriteLine("->{0}", i.Name);
    }

    static void ListProps(Type t)
    {
      Console.WriteLine("***** Properties *****");
      foreach (PropertyInfo info in t.GetProperties())
        Console.WriteLine("->{0}", info);
      Console.WriteLine();
    }

    static void ListFields(Type t)
    {
      Console.WriteLine("***** Fields *****");
      //samo GetField() će vratiti samo javne varijable
      foreach (FieldInfo field in t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
        Console.WriteLine("->{0}", field);
      Console.WriteLine();
    }

    static void ListMethods(Type t)
    {
      Console.WriteLine("***** Methods *****");
      MethodInfo[] mi = t.GetMethods();
      foreach (MethodInfo m in mi)
        Console.WriteLine("->{0}", m);
      Console.WriteLine();
    }
  }
}
