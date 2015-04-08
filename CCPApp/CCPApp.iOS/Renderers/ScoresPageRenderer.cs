using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Forms;
using CCPApp.Views;
using CCPApp.iOS.Renderers;
using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(ScoresLayout), typeof(ScoresLayoutRenderer))]
namespace CCPApp.iOS.Renderers
{
	public class ScoresLayoutRenderer : ViewRenderer
	{
		/*protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{
			if (e.OldElement == null)
			{
				ScoresLayout layout = (ScoresLayout)Element;
				UISwipeGestureRecognizer swipeRecognizer = new UISwipeGestureRecognizer(() => layout.closePage());
				swipeRecognizer.Direction = UISwipeGestureRecognizerDirection.Down;
				this.AddGestureRecognizer(swipeRecognizer);
			}
			if (e.NewElement != null)
			{
				base.OnElementChanged(e);
			}
		}*/
	}
}