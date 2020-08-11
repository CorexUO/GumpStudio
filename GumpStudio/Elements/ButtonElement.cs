// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.ButtonElement
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
    public class ButtonElement : BaseElement, IRunUOExportable
    {
        protected Bitmap Cache;
        protected string mCodeBehind;
        protected int mNormalID;
        protected int mParam;
        protected int mPressedID;
        protected ButtonStateEnum mState;
        protected ButtonTypeEnum mType;

        [Description( "The Type of button. Page buttons change the current page, and Reply buttons return a value to the script." )]
        public ButtonTypeEnum ButtonType
        {
            get => mType;
            set
            {
                mType = value;
                RefreshCache();
            }
        }

        [Editor( typeof( LargeTextPropEditor ), typeof( UITypeEditor ) )]
        [Description( "This contains script code to be executed when this button is pressed. (must be supported by the export plugin)" )]
        public string Code
        {
            get => mCodeBehind;
            set => mCodeBehind = value;
        }

        [Description( "The ID of the image to display when the button is not being pressed." )]
        [Editor( typeof( GumpIDPropEditor ), typeof( UITypeEditor ) )]
        public int NormalID
        {
            get => mNormalID;
            set
            {
                mNormalID = value;
                RefreshCache();
            }
        }

        [Description( "For Page Buttons this represents the page to switch to.  For Reply buttons this represents the value to return to the script." )]
        public int Param
        {
            get => mParam;
            set => mParam = value;
        }

        [Description( "The ID of the image to display when the button is being pressed by the user." )]
        [Editor( typeof( GumpIDPropEditor ), typeof( UITypeEditor ) )]
        public int PressedID
        {
            get => mPressedID;
            set
            {
                mPressedID = value;
                RefreshCache();
            }
        }

        [Description( "Change this to see the button in it's different states." )]
        public ButtonStateEnum State
        {
            get => mState;
            set
            {
                mState = value;
                RefreshCache();
            }
        }

        public override string Type => "Button";

        public ButtonElement()
        {
            mType = ButtonTypeEnum.Reply;
            mState = ButtonStateEnum.Normal;
            mType = ButtonTypeEnum.Reply;
            mState = ButtonStateEnum.Normal;
            mPressedID = 248;
            mNormalID = 247;
            RefreshCache();
        }

        protected ButtonElement( SerializationInfo info, StreamingContext context )
          : base( info, context )
        {
            mType = ButtonTypeEnum.Reply;
            mState = ButtonStateEnum.Normal;
            info.GetInt32( "ButtonElementVersion" );
            mPressedID = info.GetInt32( nameof( PressedID ) );
            mNormalID = info.GetInt32( nameof( NormalID ) );
            mType = (ButtonTypeEnum) info.GetInt32( nameof( Type ) );
            mState = (ButtonStateEnum) info.GetInt32( nameof( State ) );
            mCodeBehind = info.GetString( "CodeBehind" );
            mParam = info.GetInt32( nameof( Param ) );
            RefreshCache();
        }

        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );
            info.AddValue( "ButtonElementVersion", 1 );
            info.AddValue( "PressedID", mPressedID );
            info.AddValue( "NormalID", mNormalID );
            info.AddValue( "Type", (int) mType );
            info.AddValue( "State", (int) mState );
            info.AddValue( "CodeBehind", mCodeBehind );
            info.AddValue( "Param", mParam );
        }

        public override void RefreshCache()
        {
            Cache?.Dispose();
            Cache = mState != ButtonStateEnum.Normal ? Gumps.GetGump( mPressedID ) : Gumps.GetGump( mNormalID );

            if ( Cache == null )
                return;

            mSize = Cache.Size;
        }

        public override void Render( Graphics Target )
        {
            if ( Cache == null )
                RefreshCache();
            Target.DrawImage( Cache, Location );
        }

        public string ToRunUOString()
        {
            string buttonType = ButtonType == ButtonTypeEnum.Page ? "GumpButtonType.Page" : "GumpButtonType.Reply";
            return $"AddButton({X}, {Y}, {NormalID}, {PressedID}, {Name.Replace( " ", "" )}, {buttonType}, {Param});";
        }
    }
}
