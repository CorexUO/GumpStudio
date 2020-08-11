// Decompiled with JetBrains decompiler
// Type: GumpStudio.Plugins.PluginInfo
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;

namespace GumpStudio.Plugins
{
  [Serializable]
  public class PluginInfo
  {
    public string AuthorEmail;
    public string AuthorName;
    public string Description;
    public string PluginName;
    public string Version;

    public bool Equals(PluginInfo Info)
    {
      if (!(this.AuthorEmail != Info.AuthorEmail) && !(this.AuthorName != Info.AuthorName) && (!(this.Description != Info.Description) && !(this.PluginName != Info.PluginName)))
        return !(this.Version != Info.Version);
      return false;
    }

    public override string ToString()
    {
      return this.PluginName;
    }
  }
}
