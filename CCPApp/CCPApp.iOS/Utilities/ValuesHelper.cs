using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Forms;
using CCPApp.Utilities;
using CCPApp.iOS.Utilities;

[assembly: Dependency(typeof(ValuesHelper))]
namespace CCPApp.iOS.Utilities
{
	public class ValuesHelper : IValuesHelper
	{
		public string exportInstructions()
		{
			return "Please input the name you would like to give to the exported file in the box below.  " +
				"This file will be available to tranfer to a computer when you connect via iTunes";
		}
	}
}