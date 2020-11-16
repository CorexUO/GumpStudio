// Decompiled with JetBrains decompiler
// Type: GumpStudio.HuePropStringConverter
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Globalization;
using Ultima;

namespace GumpStudio
{
  public class HuePropStringConverter : StringConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      bool flag = false;
      if (sourceType == typeof (string))
        flag = true;
      return flag;
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof (Hue);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (Versioned.IsNumeric(Conversions.ToString(value)))
        return Hues.GetHue(Conversions.ToInteger(value));
      return Hues.GetHue(HexHelper.HexToDec(Conversions.ToString(value)));
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      return value.ToString();
    }
  }
}
