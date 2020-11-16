// Decompiled with JetBrains decompiler
// Type: GumpStudio.HexHelper
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using Microsoft.VisualBasic;
using System;

namespace GumpStudio
{
  public class HexHelper
  {
    protected const string Numbers = "0123456789ABCDEF";

    public static int HexToDec(string Value)
    {
      Value = Strings.UCase(Value);
      if (Strings.Len(Value) <= 2 | Strings.Len(Value) > 6 || Strings.Left(Value, 2) != "0X")
        return 0;
      int num1 = 0;
      int length = Value.Length;
      while (true)
      {
        int num2 = 3;
        if (length >= num2)
        {
          int num3 = Strings.InStr("0123456789ABCDEF", Strings.Mid(Value, length, 1), CompareMethod.Binary) - 1;
          if (num3 != -1)
          {
            num1 += (int) Math.Round(Math.Pow(16.0, Value.Length - length) * num3);
            length += -1;
          }
          else
            break;
        }
        else
          goto label_7;
      }
      return 0;
label_7:
      return num1;
    }
  }
}
