// Decompiled with JetBrains decompiler
// Type: GumpStudio.DesignerForm
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using GumpStudio.Elements;
using GumpStudio.Plugins;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Ultima;
// ReSharper disable RedundantDelegateCreation

namespace GumpStudio.Forms
{
    public class DesignerForm : Form
    {
        private TextBox m_CanvasFocus;
        private ComboBox m_cboElements;
        private PropertyGrid m_ElementProperties;
        private Label m_Label1;
        private MainMenu m_MainMenu;
        private MenuItem m_MenuItem1;
        private MenuItem m_MenuItem10;
        private MenuItem m_MenuItem3;
        private MenuItem m_MenuItem4;
        private MenuItem m_MenuItem5;
        private MenuItem m_MenuItem9;
        private ContextMenu m_mnuContextMenu;
        private MenuItem m_mnuCopy;
        private MenuItem _mnuCut;
        private MenuItem _mnuDataFile;
        private MenuItem _mnuDelete;
        private MenuItem _mnuEdit;
        private MenuItem _mnuEditRedo;
        private MenuItem _mnuEditUndo;
        private MenuItem _mnuFile;
        private MenuItem _mnuFileExit;
        private MenuItem _mnuFileExport;
        private MenuItem _mnuFileImport;
        private MenuItem _mnuFileNew;
        private MenuItem _mnuFileOpen;
        private MenuItem _mnuFileSave;
        private MenuItem _mnuGumplingAddFolder;
        private MenuItem _mnuGumplingAddGumpling;
        private ContextMenu _mnuGumplingContext;
        private MenuItem _mnuGumplingDelete;
        private MenuItem _mnuGumplingMove;
        private MenuItem _mnuGumplingRename;
        private MenuItem _mnuHelp;
        private MenuItem _mnuHelpAbout;
        private MenuItem _mnuImportGumpling;
        private MenuItem _mnuMisc;
        private MenuItem _mnuMiscLoadGumpling;
        private MenuItem _mnuPage;
        private MenuItem _mnuPageAdd;
        private MenuItem _mnuPageClear;
        private MenuItem _mnuPageDelete;
        private MenuItem _mnuPageInsert;
        private MenuItem _mnuPaste;
        private MenuItem _mnuPluginManager;
        private MenuItem _mnuPlugins;
        private MenuItem _mnuSelectAll;
        private MenuItem _mnuShow0;
        private OpenFileDialog _OpenDialog;
        private Panel _Panel1;
        private Panel _Panel2;
        private Panel _Panel3;
        private Panel _Panel4;
        private PictureBox _picCanvas;
        private Panel _pnlCanvasScroller;
        private Panel _pnlToolbox;
        private Panel _pnlToolboxHolder;
        private SaveFileDialog _SaveDialog;
        private Splitter _Splitter1;
        private Splitter _Splitter2;
        private StatusBar _StatusBar;
        private TabPage _TabPage1;
        private TabControl _TabPager;
        private TabControl _tabToolbox;
        private TabPage _tpgCustom;
        private TabPage _tpgStandard;
        private TreeView _treGumplings;
        protected string AboutElementAppend;
        public BaseElement ActiveElement;
        public string AppPath;
        public decimal ArrowKeyDelta;
        protected ArrayList AvailablePlugins;
        protected Bitmap Canvas;
        private IContainer components;
        protected ClipBoardMode CopyMode;
        protected int CurrentUndoPoint;
        protected bool ElementChanged;
        public GroupElement ElementStack;
        protected string FileName;
        public TreeFolder GumplingsFolder;
        public TreeFolder GumplingTree;
        public GumpProperties GumpProperties;
        protected Point LastPos;
        protected ArrayList LoadedPlugins;
        protected Point mAnchor;
        protected Size mAnchorOffset;
        public int MaxUndoPoints;
        protected int MoveCount;
        protected MoveModeType MoveMode;
        public bool PluginClearsCanvas;
        public PluginInfo[] PluginTypesToLoad;
        protected ArrayList RegisteredTypes;
        protected Point ScrollPos;
        protected LinearGradientBrush SelBG;
        protected Rectangle SelectionRect;
        protected Pen SelFG;
        public bool ShouldClearActiveElement;
        protected bool ShowGrid;
        protected bool ShowPage0;
        protected bool ShowSelectionRect;
        public ArrayList Stacks;
        public bool SuppressUndoPoints;
        public TreeFolder UncategorizedFolder;
        protected ArrayList UndoPoints;

        public virtual TextBox CanvasFocus
        {

            get => m_CanvasFocus;
            [DebuggerNonUserCode, MethodImpl( MethodImplOptions.Synchronized )]
            set => m_CanvasFocus = value;
        }



        public virtual MenuItem mnuFileExport
        {
            get => _mnuFileExport;
            set => _mnuFileExport = value;
        }

        public virtual MenuItem mnuFileImport
        {
            get => _mnuFileImport;
            set => _mnuFileImport = value;
        }

        public MenuItem mnuPlugins
        {
            get => _mnuPlugins;
            set => _mnuPlugins = value;
        }

        public PictureBox picCanvas
        {
            get => _picCanvas;
            set => _picCanvas = value;
        }

        public event HookKeyDownEventHandler HookKeyDown;
        public event HookPostRenderEventHandler HookPostRender;
        public event HookPreRenderEventHandler HookPreRender;

        public DesignerForm()
        {
            Closed += new EventHandler( DesignerForm_Closed );
            Closing += new CancelEventHandler( DesignerForm_Closing );
            ElementStack = new GroupElement( null, null, "CanvasStack", true );
            Stacks = new ArrayList();
            ShouldClearActiveElement = false;
            PluginClearsCanvas = false;
            AppPath = Application.StartupPath;
            ArrowKeyDelta = new decimal( 1 );
            ShowSelectionRect = false;
            MoveMode = MoveModeType.None;
            ShowGrid = false;
            ShowPage0 = true;
            ElementChanged = false;
            UndoPoints = new ArrayList();
            CurrentUndoPoint = -1;
            MaxUndoPoints = 25;
            SuppressUndoPoints = false;
            RegisteredTypes = new ArrayList();
            AvailablePlugins = new ArrayList();
            LoadedPlugins = new ArrayList();
            InitializeComponent();
            GlobalObjects.DesignerForm = this;
        }

        public void AddElement( BaseElement Element )
        {
            ElementStack.AddElement( Element );
            Element.Selected = true;
            SetActiveElement( Element, true );
            _picCanvas.Invalidate();
            CreateUndoPoint( Element.Name + " added" );
        }

        public int AddPage()
        {
            Stacks.Add( new GroupElement( null, null, "CanvasStack", true ) );
            _TabPager.TabPages.Add( new TabPage( Convert.ToString( Stacks.Count - 1 ) ) );
            _TabPager.SelectedIndex = Stacks.Count - 1;
            ChangeActiveStack( Stacks.Count - 1 );
            return Stacks.Count - 1;
        }

        public void BuildGumplingTree()
        {
            _treGumplings.Nodes.Clear();
            BuildGumplingTree( GumplingTree, null );
        }

        public void BuildGumplingTree( TreeFolder Item, TreeNode Node )
        {
            foreach ( object child in Item.GetChildren() )
            {
                TreeItem objectValue = (TreeItem) RuntimeHelpers.GetObjectValue( child );
                TreeNode treeNode = new TreeNode();
                treeNode.Text = objectValue.Text;
                treeNode.Tag = objectValue;
                if ( Node == null )
                    _treGumplings.Nodes.Add( treeNode );
                else
                    Node.Nodes.Add( treeNode );
                if ( objectValue is TreeFolder )
                    BuildGumplingTree( (TreeFolder) objectValue, treeNode );
            }
        }

        protected void BuildToolbox()
        {
            IEnumerator enumerator1 = null;
            _pnlToolbox.Controls.Clear();
            try
            {
                enumerator1 = RegisteredTypes.GetEnumerator();
                int y = 0;
                while ( enumerator1.MoveNext() )
                {
                    Type objectValue = (Type) RuntimeHelpers.GetObjectValue( enumerator1.Current );
                    BaseElement instance = (BaseElement) Activator.CreateInstance( objectValue );
                    Button button = new Button();
                    button.Text = instance.Type;
                    Point point = new Point( 0, y );
                    button.Location = point;
                    button.FlatStyle = FlatStyle.System;
                    button.Width = _pnlToolbox.Width;
                    button.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    button.Tag = objectValue;
                    y += button.Height - 1;
                    _pnlToolbox.Controls.Add( button );
                    button.Click += new EventHandler( CreateElementFromToolbox );
                    if ( instance.DispayInAbout() )
                        AboutElementAppend = AboutElementAppend + "\r\n\r\n" + instance.Type + ": " + instance.GetAboutText();
                    foreach ( object loadedPlugin in LoadedPlugins )
                        ( (BasePlugin) RuntimeHelpers.GetObjectValue( loadedPlugin ) ).InitializeElementExtenders( instance );
                }
            }
            catch ( Exception ex )
            {
                MessageBox.Show( $"Error\r\n{ex.Message}\n{ex.StackTrace}" );
            }
            finally
            {
                if ( enumerator1 is IDisposable disposable )
                    disposable.Dispose();
            }
            BaseElement.ResetID();
            GumplingTree = new TreeFolder( "Root" );
            GumplingsFolder = new TreeFolder( "My Gumplings" );
            UncategorizedFolder = new TreeFolder( "Uncategorized" );
            GumplingTree.AddItem( GumplingsFolder );
            GumplingTree.AddItem( UncategorizedFolder );
            BuildGumplingTree();
        }

        private void cboElements_Click( object sender, EventArgs e )
        {
            foreach ( object element in ElementStack.GetElements() )
                ( (BaseElement) RuntimeHelpers.GetObjectValue( element ) ).Selected = false;
            ActiveElement = null;
        }

        private void cboElements_SelectedIndexChanged( object sender, EventArgs e )
        {
            SetActiveElement( (BaseElement) m_cboElements.SelectedItem, false );
            _picCanvas.Invalidate();
        }

        protected void ChangeActiveElementEventHandler( BaseElement e, bool DeselectOthers )
        {
            SetActiveElement( e, DeselectOthers );
            _picCanvas.Invalidate();
        }

        public void ChangeActiveStack( int StackID )
        {
            if ( StackID > Stacks.Count - 1 )
                return;
            SetActiveElement( null, true );
            if ( ElementStack != null )
            {
                ElementStack.UpdateParent -= new BaseElement.UpdateParentEventHandler( ChangeActiveElementEventHandler );
                ElementStack.Repaint -= new BaseElement.RepaintEventHandler( RefreshView );
            }
            ElementStack = (GroupElement) Stacks[StackID];
            ElementStack.UpdateParent += new BaseElement.UpdateParentEventHandler( ChangeActiveElementEventHandler );
            ElementStack.Repaint += new BaseElement.RepaintEventHandler( RefreshView );
            _picCanvas.Invalidate();
        }

        public void ClearContextMenu( Menu menu )
        {
            int num = menu.MenuItems.Count - 1;
            for ( int index = 0 ; index <= num ; ++index )
            {
                MenuItem menuItem = menu.MenuItems[0];
                menu.MenuItems.RemoveAt( 0 );
            }
        }

        public void ClearGump()
        {
            _TabPager.TabPages.Clear();
            _TabPager.TabPages.Add( new TabPage( "0" ) );
            Stacks.Clear();
            BaseElement.ResetID();
            ElementStack = new GroupElement( null, null, "Element Stack", true );
            Stacks.Add( ElementStack );
            GumpProperties = new GumpProperties();
            ElementStack.UpdateParent += new BaseElement.UpdateParentEventHandler( ChangeActiveElementEventHandler );
            ElementStack.Repaint += new BaseElement.RepaintEventHandler( RefreshView );
            SetActiveElement( null );
            _picCanvas.Invalidate();
            FileName = "";
            Text = @"Gump Studio (-Unsaved Gump-)";
            ChangeActiveStack( 0 );
            UndoPoints = new ArrayList();
            CreateUndoPoint( "Blank" );
            _mnuEditUndo.Enabled = false;
            _mnuEditRedo.Enabled = false;
        }

        public void Copy()
        {
            ArrayList arrayList = new ArrayList();

            foreach ( object selectedElement in ElementStack.GetSelectedElements() )
                arrayList.Add( ( (BaseElement) RuntimeHelpers.GetObjectValue( selectedElement ) ).Clone() );

            Clipboard.SetDataObject( arrayList );
            CopyMode = ClipBoardMode.Copy;
        }

        public void CreateElementFromToolbox( object sender, EventArgs e )
        {
            AddElement( (BaseElement) Activator.CreateInstance( (Type) ( (Control) sender ).Tag ) );
            _picCanvas.Invalidate();
            _picCanvas.Focus();
        }

        public void CreateUndoPoint()
        {
            CreateUndoPoint( "Unknown Action" );
        }

        public void CreateUndoPoint( string Action )
        {
            if ( SuppressUndoPoints )
                return;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while ( CurrentUndoPoint < UndoPoints.Count - 1 )
            {
                UndoPoint undoPoint = (UndoPoint) UndoPoints[CurrentUndoPoint + 1];
                UndoPoints.RemoveAt( CurrentUndoPoint + 1 );
            }
            UndoPoint undoPoint1 = new UndoPoint( this );
            undoPoint1.Text = Action;
            if ( UndoPoints.Count > MaxUndoPoints )
                UndoPoints.RemoveAt( 0 );
            UndoPoints.Add( undoPoint1 );
            CurrentUndoPoint = UndoPoints.Count - 1;
            _mnuEditUndo.Enabled = true;
            _mnuEditRedo.Enabled = false;
            stopwatch.Stop();
        }

        public void Cut()
        {
            IEnumerator enumerator = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                foreach ( object selectedElement in ElementStack.GetSelectedElements() )
                {
                    BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue( selectedElement );
                    arrayList.Add( objectValue );
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
            Clipboard.SetDataObject( arrayList );
            DeleteSelectedElements();
            CopyMode = ClipBoardMode.Cut;
        }

        protected void DeleteSelectedElements()
        {
            IEnumerator enumerator = null;
            ArrayList arrayList = new ArrayList();
            arrayList.AddRange( ElementStack.GetElements() );
            bool flag = false;
            try
            {
                foreach ( object obj in arrayList )
                {
                    object objectValue = RuntimeHelpers.GetObjectValue( obj );
                    flag = true;
                    BaseElement e = (BaseElement) objectValue;
                    if ( e.Selected )
                        ElementStack.RemoveElement( e );
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
            SetActiveElement( GetLastSelectedControl() );
            _picCanvas.Invalidate();
            if ( !flag )
                return;
            CreateUndoPoint( "Delete Elements" );
        }

        private void DesignerForm_Closed( object sender, EventArgs e )
        {
            SelFG?.Dispose();
            SelBG?.Dispose();
            WritePluginsToLoad();
        }

        private void DesignerForm_Closing( object sender, CancelEventArgs e )
        {
            IEnumerator enumerator = null;
            try
            {
                foreach ( object availablePlugin in AvailablePlugins )
                {
                    BasePlugin objectValue = (BasePlugin) RuntimeHelpers.GetObjectValue( availablePlugin );
                    if ( objectValue.IsLoaded )
                        objectValue.Unload();
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
        }

        private void DesignerForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            XMLSettings.CurrentOptions.DesignerFormSize = WindowState != FormWindowState.Normal ? RestoreBounds.Size : Size;
            XMLSettings.Save( this, XMLSettings.CurrentOptions );
        }

        private void DesignerForm_KeyDown( object sender, KeyEventArgs e )
        {
            HookKeyDownEventHandler hookKeyDown = HookKeyDown;

            hookKeyDown?.Invoke( ActiveControl, ref e );

            if ( e.Handled || ActiveControl != CanvasFocus )
                return;

            bool flag = false;
            if ( ( e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back ? 1 : 0 ) != 0 )
            {
                DeleteSelectedElements();
                e.Handled = true;
                flag = true;
            }
            else if ( e.KeyCode == Keys.Up )
            {
                IEnumerator enumerator = null;
                try
                {
                    foreach ( object selectedElement in ElementStack.GetSelectedElements() )
                    {
                        BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue( selectedElement );
                        Point location = objectValue.Location;
                        location.Offset( 0, -Convert.ToInt32( ArrowKeyDelta ) );
                        objectValue.Location = location;
                    }
                }
                finally
                {
                    ( enumerator as IDisposable )?.Dispose();
                }
                ArrowKeyDelta = Decimal.Multiply( ArrowKeyDelta, new Decimal( 106, 0, 0, false, 2 ) );
                flag = true;
            }
            else if ( e.KeyCode == Keys.Down )
            {
                IEnumerator enumerator = null;
                try
                {
                    foreach ( object selectedElement in ElementStack.GetSelectedElements() )
                    {
                        BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue( selectedElement );
                        Point location = objectValue.Location;
                        location.Offset( 0, Convert.ToInt32( ArrowKeyDelta ) );
                        objectValue.Location = location;
                    }
                }
                finally
                {
                    ( enumerator as IDisposable )?.Dispose();
                }
                ArrowKeyDelta = Decimal.Multiply( ArrowKeyDelta, new Decimal( 106, 0, 0, false, 2 ) );
                flag = true;
            }
            else if ( e.KeyCode == Keys.Left )
            {
                IEnumerator enumerator = null;
                try
                {
                    foreach ( object selectedElement in ElementStack.GetSelectedElements() )
                    {
                        BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue( selectedElement );
                        Point location = objectValue.Location;
                        location.Offset( -Convert.ToInt32( ArrowKeyDelta ), 0 );
                        objectValue.Location = location;
                    }
                }
                finally
                {
                    ( enumerator as IDisposable )?.Dispose();
                }
                ArrowKeyDelta = Decimal.Multiply( ArrowKeyDelta, new Decimal( 106, 0, 0, false, 2 ) );
                flag = true;
            }
            else if ( e.KeyCode == Keys.Right )
            {
                IEnumerator enumerator = null;
                try
                {
                    foreach ( object selectedElement in ElementStack.GetSelectedElements() )
                    {
                        BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue( selectedElement );
                        Point location = objectValue.Location;
                        location.Offset( Convert.ToInt32( ArrowKeyDelta ), 0 );
                        objectValue.Location = location;
                    }
                }
                finally
                {
                    ( enumerator as IDisposable )?.Dispose();
                }
                ArrowKeyDelta = Decimal.Multiply( ArrowKeyDelta, new Decimal( 106, 0, 0, false, 2 ) );
                flag = true;
            }
            else if ( e.KeyCode == Keys.Next )
            {
                int index = ( ActiveElement == null ? ElementStack.GetElements().Count - 1 : ActiveElement.Z ) - 1;
                if ( index < 0 )
                    index = ElementStack.GetElements().Count - 1;
                if ( index >= 0 & index <= ElementStack.GetElements().Count - 1 )
                    SetActiveElement( (BaseElement) ElementStack.GetElements()[index], true );
            }
            else if ( e.KeyCode == Keys.Prior )
            {
                int index = ( ActiveElement == null ? ElementStack.GetElements().Count - 1 : ActiveElement.Z ) + 1;
                if ( index > ElementStack.GetElements().Count - 1 )
                    index = 0;
                SetActiveElement( (BaseElement) ElementStack.GetElements()[index], true );
            }
            if ( Decimal.Compare( ArrowKeyDelta, new Decimal( 10 ) ) > 0 )
                ArrowKeyDelta = new Decimal( 10 );
            if ( !flag )
                return;
            _picCanvas.Invalidate();
            m_ElementProperties.SelectedObjects = m_ElementProperties.SelectedObjects;
        }

        private void DesignerForm_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode != Keys.Up && e.KeyCode != Keys.Down )
            {
                int keyCode = (int) e.KeyCode;
            }
            if ( ( e.KeyCode == Keys.Right ? 1 : 0 ) == 0 )
                return;
            CreateUndoPoint( "Move element" );
            ArrowKeyDelta = new Decimal( 1 );
        }

        private void DesignerForm_Load( object sender, EventArgs e )
        {
            XMLSettings.CurrentOptions = XMLSettings.Load( this );

            if ( !File.Exists( Path.Combine( XMLSettings.CurrentOptions.ClientPath, "art.mul" ) ) && !File.Exists( Path.Combine( XMLSettings.CurrentOptions.ClientPath, "artLegacyMUL.uop" ) ) )
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog { SelectedPath = Environment.SpecialFolder.ProgramFiles.ToString(), Description = @"Select the folder that contains the UO data (.mul) files you want to use." };

                if ( folderBrowserDialog.ShowDialog() == DialogResult.OK )
                {
                    if ( File.Exists( Path.Combine( folderBrowserDialog.SelectedPath, "art.mul" ) ) || File.Exists( Path.Combine( folderBrowserDialog.SelectedPath, "artLegacyMUL.uop" ) ) )
                    {
                        XMLSettings.CurrentOptions.ClientPath = folderBrowserDialog.SelectedPath;
                        XMLSettings.Save( this, XMLSettings.CurrentOptions );
                    }
                    else
                    {
                        MessageBox.Show( @"This path does not contain a file named ""art.mul"", it is most likely not the correct path. Gump Studio can not run without valid client data files.", "Data Files" );
                        Close();
                        return;
                    }
                }
                else
                {
                    Close();
                    return;
                }
            }
            //Client.Directories.Add( XMLSettings.CurrentOptions.ClientPath );
            Files.CacheData = false;
            Files.SetMulPath( XMLSettings.CurrentOptions.ClientPath );
            Size = XMLSettings.CurrentOptions.DesignerFormSize;
            MaxUndoPoints = XMLSettings.CurrentOptions.UndoLevels;
            _picCanvas.Width = 1600;
            _picCanvas.Height = 1200;
            CenterToScreen();
            frmSplash.DisplaySplash();
            EnumeratePlugins();
            Canvas = new Bitmap( _picCanvas.Width, _picCanvas.Height, PixelFormat.Format32bppRgb );
            Activate();
            GumpProperties = new GumpProperties();
            ElementStack.UpdateParent += new BaseElement.UpdateParentEventHandler( ChangeActiveElementEventHandler );
            ElementStack.Repaint += new BaseElement.RepaintEventHandler( RefreshView );
            Stacks.Clear();
            Stacks.Add( ElementStack );
            ChangeActiveStack( 0 );
            RegisteredTypes.Clear();
            RegisteredTypes.Add( typeof( LabelElement ) );
            RegisteredTypes.Add( typeof( ImageElement ) );
            RegisteredTypes.Add( typeof( TiledElement ) );
            RegisteredTypes.Add( typeof( BackgroundElement ) );
            RegisteredTypes.Add( typeof( AlphaElement ) );
            RegisteredTypes.Add( typeof( CheckboxElement ) );
            RegisteredTypes.Add( typeof( RadioElement ) );
            RegisteredTypes.Add( typeof( ItemElement ) );
            RegisteredTypes.Add( typeof( TextEntryElement ) );
            RegisteredTypes.Add( typeof( ButtonElement ) );
            RegisteredTypes.Add( typeof( HTMLElement ) );
            BuildToolbox();
            SelFG = new Pen( Color.Blue, 2f );
            SelBG = new LinearGradientBrush( new Rectangle( 0, 0, 50, 50 ), Color.FromArgb( 90, Color.Blue ), Color.FromArgb( 110, Color.Blue ), LinearGradientMode.ForwardDiagonal );
            SelBG.WrapMode = WrapMode.TileFlipXY;
            CreateUndoPoint( "Blank" );
            _mnuEditUndo.Enabled = false;
        }

        protected void DrawBoundingBox( Graphics Target, BaseElement Element )
        {
            Rectangle bounds = Element.Bounds;
            Target.DrawRectangle( Pens.Red, bounds );
            bounds.Offset( 1, 1 );
            Target.DrawRectangle( Pens.Black, bounds );
        }

        private void ElementProperties_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
        {
            if ( e.ChangedItem.PropertyDescriptor.Name == "Name" )
            {
                m_cboElements.Items.Clear();
                m_cboElements.Items.AddRange( ElementStack.GetElements().ToArray() );
                m_cboElements.SelectedItem = RuntimeHelpers.GetObjectValue( m_ElementProperties.SelectedObject );
            }
            _picCanvas.Invalidate();
            CreateUndoPoint( "Property Changed" );
        }

        protected void EnumeratePlugins()
        {
            if ( !Directory.Exists( Application.StartupPath + "\\Plugins" ) )
                Directory.CreateDirectory( Application.StartupPath + "\\Plugins" );
            PluginTypesToLoad = GetPluginsToLoad();
            foreach ( string file in Directory.GetFiles( Application.StartupPath + "\\Plugins", "*.dll" ) )
            {
                foreach ( Type type in Assembly.LoadFile( file ).GetTypes() )
                {
                    try
                    {
                        if ( type.IsSubclassOf( typeof( BasePlugin ) ) & !type.IsAbstract )
                        {
                            BasePlugin instance = (BasePlugin) Activator.CreateInstance( type );
                            PluginInfo pluginInfo = instance.GetPluginInfo();
                            AboutElementAppend = AboutElementAppend + "\r\n" + pluginInfo.PluginName + ": " + pluginInfo.Description + "\r\nAuthor: " + pluginInfo.AuthorName + "  (" + pluginInfo.AuthorEmail + ")\r\nVersion: " + pluginInfo.Version + "\r\n";
                            AvailablePlugins.Add( instance );
                        }
                        if ( type.IsSubclassOf( typeof( BaseElement ) ) )
                            RegisteredTypes.Add( type );
                    }
                    catch ( Exception ex )
                    {
                        Exception exception = ex;
                        MessageBox.Show( "Error loading plugin: " + type.Name + "(" + file + ")\r\n\r\n" + exception.Message );
                    }
                }
            }
            if ( PluginTypesToLoad == null )
                return;
            foreach ( PluginInfo pluginInfo1 in PluginTypesToLoad )
            {
                IEnumerator enumerator = null;
                try
                {
                    foreach ( object availablePlugin in AvailablePlugins )
                    {
                        BasePlugin objectValue = (BasePlugin) RuntimeHelpers.GetObjectValue( availablePlugin );
                        PluginInfo pluginInfo2 = objectValue.GetPluginInfo();
                        if ( pluginInfo1.Equals( pluginInfo2 ) )
                        {
                            objectValue.Load( this );
                            LoadedPlugins.Add( objectValue );
                        }
                    }
                }
                finally
                {
                    ( enumerator as IDisposable )?.Dispose();
                }
            }
        }

        protected void GetContextMenu( ref BaseElement Element, ContextMenu Menu )
        {
            MenuItem GroupMenu = new MenuItem( "Grouping" );
            MenuItem PositionMenu = new MenuItem( "Positioning" );
            MenuItem OrderMenu = new MenuItem( "Order" );
            MenuItem MiscMenu = new MenuItem( "Misc" );
            MenuItem menuItem = new MenuItem( "Edit" );
            menuItem.MenuItems.Add( new MenuItem( "Cut", new EventHandler( mnuCut_Click ) ) );
            menuItem.MenuItems.Add( new MenuItem( "Copy", new EventHandler( mnuCopy_Click ) ) );
            menuItem.MenuItems.Add( new MenuItem( "Paste", new EventHandler( mnuPaste_Click ) ) );
            menuItem.MenuItems.Add( new MenuItem( "Delete", new EventHandler( mnuDelete_Click ) ) );
            Menu.MenuItems.Add( menuItem );
            Menu.MenuItems.Add( new MenuItem( "-" ) );
            Menu.MenuItems.Add( GroupMenu );
            Menu.MenuItems.Add( PositionMenu );
            Menu.MenuItems.Add( OrderMenu );
            Menu.MenuItems.Add( new MenuItem( "-" ) );
            Menu.MenuItems.Add( MiscMenu );
            if ( ElementStack.GetSelectedElements().Count >= 2 )
                GroupMenu.MenuItems.Add( new MenuItem( "Create Group", new EventHandler( mnuGroupCreate_Click ) ) );

            Element?.AddContextMenus( ref GroupMenu, ref PositionMenu, ref OrderMenu, ref MiscMenu );
            if ( GroupMenu.MenuItems.Count == 0 )
                GroupMenu.Enabled = false;
            if ( PositionMenu.MenuItems.Count == 0 )
                PositionMenu.Enabled = false;
            if ( OrderMenu.MenuItems.Count == 0 )
                OrderMenu.Enabled = false;
            if ( MiscMenu.MenuItems.Count != 0 )
                return;
            MiscMenu.Enabled = false;
        }

        public BaseElement GetLastSelectedControl()
        {
            BaseElement baseElement = null;
            IEnumerator enumerator = null;
            try
            {
                foreach ( object element in ElementStack.GetElements() )
                    baseElement = (BaseElement) RuntimeHelpers.GetObjectValue( element );
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
            return baseElement;
        }

        protected PluginInfo[] GetPluginsToLoad()
        {
            PluginInfo[] pluginInfoArray = null;
            if ( File.Exists( Application.StartupPath + "\\Plugins\\LoadInfo" ) )
            {
                FileStream fileStream = new FileStream( Application.StartupPath + "\\Plugins\\LoadInfo", FileMode.Open );
                pluginInfoArray = (PluginInfo[]) new BinaryFormatter().Deserialize( fileStream );
                fileStream.Close();
            }
            return pluginInfoArray;
        }

        protected Rectangle GetPositiveRect( Rectangle Rect )
        {
            if ( Rect.Height < 0 )
            {
                Rect.Height = Math.Abs( Rect.Height );
                Rect.Y -= Rect.Height;
            }
            if ( Rect.Width < 0 )
            {
                Rect.Width = Math.Abs( Rect.Width );
                Rect.X -= Rect.Width;
            }
            return Rect;
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
                components?.Dispose();
            base.Dispose( disposing );
        }


        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( DesignerForm ) );
            this._pnlToolboxHolder = new System.Windows.Forms.Panel();
            this._Panel4 = new System.Windows.Forms.Panel();
            this._tabToolbox = new System.Windows.Forms.TabControl();
            this._tpgStandard = new System.Windows.Forms.TabPage();
            this._pnlToolbox = new System.Windows.Forms.Panel();
            this._tpgCustom = new System.Windows.Forms.TabPage();
            this._treGumplings = new System.Windows.Forms.TreeView();
            this.m_Label1 = new System.Windows.Forms.Label();
            this._StatusBar = new System.Windows.Forms.StatusBar();
            this._Splitter1 = new System.Windows.Forms.Splitter();
            this._Panel1 = new System.Windows.Forms.Panel();
            this._Panel2 = new System.Windows.Forms.Panel();
            this._pnlCanvasScroller = new System.Windows.Forms.Panel();
            this._picCanvas = new System.Windows.Forms.PictureBox();
            this._TabPager = new System.Windows.Forms.TabControl();
            this._TabPage1 = new System.Windows.Forms.TabPage();
            this._Splitter2 = new System.Windows.Forms.Splitter();
            this._Panel3 = new System.Windows.Forms.Panel();
            this.m_cboElements = new System.Windows.Forms.ComboBox();
            this.m_ElementProperties = new System.Windows.Forms.PropertyGrid();
            this.m_CanvasFocus = new System.Windows.Forms.TextBox();
            this._OpenDialog = new System.Windows.Forms.OpenFileDialog();
            this._SaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.m_mnuContextMenu = new System.Windows.Forms.ContextMenu();
            this.m_MainMenu = new System.Windows.Forms.MainMenu( this.components );
            this._mnuFile = new System.Windows.Forms.MenuItem();
            this._mnuFileNew = new System.Windows.Forms.MenuItem();
            this.m_MenuItem9 = new System.Windows.Forms.MenuItem();
            this._mnuFileOpen = new System.Windows.Forms.MenuItem();
            this._mnuFileSave = new System.Windows.Forms.MenuItem();
            this._mnuFileImport = new System.Windows.Forms.MenuItem();
            this._mnuFileExport = new System.Windows.Forms.MenuItem();
            this.m_MenuItem5 = new System.Windows.Forms.MenuItem();
            this._mnuFileExit = new System.Windows.Forms.MenuItem();
            this._mnuEdit = new System.Windows.Forms.MenuItem();
            this._mnuEditUndo = new System.Windows.Forms.MenuItem();
            this._mnuEditRedo = new System.Windows.Forms.MenuItem();
            this.m_MenuItem3 = new System.Windows.Forms.MenuItem();
            this._mnuCut = new System.Windows.Forms.MenuItem();
            this.m_mnuCopy = new System.Windows.Forms.MenuItem();
            this._mnuPaste = new System.Windows.Forms.MenuItem();
            this._mnuDelete = new System.Windows.Forms.MenuItem();
            this.m_MenuItem4 = new System.Windows.Forms.MenuItem();
            this._mnuSelectAll = new System.Windows.Forms.MenuItem();
            this._mnuMisc = new System.Windows.Forms.MenuItem();
            this._mnuMiscLoadGumpling = new System.Windows.Forms.MenuItem();
            this._mnuImportGumpling = new System.Windows.Forms.MenuItem();
            this._mnuDataFile = new System.Windows.Forms.MenuItem();
            this._mnuPage = new System.Windows.Forms.MenuItem();
            this._mnuPageAdd = new System.Windows.Forms.MenuItem();
            this._mnuPageInsert = new System.Windows.Forms.MenuItem();
            this._mnuPageDelete = new System.Windows.Forms.MenuItem();
            this._mnuPageClear = new System.Windows.Forms.MenuItem();
            this.m_MenuItem10 = new System.Windows.Forms.MenuItem();
            this._mnuShow0 = new System.Windows.Forms.MenuItem();
            this._mnuPlugins = new System.Windows.Forms.MenuItem();
            this._mnuPluginManager = new System.Windows.Forms.MenuItem();
            this._mnuHelp = new System.Windows.Forms.MenuItem();
            this._mnuHelpAbout = new System.Windows.Forms.MenuItem();
            this._mnuGumplingContext = new System.Windows.Forms.ContextMenu();
            this._mnuGumplingRename = new System.Windows.Forms.MenuItem();
            this._mnuGumplingMove = new System.Windows.Forms.MenuItem();
            this._mnuGumplingDelete = new System.Windows.Forms.MenuItem();
            this.m_MenuItem1 = new System.Windows.Forms.MenuItem();
            this._mnuGumplingAddGumpling = new System.Windows.Forms.MenuItem();
            this._mnuGumplingAddFolder = new System.Windows.Forms.MenuItem();
            this._pnlToolboxHolder.SuspendLayout();
            this._Panel4.SuspendLayout();
            this._tabToolbox.SuspendLayout();
            this._tpgStandard.SuspendLayout();
            this._tpgCustom.SuspendLayout();
            this._Panel1.SuspendLayout();
            this._Panel2.SuspendLayout();
            this._pnlCanvasScroller.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize) ( this._picCanvas ) ).BeginInit();
            this._TabPager.SuspendLayout();
            this._Panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pnlToolboxHolder
            // 
            this._pnlToolboxHolder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._pnlToolboxHolder.Controls.Add( this._Panel4 );
            this._pnlToolboxHolder.Controls.Add( this.m_Label1 );
            this._pnlToolboxHolder.Dock = System.Windows.Forms.DockStyle.Left;
            this._pnlToolboxHolder.Location = new System.Drawing.Point( 0, 0 );
            this._pnlToolboxHolder.Name = "_pnlToolboxHolder";
            this._pnlToolboxHolder.Size = new System.Drawing.Size( 128, 685 );
            this._pnlToolboxHolder.TabIndex = 0;
            // 
            // _Panel4
            // 
            this._Panel4.Controls.Add( this._tabToolbox );
            this._Panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this._Panel4.Location = new System.Drawing.Point( 0, 16 );
            this._Panel4.Name = "_Panel4";
            this._Panel4.Size = new System.Drawing.Size( 124, 665 );
            this._Panel4.TabIndex = 1;
            // 
            // _tabToolbox
            // 
            this._tabToolbox.Controls.Add( this._tpgStandard );
            this._tabToolbox.Controls.Add( this._tpgCustom );
            this._tabToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabToolbox.Location = new System.Drawing.Point( 0, 0 );
            this._tabToolbox.Multiline = true;
            this._tabToolbox.Name = "_tabToolbox";
            this._tabToolbox.SelectedIndex = 0;
            this._tabToolbox.Size = new System.Drawing.Size( 124, 665 );
            this._tabToolbox.TabIndex = 1;
            // 
            // _tpgStandard
            // 
            this._tpgStandard.Controls.Add( this._pnlToolbox );
            this._tpgStandard.Location = new System.Drawing.Point( 4, 22 );
            this._tpgStandard.Name = "_tpgStandard";
            this._tpgStandard.Size = new System.Drawing.Size( 116, 639 );
            this._tpgStandard.TabIndex = 0;
            this._tpgStandard.Text = "Standard";
            // 
            // _pnlToolbox
            // 
            this._pnlToolbox.AutoScroll = true;
            this._pnlToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlToolbox.Location = new System.Drawing.Point( 0, 0 );
            this._pnlToolbox.Name = "_pnlToolbox";
            this._pnlToolbox.Size = new System.Drawing.Size( 116, 639 );
            this._pnlToolbox.TabIndex = 1;
            // 
            // _tpgCustom
            // 
            this._tpgCustom.Controls.Add( this._treGumplings );
            this._tpgCustom.Location = new System.Drawing.Point( 4, 22 );
            this._tpgCustom.Name = "_tpgCustom";
            this._tpgCustom.Size = new System.Drawing.Size( 116, 639 );
            this._tpgCustom.TabIndex = 1;
            this._tpgCustom.Text = "Gumplings";
            // 
            // _treGumplings
            // 
            this._treGumplings.Dock = System.Windows.Forms.DockStyle.Fill;
            this._treGumplings.Location = new System.Drawing.Point( 0, 0 );
            this._treGumplings.Name = "_treGumplings";
            this._treGumplings.Size = new System.Drawing.Size( 116, 639 );
            this._treGumplings.TabIndex = 1;
            this._treGumplings.DoubleClick += new System.EventHandler( this.treGumplings_DoubleClick );
            this._treGumplings.MouseUp += new System.Windows.Forms.MouseEventHandler( this.treGumplings_MouseUp );
            // 
            // m_Label1
            // 
            this.m_Label1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.m_Label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_Label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_Label1.Location = new System.Drawing.Point( 0, 0 );
            this.m_Label1.Name = "m_Label1";
            this.m_Label1.Size = new System.Drawing.Size( 124, 16 );
            this.m_Label1.TabIndex = 0;
            this.m_Label1.Text = "Toolbox";
            this.m_Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _StatusBar
            // 
            this._StatusBar.Location = new System.Drawing.Point( 0, 685 );
            this._StatusBar.Name = "_StatusBar";
            this._StatusBar.Size = new System.Drawing.Size( 1350, 23 );
            this._StatusBar.TabIndex = 0;
            // 
            // _Splitter1
            // 
            this._Splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._Splitter1.Location = new System.Drawing.Point( 128, 0 );
            this._Splitter1.MinSize = 80;
            this._Splitter1.Name = "_Splitter1";
            this._Splitter1.Size = new System.Drawing.Size( 3, 685 );
            this._Splitter1.TabIndex = 1;
            this._Splitter1.TabStop = false;
            // 
            // _Panel1
            // 
            this._Panel1.Controls.Add( this._Panel2 );
            this._Panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._Panel1.Location = new System.Drawing.Point( 131, 0 );
            this._Panel1.Name = "_Panel1";
            this._Panel1.Size = new System.Drawing.Size( 1219, 685 );
            this._Panel1.TabIndex = 2;
            // 
            // _Panel2
            // 
            this._Panel2.Controls.Add( this._pnlCanvasScroller );
            this._Panel2.Controls.Add( this._TabPager );
            this._Panel2.Controls.Add( this._Splitter2 );
            this._Panel2.Controls.Add( this._Panel3 );
            this._Panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this._Panel2.Location = new System.Drawing.Point( 0, 0 );
            this._Panel2.Name = "_Panel2";
            this._Panel2.Size = new System.Drawing.Size( 1219, 685 );
            this._Panel2.TabIndex = 0;
            // 
            // _pnlCanvasScroller
            // 
            this._pnlCanvasScroller.AutoScroll = true;
            this._pnlCanvasScroller.AutoScrollMargin = new System.Drawing.Size( 1, 1 );
            this._pnlCanvasScroller.AutoScrollMinSize = new System.Drawing.Size( 1, 1 );
            this._pnlCanvasScroller.BackColor = System.Drawing.Color.Silver;
            this._pnlCanvasScroller.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._pnlCanvasScroller.Controls.Add( this._picCanvas );
            this._pnlCanvasScroller.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlCanvasScroller.Location = new System.Drawing.Point( 0, 24 );
            this._pnlCanvasScroller.Name = "_pnlCanvasScroller";
            this._pnlCanvasScroller.Size = new System.Drawing.Size( 949, 661 );
            this._pnlCanvasScroller.TabIndex = 2;
            this._pnlCanvasScroller.MouseLeave += new System.EventHandler( this.pnlCanvasScroller_MouseLeave );
            // 
            // _picCanvas
            // 
            this._picCanvas.BackColor = System.Drawing.Color.Black;
            this._picCanvas.Location = new System.Drawing.Point( 0, 0 );
            this._picCanvas.Name = "_picCanvas";
            this._picCanvas.Size = new System.Drawing.Size( 1600, 1200 );
            this._picCanvas.TabIndex = 0;
            this._picCanvas.TabStop = false;
            this._picCanvas.Paint += new System.Windows.Forms.PaintEventHandler( this.picCanvas_Paint );
            this._picCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler( this.picCanvas_MouseDown );
            this._picCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler( this.picCanvas_MouseMove );
            this._picCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler( this.picCanvas_MouseUp );
            // 
            // _TabPager
            // 
            this._TabPager.Controls.Add( this._TabPage1 );
            this._TabPager.Dock = System.Windows.Forms.DockStyle.Top;
            this._TabPager.HotTrack = true;
            this._TabPager.Location = new System.Drawing.Point( 0, 0 );
            this._TabPager.Name = "_TabPager";
            this._TabPager.SelectedIndex = 0;
            this._TabPager.Size = new System.Drawing.Size( 949, 24 );
            this._TabPager.TabIndex = 3;
            this._TabPager.SelectedIndexChanged += new System.EventHandler( this.TabPager_SelectedIndexChanged );
            // 
            // _TabPage1
            // 
            this._TabPage1.Location = new System.Drawing.Point( 4, 22 );
            this._TabPage1.Name = "_TabPage1";
            this._TabPage1.Size = new System.Drawing.Size( 941, 0 );
            this._TabPage1.TabIndex = 0;
            this._TabPage1.Text = "0";
            // 
            // _Splitter2
            // 
            this._Splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this._Splitter2.Location = new System.Drawing.Point( 949, 0 );
            this._Splitter2.Name = "_Splitter2";
            this._Splitter2.Size = new System.Drawing.Size( 22, 685 );
            this._Splitter2.TabIndex = 1;
            this._Splitter2.TabStop = false;
            // 
            // _Panel3
            // 
            this._Panel3.Controls.Add( this.m_cboElements );
            this._Panel3.Controls.Add( this.m_ElementProperties );
            this._Panel3.Controls.Add( this.m_CanvasFocus );
            this._Panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this._Panel3.Location = new System.Drawing.Point( 971, 0 );
            this._Panel3.Name = "_Panel3";
            this._Panel3.Size = new System.Drawing.Size( 248, 685 );
            this._Panel3.TabIndex = 0;
            // 
            // m_cboElements
            // 
            this.m_cboElements.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.m_cboElements.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cboElements.Location = new System.Drawing.Point( 0, 8 );
            this.m_cboElements.Name = "m_cboElements";
            this.m_cboElements.Size = new System.Drawing.Size( 240, 21 );
            this.m_cboElements.TabIndex = 1;
            this.m_cboElements.SelectedIndexChanged += new System.EventHandler( this.cboElements_SelectedIndexChanged );
            this.m_cboElements.Click += new System.EventHandler( this.cboElements_Click );
            // 
            // m_ElementProperties
            // 
            this.m_ElementProperties.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
            | System.Windows.Forms.AnchorStyles.Left )
            | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.m_ElementProperties.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.m_ElementProperties.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.m_ElementProperties.Location = new System.Drawing.Point( 0, 32 );
            this.m_ElementProperties.Name = "m_ElementProperties";
            this.m_ElementProperties.Size = new System.Drawing.Size( 240, 651 );
            this.m_ElementProperties.TabIndex = 0;
            this.m_ElementProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler( this.ElementProperties_PropertyValueChanged );
            // 
            // m_CanvasFocus
            // 
            this.m_CanvasFocus.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.m_CanvasFocus.Location = new System.Drawing.Point( 16, 635 );
            this.m_CanvasFocus.Name = "m_CanvasFocus";
            this.m_CanvasFocus.Size = new System.Drawing.Size( 100, 20 );
            this.m_CanvasFocus.TabIndex = 1;
            this.m_CanvasFocus.Text = "TextBox1";
            // 
            // m_MainMenu
            // 
            this.m_MainMenu.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this._mnuFile,
            this._mnuEdit,
            this._mnuMisc,
            this._mnuPage,
            this._mnuPlugins,
            this._mnuHelp} );
            // 
            // _mnuFile
            // 
            this._mnuFile.Index = 0;
            this._mnuFile.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this._mnuFileNew,
            this.m_MenuItem9,
            this._mnuFileOpen,
            this._mnuFileSave,
            this._mnuFileImport,
            this._mnuFileExport,
            this.m_MenuItem5,
            this._mnuFileExit} );
            this._mnuFile.Text = "File";
            // 
            // _mnuFileNew
            // 
            this._mnuFileNew.Index = 0;
            this._mnuFileNew.Text = "New";
            this._mnuFileNew.Click += new System.EventHandler( this.mnuFileNew_Click );
            // 
            // m_MenuItem9
            // 
            this.m_MenuItem9.Index = 1;
            this.m_MenuItem9.Text = "-";
            // 
            // _mnuFileOpen
            // 
            this._mnuFileOpen.Index = 2;
            this._mnuFileOpen.Text = "Open";
            this._mnuFileOpen.Click += new System.EventHandler( this.mnuFileOpen_Click );
            // 
            // _mnuFileSave
            // 
            this._mnuFileSave.Index = 3;
            this._mnuFileSave.Text = "Save";
            this._mnuFileSave.Click += new System.EventHandler( this.mnuFileSave_Click );
            // 
            // _mnuFileImport
            // 
            this._mnuFileImport.Enabled = false;
            this._mnuFileImport.Index = 4;
            this._mnuFileImport.Text = "Import";
            // 
            // _mnuFileExport
            // 
            this._mnuFileExport.Enabled = false;
            this._mnuFileExport.Index = 5;
            this._mnuFileExport.Text = "Export";
            // 
            // m_MenuItem5
            // 
            this.m_MenuItem5.Index = 6;
            this.m_MenuItem5.Text = "-";
            // 
            // _mnuFileExit
            // 
            this._mnuFileExit.Index = 7;
            this._mnuFileExit.Text = "Exit";
            this._mnuFileExit.Click += new System.EventHandler( this.mnuFileExit_Click );
            // 
            // _mnuEdit
            // 
            this._mnuEdit.Index = 1;
            this._mnuEdit.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this._mnuEditUndo,
            this._mnuEditRedo,
            this.m_MenuItem3,
            this._mnuCut,
            this.m_mnuCopy,
            this._mnuPaste,
            this._mnuDelete,
            this.m_MenuItem4,
            this._mnuSelectAll} );
            this._mnuEdit.Text = "Edit";
            // 
            // _mnuEditUndo
            // 
            this._mnuEditUndo.Enabled = false;
            this._mnuEditUndo.Index = 0;
            this._mnuEditUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
            this._mnuEditUndo.Text = "Undo";
            this._mnuEditUndo.Click += new System.EventHandler( this.mnuEditUndo_Click );
            // 
            // _mnuEditRedo
            // 
            this._mnuEditRedo.Enabled = false;
            this._mnuEditRedo.Index = 1;
            this._mnuEditRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
            this._mnuEditRedo.Text = "Redo";
            this._mnuEditRedo.Click += new System.EventHandler( this.mnuEditRedo_Click );
            // 
            // m_MenuItem3
            // 
            this.m_MenuItem3.Index = 2;
            this.m_MenuItem3.Text = "-";
            // 
            // _mnuCut
            // 
            this._mnuCut.Index = 3;
            this._mnuCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this._mnuCut.Text = "Cu&t";
            this._mnuCut.Click += new System.EventHandler( this.mnuCut_Click );
            // 
            // m_mnuCopy
            // 
            this.m_mnuCopy.Index = 4;
            this.m_mnuCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.m_mnuCopy.Text = "&Copy";
            this.m_mnuCopy.Click += new System.EventHandler( this.mnuCopy_Click );
            // 
            // _mnuPaste
            // 
            this._mnuPaste.Index = 5;
            this._mnuPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this._mnuPaste.Text = "&Paste";
            this._mnuPaste.Click += new System.EventHandler( this.mnuPaste_Click );
            // 
            // _mnuDelete
            // 
            this._mnuDelete.Index = 6;
            this._mnuDelete.Text = "Delete";
            this._mnuDelete.Click += new System.EventHandler( this.mnuDelete_Click );
            // 
            // m_MenuItem4
            // 
            this.m_MenuItem4.Index = 7;
            this.m_MenuItem4.Text = "-";
            // 
            // _mnuSelectAll
            // 
            this._mnuSelectAll.Index = 8;
            this._mnuSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this._mnuSelectAll.Text = "Select &All";
            this._mnuSelectAll.Click += new System.EventHandler( this.mnuSelectAll_Click );
            // 
            // _mnuMisc
            // 
            this._mnuMisc.Index = 2;
            this._mnuMisc.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this._mnuMiscLoadGumpling,
            this._mnuImportGumpling,
            this._mnuDataFile} );
            this._mnuMisc.Text = "Misc";
            // 
            // _mnuMiscLoadGumpling
            // 
            this._mnuMiscLoadGumpling.Index = 0;
            this._mnuMiscLoadGumpling.Text = "Load gumpling";
            this._mnuMiscLoadGumpling.Click += new System.EventHandler( this.MenuItem2_Click );
            // 
            // _mnuImportGumpling
            // 
            this._mnuImportGumpling.Index = 1;
            this._mnuImportGumpling.Text = "Import Gumpling";
            this._mnuImportGumpling.Click += new System.EventHandler( this.mnuImportGumpling_Click );
            // 
            // _mnuDataFile
            // 
            this._mnuDataFile.Index = 2;
            this._mnuDataFile.Text = "Data File Path";
            this._mnuDataFile.Click += new System.EventHandler( this.mnuDataFile_Click );
            // 
            // _mnuPage
            // 
            this._mnuPage.Index = 3;
            this._mnuPage.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this._mnuPageAdd,
            this._mnuPageInsert,
            this._mnuPageDelete,
            this._mnuPageClear,
            this.m_MenuItem10,
            this._mnuShow0} );
            this._mnuPage.Text = "Page";
            // 
            // _mnuPageAdd
            // 
            this._mnuPageAdd.Index = 0;
            this._mnuPageAdd.Text = "Add Page";
            this._mnuPageAdd.Click += new System.EventHandler( this.mnuAddPage_Click );
            // 
            // _mnuPageInsert
            // 
            this._mnuPageInsert.Index = 1;
            this._mnuPageInsert.Text = "Insert Page";
            this._mnuPageInsert.Click += new System.EventHandler( this.mnuPageInsert_Click );
            // 
            // _mnuPageDelete
            // 
            this._mnuPageDelete.Index = 2;
            this._mnuPageDelete.Text = "Delete Page";
            this._mnuPageDelete.Click += new System.EventHandler( this.mnuPageDelete_Click );
            // 
            // _mnuPageClear
            // 
            this._mnuPageClear.Index = 3;
            this._mnuPageClear.Text = "Clear Page";
            this._mnuPageClear.Click += new System.EventHandler( this.mnuPageClear_Click );
            // 
            // m_MenuItem10
            // 
            this.m_MenuItem10.Index = 4;
            this.m_MenuItem10.Text = "-";
            // 
            // _mnuShow0
            // 
            this._mnuShow0.Checked = true;
            this._mnuShow0.Index = 5;
            this._mnuShow0.Text = "Always Show Page 0";
            this._mnuShow0.Click += new System.EventHandler( this.mnuShow0_Click );
            // 
            // _mnuPlugins
            // 
            this._mnuPlugins.Index = 4;
            this._mnuPlugins.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this._mnuPluginManager} );
            this._mnuPlugins.Text = "Plug-Ins";
            // 
            // _mnuPluginManager
            // 
            this._mnuPluginManager.Index = 0;
            this._mnuPluginManager.Text = "Manager";
            this._mnuPluginManager.Click += new System.EventHandler( this.mnuPluginManager_Click );
            // 
            // _mnuHelp
            // 
            this._mnuHelp.Index = 5;
            this._mnuHelp.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this._mnuHelpAbout} );
            this._mnuHelp.Text = "Help";
            // 
            // _mnuHelpAbout
            // 
            this._mnuHelpAbout.Index = 0;
            this._mnuHelpAbout.Text = "About...";
            this._mnuHelpAbout.Click += new System.EventHandler( this.mnuHelpAbout_Click );
            // 
            // _mnuGumplingContext
            // 
            this._mnuGumplingContext.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this._mnuGumplingRename,
            this._mnuGumplingMove,
            this._mnuGumplingDelete,
            this.m_MenuItem1,
            this._mnuGumplingAddGumpling,
            this._mnuGumplingAddFolder} );
            // 
            // _mnuGumplingRename
            // 
            this._mnuGumplingRename.Index = 0;
            this._mnuGumplingRename.Text = "Rename";
            // 
            // _mnuGumplingMove
            // 
            this._mnuGumplingMove.Index = 1;
            this._mnuGumplingMove.Text = "Move";
            // 
            // _mnuGumplingDelete
            // 
            this._mnuGumplingDelete.Index = 2;
            this._mnuGumplingDelete.Text = "Delete";
            // 
            // m_MenuItem1
            // 
            this.m_MenuItem1.Index = 3;
            this.m_MenuItem1.Text = "-";
            // 
            // _mnuGumplingAddGumpling
            // 
            this._mnuGumplingAddGumpling.Index = 4;
            this._mnuGumplingAddGumpling.Text = "Add Gumpling";
            // 
            // _mnuGumplingAddFolder
            // 
            this._mnuGumplingAddFolder.Index = 5;
            this._mnuGumplingAddFolder.Text = "Add Folder";
            // 
            // DesignerForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.ClientSize = new System.Drawing.Size( 1350, 708 );
            this.Controls.Add( this._Panel1 );
            this.Controls.Add( this._Splitter1 );
            this.Controls.Add( this._pnlToolboxHolder );
            this.Controls.Add( this._StatusBar );
            this.Icon = ( (System.Drawing.Icon) ( resources.GetObject( "$this.Icon" ) ) );
            this.KeyPreview = true;
            this.Menu = this.m_MainMenu;
            this.Name = "DesignerForm";
            this.Text = "Gump Studio (-Unsaved Gump-)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.DesignerForm_FormClosing );
            this.Load += new System.EventHandler( this.DesignerForm_Load );
            this.KeyDown += new System.Windows.Forms.KeyEventHandler( this.DesignerForm_KeyDown );
            this.KeyUp += new System.Windows.Forms.KeyEventHandler( this.DesignerForm_KeyUp );
            this._pnlToolboxHolder.ResumeLayout( false );
            this._Panel4.ResumeLayout( false );
            this._tabToolbox.ResumeLayout( false );
            this._tpgStandard.ResumeLayout( false );
            this._tpgCustom.ResumeLayout( false );
            this._Panel1.ResumeLayout( false );
            this._Panel2.ResumeLayout( false );
            this._pnlCanvasScroller.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize) ( this._picCanvas ) ).EndInit();
            this._TabPager.ResumeLayout( false );
            this._Panel3.ResumeLayout( false );
            this._Panel3.PerformLayout();
            this.ResumeLayout( false );

        }

        public void LoadFrom( string Path )
        {
            IEnumerator enumerator = null;
            _StatusBar.Text = "Loading gump...";
            FileStream fileStream = null;
            Stacks.Clear();
            _TabPager.TabPages.Clear();
            try
            {
                fileStream = new FileStream( Path, FileMode.Open );
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                Stacks = (ArrayList) binaryFormatter.Deserialize( fileStream );
                try
                {
                    GumpProperties = (GumpProperties) binaryFormatter.Deserialize( fileStream );
                }
                catch ( Exception ex )
                {
                    Exception exception = ex;
                    GumpProperties = new GumpProperties();
                    MessageBox.Show( exception.InnerException.Message );
                }
                SetActiveElement( null, true );
                RefreshElementList();
            }
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message );
            }
            finally
            {
                fileStream?.Close();
            }
            int num1 = 0;
            try
            {
                foreach ( object stack in Stacks )
                {
                    RuntimeHelpers.GetObjectValue( stack );
                    _TabPager.TabPages.Add( new TabPage( num1.ToString() ) );
                    ++num1;
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
            ChangeActiveStack( 0 );
            ElementStack.UpdateParent += new BaseElement.UpdateParentEventHandler( ChangeActiveElementEventHandler );
            ElementStack.Repaint += new BaseElement.RepaintEventHandler( RefreshView );
            _StatusBar.Text = "";
        }

        private void MenuItem2_Click( object sender, EventArgs e )
        {
            _OpenDialog.Filter = "Gumpling (*.gumpling)|*.gumpling|Gump (*.gump)|*.gump";
            if ( _OpenDialog.ShowDialog() != DialogResult.OK )
                return;
            FileStream fileStream = new FileStream( _OpenDialog.FileName, FileMode.Open );
            GroupElement groupElement = (GroupElement) new BinaryFormatter().Deserialize( fileStream );
            groupElement.mIsBaseWindow = false;
            groupElement.RecalculateBounds();
            Point point = new Point( 0, 0 );
            groupElement.Location = point;
            fileStream.Close();
            AddElement( groupElement );
        }

        private void mnuAddPage_Click( object sender, EventArgs e )
        {
            AddPage();
            CreateUndoPoint( "Add page" );
        }

        private void mnuCopy_Click( object sender, EventArgs e )
        {
            Copy();
        }

        private void mnuCut_Click( object sender, EventArgs e )
        {
            Cut();
            CreateUndoPoint();
        }

        private void mnuDataFile_Click( object sender, EventArgs e )
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select the folder that contains the UO data (.mul) files you want to use.";
            if ( folderBrowserDialog.ShowDialog() != DialogResult.OK )
                return;
            if ( File.Exists( Path.Combine( folderBrowserDialog.SelectedPath, "art.mul" ) ) )
            {
                XMLSettings.CurrentOptions.ClientPath = folderBrowserDialog.SelectedPath;
                XMLSettings.Save( this, XMLSettings.CurrentOptions );
                //int num = (int) Interaction.MsgBox( (object) "New path set, please restart Gump Studio to activate your changes.", MsgBoxStyle.OkOnly, (object) "Data Files" );
                MessageBox.Show( "New path set, please restart Gump Studio to activate your changes.", "Data Files" );
            }
            else
            {
                //int num1 = (int) Interaction.MsgBox( (object) "This path does not contain a file named \"art.mul\", it is most likely not the correct path.", MsgBoxStyle.OkOnly, (object) "Data Files" );
                MessageBox.Show( "This path does not contain a file named \"art.mul\", it is most likely not the correct path.", "Data Files" );
            }
        }

        private void mnuDelete_Click( object sender, EventArgs e )
        {
            DeleteSelectedElements();
        }

        private void mnuEditRedo_Click( object sender, EventArgs e )
        {
            Redo();
        }

        private void mnuEditUndo_Click( object sender, EventArgs e )
        {
            Undo();
        }

        private void mnuFileExit_Click( object sender, EventArgs e )
        {
            Close();
        }

        private void mnuFileNew_Click( object sender, EventArgs e )
        {
            //if ( Interaction.MsgBox( (object) "Are you sure you want to start a new gump?", MsgBoxStyle.YesNo | MsgBoxStyle.Question, (object) null ) != MsgBoxResult.Yes )
            //    return;

            var result = MessageBox.Show( "Are you sure you want to start a new gump?", "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question );

            if ( result != DialogResult.OK )
                return;

            ClearGump();
        }

        private void mnuFileOpen_Click( object sender, EventArgs e )
        {
            _OpenDialog.CheckFileExists = true;
            _OpenDialog.Filter = @"Gump|*.gump";
            if ( _OpenDialog.ShowDialog() == DialogResult.OK )
            {
                LoadFrom( _OpenDialog.FileName );
                FileName = Path.GetFileName( _OpenDialog.FileName );
                Text = "Gump Studio (" + FileName + ")";
            }
            _picCanvas.Invalidate();
        }

        private void mnuFileSave_Click( object sender, EventArgs e )
        {
            _SaveDialog.AddExtension = true;
            _SaveDialog.DefaultExt = "gump";
            _SaveDialog.Filter = "Gump|*.gump";
            if ( _SaveDialog.ShowDialog() != DialogResult.OK )
                return;
            SaveTo( _SaveDialog.FileName );
            FileName = Path.GetFileName( _SaveDialog.FileName );
            Text = "Gump Studio (" + FileName + ")";
        }

        private void mnuGroupCreate_Click( object sender, EventArgs e )
        {
            IEnumerator enumerator1 = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                foreach ( object element in ElementStack.GetElements() )
                {
                    BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue( element );
                    if ( objectValue.Selected )
                        arrayList.Add( objectValue );
                }
            }
            finally
            {
                ( enumerator1 as IDisposable )?.Dispose();
            }
            if ( arrayList.Count >= 2 )
            {
                IEnumerator enumerator2 = null;
                GroupElement groupElement = new GroupElement( ElementStack, null, "New Group" );
                try
                {
                    foreach ( object obj in arrayList )
                    {
                        BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue( obj );
                        groupElement.AddElement( objectValue );
                        ElementStack.RemoveElement( objectValue );
                        ElementStack.RemoveEvents( objectValue );
                    }
                }
                finally
                {
                    ( enumerator2 as IDisposable )?.Dispose();
                }
                AddElement( groupElement );
                _picCanvas.Invalidate();
            }
            CreateUndoPoint();
        }

        private void mnuHelpAbout_Click( object sender, EventArgs e )
        {
            frmAboutBox frmAboutBox = new frmAboutBox();
            frmAboutBox.SetText( AboutElementAppend );
            int num = (int) frmAboutBox.ShowDialog();
        }

        private void mnuImportGumpling_Click( object sender, EventArgs e )
        {
            _OpenDialog.Filter = @"Gumpling (*.gumpling)|*.gumpling|Gump (*.gump)|*.gump";
            if ( _OpenDialog.ShowDialog() != DialogResult.OK )
                return;
            FileStream fileStream = new FileStream( _OpenDialog.FileName, FileMode.Open );
            GroupElement Gumpling = (GroupElement) new BinaryFormatter().Deserialize( fileStream );
            Gumpling.mIsBaseWindow = false;
            Gumpling.RecalculateBounds();
            Point point = new Point( 0, 0 );
            Gumpling.Location = point;
            fileStream.Close();
            UncategorizedFolder.AddItem( new TreeGumpling( Path.GetFileName( _OpenDialog.FileName ), Gumpling ) );
            BuildGumplingTree();
        }

        private void mnuPageClear_Click( object sender, EventArgs e )
        {
            ElementStack = new GroupElement( null, null, "Element Stack", true );
            CreateUndoPoint( "Clear Page" );
        }

        private void mnuPageDelete_Click( object sender, EventArgs e )
        {
            if ( _TabPager.SelectedIndex == 0 )
            {
                MessageBox.Show( @"Page 0 can not be deleted." );

            }
            else
            {
                int selectedIndex = _TabPager.SelectedIndex;
                int num2 = _TabPager.TabCount - 1;
                for ( int index = selectedIndex + 1 ; index <= num2 ; ++index )
                    _TabPager.TabPages[index].Text = Convert.ToString( index - 1 );
                Stacks.RemoveAt( selectedIndex );
                _TabPager.TabPages.RemoveAt( selectedIndex );
                ChangeActiveStack( selectedIndex - 1 );
                CreateUndoPoint( "Delete page" );
            }
        }

        private void mnuPageInsert_Click( object sender, EventArgs e )
        {
            if ( _TabPager.SelectedIndex == 0 )
            {
                //int num1 = (int) Interaction.MsgBox( (object) "Page 0 may not be moved.", MsgBoxStyle.OkOnly, (object) null );
                MessageBox.Show( @"Page 0 may not be moved." );
            }
            else
            {
                int tabCount = _TabPager.TabCount;
                int selectedIndex = _TabPager.SelectedIndex;
                int num2 = _TabPager.TabCount - 1;
                for ( int index = selectedIndex ; index <= num2 ; ++index )
                    _TabPager.TabPages.RemoveAt( selectedIndex );
                _TabPager.TabPages.Add( new TabPage( selectedIndex.ToString() ) );
                int num3 = tabCount;
                for ( int index = selectedIndex + 1 ; index <= num3 ; ++index )
                    _TabPager.TabPages.Add( new TabPage( index.ToString() ) );
                GroupElement groupElement = new GroupElement( null, null, "Element Stack", true );
                Stacks.Insert( selectedIndex, groupElement );
                ChangeActiveStack( selectedIndex );
                _TabPager.SelectedIndex = selectedIndex;
                CreateUndoPoint( "Insert page" );
            }
        }

        private void mnuPaste_Click( object sender, EventArgs e )
        {
            Paste();
            CreateUndoPoint();
        }

        private void mnuPluginManager_Click( object sender, EventArgs e )
        {
            int num = (int) new PluginManager()
            {
                AvailablePlugins = AvailablePlugins,
                LoadedPlugins = LoadedPlugins,
                OrderList = PluginTypesToLoad,
                MainForm = this
            }.ShowDialog();
        }

        private void mnuSelectAll_Click( object sender, EventArgs e )
        {
            SelectAll();
        }

        private void mnuShow0_Click( object sender, EventArgs e )
        {
            ShowPage0 = !ShowPage0;
            _mnuShow0.Checked = ShowPage0;
            _picCanvas.Refresh();
        }

        public void Paste()
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            ArrayList arrayList = new ArrayList();
            ArrayList data = (ArrayList) dataObject.GetData( typeof( ArrayList ) );
            if ( data != null )
            {
                SetActiveElement( null, true );

                foreach ( object obj in data )
                {
                    BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue( obj );
                    if ( CopyMode == ClipBoardMode.Copy )
                        objectValue.Name = "Copy of " + objectValue.Name;
                    objectValue.Selected = true;
                    ElementStack.AddElement( objectValue );
                }
            }
            _picCanvas.Invalidate();
        }

        private void picCanvas_MouseDown( object sender, MouseEventArgs e )
        {
            CanvasFocus.Focus();
            Point point = new Point( e.X, e.Y );
            mAnchor = point;
            BaseElement Element = ElementStack.GetElementFromPoint( point );
            if ( ( ActiveElement == null || ActiveElement.HitTest( point ) == MoveModeType.None ? 0 : 1 ) != 0 )
                Element = ActiveElement;
            if ( Element != null )
            {
                MoveMode = Element.HitTest( point );
                if ( ( ActiveElement == null || ActiveElement.HitTest( point ) != MoveModeType.None ? 0 : 1 ) != 0 )
                {
                    if ( Element.Selected )
                    {
                        if ( ( ModifierKeys & Keys.Control ) > Keys.None )
                            Element.Selected = false;
                        else
                            SetActiveElement( Element, false );
                    }
                    else
                        SetActiveElement( Element, ( ModifierKeys & Keys.Control ) <= Keys.None );
                }
                else if ( ActiveElement == null )
                    SetActiveElement( Element, false );
                else if ( ActiveElement != null && ( ModifierKeys & Keys.Control ) > Keys.None )
                {
                    ActiveElement.Selected = false;
                    ArrayList selectedElements = ElementStack.GetSelectedElements();
                    if ( selectedElements.Count > 0 )
                    {
                        SetActiveElement( (BaseElement) selectedElements[0], false );
                    }
                    else
                    {
                        SetActiveElement( null, true );
                        MoveMode = MoveModeType.None;
                    }
                }
            }
            else
            {
                MoveMode = MoveModeType.None;
                if ( ( e.Button & MouseButtons.Left ) > MouseButtons.None )
                    SetActiveElement( null, ( ModifierKeys & Keys.Control ) <= Keys.None );
            }
            _picCanvas.Invalidate();
            LastPos = point;
            if ( ActiveElement != null )
            {
                mAnchorOffset.Width = ActiveElement.X - point.X;
                mAnchorOffset.Height = ActiveElement.Y - point.Y;
            }
            ElementChanged = false;
            MoveCount = 0;
        }

        private void picCanvas_MouseMove( object sender, MouseEventArgs e )
        {
            Point point1 = new Point( e.X, e.Y );
            int num1 = point1.X - LastPos.X;
            int num2 = point1.Y - LastPos.Y;
            BaseElement baseElement = ElementStack.GetElementFromPoint( point1 );
            if ( ( ActiveElement == null || ActiveElement.HitTest( point1 ) == MoveModeType.None ? 0 : 1 ) != 0 )
                baseElement = ActiveElement;
            if ( MoveMode == MoveModeType.Move )
                point1.Offset( mAnchorOffset.Width, mAnchorOffset.Height );
            MouseMoveHookEventArgs e1 = new MouseMoveHookEventArgs();
            e1.Keys = ModifierKeys;
            e1.MouseButtons = e.Button;
            e1.MouseLocation = point1;
            e1.MoveMode = MoveMode;

            foreach ( object loadedPlugin in LoadedPlugins )
            {
                ( (BasePlugin) RuntimeHelpers.GetObjectValue( loadedPlugin ) ).MouseMoveHook( ref e1 );
                point1 = e1.MouseLocation;
            }

            if ( ( MoveMode != MoveModeType.None || Math.Abs( num1 ) <= 0 || Math.Abs( num2 ) <= 0 ? 0 : 1 ) != 0 )
                MoveMode = MoveModeType.SelectionBox;
            if ( e.Button != MouseButtons.Left )
            {
                if ( baseElement != null )
                {
                    switch ( baseElement.HitTest( point1 ) )
                    {
                        case MoveModeType.ResizeTopLeft:
                        case MoveModeType.ResizeBottomRight:
                            Cursor = Cursors.SizeNWSE;
                            break;
                        case MoveModeType.ResizeTopRight:
                        case MoveModeType.ResizeBottomLeft:
                            Cursor = Cursors.SizeNESW;
                            break;
                        case MoveModeType.Move:
                            Cursor = Cursors.SizeAll;
                            break;
                        case MoveModeType.ResizeLeft:
                        case MoveModeType.ResizeRight:
                            Cursor = Cursors.SizeWE;
                            break;
                        case MoveModeType.ResizeTop:
                        case MoveModeType.ResizeBottom:
                            Cursor = Cursors.SizeNS;
                            break;
                        default:
                            Cursor = Cursors.Default;
                            break;
                    }
                }
                else
                    Cursor = Cursors.Default;
            }
            else
            {
                ++MoveCount;
                if ( MoveCount > 100 )
                    MoveCount = 2;
                Rectangle rectangle = new Rectangle( 0, 0, _picCanvas.Width, _picCanvas.Height );
                Cursor.Clip = _picCanvas.RectangleToScreen( rectangle );
                if ( MoveMode != MoveModeType.None )
                {
                    switch ( MoveMode )
                    {
                        case MoveModeType.ResizeTopLeft:
                        case MoveModeType.ResizeBottomRight:
                            Cursor = Cursors.SizeNWSE;
                            break;
                        case MoveModeType.ResizeTopRight:
                        case MoveModeType.ResizeBottomLeft:
                            Cursor = Cursors.SizeNESW;
                            break;
                        case MoveModeType.Move:
                            Cursor = Cursors.SizeAll;
                            break;
                        case MoveModeType.ResizeLeft:
                        case MoveModeType.ResizeRight:
                            Cursor = Cursors.SizeWE;
                            break;
                        case MoveModeType.ResizeTop:
                        case MoveModeType.ResizeBottom:
                            Cursor = Cursors.SizeNS;
                            break;
                        default:
                            Cursor = Cursors.Default;
                            break;
                    }
                    if ( MoveCount >= 2 )
                        ElementChanged = true;
                }
                switch ( MoveMode )
                {
                    case MoveModeType.SelectionBox:
                        rectangle = new Rectangle( mAnchor, new Size( point1.X - mAnchor.X, point1.Y - mAnchor.Y ) );
                        SelectionRect = GetPositiveRect( rectangle );
                        ShowSelectionRect = true;
                        _picCanvas.Invalidate();
                        break;
                    case MoveModeType.ResizeTopLeft:
                        point1.Offset( 3, 0 );
                        Point point2 = new Point( ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height );
                        ActiveElement.Location = point1;
                        Size size1 = ActiveElement.Size;
                        Point location1 = ActiveElement.Location;
                        size1.Width = point2.X - point1.X;
                        size1.Height = point2.Y - point1.Y;
                        if ( size1.Width < 1 )
                        {
                            location1.X = point2.X - 1;
                            size1.Width = 1;
                        }
                        if ( size1.Height < 1 )
                        {
                            location1.Y = point2.Y - 1;
                            size1.Height = 1;
                        }
                        ActiveElement.Size = size1;
                        ActiveElement.Location = location1;
                        _picCanvas.Invalidate();
                        break;
                    case MoveModeType.ResizeTopRight:
                        point1.Offset( -3, 0 );
                        Point point3 = new Point( ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height );
                        Point location2 = ActiveElement.Location;
                        location2.Y = point1.Y;
                        ActiveElement.Location = location2;
                        Size size2 = ActiveElement.Size;
                        size2.Height = point3.Y - point1.Y;
                        size2.Width = point1.X - ActiveElement.X;
                        if ( size2.Height < 1 )
                        {
                            location2.Y = point3.Y - 1;
                            size2.Height = 1;
                        }
                        if ( size2.Width < 1 )
                            size2.Width = 1;
                        location2.X = ActiveElement.Location.X;
                        ActiveElement.Size = size2;
                        ActiveElement.Location = location2;
                        _picCanvas.Invalidate();
                        break;
                    case MoveModeType.ResizeBottomRight:

                        if ( ActiveElement == null )
                            break;

                        point1.Offset( -3, -3 );
                        Size size3 = ActiveElement.Size;
                        size3.Width = point1.X - ActiveElement.X;
                        size3.Height = point1.Y - ActiveElement.Y;
                        if ( size3.Width < 1 )
                            size3.Width = 1;
                        if ( size3.Height < 1 )
                            size3.Height = 1;
                        ActiveElement.Size = size3;
                        _picCanvas.Invalidate();
                        break;
                    case MoveModeType.ResizeBottomLeft:

                        if ( ActiveElement == null )
                            break;

                        point1.Offset( 0, -3 );
                        Point point4 = new Point( ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height );
                        Point location3 = ActiveElement.Location;
                        location3.X = point1.X;
                        ActiveElement.Location = location3;
                        Size size4 = ActiveElement.Size;
                        size4.Width = point4.X - point1.X;
                        size4.Height = point1.Y - ActiveElement.Y;
                        if ( size4.Width < 1 )
                        {
                            location3.X = point4.X - 1;
                            size4.Width = 1;
                        }
                        if ( size4.Height < 1 )
                            size4.Height = 1;
                        location3.Y = ActiveElement.Y;
                        ActiveElement.Size = size4;
                        ActiveElement.Location = location3;
                        _picCanvas.Invalidate();
                        break;
                    case MoveModeType.Move:

                        if ( ActiveElement == null )
                            break;

                        IEnumerator enumerator2 = null;
                        Point location4 = ActiveElement.Location;
                        ActiveElement.Location = point1;
                        int dx = ActiveElement.X - location4.X;
                        int dy = ActiveElement.Y - location4.Y;
                        try
                        {
                            foreach ( object selectedElement in ElementStack.GetSelectedElements() )
                            {
                                BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue( selectedElement );
                                if ( objectValue != ActiveElement )
                                {
                                    Point location5 = objectValue.Location;
                                    location5.Offset( dx, dy );
                                    objectValue.Location = location5;
                                }
                            }
                        }
                        finally
                        {
                            ( enumerator2 as IDisposable )?.Dispose();
                        }
                        _picCanvas.Invalidate();
                        break;
                    case MoveModeType.ResizeLeft:
                        point1.Offset( 3, 0 );
                        Point point5 = new Point( ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height );
                        int y = ActiveElement.Y;
                        ActiveElement.Location = point1;
                        Size size5 = ActiveElement.Size;
                        Point location6 = ActiveElement.Location;
                        size5.Width = point5.X - point1.X;
                        if ( size5.Width < 1 )
                        {
                            location6.X = point5.X - 1;
                            size5.Width = 1;
                        }
                        location6.Y = y;
                        ActiveElement.Size = size5;
                        ActiveElement.Location = location6;
                        _picCanvas.Invalidate();
                        break;
                    case MoveModeType.ResizeTop:
                        point1.Offset( 0, 3 );
                        Point point6 = new Point( ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height );
                        int x = ActiveElement.X;
                        ActiveElement.Location = point1;
                        Size size6 = ActiveElement.Size;
                        Point location7 = ActiveElement.Location;
                        size6.Height = point6.Y - point1.Y;
                        if ( size6.Height < 1 )
                        {
                            location7.Y = point6.Y - 1;
                            size6.Height = 1;
                        }
                        location7.X = x;
                        ActiveElement.Size = size6;
                        ActiveElement.Location = location7;
                        _picCanvas.Invalidate();
                        break;
                    case MoveModeType.ResizeRight:
                        point1.Offset( -3, 0 );
                        Size size7 = ActiveElement.Size;
                        size7.Width = point1.X - ActiveElement.X;
                        if ( size7.Width < 1 )
                            size7.Width = 1;
                        ActiveElement.Size = size7;
                        _picCanvas.Invalidate();
                        break;
                    case MoveModeType.ResizeBottom:
                        point1.Offset( 0, -3 );
                        Size size8 = ActiveElement.Size;
                        size8.Height = point1.Y - ActiveElement.Y;
                        if ( size8.Height < 1 )
                            size8.Height = 1;
                        ActiveElement.Size = size8;
                        _picCanvas.Invalidate();
                        break;
                }
            }
            LastPos = point1;
        }

        private void picCanvas_MouseUp( object sender, MouseEventArgs e )
        {
            Rectangle rectangle = new Rectangle();
            Point point = new Point( e.X, e.Y );
            ElementStack.GetElementFromPoint( point );
            ShowSelectionRect = false;
            Cursor.Clip = rectangle;
            if ( MoveMode == MoveModeType.SelectionBox )
            {
                BaseElement Element = null;

                foreach ( object element in ElementStack.GetElements() )
                {
                    BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue( element );
                    if ( objectValue.ContainsTest( SelectionRect ) )
                    {
                        objectValue.Selected = true;
                        Element = objectValue;
                    }
                    else if ( ( ModifierKeys & Keys.Control ) <= Keys.None )
                        objectValue.Selected = false;
                }

                SetActiveElement( Element, false );
            }
            if ( ( MoveMode == MoveModeType.None || MoveMode == MoveModeType.SelectionBox || !ElementChanged ? 0 : 1 ) != 0 )
            {
                CreateUndoPoint( "Element Moved" );
                ElementChanged = false;
            }
            if ( ( e.Button & MouseButtons.Right ) > MouseButtons.None )
            {
                ContextMenu mnuContextMenu = m_mnuContextMenu;
                GetContextMenu( ref ActiveElement, mnuContextMenu );
                mnuContextMenu.Show( _picCanvas, point );
                ClearContextMenu( mnuContextMenu );
            }
            SetActiveElement( ActiveElement, false );
            _picCanvas.Invalidate();
            MoveMode = MoveModeType.None;
            mAnchorOffset = new Size( 0, 0 );
        }

        private void picCanvas_Paint( object sender, PaintEventArgs e )
        {
            Render( e.Graphics );
        }

        private void pnlCanvasScroller_MouseLeave( object sender, EventArgs e )
        {
            Cursor = Cursors.Default;
        }

        public void RebuildTabPages()
        {
            _TabPager.TabPages.Clear();
            int num = -1;

            foreach ( object stack in Stacks )
            {
                object objectValue = RuntimeHelpers.GetObjectValue( stack );
                ++num;
                _TabPager.TabPages.Add( new TabPage( Convert.ToString( num ) ) );
                if ( ElementStack == objectValue )
                    _TabPager.SelectedIndex = num;
            }
        }

        public void Redo()
        {
            if ( CurrentUndoPoint < UndoPoints.Count )
            {
                ++CurrentUndoPoint;
                RevertToUndoPoint( CurrentUndoPoint );
            }
            if ( CurrentUndoPoint == UndoPoints.Count - 1 )
                _mnuEditRedo.Enabled = false;
            _mnuEditUndo.Enabled = true;
        }

        public void RefreshElementList()
        {
            m_cboElements.Items.Clear();
            m_cboElements.Items.AddRange( ElementStack.GetElements().ToArray() );
        }

        public void RefreshView( object sender )
        {
            RefreshElementList();
            m_cboElements.SelectedItem = ActiveElement;
            if ( ElementStack.GetSelectedElements().Count > 1 )
                m_ElementProperties.SelectedObjects = ElementStack.GetSelectedElements().ToArray();
            else
                m_ElementProperties.SelectedObject = ActiveElement;
        }

        protected void Render( Graphics Target )
        {
            Graphics Target1 = Graphics.FromImage( Canvas );
            if ( !PluginClearsCanvas )
                Target1.Clear( Color.Black );
            HookPreRenderEventHandler hookPreRender = HookPreRender;
            hookPreRender?.Invoke( Canvas );
            if ( ( !ShowPage0 || ElementStack == Stacks[0] ? 0 : 1 ) != 0 )
                ( (BaseElement) Stacks[0] ).Render( Target1 );
            ElementStack.Render( Target1 );

            foreach ( object element in ElementStack.GetElements() )
            {
                BaseElement objectValue = (BaseElement) RuntimeHelpers.GetObjectValue( element );
                if ( ( !objectValue.Selected || objectValue == ActiveElement ? 0 : 1 ) != 0 )
                    objectValue.DrawBoundingBox( Target1, false );
            }

            ActiveElement?.DrawBoundingBox( Target1, true );
            if ( ShowSelectionRect )
            {
                Target1.FillRectangle( SelBG, SelectionRect );
                Target1.DrawRectangle( SelFG, SelectionRect );
            }
            HookPostRenderEventHandler hookPostRender = HookPostRender;
            hookPostRender?.Invoke( Canvas );
            Target1.Dispose();
            Target.DrawImage( Canvas, 0, 0 );
        }

        public void RevertToUndoPoint( int Index )
        {
            UndoPoint undoPoint = (UndoPoint) UndoPoints[Index];
            GumpProperties = (GumpProperties) undoPoint.GumpProperties.Clone();
            Stacks = new ArrayList();
            foreach ( object obj in undoPoint.Stack )
            {
                GroupElement objectValue = (GroupElement) RuntimeHelpers.GetObjectValue( obj );
                GroupElement groupElement = (GroupElement) objectValue.Clone();
                Stacks.Add( groupElement );
                if ( undoPoint.ElementStack == objectValue )
                    ElementStack = groupElement;
            }

            RebuildTabPages();
            _picCanvas.Invalidate();
            SetActiveElement( null, true );
            CurrentUndoPoint = Index;
        }

        public void SaveTo( string Path )
        {
            _StatusBar.Text = $@"Saving gump...";
            ElementStack.UpdateParent -= new BaseElement.UpdateParentEventHandler( ChangeActiveElementEventHandler );
            ElementStack.Repaint -= new BaseElement.RepaintEventHandler( RefreshView );
            FileStream fileStream = new FileStream( Path, FileMode.Create );
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize( fileStream, Stacks );
            binaryFormatter.Serialize( fileStream, GumpProperties );
            fileStream.Close();
            ElementStack.UpdateParent += new BaseElement.UpdateParentEventHandler( ChangeActiveElementEventHandler );
            ElementStack.Repaint += new BaseElement.RepaintEventHandler( RefreshView );
            _StatusBar.Text = "";
        }

        public void SelectAll()
        {
            foreach ( object selectedElement in ElementStack.GetSelectedElements() )
                ( (BaseElement) RuntimeHelpers.GetObjectValue( selectedElement ) ).Selected = true;
            _picCanvas.Invalidate();
        }

        public void SetActiveElement( BaseElement e )
        {
            SetActiveElement( e, false );
        }

        public void SetActiveElement( BaseElement Element, bool DeselectOthers )
        {
            if ( DeselectOthers )
            {
                foreach ( object element in ElementStack.GetElements() )
                    ( (BaseElement) RuntimeHelpers.GetObjectValue( element ) ).Selected = false;
            }
            if ( ActiveElement != Element )
            {
                RefreshElementList();
                ActiveElement = Element;
                m_cboElements.SelectedItem = Element;
                if ( Element != null )
                    Element.Selected = true;
            }
            if ( ElementStack.GetSelectedElements().Count > 1 )
                m_ElementProperties.SelectedObjects = ElementStack.GetSelectedElements().ToArray();
            else if ( Element != null )
                m_ElementProperties.SelectedObject = Element;
            else
                m_ElementProperties.SelectedObject = GumpProperties;
        }

        public Point SnapLocToGrid( Point Position, Size GridSize )
        {
            Point point = Position;
            point.X = point.X / GridSize.Width * GridSize.Width;
            point.Y = point.Y / GridSize.Height * GridSize.Height;
            return point;
        }

        private void TabPager_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( _TabPager.SelectedIndex != -1 )
                ChangeActiveStack( _TabPager.SelectedIndex );
            RefreshElementList();
        }

        private void treGumplings_DoubleClick( object sender, EventArgs e )
        {
            if ( _treGumplings.SelectedNode.Tag == null || !( _treGumplings.SelectedNode.Tag is TreeGumpling ) )
                return;
            GroupElement groupElement = (GroupElement) ( (TreeGumpling) _treGumplings.SelectedNode.Tag ).Gumpling.Clone();
            groupElement.mIsBaseWindow = false;
            groupElement.RecalculateBounds();
            Point point = new Point( 0, 0 );
            groupElement.Location = point;
            AddElement( groupElement );
        }

        private void treGumplings_MouseUp( object sender, MouseEventArgs e )
        {
            _treGumplings.SelectedNode = _treGumplings.GetNodeAt( new Point( e.X, e.Y ) );
        }

        public void Undo()
        {
            --CurrentUndoPoint;
            RevertToUndoPoint( CurrentUndoPoint );
            if ( CurrentUndoPoint == 0 )
                _mnuEditUndo.Enabled = false;
            _mnuEditRedo.Enabled = true;
        }

        public void WritePluginsToLoad()
        {
            if ( PluginTypesToLoad != null )
            {
                FileStream fileStream = new FileStream( Application.StartupPath + "\\Plugins\\LoadInfo", FileMode.Create );
                new BinaryFormatter().Serialize( fileStream, PluginTypesToLoad );
                fileStream.Close();
            }
            else
            {
                if ( !File.Exists( Application.StartupPath + "\\Plugins\\LoadInfo" ) )
                    return;
                File.Delete( Application.StartupPath + "\\Plugins\\LoadInfo" );
            }
        }

        public delegate void HookKeyDownEventHandler( object sender, ref KeyEventArgs e );
        public delegate void HookPostRenderEventHandler( Bitmap Target );
        public delegate void HookPreRenderEventHandler( Bitmap Target );
    }
}
