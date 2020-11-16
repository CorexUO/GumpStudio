// Decompiled with JetBrains decompiler
// Type: GumpStudio.HuePropEditor
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms.Design;
using Ultima;

namespace GumpStudio
{
  public class HuePropEditor : UITypeEditor
  {
    protected IWindowsFormsEditorService edSvc;
    protected Hue ReturnValue;

    protected static Color Convert555ToARGB(short Col)
    {
      return Color.FromArgb(((short) (Col >> 10) & 31) * 8, ((short) (Col >> 5) & 31) * 8, (Col & 31) * 8);
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      if (value == null)
        value = Hues.GetHue(0);

      if ( !( value is Hue hue ) )
      {
          return value;
      }

      edSvc = (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));

      if ( edSvc == null )
      {
          return value;
      }

      HuePickerControl huePickerControl = new HuePickerControl(hue);

      huePickerControl.ValueChanged += new HuePickerControl.ValueChangedEventHandler(ValueSelected);

      edSvc.DropDownControl(huePickerControl);
      if (ReturnValue != null)
      {
          huePickerControl.Dispose();
          return ReturnValue;
      }
      huePickerControl.Dispose();
      return value;
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.DropDown;
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public override bool GetPaintValueSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public override void PaintValue(PaintValueEventArgs e)
    {
      Graphics graphics = e.Graphics;
      graphics.FillRectangle(Brushes.White, e.Bounds);
      float num1 = (e.Bounds.Width - 3) / 32f;
      Hue hue = (Hue) e.Value;
      if (hue == null)
        return;
      int num2 = 0;
      foreach (short color in hue.Colors)
      {
        Rectangle bounds = e.Bounds;
        int x = (int) Math.Round(bounds.X + num2 * (double) num1);
        bounds = e.Bounds;
        int y = bounds.Y;
        int width = (int) Math.Round(num1) + 1;
        bounds = e.Bounds;
        int height = bounds.Height;
        Rectangle rect = new Rectangle(x, y, width, height);
        graphics.FillRectangle(new SolidBrush(Convert555ToARGB(color)), rect);
        ++num2;
      }
    }

    protected void ValueSelected(Hue Hue)
    {
      edSvc.CloseDropDown();
      ReturnValue = Hue;
    }
  }
}
