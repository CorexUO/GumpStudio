// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.TiledElement
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;
using System.Windows.Forms;
using GumpStudio.Properties;
using Ultima;

namespace GumpStudio.Elements
{
    [Serializable]
    public class TiledElement : ResizeableElement, IRunUOExportable
    {
        protected bool DoingRenderRetry;
        protected Bitmap ImageCache;
        protected int mGumpID;
        protected Hue mHue;
        protected Size mTileSize;

        [Editor( typeof( GumpIDPropEditor ), typeof( UITypeEditor ) )]
        public virtual int GumpID
        {
            get => mGumpID;
            set
            {
                mGumpID = value;
                RefreshCache();
            }
        }

        [Editor( typeof( HuePropEditor ), typeof( UITypeEditor ) )]
        [Browsable( true )]
        [TypeConverter( typeof( HuePropStringConverter ) )]
        public Hue Hue
        {
            get => mHue;
            set
            {
                mHue = value;
                RefreshCache();
            }
        }

        [Description( "The size of the image being tiled" )]
        public Size TileSize => mTileSize;

        public override string Type => "Tiled Image";

        public TiledElement() : this( 30089 )
        {
            GumpID = 30089;
            RefreshCache();
        }

        public TiledElement( int gumpID )
        {
            DoingRenderRetry = false;
            mHue = Hues.GetHue( 0 );
            GumpID = gumpID;
            mSize = mTileSize;
        }

        public TiledElement( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
            DoingRenderRetry = false;
            mHue = Hues.GetHue( 0 );
            int int32 = info.GetInt32( "TiledElementVersion" );
            GumpID = info.GetInt32( nameof( GumpID ) );
            mHue = int32 < 2 ? Hues.GetHue( 0 ) : Hues.GetHue( info.GetInt32( "HueIndex" ) );
            RefreshCache();
        }

        public string ToRunUOString()
        {
            return $"AddImageTiled({X}, {Y}, {Width}, {Height}, {GumpID});";
        }

        public override void AddContextMenus( ref MenuItem GroupMenu, ref MenuItem PositionMenu, ref MenuItem OrderMenu, ref MenuItem MiscMenu )
        {
            base.AddContextMenus( ref GroupMenu, ref PositionMenu, ref OrderMenu, ref MiscMenu );

            if ( PositionMenu.MenuItems.Count > 1 )
            {
                PositionMenu.MenuItems.Add( new MenuItem( "-" ) );
            }

            PositionMenu.MenuItems.Add( new MenuItem( Resources.Reset_Size, DoResetSizeMenu ) );
        }

        protected virtual void DoResetSizeMenu( object sender, EventArgs e )
        {
            mSize = mTileSize;
            RaiseUpdateEvent( this, false );
            GlobalObjects.DesignerForm.CreateUndoPoint();
        }

        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );
            info.AddValue( "TiledElementVersion", 2 );
            info.AddValue( "GumpID", mGumpID );
            info.AddValue( "HueIndex", mHue.Index );
        }

        public override void RefreshCache()
        {
            ImageCache?.Dispose();

            ImageCache = Gumps.GetGump( mGumpID );

            if ( ImageCache == null )
            {
                GumpID = 0;
            }

            if ( mHue.Index != 0 )
            {
                mHue.ApplyTo( ImageCache, false );
            }

            if ( ImageCache != null )
            {
                mTileSize = ImageCache.Size;
            }
        }

        public override void Render( Graphics Target )
        {
            if ( ImageCache != null )
            {
                Region clip = Target.Clip;
                Region region = new Region( Bounds );
                Target.Clip = region;
                int width1 = mTileSize.Width;
                int width2 = Width;
                int dx = 0;

                while ( ( ( width1 >> 31 ) ^ dx ) <= ( ( width1 >> 31 ) ^ width2 ) )
                {
                    int height1 = mTileSize.Height;
                    int height2 = Height;
                    int dy = 0;

                    while ( ( ( height1 >> 31 ) ^ dy ) <= ( ( height1 >> 31 ) ^ height2 ) )
                    {
                        Point location = Location;
                        location.Offset( dx, dy );
                        Target.DrawImage( ImageCache, location );
                        dy += height1;
                    }

                    dx += width1;
                }

                Target.Clip = clip;
                region.Dispose();
            }
            else if ( !DoingRenderRetry )
            {
                DoingRenderRetry = true;
                GumpID = mGumpID;
                Render( Target );
            }
            else
            {
                Graphics graphics1 = Target;
                Pen red1 = Pens.Red;
                int x1 = Location.X;
                int y1 = Location.Y;
                Point location = Location;
                int x2 = location.X + Size.Width;
                location = Location;
                int y2_1 = location.Y + Size.Height;
                graphics1.DrawLine( red1, x1, y1, x2, y2_1 );
                Graphics graphics2 = Target;
                Pen red2 = Pens.Red;
                location = Location;
                int x1_1 = location.X + Size.Width;
                location = Location;
                int y2 = location.Y;
                location = Location;
                int x3 = location.X;
                location = Location;
                int y2_2 = location.Y + Size.Height;
                graphics2.DrawLine( red2, x1_1, y2, x3, y2_2 );
            }
        }
    }
}