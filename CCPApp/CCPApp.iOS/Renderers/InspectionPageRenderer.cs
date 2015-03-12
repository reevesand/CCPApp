using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Forms;
using CCPApp.Views;
using CCPApp.iOS;
using Xamarin.Forms.Platform.iOS;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreImage;

//[assembly: ExportRenderer (typeof(InspectionPage), typeof(InspectionPageRenderer))]
namespace CCPApp.iOS.Renderers
{
	public class InspectionPageRenderer : TabbedRenderer
	{
		/*protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);
			this.TabBar.BackgroundColor = UIColor.Black;
		}
		public override void ViewDidLayoutSubviews()
		{
			UITabBarItem[] items = this.TabBar.Items;
			UITabBarItem item = this.TabBar.SelectedItem;

			CIImage image = new CIImage(new CIColor(UIColor.Red));
			//items[0].Image = new UIImage(image);
			base.ViewDidLayoutSubviews();
		}*/
	}
}