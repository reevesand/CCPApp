using CCPApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class PptPage : ContentPage
	{
		public string FileName { get; set; }
		public PptPage(string fileName)
		{
			FileName = fileName;
		}

		public static string GeneratePpt()
		{
			string fileName = "Presentation.ppt";
			IPptGenerator pptMaker = DependencyService.Get<IPptGenerator>();
			pptMaker.CreateBlankFile(fileName);

			return fileName;
		}
	}
}
