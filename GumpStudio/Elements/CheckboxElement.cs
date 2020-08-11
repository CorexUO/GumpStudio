// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.CheckboxElement
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
    public class CheckboxElement : BaseElement, IRunUOExportable
    {
        protected Image Image1Cache;
        protected Image Image2Cache;
        protected bool mChecked;
        protected int mCheckedID;
        protected int mGroupID;
        protected int mUncheckedID;

        [Description( "Sets the initial state of the checkbox." )]
        public virtual bool Checked
        {
            get => mChecked;
            set
            {
                mChecked = value;
                RefreshCache();
            }
        }

        [Editor( typeof( GumpIDPropEditor ), typeof( UITypeEditor ) )]
        public virtual int CheckedID
        {
            get => mCheckedID;
            set
            {
                mCheckedID = value;
                RefreshCache();
            }
        }

        [Description( "The Value of the checkbox returned to the script." )]
        public virtual int Group
        {
            get => mGroupID;
            set => mGroupID = value;
        }

        public override string Type => "Checkbox";

        [Editor( typeof( GumpIDPropEditor ), typeof( UITypeEditor ) )]
        public virtual int UnCheckedID
        {
            get => mUncheckedID;
            set
            {
                mUncheckedID = value;
                RefreshCache();
            }
        }

        public CheckboxElement()
        {
            mUncheckedID = 210;
            mCheckedID = 211;
            RefreshCache();
        }

        public CheckboxElement( SerializationInfo info, StreamingContext context )
          : base( info, context )
        {
            info.GetInt32( "CheckboxVersion" );
            mChecked = info.GetBoolean( nameof( Checked ) );
            mCheckedID = info.GetInt32( nameof( CheckedID ) );
            mUncheckedID = info.GetInt32( "UncheckedID" );
            mGroupID = info.GetInt32( "GroupID" );
            RefreshCache();
        }

        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );
            info.AddValue( "CheckboxVersion", 1 );
            info.AddValue( "Checked", mChecked );
            info.AddValue( "CheckedID", mCheckedID );
            info.AddValue( "UncheckedID", mUncheckedID );
            info.AddValue( "GroupID", mGroupID );
        }

        public override void RefreshCache()
        {
            Image1Cache?.Dispose();

            if ( Image2Cache != null )
            {
                Image1Cache?.Dispose();
            }

            Image1Cache = Gumps.GetGump( mUncheckedID );

            if ( Image1Cache == null )
            {
                UnCheckedID = 210;
            }

            Image2Cache = Gumps.GetGump( mCheckedID );

            if ( Image2Cache == null )
            {
                CheckedID = 211;
            }

            mSize = mChecked ? Image2Cache.Size : Image1Cache.Size;
        }

        public override void Render( Graphics Target )
        {
            if ( Image1Cache == null | Image2Cache == null )
                RefreshCache();

            Target.DrawImage( mChecked ? Image2Cache : Image1Cache, Location );
        }

        public string ToRunUOString()
        {
            string typeText = "AddCheck";

            if ( this is RadioElement )
                typeText = "AddRadio";

            return $"{typeText}({X}, {Y}, {UnCheckedID}, {CheckedID}, {Checked.ToString().ToLower()}, {Name.Replace( " ", "" )});";
        }
    }
}
