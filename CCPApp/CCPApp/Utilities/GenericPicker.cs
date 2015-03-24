using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Items
{
	public class GenericPicker<T> : Picker
	{
		public IList<T> TItems = new List<T>();

		public void AddItem(T item)
		{
			TItems.Add(item);
			Items.Add(item.ToString());
		}
		public void ClearItems()
		{
			TItems.Clear();
			Items.Clear();
		}

		public T SelectedItem
		{
			get
			{
				return TItems[SelectedIndex];
			}
		}
	}
}
