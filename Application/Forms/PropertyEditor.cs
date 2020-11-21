using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GumpStudio
{
	public partial class PropertyEditor : Form
	{
		public object SourceObject
		{
			get => _Properties.SelectedObject;
			set => _Properties.SelectedObject = value;
		}

		public event PropertyValueChangedEventHandler PropertyValueChanged
		{
			add => _Properties.PropertyValueChanged += value;
			remove => _Properties.PropertyValueChanged -= value;
		}

		public bool ChangesPending { get; private set; }

		public PropertyEditor()
		{
			InitializeComponent();

			PropertyValueChanged += OnPropertyValueChanged;

			FormClosed += OnClosed;
		}

		private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			ChangesPending = true;
		}

		private void OnClosed(object sender, FormClosedEventArgs e)
		{
			ChangesPending = false;
		}
	}
}
