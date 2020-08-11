// Decompiled with JetBrains decompiler
// Type: GumpStudio.GumpCacheEntry
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Drawing;

namespace GumpStudio
{
  [Serializable]
  public class GumpCacheEntry
  {
    public int ID;
    [NonSerialized]
    public Image ImageCache;
    public string Name;
    public Size Size;

    public override string ToString()
    {
      return this.ID.ToString();
    }
  }
}
