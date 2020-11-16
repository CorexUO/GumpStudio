// Decompiled with JetBrains decompiler
// Type: GumpStudio.TreeFolder
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System.Collections;

namespace GumpStudio
{
  public class TreeFolder : TreeItem
  {
    protected ArrayList Children = new ArrayList();

    public TreeFolder(string Text)
    {
      this.Text = Text;
    }

    public void AddItem(TreeItem Item)
    {
      this.Children.Add(Item);
      Item.Parent = this;
    }

    public ArrayList GetChildren()
    {
      return this.Children;
    }

    public void RemoveItem(TreeItem Item)
    {
      this.Children.Remove(Item);
      Item.Parent = null;
    }
  }
}
