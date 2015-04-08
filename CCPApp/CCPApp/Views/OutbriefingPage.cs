using CCPApp.Models;
using CCPApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class OutbriefingPage : ContentPage
	{
		Inspection inspection;
		Entry fileNameEntry = new Entry();
		public OutbriefingPage(Inspection inspection)
		{
			this.inspection = inspection;
			Button saveButton = new Button { Text = "Save" };
			saveButton.Clicked += SaveOutbriefing;

			Content = new StackLayout
			{
				Children = {
					new Label { Text = DependencyService.Get<IValuesHelper>().outbriefingInstructions() },
					fileNameEntry,
					saveButton
				}
			};
		}
		public async void SaveOutbriefing(object sender, EventArgs e)
		{
			string filename = fileNameEntry.Text;
			if (!filename.EndsWith(".xml"))
			{
				filename = filename + ".xml";
			}
			DependencyService.Get<ISaveInspection>().ExportInspection(inspection, filename);
			//maybe throw a popup here or something explaining what to do with the xml file.
			await App.Navigation.PopAsync();
		}
	}
}
