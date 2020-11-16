// Decompiled with JetBrains decompiler
// Type: GumpStudio.TreeGumpling
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using GumpStudio.Elements;

namespace GumpStudio
{
  public class TreeGumpling : TreeItem
  {
    public GroupElement Gumpling;

    public TreeGumpling(string Text, GroupElement Gumpling)
    {
      this.Text = Text;
      this.Gumpling = Gumpling;
    }
  }
}
