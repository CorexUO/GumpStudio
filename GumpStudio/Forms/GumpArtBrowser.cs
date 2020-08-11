// Decompiled with JetBrains decompiler
// Type: GumpStudio.GumpArtBrowser
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Ultima;
// ReSharper disable ArrangeThisQualifier
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantDelegateCreation

namespace GumpStudio
{
    public class GumpArtBrowser : Form
    {
        private Button _cmdCache;
        private Button _cmdOK;
        private Label _lblSize;
        private Label _lblWait;
        private ListBox _lstGump;
        private Panel _Panel1;
        private PictureBox _picFullSize;
        private ToolTip _ToolTip1;
        protected static GumpCacheEntry[] Cache;
        private IContainer components;
        public int GumpID;

        public GumpArtBrowser()
        {
            Load += GumpArtBrowser_Load;
            InitializeComponent();
        }

        protected void BuildCache()
        {
            _lblWait.Text = @"Please Wait, Generating Art Cache...";
            Show();
            Cache = null;
            _lstGump.Items.Clear();
            _lblWait.Visible = true;
            Application.DoEvents();
            int index = 0;
            int maxValue = ushort.MaxValue;
            try
            {
                do
                {
                    _lblWait.Text = $@"Please Wait, Generating Art Cache...  {(int) ( 100 * index / (double) maxValue )}%";
                    Application.DoEvents();
                    Bitmap gump;
                    try
                    {
                        gump = Gumps.GetGump( index );
                    }
                    catch ( Exception )
                    {
                        ++index;

                        return;
                    }
                    if ( gump != null )
                    {
                        if ( Cache != null )
                        {
                            Array.Resize( ref Cache, Cache.Length + 1 );
                        }
                        else
                        {
                            Cache = new GumpCacheEntry[1];
                        }

                        Cache[Cache.Length - 1] = new GumpCacheEntry { ID = index, Size = gump.Size };
                        gump.Dispose();
                    }
                    ++index;
                }
                while ( index <= maxValue );

                using ( FileStream fileStream = new FileStream( Application.StartupPath + "/GumpArt.cache", FileMode.Create ) )
                {
                    new BinaryFormatter().Serialize( fileStream, Cache ?? throw new InvalidOperationException() );
                }
            }
            catch ( Exception ex )
            {
                MessageBox.Show( @"Error creating cache file:" + ex.Message );
            }
            finally
            {
                _lblWait.Visible = false;
                Application.DoEvents();
            }
        }

        private void cmdCache_Click( object sender, EventArgs e )
        {
            _cmdOK.Enabled = false;

            DialogResult result = MessageBox.Show( @"Rebuilding the cache may take several minutes depending on the speed of your computer.\r\nAre you sure you want to continue?", @"Rebuild Cache", MessageBoxButtons.OKCancel, MessageBoxIcon.Information );

            if ( result == DialogResult.OK )
            {
                BuildCache();
                PopulateListbox();
            }
            _cmdOK.Enabled = true;
        }

        private void cmdOK_Click( object sender, EventArgs e )
        {
            GumpID = Convert.ToInt32( _lstGump.SelectedItem );
            DialogResult = DialogResult.OK;
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
                components?.Dispose();
            base.Dispose( disposing );
        }

        private void GumpArtBrowser_Load( object sender, EventArgs e )
        {
            if ( Cache == null )
            {
                FileStream fileStream = null;
                if ( !File.Exists( Application.StartupPath + "/GumpArt.cache" ) )
                {
                    BuildCache();
                }
                else
                {
                    try
                    {
                        fileStream = new FileStream( Application.StartupPath + "/GumpArt.cache", FileMode.Open );
                        Cache = (GumpCacheEntry[]) new BinaryFormatter().Deserialize( fileStream );
                    }
                    catch ( Exception ex )
                    {
                        MessageBox.Show( @"Error Reading cache file:\r\n" + ex.Message );
                    }
                    finally
                    {
                        fileStream?.Close();
                    }
                }
            }
            PopulateListbox();
        }


        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._lstGump = new System.Windows.Forms.ListBox();
            this._Panel1 = new System.Windows.Forms.Panel();
            this._picFullSize = new System.Windows.Forms.PictureBox();
            this._lblSize = new System.Windows.Forms.Label();
            this._lblWait = new System.Windows.Forms.Label();
            this._cmdCache = new System.Windows.Forms.Button();
            this._ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._cmdOK = new System.Windows.Forms.Button();
            this._Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._picFullSize)).BeginInit();
            this.SuspendLayout();
            // 
            // _lstGump
            // 
            this._lstGump.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                                   | System.Windows.Forms.AnchorStyles.Left;
            this._lstGump.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this._lstGump.IntegralHeight = false;
            this._lstGump.Location = new System.Drawing.Point(8, 8);
            this._lstGump.Name = "_lstGump";
            this._lstGump.Size = new System.Drawing.Size(184, 320);
            this._lstGump.TabIndex = 0;
            this._lstGump.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstGump_DrawItem);
            this._lstGump.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lstGump_MeasureItem);
            this._lstGump.SelectedIndexChanged += new System.EventHandler(this.lstGump_SelectedIndexChanged);
            this._lstGump.DoubleClick += new System.EventHandler(this.lstGump_DoubleClick);
            // 
            // _Panel1
            // 
            this._Panel1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                                   | System.Windows.Forms.AnchorStyles.Left) 
                                  | System.Windows.Forms.AnchorStyles.Right;
            this._Panel1.AutoScroll = true;
            this._Panel1.BackColor = System.Drawing.Color.Black;
            this._Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._Panel1.Controls.Add(this._picFullSize);
            this._Panel1.Location = new System.Drawing.Point(200, 8);
            this._Panel1.Name = "_Panel1";
            this._Panel1.Size = new System.Drawing.Size(312, 288);
            this._Panel1.TabIndex = 1;
            // 
            // _picFullSize
            // 
            this._picFullSize.Location = new System.Drawing.Point(0, 0);
            this._picFullSize.Name = "_picFullSize";
            this._picFullSize.Size = new System.Drawing.Size(100, 50);
            this._picFullSize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._picFullSize.TabIndex = 0;
            this._picFullSize.TabStop = false;
            // 
            // _lblSize
            // 
            this._lblSize.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this._lblSize.AutoSize = true;
            this._lblSize.Location = new System.Drawing.Point(200, 307);
            this._lblSize.Name = "_lblSize";
            this._lblSize.Size = new System.Drawing.Size(0, 13);
            this._lblSize.TabIndex = 2;
            // 
            // _lblWait
            // 
            this._lblWait.BackColor = System.Drawing.Color.Transparent;
            this._lblWait.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lblWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this._lblWait.Location = new System.Drawing.Point(168, 131);
            this._lblWait.Name = "_lblWait";
            this._lblWait.Size = new System.Drawing.Size(184, 72);
            this._lblWait.TabIndex = 1;
            this._lblWait.Text = @"Please Wait, Generating Art Cache...";
            this._lblWait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._lblWait.Visible = false;
            // 
            // _cmdCache
            // 
            this._cmdCache.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this._cmdCache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._cmdCache.Location = new System.Drawing.Point(480, 304);
            this._cmdCache.Name = "_cmdCache";
            this._cmdCache.Size = new System.Drawing.Size(32, 23);
            this._cmdCache.TabIndex = 3;
            this._ToolTip1.SetToolTip(this._cmdCache, "Rebuild Cache");
            this._cmdCache.Click += new System.EventHandler(this.cmdCache_Click);
            // 
            // _cmdOK
            // 
            this._cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this._cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._cmdOK.Location = new System.Drawing.Point(400, 304);
            this._cmdOK.Name = "_cmdOK";
            this._cmdOK.Size = new System.Drawing.Size(75, 23);
            this._cmdOK.TabIndex = 4;
            this._cmdOK.Text = @"OK";
            this._cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // GumpArtBrowser
            // 
            this.AcceptButton = this._cmdOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(520, 334);
            this.Controls.Add(this._cmdOK);
            this.Controls.Add(this._cmdCache);
            this.Controls.Add(this._lblWait);
            this.Controls.Add(this._lblSize);
            this.Controls.Add(this._Panel1);
            this.Controls.Add(this._lstGump);
            this.Name = "GumpArtBrowser";
            this.Text = @"GumpID Browser";
            this._Panel1.ResumeLayout(false);
            this._Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._picFullSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void lstGump_DoubleClick( object sender, EventArgs e )
        {
            GumpID = Convert.ToInt32( _lstGump.SelectedItem );
            DialogResult = DialogResult.OK;
        }

        private void lstGump_DrawItem( object sender, DrawItemEventArgs e )
        {
            try
            {
                if ( e.Index == -1 )
                    return;
                Size size1 = new Size();
                Graphics graphics = e.Graphics;
                Bitmap gump = Gumps.GetGump( Cache[e.Index].ID );
                Size size2 = Cache[e.Index].Size;
                size1.Width = size2.Width <= 100 ? size2.Width : 100;
                size1.Height = size2.Height <= 100 ? size2.Height : 100;
                Rectangle rect = new Rectangle( e.Bounds.Location, size1 );
                rect.Offset( 45, 3 );
                graphics.FillRectangle( ( e.State & DrawItemState.Selected ) > DrawItemState.None ? SystemBrushes.Highlight : SystemBrushes.Window, e.Bounds );
                graphics.DrawString( "0x" + Cache[e.Index].ID.ToString( "X" ), Font, SystemBrushes.WindowText, e.Bounds.X, e.Bounds.Y );
                graphics.DrawImage( gump, rect );
                gump.Dispose();
            }
            catch ( Exception ex )
            {
                MessageBox.Show( @"There was an error rendering the gump art, try rebuilding the cache.\r\n\r\n" + ex.Message );
            }
        }

        private void lstGump_MeasureItem( object sender, MeasureItemEventArgs e )
        {
            int height = Cache[e.Index].Size.Height;
            int num = height <= 100 ? ( height >= 15 ? height : 15 ) : 100;
            e.ItemHeight = num + 5;
        }

        private void lstGump_SelectedIndexChanged( object sender, EventArgs e )
        {
            _picFullSize.Image?.Dispose();
            _picFullSize.Image = Gumps.GetGump( Convert.ToInt32( _lstGump.SelectedItem ) );
            _lblSize.Text = @"Width: " + Convert.ToString( _picFullSize.Image.Width ) + @"   Height: " + Convert.ToString( _picFullSize.Image.Height );
        }

        private void PopulateListbox()
        {
            _lstGump.Items.Clear();
            foreach ( GumpCacheEntry gumpCacheEntry in Cache )
                _lstGump.Items.Add( gumpCacheEntry.ID );
            _lstGump.SelectedItem = GumpID;
        }
    }
}
