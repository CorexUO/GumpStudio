using System.Drawing;
using GumpStudio;
using GumpStudio.Forms;
using GumpStudio.Plugins;

namespace GumpStudioCore.Plugins
{
    public class SnapToGridExtender : ElementExtender
    {
        private DesignerForm _designer;
        public GridConfiguration Config { get; set; }

        public SnapToGridExtender( DesignerForm designerForm )
        {
            _designer = designerForm;
        }

        public int SnapXToGrid( int X ) => X / Config.GridSize.Width * Config.GridSize.Width;
        public int SnapYToGrid( int Y ) => Y / Config.GridSize.Height * Config.GridSize.Height;

        public Point SnapToGrid( Point Position )
        {
            Point result = Position;
            result.X = SnapXToGrid( Position.X );
            result.Y = SnapXToGrid( Position.Y );

            return result;
        }
    }
}