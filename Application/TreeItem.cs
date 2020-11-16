// Decompiled with JetBrains decompiler
// Type: GumpStudio.TreeItem
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

namespace GumpStudio
{
  public abstract class TreeItem
  {
    public TreeItem Parent;
    public string Text;

    public override string ToString()
    {
      return this.Text;
    }
  }
}
