using GumpStudio.Elements;
using GumpStudio.Forms;

namespace GumpStudio.Plugins
{
	public abstract class BasePlugin
	{
		public abstract PluginInfo Info { get; }

		public virtual string Name => Info?.Name;

		public bool IsLoaded { get; protected set; }

		public virtual void InitializeElementExtenders(BaseElement element)
		{ }

		public virtual void Load(DesignerForm designer)
		{
			IsLoaded = true;
		}

		public virtual void Unload()
		{ }

		public virtual void MouseMoveHook(ref MouseMoveHookEventArgs e)
		{ }
	}
}