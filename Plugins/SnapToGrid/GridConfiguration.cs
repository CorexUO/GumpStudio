using System;
using System.Drawing;

namespace GumpStudioCore.Plugins
{
    [Serializable]
    public class GridConfiguration
    {
        public Size GridSize { get; set; }
        public Color GridColor { get; set; }
        public bool ShowGrid { get; set; }

        public GridConfiguration( Size gridSize, Color gridColor, bool showGrid )
        {
            GridSize = gridSize;
            GridColor = gridColor;
            ShowGrid = showGrid;
        }
    }
}