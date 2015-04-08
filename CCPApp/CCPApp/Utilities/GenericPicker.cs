using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Utilities
{
	public class GenericPicker<T> : Picker
	{
		public List<T> TItems = new List<T>();

		/// <summary>
		/// Adds a templated item to the picker
		/// </summary>
		/// <param name="item"></param>
		public void AddItem(T item)
		{
			TItems.Add(item);
			Items.Add(item.ToString());
		}
		/// <summary>
		/// Removes all templated items from the picker.
		/// </summary>
		public void ClearItems()
		{
			TItems.Clear();
			Items.Clear();
		}

		/// <summary>
		/// Gets the currently selected item
		/// </summary>
		public T SelectedItem
		{
			get
			{
				return TItems[SelectedIndex];
			}
			set
			{
				for (int i = 0; i < TItems.Count; i++)
				{
					if (TItems.ElementAt(i).Equals(value))
					{
						SelectedIndex = i;
						return;
					}
				}
				throw new KeyNotFoundException("The selected item is not in the collection.");
			}
		}
	}
}
