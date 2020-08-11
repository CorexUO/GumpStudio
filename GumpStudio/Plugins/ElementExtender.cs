// Decompiled with JetBrains decompiler
// Type: GumpStudio.Plugins.ElementExtender
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System.Windows.Forms;

namespace GumpStudio.Plugins
{
  public abstract class ElementExtender
  {
    public virtual void AddContextMenus(ref MenuItem GroupMenu, ref MenuItem PositionMenu, ref MenuItem OrderMenu, ref MenuItem MiscMenu)
    {
    }
  }
}
