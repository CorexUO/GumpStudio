// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.TextEntryElement
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;
using Ultima;
using UOFont;
using UnicodeFonts = UOFont.UnicodeFonts;

namespace GumpStudio.Elements
{
    [Serializable]
    public class TextEntryElement : ResizeableElement, IRunUOExportable
    {
        protected Bitmap mCache;
        protected Hue mHue;
        protected int mID;
        protected string mInitialText;
        protected int mMaxLength;

        [TypeConverter( typeof( HuePropStringConverter ) )]
        [Description( "The Hue of the text, Only the right-most color of the Hue is used." )]
        [Browsable( true )]
        [Editor( typeof( HuePropEditor ), typeof( UITypeEditor ) )]
        public Hue Hue
        {
            get => mHue;
            set
            {
                mHue = value;
                RefreshCache();
            }
        }

        [Description( "The ID of this text entry element returned to script." )]
        [MergableProperty( false )]
        public int ID
        {
            get => mID;
            set => mID = value;
        }

        [Description( "The text in the text entry area when the gump is initially opened." )]
        public string InitialText
        {
            get => mInitialText;
            set
            {
                mInitialText = value;
                RefreshCache();
            }
        }

        [Description( "MaxLength sets the maximum number of characters allowed in this TextEntry element. Set to 0 for no limit." )]
        [MergableProperty( true )]
        public int MaxLength
        {
            get => mMaxLength;
            set => mMaxLength = value;
        }

        public override string Type => "Text Entry";

        public TextEntryElement()
        {
            mSize = new Size( 200, 20 );
            mHue = Hues.GetHue( 0 );
        }

        public TextEntryElement( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
            int int32 = info.GetInt32( "TextEntryElementVersion" );
            mInitialText = info.GetString( "Text" );
            mHue = Hues.GetHue( info.GetInt32( "HueIndex" ) );

            if ( int32 >= 2 )
            {
                mID = info.GetInt32( nameof( ID ) );
                mMaxLength = info.GetInt32( nameof( MaxLength ) );
            }

            RefreshCache();
        }

        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );
            info.AddValue( "TextEntryElementVersion", 2 );
            info.AddValue( "Text", mInitialText );
            info.AddValue( "HueIndex", mHue.Index );
            info.AddValue( "ID", mID );
            info.AddValue( "MaxLength", mMaxLength );
        }

        public override void RefreshCache()
        {
            if ( mHue == null )
            {
                mHue = Hues.GetHue( 0 );
            }

            if ( mCache != null )
            {
                mCache.Dispose();
            }

            mCache = UnicodeFonts.GetStringImage( 2, mInitialText + " " );

            if ( ( mHue == null || mHue.Index == 0 ? 0 : 1 ) == 0 )
            {
                return;
            }

            mHue.ApplyTo( mCache, false );
        }

        public override void Render( Graphics Target )
        {
            if ( mCache == null )
            {
                RefreshCache();
            }

            Region clip = Target.Clip;
            Region region = new Region( Bounds );
            Target.Clip = region;
            SolidBrush solidBrush = new SolidBrush( Color.FromArgb( 50, Color.Yellow ) );
            Target.FillRectangle( solidBrush, Bounds );
            Target.DrawImage( mCache, Location );
            solidBrush.Dispose();
            Target.Clip = clip;
            region.Dispose();
        }

        public string ToRunUOString()
        {
            return $"AddTextEntry({X}, {Y}, {Width}, {Height}, {Hue}, {Name.Replace( " ", "" )}, {InitialText.Replace( "\"", "\\\"" )});";
        }
    }
}