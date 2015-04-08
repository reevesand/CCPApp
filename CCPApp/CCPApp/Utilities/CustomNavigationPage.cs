using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Utilities
{
	public class CustomNavigationPage : NavigationPage
	{
		public CustomNavigationPage(Page page) : base(page) { }
		protected override bool OnBackButtonPressed()
		{
			return base.OnBackButtonPressed();
		}
		public override void PopAsync()
		{
			base.PopAsync();
		}
	}
}
