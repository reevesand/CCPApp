using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using CCPApp.iOS.Renderers;
using CCPApp.Views;
using CCPApp.Items;

//[assembly: ExportRenderer(typeof(ResizingLayout), typeof(ResizingLayoutRenderer))]
namespace CCPApp.iOS.Renderers
{
	public class ResizingLayoutRenderer : ViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged(e);
			if (e.NewElement != null && e.OldElement == null)
			{
				Element.PropertyChanged += (s_, e_) => SetNeedsDisplay();
			}
		}
		//public override void Draw(System.Drawing.RectangleF rect)
		//{
		//	base.Draw(rect);
		//}
	}
}