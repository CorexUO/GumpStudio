// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.RadioElement
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace GumpStudio.Elements
{
    [Serializable]
    public class RadioElement : CheckboxElement
    {
        protected int mValue;

        public override bool Checked
        {
            get => base.Checked;
            set
            {
                mChecked = value;
                if ( !mChecked )
                    return;

                foreach ( object obj in mParent.GetElementsRecursive() )
                {
                    object objectValue = RuntimeHelpers.GetObjectValue( obj );

                    if ( !( objectValue is RadioElement ) )
                    {
                        continue;
                    }

                    RadioElement radioElement = (RadioElement) objectValue;

                    if ( radioElement != this && radioElement.Checked & radioElement.Group == Group )
                    {
                        radioElement.Checked = false;
                    }
                }
            }
        }

        [Description( "The Group that the radio buttons belongs to.  Only one button in a group may be selected at a time." )]
        public override int Group
        {
            get => mGroupID;
            set => mGroupID = value;
        }

        public override string Type => "Radio Button";

        [MergableProperty( false )]
        [Description( "The value fo this radio button returned to the script" )]
        public int Value
        {
            get => mValue;
            set => mValue = value;
        }

        public RadioElement()
        {
            CheckedID = 208;
            UnCheckedID = 209;
        }

        public RadioElement( SerializationInfo info, StreamingContext context )
          : base( info, context )
        {
            if ( info.GetInt32( "RadioElementVersion" ) >= 2 )
                mValue = info.GetInt32( nameof( Value ) );
            RefreshCache();
        }

        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );
            info.AddValue( "RadioElementVersion", 2 );
            info.AddValue( "Value", mValue );
        }
    }
}
