// Decompiled with JetBrains decompiler
// Type: GumpStudio.NewStaticArtBrowser
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using GumpStudio.Properties;
using Ultima;
// ReSharper disable RedundantNameQualifier

namespace GumpStudio
{
    public class StaticArtBrowser : Form
    {
        private bool SearchSomething;
        private Button _cmdCache;
        private Button _cmdSearch;
        private Label _lblID;
        private Label _lblName;
        private Label _lblSize;
        private Label _lblWait;
        private PictureBox _picCanvas;
        private ToolTip _ToolTip1;
        private TextBox _txtSearch;
        private VScrollBar _vsbScroller;
        protected static Bitmap BlankCache;
        protected bool BuildingCache;
        protected static GumpCacheEntry[] Cache;
        private IContainer components;
        protected Size DisplaySize;
        protected Point HoverPos;
        protected int NumX;
        protected int NumY;
        protected static Bitmap[] RowCache;
        protected int SelectedIndex;
        protected int StartIndex;

        public int ItemID
        {
            get => Cache[SelectedIndex].ID;
            set
            {
                if ( Cache == null )
                    return;

                for ( int index = 0 ; index < Cache.Length ; index++ )
                {
                    if ( Cache[index].ID != value )
                    {
                        continue;
                    }

                    SelectedIndex = index;
                    _lblName.Text = Resources.Name__ + TileData.ItemTable[ItemID].Name;
                    _lblSize.Text = Resources.Size__ + Conversions.ToString( Art.GetStatic( ItemID ).Width ) + " x " + Conversions.ToString( Art.GetStatic( ItemID ).Height );
                }
            }
        }

        internal virtual ToolTip ToolTip1
        {

            get => _ToolTip1;
            [DebuggerNonUserCode, MethodImpl( MethodImplOptions.Synchronized )]
            set => _ToolTip1 = value;
        }

        internal virtual TextBox txtSearch
        {

            get => _txtSearch;
            [DebuggerNonUserCode, MethodImpl( MethodImplOptions.Synchronized )]
            set => _txtSearch = value;
        }

        internal virtual VScrollBar vsbScroller
        {

            get => _vsbScroller;
            [DebuggerNonUserCode, MethodImpl( MethodImplOptions.Synchronized )]
            set
            {
                ScrollEventHandler scrollEventHandler = new ScrollEventHandler( vsbScroller_Scroll );
                if ( _vsbScroller != null )
                    _vsbScroller.Scroll -= scrollEventHandler;
                _vsbScroller = value;
                if ( _vsbScroller == null )
                    return;
                _vsbScroller.Scroll += scrollEventHandler;
            }
        }

        public StaticArtBrowser()
        {
            Load += new EventHandler( NewStaticArtBrowser_Load );
            Resize += new EventHandler( NewStaticArtBrowser_Resize );
            DisplaySize = new Size( 45, 45 );
            HoverPos = new Point( -1, -1 );
            SelectedIndex = 0;
            BuildingCache = false;
            InitializeComponent();
        }

        protected void BuildCache()
        {
            if ( BuildingCache )
                return;

            BuildingCache = true;

            _lblWait.Text = Resources.Generating_static_art_cache;

            Show();

            FileStream fileStream = null;

            try
            {
                Cache = null;

                _lblWait.Visible = true;

                Application.DoEvents();

                int upperBound = TileData.ItemTable.GetUpperBound( 0 );

                for ( int index = 0 ; index <= upperBound ; ++index )
                {
                    if ( index / 128.0 == Conversion.Int( index / 128.0 ) )
                    {
                        _lblWait.Text = Resources.Generating_static_art_cache + Strings.Format( index / (double) TileData.ItemTable.GetUpperBound( 0 ) * 100.0, "Fixed" ) + "%";
                        Application.DoEvents();
                    }

                    Bitmap bitmap = Art.GetStatic( index );

                    if ( bitmap == null )
                    {
                        continue;
                    }

                    Cache = Cache != null ? (GumpCacheEntry[]) Utils.CopyArray( Cache, new GumpCacheEntry[Cache.Length + 1] ) : new GumpCacheEntry[1];
                    Cache[Cache.Length - 1] = new GumpCacheEntry { ID = index, Size = bitmap.Size, Name = TileData.ItemTable[index].Name };
                }

                fileStream = new FileStream( Application.StartupPath + "/StaticArt.cache", FileMode.Create );
                new BinaryFormatter().Serialize( fileStream, Cache );
            }
            catch ( Exception ex )
            {
                ProjectData.SetProjectError( ex );
                MessageBox.Show( Resources.Error_creating_cache + ex.Message );
                ProjectData.ClearProjectError();
            }
            finally
            {
                fileStream?.Close();
                _lblWait.Visible = false;
                Application.DoEvents();
                BuildingCache = false;
            }
        }

        private void cmdCache_Click( object sender, EventArgs e )
        {
            DialogResult result = MessageBox.Show( Resources.Rebuild_longtime, Resources.Question, MessageBoxButtons.OKCancel, MessageBoxIcon.Question );

            if ( result != DialogResult.OK )
                return;

            BuildCache();
            RowCache = new Bitmap[(int) Math.Round( Cache.Length / (double) NumX ) + 1 + 1];
            ItemID = 0;
            _picCanvas.Invalidate();
        }

        private void cmdSearch_Click( object sender, EventArgs e )
        {
            int index1 = -1;
            int index2 = SelectedIndex == -1 ? 0 : SelectedIndex;
            while ( index1 == -1 & index2 < Cache.Length - 1 )
            {
                ++index2;
                if ( Strings.InStr( Cache[index2].Name, txtSearch.Text, CompareMethod.Text ) > 0 )
                    index1 = index2;
            }
            if ( index1 != -1 )
                ItemID = Cache[index1].ID;
            if ( index1 == -1 & index2 > 0 && !SearchSomething )
            {
                SelectedIndex = 0;
                SearchSomething = true;
                cmdSearch_Click( RuntimeHelpers.GetObjectValue( sender ), e );
            }
            vsbScroller.Value = SelectedIndex / NumX;
            vsbScroller_Scroll( vsbScroller, null );
            SearchSomething = false;
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
                components?.Dispose();
            base.Dispose( disposing );
        }

        protected void DrawGrid( Graphics g )
        {
            int numX = NumX;
            for ( int index = 0 ; index <= numX ; ++index )
                g.DrawLine( Pens.Black, index * DisplaySize.Width, 0, index * DisplaySize.Width, ( NumY + 1 ) * DisplaySize.Height );
            int num = NumY + 1;
            for ( int index = 0 ; index <= num ; ++index )
                g.DrawLine( Pens.Black, 0, index * DisplaySize.Height, NumX * DisplaySize.Width, index * DisplaySize.Height );
        }

        protected void DrawHover( Graphics g )
        {
            int x = HoverPos.X;
            int y = HoverPos.Y;
            int index = StartIndex + x + y * NumX;
            if ( index >= Cache.Length )
                return;
            int id = Cache[index].ID;
            Bitmap bitmap = Art.GetStatic( id );
            Point point = new Point();
            point.X = (int) Math.Round( x * DisplaySize.Width + DisplaySize.Width / 2.0 ) - (int) Math.Round( bitmap.Width / 2.0 ) - 3;
            if ( point.X < 0 )
                point.X = 0;
            if ( point.X + bitmap.Width > _picCanvas.Width )
                point.X = _picCanvas.Width - bitmap.Width - 3;
            point.Y = (int) Math.Round( y * DisplaySize.Height + DisplaySize.Height / 2.0 ) - (int) Math.Round( bitmap.Height / 2.0 ) - 3;
            if ( point.Y < 0 )
                point.Y = 0;
            if ( point.Y + bitmap.Height > _picCanvas.Height )
                point.Y = _picCanvas.Height - bitmap.Height - 3;
            Rectangle rect = new Rectangle( point, bitmap.Size );
            SolidBrush solidBrush = new SolidBrush( Color.FromArgb( sbyte.MaxValue, Color.Black ) );
            g.FillRectangle( solidBrush, point.X + 5, point.Y + 5, rect.Width, rect.Height );
            g.FillRectangle( Brushes.White, rect );
            g.DrawRectangle( Pens.Black, rect );
            g.DrawImage( bitmap, point );
            _lblName.Text = Resources.Name__ + TileData.ItemTable[id].Name;
            _lblSize.Text = Resources.Size__ + Conversions.ToString( bitmap.Width ) + " x " + Conversions.ToString( bitmap.Height );
            _lblID.Text = @"ID: " + Conversions.ToString( id ) + @" - hex:" + Conversion.Hex( id );
            bitmap.Dispose();
            solidBrush.Dispose();
        }

        protected Bitmap GetRowImage( int Row )
        {
            if ( Row >= RowCache.Length )
            {
                if ( BlankCache != null )
                    return BlankCache;
                Bitmap bitmap = new Bitmap( NumX * DisplaySize.Width, DisplaySize.Height, PixelFormat.Format16bppRgb565 );
                Graphics graphics = Graphics.FromImage( bitmap );
                graphics.Clear( Color.Gray );
                graphics.Dispose();
                BlankCache = bitmap;
                return bitmap;
            }
            if ( RowCache[Row] != null )
                return RowCache[Row];
            Bitmap bitmap1 = new Bitmap( NumX * DisplaySize.Width, DisplaySize.Height, PixelFormat.Format16bppRgb565 );
            Graphics graphics1 = Graphics.FromImage( bitmap1 );
            graphics1.Clear( Color.Gray );
            Region clip = graphics1.Clip;
            Rectangle rect = new Rectangle( 0, 0, NumX * DisplaySize.Width, NumY * DisplaySize.Height );
            Region region1 = new Region( rect );
            graphics1.Clip = region1;
            int num = NumX - 1;
            for ( int index1 = 0 ; index1 <= num ; ++index1 )
            {
                int index2 = Row * NumX + index1;
                if ( index2 < Cache.Length )
                {
                    Bitmap bitmap2 = Art.GetStatic( Cache[index2].ID );
                    rect = new Rectangle( index1 * DisplaySize.Width, 0, DisplaySize.Width, DisplaySize.Height );
                    Region region2 = new Region( rect );
                    graphics1.Clip = region2;
                    graphics1.FillRectangle( Brushes.White, index1 * DisplaySize.Width, 0, DisplaySize.Width, DisplaySize.Height );
                    graphics1.DrawImage( bitmap2, index1 * DisplaySize.Width + 1, 0 );
                    bitmap2.Dispose();
                    region2.Dispose();
                }
            }
            graphics1.Clip = clip;
            graphics1.Dispose();
            RowCache[Row] = bitmap1;
            return bitmap1;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._picCanvas = new System.Windows.Forms.PictureBox();
            this._vsbScroller = new System.Windows.Forms.VScrollBar();
            this._txtSearch = new System.Windows.Forms.TextBox();
            this._cmdSearch = new System.Windows.Forms.Button();
            this._lblName = new System.Windows.Forms.Label();
            this._lblSize = new System.Windows.Forms.Label();
            this._cmdCache = new System.Windows.Forms.Button();
            this._lblWait = new System.Windows.Forms.Label();
            this._ToolTip1 = new System.Windows.Forms.ToolTip( this.components );
            this._lblID = new System.Windows.Forms.Label();
            ( (System.ComponentModel.ISupportInitialize) ( this._picCanvas ) ).BeginInit();
            this.SuspendLayout();
            // 
            // _picCanvas
            // 
            this._picCanvas.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
            | System.Windows.Forms.AnchorStyles.Left ) ) );
            this._picCanvas.Location = new System.Drawing.Point( 0, 0 );
            this._picCanvas.Name = "_picCanvas";
            this._picCanvas.Size = new System.Drawing.Size( 488, 396 );
            this._picCanvas.TabIndex = 0;
            this._picCanvas.TabStop = false;
            this._picCanvas.Paint += new System.Windows.Forms.PaintEventHandler( this.picCanvas_Paint );
            this._picCanvas.DoubleClick += new System.EventHandler( this.picCanvas_DoubleClick );
            this._picCanvas.MouseLeave += new System.EventHandler( this.picCanvas_MouseLeave );
            this._picCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler( this.picCanvas_MouseMove );
            this._picCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler( this.picCanvas_MouseUp );
            // 
            // _vsbScroller
            // 
            this._vsbScroller.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
            | System.Windows.Forms.AnchorStyles.Left ) ) );
            this._vsbScroller.Location = new System.Drawing.Point( 488, 0 );
            this._vsbScroller.Name = "_vsbScroller";
            this._vsbScroller.Size = new System.Drawing.Size( 17, 396 );
            this._vsbScroller.TabIndex = 3;
            this._vsbScroller.Scroll += new System.Windows.Forms.ScrollEventHandler( this.vsbScroller_Scroll );
            // 
            // _txtSearch
            // 
            this._txtSearch.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this._txtSearch.Location = new System.Drawing.Point( 56, 403 );
            this._txtSearch.Name = "_txtSearch";
            this._txtSearch.Size = new System.Drawing.Size( 100, 20 );
            this._txtSearch.TabIndex = 4;
            // 
            // _cmdSearch
            // 
            this._cmdSearch.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this._cmdSearch.Location = new System.Drawing.Point( 160, 403 );
            this._cmdSearch.Name = "_cmdSearch";
            this._cmdSearch.Size = new System.Drawing.Size( 50, 20 );
            this._cmdSearch.TabIndex = 5;
            this._cmdSearch.Text = "Search";
            this._cmdSearch.Click += new System.EventHandler( this.cmdSearch_Click );
            // 
            // _lblName
            // 
            this._lblName.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this._lblName.AutoSize = true;
            this._lblName.Location = new System.Drawing.Point( 216, 405 );
            this._lblName.Name = "_lblName";
            this._lblName.Size = new System.Drawing.Size( 38, 13 );
            this._lblName.TabIndex = 6;
            this._lblName.Text = "Name:";
            // 
            // _lblSize
            // 
            this._lblSize.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this._lblSize.AutoSize = true;
            this._lblSize.Location = new System.Drawing.Point( 408, 405 );
            this._lblSize.Name = "_lblSize";
            this._lblSize.Size = new System.Drawing.Size( 30, 13 );
            this._lblSize.TabIndex = 7;
            this._lblSize.Text = "Size:";
            // 
            // _cmdCache
            // 
            this._cmdCache.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this._cmdCache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._cmdCache.Location = new System.Drawing.Point( 0, 402 );
            this._cmdCache.Name = "_cmdCache";
            this._cmdCache.Size = new System.Drawing.Size( 50, 23 );
            this._cmdCache.TabIndex = 9;
            this._cmdCache.Text = "Cache";
            this._ToolTip1.SetToolTip( this._cmdCache, "Rebuild Art Cache" );
            this._cmdCache.Click += new System.EventHandler( this.cmdCache_Click );
            // 
            // _lblWait
            // 
            this._lblWait.BackColor = System.Drawing.Color.Transparent;
            this._lblWait.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lblWait.Font = new System.Drawing.Font( "Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
            this._lblWait.Location = new System.Drawing.Point( 164, 159 );
            this._lblWait.Name = "_lblWait";
            this._lblWait.Size = new System.Drawing.Size( 184, 104 );
            this._lblWait.TabIndex = 10;
            this._lblWait.Text = "Please Wait, Generating Static Art Cache...";
            this._lblWait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._lblWait.Visible = false;
            // 
            // _lblID
            // 
            this._lblID.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this._lblID.AutoSize = true;
            this._lblID.Location = new System.Drawing.Point( 216, 429 );
            this._lblID.Name = "_lblID";
            this._lblID.Size = new System.Drawing.Size( 21, 13 );
            this._lblID.TabIndex = 11;
            this._lblID.Text = "ID:";
            // 
            // StaticArtBrowser
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.ClientSize = new System.Drawing.Size( 505, 451 );
            this.Controls.Add( this._lblID );
            this.Controls.Add( this._lblWait );
            this.Controls.Add( this._cmdCache );
            this.Controls.Add( this._lblSize );
            this.Controls.Add( this._lblName );
            this.Controls.Add( this._cmdSearch );
            this.Controls.Add( this._txtSearch );
            this.Controls.Add( this._vsbScroller );
            this.Controls.Add( this._picCanvas );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximumSize = new System.Drawing.Size( 521, 3000 );
            this.MinimumSize = new System.Drawing.Size( 521, 200 );
            this.Name = "StaticArtBrowser";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Static Art Browser";
            this.Load += new System.EventHandler( this.NewStaticArtBrowser_Load );
            this.Resize += new System.EventHandler( this.NewStaticArtBrowser_Resize );
            ( (System.ComponentModel.ISupportInitialize) ( this._picCanvas ) ).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        private void NewStaticArtBrowser_Load( object sender, EventArgs e )
        {
            if ( Cache == null )
            {
                FileStream fileStream = null;
                if ( !File.Exists( Application.StartupPath + "/StaticArt.cache" ) )
                {
                    BuildCache();
                }
                else
                {
                    try
                    {
                        fileStream = new FileStream( Application.StartupPath + "/StaticArt.cache", FileMode.Open );
                        Cache = (GumpCacheEntry[]) new BinaryFormatter().Deserialize( fileStream );
                    }
                    catch ( Exception ex )
                    {
                        ProjectData.SetProjectError( ex );
                        //int num = (int) Interaction.MsgBox( (object) ( "Error reading cache file:\r\n" + ex.Message ), MsgBoxStyle.OkOnly, (object) null );
                        MessageBox.Show( "Error reading cache file:\r\n" + ex.Message );
                        ProjectData.ClearProjectError();
                    }
                    finally
                    {
                        fileStream?.Close();
                    }
                }
            }
            _picCanvas.Width = ClientSize.Width - vsbScroller.Width;
            Show();
            vsbScroller.Maximum = (int) Math.Round( Cache.Length / (double) NumX ) + 1;
            vsbScroller.LargeChange = NumY - 1;
            if ( RowCache == null )
                RowCache = new Bitmap[(int) Math.Round( Cache.Length / (double) NumX ) + 1 + 1];
            vsbScroller.Value = SelectedIndex / NumX;
            vsbScroller_Scroll( vsbScroller, null );
            _lblName.Text = Resources.Name__ + TileData.ItemTable[Cache[SelectedIndex].ID].Name;
            _lblSize.Text = Resources.Size__ + Conversions.ToString( Cache[SelectedIndex].Size.Width ) + " x " + Conversions.ToString( Cache[SelectedIndex].Size.Height );
        }

        private void NewStaticArtBrowser_Resize( object sender, EventArgs e )
        {
            int num1 = 11;
            int num2 = _picCanvas.Height / DisplaySize.Height;
            if ( !( num1 != NumX | num2 != NumY ) )
                return;
            NumX = num1;
            NumY = num2;
            if ( Cache == null )
                return;
            vsbScroller.Maximum = (int) Math.Round( Cache.Length / (double) NumX ) + 1;
            vsbScroller.LargeChange = NumY - 1;
            _picCanvas.Invalidate();
        }

        private void picCanvas_DoubleClick( object sender, EventArgs e )
        {
            if ( BuildingCache )
                return;
            DialogResult = DialogResult.OK;
        }

        private void picCanvas_MouseLeave( object sender, EventArgs e )
        {
            HoverPos = new Point( -1, -1 );
            _picCanvas.Invalidate();
            _lblName.Text = Resources.Name__ + TileData.ItemTable[Cache[SelectedIndex].ID].Name;
            _lblSize.Text = Resources.Size__ + Conversions.ToString( Cache[SelectedIndex].Size.Width ) + " x " + Conversions.ToString( Cache[SelectedIndex].Size.Height );
            _lblID.Text = "ID: " + Conversions.ToString( Cache[SelectedIndex].ID ) + "(0x" + Conversion.Hex( Cache[SelectedIndex].ID ) + ")";
        }

        private void picCanvas_MouseMove( object sender, MouseEventArgs e )
        {
            Point point = new Point( e.X / DisplaySize.Width, e.Y / DisplaySize.Height );
            if ( point.X >= 11 || !( point.X != HoverPos.X | point.Y != HoverPos.Y ) )
                return;
            HoverPos = point;
            _picCanvas.Invalidate();
        }

        private void picCanvas_MouseUp( object sender, MouseEventArgs e )
        {
            int index = e.X / DisplaySize.Width + e.Y / DisplaySize.Height * NumX + StartIndex;
            if ( index >= Cache.Length )
                return;
            ItemID = Cache[index].ID;
            _picCanvas.Invalidate();
        }

        private void picCanvas_Paint( object sender, PaintEventArgs e )
        {
            try
            {
                Render( e.Graphics );
                if ( HoverPos.Equals( (object) new Point( -1, -1 ) ) )
                    return;
                DrawHover( e.Graphics );
            }
            catch ( Exception ex )
            {
            }
        }

        public void Render( Graphics g )
        {
            if ( Cache == null | RowCache == null )
                return;
            Rectangle rect = new Rectangle();
            g.Clear( Color.Gray );
            DrawGrid( g );
            Region clip = g.Clip;
            int num = StartIndex / NumX;
            bool flag = false;
            int numY = NumY;
            for ( int index = 0 ; index <= numY ; ++index )
            {
                g.DrawImage( GetRowImage( index + num ), 0, index * DisplaySize.Height );
                if ( ( flag || index + num != SelectedIndex / NumX ? 0 : 1 ) != 0 )
                {
                    flag = true;
                    rect = new Rectangle( SelectedIndex % NumX * DisplaySize.Width, index * DisplaySize.Height, DisplaySize.Width, DisplaySize.Height );
                }
            }
            DrawGrid( g );
            if ( flag )
            {
                Region region = new Region( rect );
                rect.Inflate( 5, 5 );
                SolidBrush solidBrush = new SolidBrush( Color.FromArgb( sbyte.MaxValue, Color.Blue ) );
                g.FillRectangle( solidBrush, rect );
                g.DrawRectangle( Pens.Blue, rect );
                solidBrush.Dispose();
                rect.Inflate( -5, -5 );
                g.Clip = region;
                g.DrawImage( Art.GetStatic( Cache[SelectedIndex].ID ), rect.Location );
                g.Clip = clip;
                region.Dispose();
            }
            g.Clip = clip;
        }

        private void vsbScroller_Scroll( object sender, ScrollEventArgs e )
        {
            StartIndex = vsbScroller.Value * NumX;
            _picCanvas.Invalidate();
        }
    }
}
