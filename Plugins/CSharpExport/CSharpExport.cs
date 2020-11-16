using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GumpStudio.Elements;
using GumpStudio.Forms;
using GumpStudio.Plugins;

namespace CSharpExport
{
	public class CSharpExport : BasePlugin
	{
		private static readonly string Template = @"
#region References
using System;

using Server;
using Server.Commands;
#endregion

namespace Server.Gumps
{
	public class ~gump_type~ : Gump
	{
		public static void Configure()
		{
			CommandSystem.Register(""~gump_type~"", AccessLevel.Administrator, e => DisplayTo(e.Mobile));
		}

		public static ~gump_type~ DisplayTo(Mobile user)
		{
			if (user == null || user.Deleted || !user.Player || user.NetState == null)
				return null;

			user.CloseGump(typeof(~gump_type~));

			var gump = new ~gump_type~(user);

			user.SendGump(gump);

			return gump;
		}

		public Mobile User { get; }

		private ~gump_type~(Mobile user) 
			: base(~gump_location~)
		{
			User = user;

			Dragable = true;
			Closable = true;
			Resizable = false;
			Disposable = false;

			AddPage(0);
			~gump_layout~
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
		}

		public override void OnServerClose(NetState owner)
		{
		}
	}
}
";

		private DesignerForm _Designer;

		public override PluginInfo Info { get; } = new PluginInfo("C# Exporter", "1.0", "Vorspire", "admin@vita-nex.com", "Exports a C# file compatible with emulators targeting .NET");

		public override void Load(DesignerForm designer)
		{
			_Designer = designer;

			_Designer.mnuFileExport.Enabled = true;
			_Designer.mnuFileExport.MenuItems.Add(new MenuItem("C# Export", ExportClick));
		}

		private void ExportClick(object sender, EventArgs e)
		{
			var fullPath = $"{Path.GetTempFileName()}.txt";

			var indent = new StringBuilder();

			var layoutBegin = Template.IndexOf("~gump_layout~");

			while (--layoutBegin >= 0) {
				if (Template[layoutBegin] == '\r' || Template[layoutBegin] == '\n') {
					break;
				}

				if (!Char.IsWhiteSpace(Template, layoutBegin)) {
					break;
				}

				indent.Insert(0, Template[layoutBegin]);
			}

			var tabs = indent.ToString();

			var template = new StringBuilder(Template);

			template = template.Replace("~gump_type~", "CustomGump");

			var stacks = new Dictionary<GroupElement, ICSharpExportable[]>();

			foreach (var stack in _Designer.Stacks.Cast<GroupElement>()) {
				var elements = stack.GetElementsRecursive().OfType<ICSharpExportable>().ToArray();

				if (elements.Length > 0) {
					stacks[stack] = elements;
				}
			}

			var location = new Point(Int32.MaxValue, Int32.MaxValue);

			foreach (var element in stacks.Values.SelectMany(o => o.OfType<BaseElement>())) {
				location.X = Math.Min(location.X, element.X);
				location.Y = Math.Min(location.Y, element.Y);
			}

			if (location.X < Int32.MaxValue) {
				location.X = Math.Max(0, Math.Min(0x800, location.X));
			}
			else {
				location.X = 0;
			}

			if (location.Y < Int32.MaxValue) {
				location.Y = Math.Max(0, Math.Min(0x400, location.Y));
			}
			else {
				location.Y = 0;
			}

			template = template.Replace("~gump_location~", $"{location.X}, {location.Y}");

			var layout = new StringBuilder();

			var page = -1;

			foreach (var entry in stacks) {
				if (++page >= 1) {
					layout.AppendLine($"{tabs}AddPage({page});");
				}

				foreach (var exportable in entry.Value) {
					if (exportable is BaseElement element) {
						element.X -= location.X;
						element.Y -= location.Y;

						layout.AppendLine($"{tabs}{exportable.ToCSharpString()}");

						element.X += location.X;
						element.Y += location.Y;
					}
				}
			}

			template = template.Replace("~gump_layout~", layout.ToString().Trim());

			File.WriteAllText(fullPath, template.ToString());

			try {
				Process.Start(new ProcessStartInfo(fullPath) {
					UseShellExecute = true
				});
			}
			catch { }
		}
	}
}
