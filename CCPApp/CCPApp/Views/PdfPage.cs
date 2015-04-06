using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public abstract class PdfPage : ContentPage
	{
		public string FileName { get; set; }
		public int PageNumber { get; set; }
		public bool useTemp = false;
		public PdfPage(string fileName, int pageNumber)
		{
			FileName = fileName;
			PageNumber = pageNumber;
		}
	}
}
