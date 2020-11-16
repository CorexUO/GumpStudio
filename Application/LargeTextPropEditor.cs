// Decompiled with JetBrains decompiler
// Type: GumpStudio.LargeTextPropEditor
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace GumpStudio
{
  public class LargeTextPropEditor : UITypeEditor
  {
    protected IWindowsFormsEditorService edSvc;
    protected int ReturnValue;

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      this.edSvc = (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));
      if (this.edSvc == null)
        return value;
      LargeTextEditor largeTextEditor = new LargeTextEditor();
      largeTextEditor.txtText.Text = Conversions.ToString(value);
      if (this.edSvc.ShowDialog(largeTextEditor) == DialogResult.OK)
        return largeTextEditor.txtText.Text;
      return value;
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.Modal;
    }
  }
}
