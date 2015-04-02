using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Utilities
{
	/// <summary>
	/// A layout designed to take two views and lay them out on the left and right sides of the parent element.
	/// </summary>
	public class BothSidesLayout : RelativeLayout
	{
		public BothSidesLayout(View first, View second)
		{
			/*Children.Add(first,
				Constraint.RelativeToParent((parent) =>
				{
					return 0;
				}),
				Constraint.RelativeToParent((parent) =>
				{
					return parent.Width - 100;
				})
			);*/
			Children.Add(first,
				Constraint.RelativeToParent((parent) =>
				{
					return 0;
				})
			);
			Children.Add(second,
				Constraint.RelativeToParent((parent) =>
				{
					return parent.Width - second.Width;
				})
			);
		}
	}
}
