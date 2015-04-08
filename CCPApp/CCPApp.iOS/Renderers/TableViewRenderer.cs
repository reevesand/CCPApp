using CCPApp.iOS.Renderers;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TableView), typeof(CCPTableViewRenderer))]
namespace CCPApp.iOS.Renderers
{
	public class CCPTableViewRenderer : TableViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
				return;

			UITableView tableView = Control as UITableView;
			//tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
		}
	}
}
