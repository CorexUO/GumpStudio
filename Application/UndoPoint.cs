// Decompiled with JetBrains decompiler
// Type: GumpStudio.UndoPoint
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using GumpStudio.Elements;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GumpStudio.Forms;

namespace GumpStudio
{
  public class UndoPoint
  {
    public GroupElement ElementStack;
    public GumpProperties GumpProperties;
    public ArrayList Stack;
    public string Text;

    public UndoPoint(DesignerForm Designer)
    {
      IEnumerator enumerator = null;
      this.Stack = new ArrayList();
      this.GumpProperties = (GumpProperties) Designer.GumpProperties.Clone();
      try
      {
        foreach (object stack in Designer.Stacks)
        {
          GroupElement objectValue = (GroupElement) RuntimeHelpers.GetObjectValue(stack);
          GroupElement groupElement = (GroupElement) objectValue.Clone();
          this.Stack.Add(groupElement);
          if (objectValue == Designer.ElementStack)
            this.ElementStack = groupElement;
        }
      }
      finally
      {
        if (enumerator is IDisposable)
          (enumerator as IDisposable).Dispose();
      }
    }
  }
}
