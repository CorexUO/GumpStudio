// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.BaseElement
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using GumpStudio.Plugins;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace GumpStudio.Elements
{
  [Serializable]
  public abstract class BaseElement : ISerializable, ICloneable
  {
    protected static Hashtable mIDs = new Hashtable();
    protected string mComment;
    protected static ArrayList mExtenders;
    protected Point mLocation;
    protected string mName;
    protected GroupElement mParent;
    protected bool mSelected;
    protected Size mSize;

    [Browsable(false)]
    public virtual Rectangle Bounds => new Rectangle(this.mLocation, this.mSize);

    [Description("A user defined comment for this element. UO does not use this.")]
    [MergableProperty(false)]
    [ParenthesizePropertyName(true)]
    public string Comment
    {
      get => this.mComment;
      set => this.mComment = value;
    }

    [Browsable(false)]
    public virtual int Height
    {
      get => this.mSize.Height;
      set
      {
      }
    }

    [MergableProperty(false)]
    public Point Location
    {
      get => this.mLocation;
      set => this.mLocation = value;
    }

    [MergableProperty(false)]
    [ParenthesizePropertyName(true)]
    [Description("A name used to identify the element in the Editor, or in script.  UO does not use this.")]
    public string Name
    {
      get => this.mName;
      set => this.mName = value;
    }

    [Browsable(false)]
    [Description("The group elements that this element belongs to.")]
    public GroupElement Parent => this.mParent;

    [Browsable(false)]
    public bool Selected
    {
      get => this.mSelected;
      set => this.mSelected = value;
    }

    [Browsable(false)]
    [MergableProperty(true)]
    public virtual Size Size
    {
      get => this.mSize;
      set
      {
      }
    }

    [MergableProperty(false)]
    public abstract string Type { get; }

    [Browsable(false)]
    public virtual int Width
    {
      get => this.mSize.Width;
      set
      {
      }
    }

    [Browsable(false)]
    public int X
    {
      get => this.mLocation.X;
      set => this.mLocation.X = value;
    }

    [Browsable(false)]
    public int Y
    {
      get => this.mLocation.Y;
      set => this.mLocation.Y = value;
    }

    [Description("The Z order of this element. Highest is on top.")]
    [MergableProperty(false)]
    public int Z
    {
      get => this.mParent.mElements.IndexOf(this);
      set
      {
        this.mParent.mElements.Remove(this);
        this.mParent.mElements.Insert(value, this);
      }
    }

    public event BaseElement.RepaintEventHandler Repaint;

    public event BaseElement.UpdateParentEventHandler UpdateParent;

    public BaseElement()
    {
      long num1;
      if (BaseElement.mIDs.Contains(this.Type))
      {
        long num2 = Conversions.ToLong(BaseElement.mIDs[this.Type]) + 1L;
        BaseElement.mIDs[this.Type] = num2;
        num1 = num2;
      }
      else
      {
        BaseElement.mIDs.Add(this.Type, 1);
        num1 = 1L;
      }
      this.mName = this.Type + " " + Conversions.ToString(num1);
    }

    public BaseElement(string Name)
    {
      this.mName = Name;
    }

    protected BaseElement(SerializationInfo info, StreamingContext context)
    {
      int int32 = info.GetInt32("BaseElementVersion");
      this.mName = info.GetString(nameof (Name));
      this.mLocation = (Point) info.GetValue(nameof (Location), typeof (Point));
      this.mSize = (Size) info.GetValue(nameof (Size), typeof (Size));
      this.mParent = (GroupElement) info.GetValue(nameof (Parent), typeof (GroupElement));
      if (int32 >= 2)
        this.mComment = info.GetString(nameof (Comment));
      else
        this.mComment = "";
    }

    public virtual void AddContextMenus(ref MenuItem GroupMenu, ref MenuItem PositionMenu, ref MenuItem OrderMenu, ref MenuItem MiscMenu)
    {
      int num = this.mParent.GetElements().Count - 1;
      if (num > 0)
      {
        if (this.Z < num)
        {
          OrderMenu.MenuItems.Add(new MenuItem("Move Front", new EventHandler(this.DoMoveFrontMenu)));
          OrderMenu.MenuItems.Add(new MenuItem("Move First", new EventHandler(this.DoMoveFirstMenu)));
        }
        if (this.Z >= 1)
        {
          OrderMenu.MenuItems.Add(new MenuItem("Move Back", new EventHandler(this.DoMoveBackMenu)));
          OrderMenu.MenuItems.Add(new MenuItem("Move Last", new EventHandler(this.DoMoveLastMenu)));
        }
      }
      if (this.mParent.GetSelectedElements().Count > 1)
      {
        PositionMenu.MenuItems.Add(new MenuItem("Align Lefts", new EventHandler(this.DoAlignLeftsMenu)));
        PositionMenu.MenuItems.Add(new MenuItem("Align Rights", new EventHandler(this.DoAlignRightsMenu)));
        PositionMenu.MenuItems.Add(new MenuItem("Align Tops", new EventHandler(this.DoAlignTopsMenu)));
        PositionMenu.MenuItems.Add(new MenuItem("Align Bottoms", new EventHandler(this.DoAlignBottomsMenu)));
        PositionMenu.MenuItems.Add(new MenuItem("-"));
        PositionMenu.MenuItems.Add(new MenuItem("Center Horizontally", new EventHandler(this.DoAlignCentersMenu)));
        PositionMenu.MenuItems.Add(new MenuItem("Center Vertically", new EventHandler(this.DoAlignMiddlesMenu)));
        if (this.mParent.GetSelectedElements().Count > 2)
        {
          PositionMenu.MenuItems.Add(new MenuItem("-"));
          PositionMenu.MenuItems.Add(new MenuItem("Equalize Vertical Spacing", new EventHandler(this.DoVerticalSpacingMenu)));
          PositionMenu.MenuItems.Add(new MenuItem("Equalize Horizontal Spacing", new EventHandler(this.DoHorizontalSpacingMenu)));
        }
      }
      if (BaseElement.mExtenders == null)
        return;
      IEnumerator enumerator = null;
      try
      {
        foreach (object mExtender in BaseElement.mExtenders)
          ((ElementExtender) RuntimeHelpers.GetObjectValue(mExtender)).AddContextMenus(ref GroupMenu, ref PositionMenu, ref OrderMenu, ref MiscMenu);
      }
      finally
      {
        if (enumerator is IDisposable)
          (enumerator as IDisposable).Dispose();
      }
    }

    [Description("Inserts an element extender into this element type's extendor lsit.")]
    public void AddExtender(ElementExtender Extender)
    {
      if (BaseElement.mExtenders == null)
        BaseElement.mExtenders = new ArrayList();
      if (BaseElement.mExtenders.Contains(Extender))
        return;
      BaseElement.mExtenders.Add(Extender);
    }

    [Description("Creates a copy of this element.")]
    public BaseElement Clone()
    {
      return (BaseElement) this.CloneMe();
    }

    [Description("Creates a copy of this element.")]
    protected virtual object CloneMe()
    {
      BaseElement baseElement = (BaseElement) this.MemberwiseClone();
      baseElement.RefreshCache();
      this.RefreshCache();
      return baseElement;
    }

    public virtual bool ContainsTest(Rectangle Rect)
    {
      return Rect.IntersectsWith(this.Bounds);
    }

    public virtual void DebugDump()
    {
    }

    public virtual bool DispayInAbout()
    {
      return false;
    }

    protected virtual void DoAlignBottomsMenu(object sender, EventArgs e)
    {
      IEnumerator enumerator = null;
      try
      {
        foreach (object selectedElement in this.mParent.GetSelectedElements())
        {
          BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue(selectedElement);
          objectValue.Y = this.Y + this.Height - objectValue.Height;
        }
      }
      finally
      {
        if (enumerator is IDisposable)
          (enumerator as IDisposable).Dispose();
      }
      GlobalObjects.DesignerForm.CreateUndoPoint();
    }

    protected virtual void DoAlignCentersMenu(object sender, EventArgs e)
    {
      IEnumerator enumerator = null;
      try
      {
        foreach (object selectedElement in this.mParent.GetSelectedElements())
        {
          object objectValue = RuntimeHelpers.GetObjectValue(selectedElement);
          if (objectValue != this)
          {
            BaseElement baseElement = (BaseElement) objectValue;
            baseElement.X = (int) Math.Round(this.X + this.Width / 2.0 - baseElement.Width / 2.0);
          }
        }
      }
      finally
      {
        if (enumerator is IDisposable)
          (enumerator as IDisposable).Dispose();
      }
      GlobalObjects.DesignerForm.CreateUndoPoint();
    }

    protected virtual void DoAlignLeftsMenu(object sender, EventArgs e)
    {
      IEnumerator enumerator = null;
      try
      {
        foreach (object selectedElement in this.mParent.GetSelectedElements())
          ((BaseElement) RuntimeHelpers.GetObjectValue(selectedElement)).X = this.X;
      }
      finally
      {
        if (enumerator is IDisposable)
          (enumerator as IDisposable).Dispose();
      }
      GlobalObjects.DesignerForm.CreateUndoPoint();
    }

    protected virtual void DoAlignMiddlesMenu(object sender, EventArgs e)
    {
      IEnumerator enumerator = null;
      try
      {
        foreach (object selectedElement in this.mParent.GetSelectedElements())
        {
          object objectValue = RuntimeHelpers.GetObjectValue(selectedElement);
          if (objectValue != this)
          {
            BaseElement baseElement = (BaseElement) objectValue;
            baseElement.Y = (int) Math.Round(this.Y + this.Height / 2.0 - baseElement.Height / 2.0);
          }
        }
      }
      finally
      {
        if (enumerator is IDisposable)
          (enumerator as IDisposable).Dispose();
      }
      GlobalObjects.DesignerForm.CreateUndoPoint();
    }

    protected virtual void DoAlignRightsMenu(object sender, EventArgs e)
    {
      IEnumerator enumerator = null;
      try
      {
        foreach (object selectedElement in this.mParent.GetSelectedElements())
        {
          BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue(selectedElement);
          objectValue.X = this.X + this.Width - objectValue.Width;
        }
      }
      finally
      {
        if (enumerator is IDisposable)
          (enumerator as IDisposable).Dispose();
      }
      GlobalObjects.DesignerForm.CreateUndoPoint();
    }

    protected virtual void DoAlignTopsMenu(object sender, EventArgs e)
    {
      IEnumerator enumerator = null;
      try
      {
        foreach (object selectedElement in this.mParent.GetSelectedElements())
          ((BaseElement) RuntimeHelpers.GetObjectValue(selectedElement)).Y = this.Y;
      }
      finally
      {
        if (enumerator is IDisposable)
          (enumerator as IDisposable).Dispose();
      }
      GlobalObjects.DesignerForm.CreateUndoPoint();
    }

    protected virtual void DoDebugDumpMenu(object sender, EventArgs e)
    {
      this.DebugDump();
    }

    protected virtual void DoHorizontalSpacingMenu(object sender, EventArgs e)
    {
      int num1 = 0;
      IEnumerator enumerator1 = null;
      IEnumerator enumerator2 = null;
      ArrayList arrayList = new ArrayList();
      int num2 = int.MaxValue;
      try
      {
        foreach (object selectedElement in this.mParent.GetSelectedElements())
        {
          BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue(selectedElement);
          int num3 = objectValue.Width / 2 + objectValue.X;
          if (num3 < num2)
            num2 = num3;
          if (num3 > num1)
            num1 = num3;
          arrayList.Add(objectValue);
        }
      }
      finally
      {
        if (enumerator1 is IDisposable)
          (enumerator1 as IDisposable).Dispose();
      }
      arrayList.Sort(new BaseElement.ElementHorizontalSorter());
      double num4 = (num1 - num2) / (double) (arrayList.Count - 1);
      double num5 = num2;
      try
      {
        foreach (object obj in arrayList)
        {
          BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue(obj);
          objectValue.X = (int) Math.Round(num5 - objectValue.Width / 2.0);
          num5 += num4;
        }
      }
      finally
      {
        if (enumerator2 is IDisposable)
          (enumerator2 as IDisposable).Dispose();
      }
      this.RaiseRepaintEvent(this);
      GlobalObjects.DesignerForm.CreateUndoPoint();
    }

    protected virtual void DoMoveBackMenu(object sender, EventArgs e)
    {
      this.MoveBack();
    }

    protected virtual void DoMoveFirstMenu(object sender, EventArgs e)
    {
      this.MoveFirst();
    }

    protected virtual void DoMoveFrontMenu(object sender, EventArgs e)
    {
      this.MoveFront();
    }

    protected virtual void DoMoveLastMenu(object sender, EventArgs e)
    {
      this.MoveLast();
    }

    protected virtual void DoVerticalSpacingMenu(object sender, EventArgs e)
    {
      int num1 = 0;
      IEnumerator enumerator1 = null;
      IEnumerator enumerator2 = null;
      ArrayList arrayList = new ArrayList();
      int num2 = int.MaxValue;
      try
      {
        foreach (object selectedElement in this.mParent.GetSelectedElements())
        {
          BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue(selectedElement);
          int num3 = objectValue.Height / 2 + objectValue.Y;
          if (num3 < num2)
            num2 = num3;
          if (num3 > num1)
            num1 = num3;
          arrayList.Add(objectValue);
        }
      }
      finally
      {
        if (enumerator1 is IDisposable)
          (enumerator1 as IDisposable).Dispose();
      }
      arrayList.Sort(new BaseElement.ElementVerticalSorter());
      double num4 = (num1 - num2) / (double) (arrayList.Count - 1);
      double num5 = num2;
      try
      {
        foreach (object obj in arrayList)
        {
          BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue(obj);
          objectValue.Y = (int) Math.Round(num5 - objectValue.Height / 2.0);
          num5 += num4;
        }
      }
      finally
      {
        if (enumerator2 is IDisposable)
          (enumerator2 as IDisposable).Dispose();
      }
      this.RaiseRepaintEvent(this);
      GlobalObjects.DesignerForm.CreateUndoPoint();
    }

    public virtual void DrawBoundingBox(Graphics Target, bool Active)
    {
      Rectangle bounds = this.Bounds;
      bounds.Inflate(3, 3);
      Pen pen = !Active ? new Pen(Color.DarkGray) : new Pen(Color.White);
      Target.DrawRectangle(Pens.Gray, bounds);
      bounds.Inflate(1, 1);
      Target.DrawRectangle(pen, bounds);
      pen.Dispose();
    }

    public virtual string GetAboutText()
    {
      return "You should override GetAboutText() to change this.";
    }

    [Description("Returns the Element's location, taking into account the offset of all parent elements.  Export plugins should use this to get the position for scripts.")]
    public virtual Point GetAbsolutePosition()
    {
      if (this.Parent == null)
        return this.Location;
      Point absolutePosition = this.Parent.GetAbsolutePosition();
      absolutePosition.Offset(this.X, this.Y);
      return absolutePosition;
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("BaseElementVersion", 2);
      info.AddValue("Name", this.mName);
      info.AddValue("Location", this.mLocation);
      info.AddValue("Size", this.mSize);
      info.AddValue("Parent", this.mParent);
      info.AddValue("Comment", this.mComment);
    }

    public virtual MoveModeType HitTest(Point Location)
    {
      Rectangle rectangle = Rectangle.Inflate(this.Bounds, 3, 3);
      MoveModeType moveModeType = MoveModeType.None;
      if (rectangle.Contains(Location))
        moveModeType = MoveModeType.Move;
      return moveModeType;
    }

    public void MoveBack()
    {
      if (this.Z > 0)
        --this.Z;
      GlobalObjects.DesignerForm.CreateUndoPoint("Move back");
    }

    public void MoveFirst()
    {
      this.mParent.mElements.Remove(this);
      this.mParent.mElements.Add(this);
      GlobalObjects.DesignerForm.CreateUndoPoint("Move first");
    }

    public void MoveFront()
    {
      if (this.Z < this.Parent.mElements.Count - 1)
        ++this.Z;
      GlobalObjects.DesignerForm.CreateUndoPoint("Move front");
    }

    public void MoveLast()
    {
      this.Z = 0;
      GlobalObjects.DesignerForm.CreateUndoPoint("Move last");
    }

    public void RaiseRepaintEvent(object sender)
    {
      BaseElement.RepaintEventHandler repaint = this.Repaint;
      if (repaint == null)
        return;
      repaint(RuntimeHelpers.GetObjectValue(sender));
    }

    public void RaiseUpdateEvent(BaseElement Element, bool ClearSelected)
    {
      BaseElement.UpdateParentEventHandler updateParent = this.UpdateParent;
      if (updateParent == null)
        return;
      updateParent(Element, ClearSelected);
    }

    public abstract void RefreshCache();

    public abstract void Render(Graphics Target);

    public void Reparent(GroupElement Parent)
    {
      if (this.mParent != null)
        this.UpdateParent -= new BaseElement.UpdateParentEventHandler(this.mParent.RaiseUpdateEvent);
      this.mParent = Parent;
    }

    internal static void ResetID()
    {
      BaseElement.mIDs = new Hashtable();
    }

    public override string ToString()
    {
      return this.mName;
    }

    object ICloneable.Clone()
    {
      return null;
    }

    [Description("A sorter that will sort elemenets in the ascending order of thier Horizontal center point.")]
    protected class ElementHorizontalSorter : IComparer
    {
      public int Compare(object x, object y)
      {
        BaseElement baseElement1 = (BaseElement) x;
        BaseElement baseElement2 = (BaseElement) y;
        int num1 = (baseElement1.X + baseElement1.Width) / 2;
        int num2 = (baseElement2.X + baseElement2.Width) / 2;
        if (num1 > num2)
          return 1;
        return num1 >= num2 ? 0 : -1;
      }
    }

    [Description("A sorter that will sort elemenets in the ascending order of thier Vertical center point.")]
    protected class ElementVerticalSorter : IComparer
    {
      public int Compare(object x, object y)
      {
        BaseElement baseElement1 = (BaseElement) x;
        BaseElement baseElement2 = (BaseElement) y;
        int num1 = (baseElement1.Y + baseElement1.Height) / 2;
        int num2 = (baseElement2.Y + baseElement2.Height) / 2;
        if (num1 > num2)
          return 1;
        return num1 >= num2 ? 0 : -1;
      }
    }

    public delegate void RepaintEventHandler(object sender);

    public delegate void UpdateParentEventHandler(BaseElement Element, bool ClearSelected);
  }
}
