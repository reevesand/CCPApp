using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Forms.Platform.iOS;
using CCPApp.Views;
using Xamarin.Forms;
using CCPApp.iOS.Renderers;
using System.Drawing;
using System.Threading.Tasks;
using MonoTouch.CoreGraphics;
using System.IO;

//[assembly:ExportRenderer(typeof(PdfViewer), typeof(PdfViewerRenderer))]
[assembly:ExportRenderer(typeof(PdfPage), typeof(PdfPageRenderer))]
namespace CCPApp.iOS.Renderers
{
	/*public class PdfViewerRenderer : ViewRenderer<PdfViewer, UIWebView>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<PdfViewer> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
			{
				return;
			}
			UIWebView webView = new UIWebView();

			string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string path = System.IO.Path.Combine(documentsFolder, Element.FileName);

			webView.LoadRequest(new NSUrlRequest(new NSUrl(path, false)));
			webView.ScalesPageToFit = true;

			SetNativeControl(webView);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			UIScrollView scroll = Control.ScrollView;
			new Action(async () =>
			{
				await Task.Delay(2000);
				BeginInvokeOnMainThread(new NSAction(() =>
				{
					//scroll.ScrollRectToVisible(new RectangleF(0, 1000, scroll.Bounds.Width, scroll.Bounds.Height), false);
				}));
			}).Invoke();
		}
	}*/

	public class PdfPageRenderer : PageRenderer
	{
		int NumberOfPages = 0;
		float PageLength = 0;
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
			{
				return;
			}
			PdfPage page = (PdfPage)Element;
			string path;
			if (page.useTemp)
			{
				string tempFolder = new FileManage().GetTempFolder();
				path = System.IO.Path.Combine(tempFolder, page.FileName);
			}
			else
			{
				string libraryFolder = new FileManage().GetLibraryFolder();
				path = System.IO.Path.Combine(libraryFolder, page.FileName);
			}

			UIWebView webView = new UIWebView();

			if (path.EndsWith(".pdf"))
			{
				CGPDFDocument doc = CGPDFDocument.FromFile(path);
				NumberOfPages = doc.Pages;
			}

			NSUrlRequest request = new NSUrlRequest(new NSUrl(path,false));
			webView.LoadRequest(new NSUrlRequest(new NSUrl(path, false)));
			webView.PaginationMode = UIWebPaginationMode.TopToBottom;
			webView.ScalesPageToFit = true;

			View = webView;


		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			PdfPage page = (PdfPage)Element;
			if (page.PageNumber > 1 && NumberOfPages > 0)
			{
				UIWebView webView = (UIWebView)View;
				UIScrollView scroll = webView.ScrollView;
				PageLength = scroll.ContentSize.Height / NumberOfPages;
				float pixelDistance = (page.PageNumber - 1) * PageLength - 3;
				scroll.ScrollRectToVisible(new RectangleF(0, pixelDistance, scroll.Bounds.Width, scroll.Bounds.Height), false);
			}
		}
	}
}