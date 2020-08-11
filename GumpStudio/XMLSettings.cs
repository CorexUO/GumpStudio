using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using GumpStudio.Forms;

namespace GumpStudio
{
    public class XMLSettings
    {
        public string ClientPath { get; set; } = Environment.SpecialFolder.ProgramFiles.ToString();
        public Size DesignerFormSize { get; set; } = new Size( 1366, 768 );
        public int UndoLevels { get; set; } = 25;

        public static XMLSettings CurrentOptions { get; set; } = new XMLSettings();

        public static XMLSettings Load( DesignerForm designerForm )
        {
            string fullPath = Path.Combine( designerForm.AppPath, "settings.xml" );

            if ( !File.Exists( fullPath ) )
                return new XMLSettings();

            using ( XmlTextReader xml = new XmlTextReader( fullPath ) )
            {
                XmlSerializer serializer = new XmlSerializer( typeof( XMLSettings ) );

                return (XMLSettings) serializer.Deserialize( xml );
            }
        }

        public static void Save( DesignerForm designerForm, XMLSettings options )
        {
            string fullPath = Path.Combine( designerForm.AppPath, "settings.xml" );

            using ( XmlWriter xml = new XmlTextWriter( fullPath, Encoding.UTF8 ) {Formatting = Formatting.Indented} )
            {
                XmlSerializer serializer = new XmlSerializer( typeof( XMLSettings ) );

                serializer.Serialize( xml, options );
            }
        }
    }
}