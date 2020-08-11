using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GumpStudio;
using GumpStudio.Elements;
using GumpStudio.Forms;
using GumpStudio.Plugins;

namespace RunUOExport
{
    public class RunUOExport : BasePlugin
    {
        public override string Name => GetPluginInfo().PluginName;

        private DesignerForm _designer;
        private static readonly string CSTemplate = "using System;\r\nusing Server;\r\nusing Server.Gumps;\r\n\r\nnamespace Server.Gumps\r\n{\r\n\tpublic class CustomGump : Gump\r\n\t{\r\n\t\tpublic CustomGump() : base( 100, 100 )\r\n\t\t{\r\n{gump_commands}\t\t}\r\n\t}\r\n}";

        public override PluginInfo GetPluginInfo()
        {
            PluginInfo val = new PluginInfo
            {
                AuthorEmail = "reeeetus@gmail.com",
                AuthorName = "Reetus",
                Description = "Simple RunUO Exporter (Extracted from original exporter plugin by Daegon / Eric Brown.",
                PluginName = "RunUO Exporter",
                Version = "1.0"
            };

            return val;
        }

        public override void Load( DesignerForm frmDesigner )
        {
            _designer = frmDesigner;

            MenuItem menuItem = new MenuItem( "RunUO Export", ExportClick );

            _designer.mnuFileExport.Enabled = true;
            _designer.mnuFileExport.MenuItems.Add( menuItem );
        }

        private void ExportClick( object sender, EventArgs e )
        {
            string fullPath = Path.GetTempFileName() + ".txt";

            StringBuilder elementText = new StringBuilder();

            string text = CSTemplate;

            elementText.AppendLine( "\t\t\tDragable = true;" );
            elementText.AppendLine( "\t\t\tClosable = true;" );
            elementText.AppendLine( "\t\t\tResizable = false;" );
            elementText.AppendLine( "\t\t\tDisposable = false;" );

            int page = 0;

            foreach ( GroupElement stack in _designer.Stacks )
            {
                elementText.AppendLine( $"\t\t\tAddPage({page});" );

                if ( stack.Elements == null || stack.Elements.Length <= 0 )
                {
                    continue;
                }

                foreach ( BaseElement element in stack.GetElementsRecursive() )
                {
                    if ( element is IRunUOExportable exportable )
                    {
                        elementText.AppendLine( "\t\t\t" + exportable.ToRunUOString() );
                    }
                }
            }

            text = text.Replace( "{gump_commands}", elementText.ToString() );

            File.WriteAllText( fullPath, text );

            Process p = new Process { StartInfo = new ProcessStartInfo( fullPath ) { UseShellExecute = true } };
            p.Start();
        }
    }
}
