using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace GumpStudio
{
	public enum ConfigFormat
	{ 
		Xml,
		Bin
	}

	public abstract class BaseConfig
	{
		[NonSerialized, XmlIgnore]
		private readonly PropertyEditor _Editor = new PropertyEditor();

		[NonSerialized, XmlIgnore]
		private readonly Type _Type;

		[NonSerialized, XmlIgnore]
		private readonly PropertyInfo[] _Props;

		[NonSerialized, XmlIgnore]
		private XmlSerializer _XmlSerializer;

		[NonSerialized, XmlIgnore]
		private BinaryFormatter _BinSerializer;

		[XmlIgnore, Browsable(false)]
		public virtual ConfigFormat Format => ConfigFormat.Xml;

		[XmlIgnore, Browsable(false)]
		public virtual string Name => _Type.Name;

		[XmlIgnore, Browsable(false)]
		public virtual string FileName => $"{_Type.DeclaringType?.Name ?? _Type.Namespace}.{_Type.Name}.{Format.ToString().ToLower()}";

		[XmlIgnore, Browsable(false)]
		public bool ChangesPending => _Editor.ChangesPending;

		[Browsable(false)]
		public event PropertyValueChangedEventHandler ValueChanged
		{
			add => _Editor.PropertyValueChanged += value;
			remove => _Editor.PropertyValueChanged -= value;
		}

		public BaseConfig()
		{
			_Type = GetType();
			_Props = _Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			_Editor.SourceObject = this;
			_Editor.Text = Name;

			_Editor.PropertyValueChanged += OnValueChanged;
			_Editor.FormClosing += OnEditorClosing;
		}

		protected virtual void OnValueChanged(object s, PropertyValueChangedEventArgs e)
		{
		}

		protected virtual void OnEditorClosing(object sender, FormClosingEventArgs e)
		{
			if (ChangesPending)
			{
				Save();
			}
		}

		public override string ToString()
		{
			return Name ?? _Type.Name;
		}

		public void Close()
		{
			Close(true);
		}

		public void Close(bool save)
		{
			if (!save)
			{
				_Editor.ChangesPending = false;
			}
			else if (ChangesPending)
			{
				Save();
			}

			_Editor.Close();
		}

		public void Edit()
		{
			_Editor.Show(Form.ActiveForm);
		}

		public void Save()
		{
			switch (Format)
			{
				case ConfigFormat.Xml:
				{
					if (_XmlSerializer == null)
					{
						_XmlSerializer = new XmlSerializer(_Type);
					}

					using (var xml = new XmlTextWriter(FileName, Encoding.UTF8) { Formatting = Formatting.Indented })
					{
						_XmlSerializer.Serialize(xml, this);
					}
				}
				break;

				case ConfigFormat.Bin:
				{
					if (_BinSerializer == null)
					{
						_BinSerializer = new BinaryFormatter();
					}

					using (var fileStream = new FileStream(FileName, FileMode.Create))
					{
						_BinSerializer.Serialize(fileStream, this);
					}
				}
				break;
			}
		}

		public void Load()
		{
			if (!File.Exists(FileName))
			{
				return;
			}

			object loaded = null;

			switch (Format)
			{
				case ConfigFormat.Xml:
				{
					if (_XmlSerializer == null)
					{
						_XmlSerializer = new XmlSerializer(_Type);
					}

					using (var xml = new XmlTextReader(FileName))
					{
						loaded = _XmlSerializer.Deserialize(xml);
					}
				}
				break;

				case ConfigFormat.Bin:
				{
					if (_BinSerializer == null)
					{
						_BinSerializer = new BinaryFormatter();
					}
					
					using (var fileStream = new FileStream(FileName, FileMode.Open))
					{
						loaded = _BinSerializer.Deserialize(fileStream);
					}
				}
				break;
			}

			if (loaded == null)
			{
				return;
			}

			foreach (var p in _Props.Where(p => p.CanRead && p.CanWrite))
			{
				p.SetValue(this, p.GetValue(loaded));
			}
		}
	}
}
