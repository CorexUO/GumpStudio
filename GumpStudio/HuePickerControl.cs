// Decompiled with JetBrains decompiler
// Type: GumpStudio.HuePickerControl
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Ultima;

namespace GumpStudio
{
    public class HuePickerControl : UserControl
    {
        private ComboBox _cboQuick;
        private ListBox _lstHue;
        private StatusBar _StatusBar;
        private ToolTip _ToolTip1;
        private object[] _hueNames;
        private IContainer components;
        protected Hue mHue;

        public Hue Hue
        {
            get => this.mHue;
            set => this.mHue = value;
        }

        public event HuePickerControl.ValueChangedEventHandler ValueChanged;

        public HuePickerControl()
        {
            this.Load += new EventHandler( this.HuePickerControl_Load );
            this.InitializeComponent();
        }

        public HuePickerControl( Hue InitialHue )
          : this()
        {
            this.mHue = InitialHue;
        }

        private void cboQuick_SelectedIndexChanged( object sender, EventArgs e )
        {
            string s = "0";
            switch ( this._cboQuick.Text )
            {
                case "Colors":
                    s = "0";
                    break;
                case "Skin":
                    s = "1001";
                    break;
                case "Hair":
                    s = "1101";
                    break;
                case "Interesting #1":
                    s = "1049";
                    break;
                case "Pinks":
                    s = "1200";
                    break;
                case "Elemental Weapons":
                    s = "1254";
                    break;
                case "Interesting #2":
                    s = "1278";
                    break;
                case "Blues":
                    s = "1300";
                    break;
                case "Elemental Wear":
                    s = "1354";
                    break;
                case "Greens":
                    s = "1400";
                    break;
                case "Oranges":
                    s = "1500";
                    break;
                case "Reds":
                    s = "1600";
                    break;
                case "Yellows":
                    s = "1700";
                    break;
                case "Neutrals":
                    s = "1800";
                    break;
                case "Snakes":
                    s = "2000";
                    break;
                case "Birds":
                    s = "2100";
                    break;
                case "Slimes":
                    s = "2200";
                    break;
                case "Animals":
                    s = "2300";
                    break;
                case "Metals":
                    s = "2400";
                    break;
            }
            this._lstHue.SelectedIndex = this._lstHue.FindString( s );
            this._lstHue.Focus();
        }

        protected static Color Convert555ToARGB( short Col )
        {
            return Color.FromArgb( ( (short) ( Col >> 10 ) & 31 ) * 8, ( (short) ( Col >> 5 ) & 31 ) * 8, ( Col & 31 ) * 8 );
        }

        private void HuePickerControl_Load( object sender, EventArgs e )
        {
            this._lstHue.Items.Clear();
            foreach ( Hue hue in Hues.List )
            {
                if ( hue.Index == this.mHue.Index )
                    this._lstHue.SelectedIndex = this._lstHue.Items.Add( hue );
                else
                    this._lstHue.Items.Add( hue );
            }
            this._StatusBar.Text = Conversions.ToString( this.mHue.Index ) + ": " + this.mHue.Name;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._lstHue = new System.Windows.Forms.ListBox();
            this._StatusBar = new System.Windows.Forms.StatusBar();
            this._cboQuick = new System.Windows.Forms.ComboBox();
            this._ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // _lstHue
            // 
            this._lstHue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lstHue.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._lstHue.IntegralHeight = false;
            this._lstHue.Location = new System.Drawing.Point(0, 0);
            this._lstHue.Name = "_lstHue";
            this._lstHue.Size = new System.Drawing.Size(208, 244);
            this._lstHue.TabIndex = 0;
            this._lstHue.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstHue_DrawItem);
            this._lstHue.SelectedIndexChanged += new System.EventHandler(this.lstHue_SelectedIndexChanged);
            this._lstHue.DoubleClick += new System.EventHandler(this.lstHue_DoubleClick);
            // 
            // _StatusBar
            // 
            this._StatusBar.Location = new System.Drawing.Point(0, 0);
            this._StatusBar.Name = "_StatusBar";
            this._StatusBar.Size = new System.Drawing.Size(100, 22);
            this._StatusBar.TabIndex = 0;
            // 
            // _cboQuick
            // 
            this._cboQuick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboQuick.DropDownWidth = 120;
            this._cboQuick.Items.AddRange(new object[] {
            "Colors",
            "Skin",
            "Hair",
            "Interesting #1",
            "Pinks",
            "Elemental Weapons",
            "Interesting #2",
            "Blues",
            "Elemental Wear",
            "Greens",
            "Oranges",
            "Reds",
            "Yellows",
            "Neutrals",
            "Snakes",
            "Birds",
            "Slimes",
            "Animals",
            "Metals"});
            this._cboQuick.Location = new System.Drawing.Point(144, 243);
            this._cboQuick.Name = "_cboQuick";
            this._cboQuick.Size = new System.Drawing.Size(64, 21);
            this._cboQuick.TabIndex = 2;
            this._ToolTip1.SetToolTip(this._cboQuick, "Bookmarks");
            this._cboQuick.SelectedIndexChanged += new System.EventHandler(this.cboQuick_SelectedIndexChanged);
            // 
            // HuePickerControl
            // 
            this.Controls.Add( this._cboQuick );
            this.Controls.Add( this._StatusBar );
            this.Controls.Add( this._lstHue );
            this.Name = "HuePickerControl";
            this.Size = new System.Drawing.Size(208, 264);
            this.ResumeLayout(false);
        }

        private void lstHue_DoubleClick( object sender, EventArgs e )
        {
            HuePickerControl.ValueChangedEventHandler valueChanged = this.ValueChanged;
            valueChanged?.Invoke( this.mHue );
        }

        private void lstHue_DrawItem( object sender, DrawItemEventArgs e )
        {
            Graphics graphics1 = e.Graphics;
            graphics1.FillRectangle( Brushes.White, e.Bounds );
            if ( ( e.State & DrawItemState.Selected ) > DrawItemState.None )
            {
                Rectangle rect = new Rectangle( e.Bounds.X, e.Bounds.Y, 50, this._lstHue.ItemHeight );
                graphics1.FillRectangle( SystemBrushes.Highlight, rect );
            }
            else
            {
                Rectangle rect = new Rectangle( e.Bounds.X, e.Bounds.Y, 50, this._lstHue.ItemHeight );
                graphics1.FillRectangle( SystemBrushes.Window, rect );
            }
            float num1 = ( e.Bounds.Width - 35 ) / 32f;
            Hue hue = (Hue) this._lstHue.Items[e.Index];
            Graphics graphics2 = graphics1;
            string s = hue.Index.ToString();
            Font font = e.Font;
            Brush black = Brushes.Black;
            Rectangle bounds1 = e.Bounds;
            double num2 = bounds1.X + 3;
            bounds1 = e.Bounds;
            double y1 = bounds1.Y;
            graphics2.DrawString( s, font, black, (float) num2, (float) y1 );
            int num3 = 0;
            foreach ( short color in hue.Colors )
            {
                Rectangle bounds2 = e.Bounds;
                int x = bounds2.X + 35 + (int) Math.Round( num3 * (double) num1 );
                bounds2 = e.Bounds;
                int y2 = bounds2.Y;
                int width = (int) Math.Round( num1 + 1.0 );
                bounds2 = e.Bounds;
                int height = bounds2.Height;
                Rectangle rect = new Rectangle( x, y2, width, height );
                graphics1.FillRectangle( new SolidBrush( HuePickerControl.Convert555ToARGB( color ) ), rect );
                ++num3;
            }
        }

        private void lstHue_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.mHue = (Hue) this._lstHue.SelectedItem;
            if ( this.mHue == null )
                return;
            this._StatusBar.Text = Conversions.ToString( this.mHue.Index ) + ": " + this.mHue.Name;
        }

        public delegate void ValueChangedEventHandler( Hue Hue );
    }
}
