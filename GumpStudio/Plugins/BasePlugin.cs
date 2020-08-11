// Decompiled with JetBrains decompiler
// Type: GumpStudio.Plugins.BasePlugin
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using GumpStudio.Elements;
using GumpStudio.Forms;

namespace GumpStudio.Plugins
{
  public abstract class BasePlugin
  {
    protected bool mIsLoaded;

    public bool IsLoaded => this.mIsLoaded;

    public abstract string Name { get; }

    public abstract PluginInfo GetPluginInfo();

    public virtual void InitializeElementExtenders(BaseElement Element)
    {
    }

    public virtual void Load(DesignerForm frmDesigner)
    {
      this.mIsLoaded = true;
    }

    public virtual void MouseMoveHook(ref MouseMoveHookEventArgs e)
    {
    }

    public virtual void Unload()
    {
    }
  }
}
