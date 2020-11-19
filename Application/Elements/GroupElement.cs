// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.GroupElement
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using Microsoft.VisualBasic.CompilerServices;

namespace GumpStudio.Elements
{
	[Serializable]
	public class GroupElement : BaseElement
	{
		internal ArrayList mElements;
		internal bool mIsBaseWindow;

		public BaseElement[] Elements
		{
			get
			{
				BaseElement[] baseElementArray = null;
				IEnumerator enumerator = null;
				try
				{
					foreach (var mElement in mElements)
					{
						var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(mElement);
						baseElementArray = baseElementArray != null ? (BaseElement[])Utils.CopyArray(baseElementArray, new BaseElement[baseElementArray.Length + 1]) : new BaseElement[1];
						baseElementArray[baseElementArray.Length - 1] = objectValue;
					}
				}
				finally
				{
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
				return baseElementArray;
			}
		}

		public int Items => mElements.Count;

		public override string Type => "Group";

		public IEnumerable<BaseElement> AllElements => GetElementsRecursive().OfType<BaseElement>();

		public GroupElement(GroupElement Parent)
		  : this(Parent, null, null, false)
		{
		}

		protected GroupElement(SerializationInfo info, StreamingContext context)
		  : base(info, context)
		{
			mIsBaseWindow = false;
			info.GetInt32("GroupElementVersion");
			mElements = (ArrayList)info.GetValue(nameof(Elements), typeof(ArrayList));
			mIsBaseWindow = info.GetBoolean("IsBaseWindow");
		}

		public GroupElement(GroupElement Parent, ArrayList Elements, string Name)
		  : this(Parent, Elements, Name, false)
		{
		}

		public GroupElement(GroupElement Parent, ArrayList Elements, string Name, bool IsBaseWindow)
		{
			mIsBaseWindow = false;
			mIsBaseWindow = IsBaseWindow;
			mElements = new ArrayList();
			if (Name != null)
			{
				mName = Name;
			}

			if (Elements != null)
			{
				IEnumerator enumerator = null;
				try
				{
					foreach (var element in Elements)
					{
						mElements.Add((BaseElement)RuntimeHelpers.GetObjectValue(element));
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
			Location = new Point(0, 0);
		}

		public override void AddContextMenus(ref MenuItem GroupMenu, ref MenuItem PositionMenu, ref MenuItem OrderMenu, ref MenuItem MiscMenu)
		{
			base.AddContextMenus(ref GroupMenu, ref PositionMenu, ref OrderMenu, ref MiscMenu);
			if (mParent.GetSelectedElements().Count > 1)
			{
				GroupMenu.MenuItems.Add(new MenuItem("Add Selection to Group", new EventHandler(DoAddMenu)));
			}

			GroupMenu.MenuItems.Add(new MenuItem("Break Group", new EventHandler(DoBreakGroupMenu)));
			MiscMenu.MenuItems.Add(new MenuItem("Export Gumpling", new EventHandler(DoExportGumplingMenu)));
		}

		public virtual void AddElement(BaseElement e)
		{
			if (mElements.Contains(e))
			{
				return;
			}

			if (!mIsBaseWindow)
			{
				Rectangle rectangle;
				if (mElements.Count == 0)
				{
					rectangle = e.Bounds;
				}
				else
				{
					rectangle = Rectangle.Union(Bounds, e.Bounds);
					if (X != rectangle.X | Y != rectangle.Y)
					{
						IEnumerator enumerator = null;
						var dx = X - rectangle.X;
						var dy = Y - rectangle.Y;
						try
						{
							foreach (var mElement in mElements)
							{
								var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(mElement);
								var location = objectValue.Location;
								location.Offset(dx, dy);
								objectValue.Location = location;
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
				Location = rectangle.Location;
				mSize = rectangle.Size;
				var location1 = e.Location;
				location1.X -= rectangle.Location.X;
				location1.Y -= rectangle.Location.Y;
				e.Location = location1;
			}
			mElements.Add(e);
			e.Reparent(this);
			AttachEvents(e);
		}

		public void AttachEvents(BaseElement Element)
		{
			Element.UpdateParent += new BaseElement.UpdateParentEventHandler(RaiseUpdateEvent);
			Element.Repaint += new BaseElement.RepaintEventHandler(RaiseRepaintEvent);
		}

		public void BreakGroup()
		{
			IEnumerator enumerator = null;
			try
			{
				foreach (var mElement in mElements)
				{
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(mElement);
					mParent.AddElement(objectValue);
					objectValue.Selected = true;
					var point = new Point(X + objectValue.X, Y + objectValue.Y);
					objectValue.Location = point;
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			mParent.RemoveElement(this);
		}

		protected override object CloneMe()
		{
			IEnumerator enumerator = null;
			var Parent = (GroupElement)base.CloneMe();
			Parent.mElements = new ArrayList();
			try
			{
				foreach (var mElement in mElements)
				{
					var Element = ((BaseElement)RuntimeHelpers.GetObjectValue(mElement)).Clone();
					Parent.mElements.Add(Element);
					Parent.AttachEvents(Element);
					Element.Reparent(Parent);
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			return Parent;
		}

		public override void DebugDump()
		{
			IEnumerator enumerator = null;
			base.DebugDump();
			try
			{
				foreach (var mElement in mElements)
				{
					((BaseElement)RuntimeHelpers.GetObjectValue(mElement)).DebugDump();
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

		protected void DoAddMenu(object sender, EventArgs e)
		{
			IEnumerator enumerator = null;
			var arrayList = new ArrayList();
			arrayList.AddRange(mParent.GetElements());
			try
			{
				foreach (var obj in arrayList)
				{
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(obj);
					if (objectValue != this & objectValue.Selected)
					{
						AddElement(objectValue);
						mParent.RemoveElement(objectValue);
						objectValue.Selected = false;
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

		protected void DoBreakGroupMenu(object sender, EventArgs e)
		{
			BreakGroup();
			mParent.RaiseUpdateEvent(null, false);
			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected void DoExportGumplingMenu(object sender, EventArgs e)
		{
			try
			{
				var saveFileDialog = new SaveFileDialog
				{
					Filter = "Gumpling|*.gumpling",
					AddExtension = true
				};
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					var mParent = this.mParent;
					this.mParent.RemoveElement(this);
					this.mParent = null;
					var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create);
					new BinaryFormatter().Serialize(fileStream, this);
					fileStream.Close();
					this.mParent = mParent;
					this.mParent.AddElement(this);
				}
				saveFileDialog.Dispose();
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				//int num = (int) Interaction.MsgBox((object) ex.Message, MsgBoxStyle.OkOnly, (object) null);
				MessageBox.Show(ex.Message);
				ProjectData.ClearProjectError();
			}
		}

		public BaseElement GetElementFromPoint(Point p)
		{
			BaseElement baseElement = null;
			IEnumerator enumerator = null;
			try
			{
				foreach (var mElement in mElements)
				{
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(mElement);
					var bounds = objectValue.Bounds;
					bounds.Inflate(7, 7);
					if (bounds.Contains(p))
					{
						baseElement = objectValue;
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
			return baseElement;
		}

		public ArrayList GetElements()
		{
			return mElements;
		}

		public ArrayList GetElementsRecursive()
		{
			IEnumerator enumerator = null;
			var arrayList = new ArrayList();
			try
			{
				foreach (var mElement in mElements)
				{
					var objectValue = RuntimeHelpers.GetObjectValue(mElement);
					if (objectValue is GroupElement)
					{
						arrayList.AddRange(((GroupElement)objectValue).GetElementsRecursive());
					}
					else
					{
						arrayList.Add(RuntimeHelpers.GetObjectValue(objectValue));
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
			return arrayList;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("GroupElementVersion", 1);
			info.AddValue("Elements", mElements);
			info.AddValue("IsBaseWindow", mIsBaseWindow);
		}

		public ArrayList GetSelectedElements()
		{
			IEnumerator enumerator = null;
			var arrayList = new ArrayList();
			try
			{
				foreach (var mElement in mElements)
				{
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(mElement);
					if (objectValue.Selected)
					{
						arrayList.Add(objectValue);
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
			return arrayList;
		}

		public void RecalculateBounds()
		{
			var count = mElements.Count;
			if (count < 1)
			{
				return;
			}

			var a = ((BaseElement)mElements[0]).Bounds;
			if (count >= 2)
			{
				var num = count - 1;
				for (var index = 1; index <= num; ++index)
				{
					a = Rectangle.Union(a, ((BaseElement)mElements[index]).Bounds);
				}
			}
			mSize = a.Size;
			RaiseRepaintEvent(this);
		}

		public override void RefreshCache()
		{
			IEnumerator enumerator = null;
			try
			{
				foreach (var mElement in mElements)
				{
					((BaseElement)RuntimeHelpers.GetObjectValue(mElement)).RefreshCache();
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

		public void RemoveElement(BaseElement e)
		{
			mElements.Remove(e);
			RemoveEvents(e);
		}

		public void RemoveEvents(BaseElement Element)
		{
			Element.UpdateParent -= new BaseElement.UpdateParentEventHandler(RaiseUpdateEvent);
			Element.Repaint -= new BaseElement.RepaintEventHandler(RaiseRepaintEvent);
		}

		public override void Render(Graphics Target)
		{
			IEnumerator enumerator = null;
			Target.TranslateTransform(X, Y);
			try
			{
				foreach (var mElement in mElements)
				{
					((BaseElement)RuntimeHelpers.GetObjectValue(mElement)).Render(Target);
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			Target.TranslateTransform(-X, -Y);
		}
	}
}
