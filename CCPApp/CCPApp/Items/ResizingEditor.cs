using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Items
{
	public class ResizingEditor : Editor
	{
		public ResizingEditor()
		{
			this.TextChanged += ResizingEditor_TextChanged;
		}
		private void ResizingEditor_TextChanged(object sender, EventArgs e)
		{//sender will always be equivalent to this.
			this.OnPropertyChanged();
		}
	}
}
