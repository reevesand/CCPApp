using CCPApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp
{
	public class InspectorHelper
	{
		public static async void openInspectorsPage(object sender, EventArgs e)
		{
			ToolbarItem button = (ToolbarItem)sender;
			InspectorsPage page = new InspectorsPage();
			await App.Navigation.PushAsync(page);
		}
	}
}
