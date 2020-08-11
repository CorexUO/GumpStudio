// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.ImageElement
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;
using Ultima;

namespace GumpStudio.Elements
{
    [Serializable]
    public class ImageElement : BaseElement, IRunUOExportable
    {
        protected Bitmap ImageCache;
        protected int mGumpID;
        protected Hue mHue;

        [Editor( typeof( GumpIDPropEditor ), typeof( UITypeEditor ) )]
        public int GumpID
        {
            get => mGumpID;
            set
            {
                mGumpID = value;
                RefreshCache();
            }
        }

        [Editor( typeof( HuePropEditor ), typeof( UITypeEditor ) )]
        [TypeConverter( typeof( HuePropStringConverter ) )]
        [Browsable( true )]
        public Hue Hue
        {
            get => mHue;
            set
            {
                mHue = value;
                RefreshCache();
            }
        }

        public override string Type => "Image";

        public ImageElement()
          : this( 1 )
        {
        }

        public ImageElement( int GumpID )
        {
            mHue = Hues.GetHue( 0 );
            this.GumpID = GumpID;
        }

        public ImageElement( SerializationInfo info, StreamingContext context )
          : base( info, context )
        {
            mHue = Hues.GetHue( 0 );
            int int32 = info.GetInt32( "ImageElementVersion" );
            mGumpID = info.GetInt32( nameof( GumpID ) );
            if ( int32 >= 2 )
                mHue = Hues.GetHue( info.GetInt32( "HueIndex" ) );
            else
                mHue = Hues.GetHue( 0 );
        }

        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );
            info.AddValue( "ImageElementVersion", 2 );
            info.AddValue( "GumpID", mGumpID );
            info.AddValue( "HueIndex", mHue.Index );
        }

        public override void RefreshCache()
        {
            if ( ImageCache != null )
                ImageCache.Dispose();
            ImageCache = Gumps.GetGump( mGumpID );
            if ( ImageCache == null )
                GumpID = 0;
            if ( mHue.Index != 0 )
                mHue.ApplyTo( ImageCache, false );
            mSize = ImageCache.Size;
        }

        public override void Render( Graphics Target )
        {
            if ( ImageCache == null )
                RefreshCache();
            if ( ImageCache != null )
            {
                Target.DrawImage( ImageCache, Location );
            }
            else
            {
                Target.DrawLine( Pens.Red, X, Y, X + 30, Y + 30 );
                Target.DrawLine( Pens.Red, X + 30, Y, X, Y + 30 );
            }
        }

        public string ToRunUOString()
        {
            return $"AddImage({X}, {Y}, {GumpID});";
        }
    }
}
