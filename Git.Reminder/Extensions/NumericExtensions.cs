using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public static  class NumericExtensions
    {
      public static bool Between(this int value, int min, int max)
      {
          return value >= min && value <= max;
      }
    }
}
