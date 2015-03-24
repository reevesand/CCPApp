using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Utilities
{
	public class BoundMenuItem<T> : MenuItem
	{
		public T BoundObject { get; set; }
	}
}
