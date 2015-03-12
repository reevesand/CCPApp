using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class ReferencePage : ContentPage
	{
		public string FileName { get; set; }
		public int PageNumber { get; set; }
		public ReferencePage()
		{
			FileName = "Reference List.doc";
			PageNumber = 2;
		}
	}
}
