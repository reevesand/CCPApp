using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using CCPApp.Items;
using CCPApp.iOS.Renderers;

//[assembly: ExportRenderer(typeof(ResizingEditor), typeof(ResizingEditorRenderer))]
namespace CCPApp.iOS.Renderers
{
	public class ResizingEditorRenderer : EditorRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);
			if (e.NewElement != null && e.OldElement == null)
			{
				Element.PropertyChanged += (s_, e_) => SetNeedsDisplay();
			}
		}
		public override void Draw(System.Drawing.RectangleF rect)
		{
			base.Draw(rect);
		}
	}
}