// Decompiled with JetBrains decompiler
// Type: GumpStudio.UndoPoint
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Collections;
using System.Runtime.CompilerServices;

using GumpStudio.Elements;
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
			Stack = new ArrayList();
			GumpProperties = (GumpProperties)Designer.GumpProperties.Clone();
			try
			{
				foreach (var stack in Designer.Stacks)
				{
					var objectValue = (GroupElement)RuntimeHelpers.GetObjectValue(stack);
					var groupElement = (GroupElement)objectValue.Clone();
					Stack.Add(groupElement);
					if (objectValue == Designer.ElementStack)
					{
						ElementStack = groupElement;
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
		}
	}
}
