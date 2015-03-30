using CCPApp.Models;
using CCPApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class ChecklistPage : ContentPage
	{
		ChecklistModel checklist;
		//public string ChecklistTitle { get; set; }
		public ChecklistPage(ChecklistModel checklist)
		{
			Title = checklist.Title;
			Padding = 20;
			this.checklist = checklist;
			ResetInspections();
		}
		public void ResetInspections()
		{
			TableView view = new TableView();
			TableRoot root = new TableRoot("Inspections for " + Title);
			TableSection section = new TableSection();
			List<ViewCell> cells = new List<ViewCell>();
			foreach (Inspection inspection in checklist.Inspections)
			{
				ViewCell cell = new ViewCell();
				Button button = new InspectionButton(inspection);
				button.Clicked += InspectionHelper.SelectInspectionButtonClicked;
				cell.View = button;

				BoundMenuItem<Inspection> Edit = new BoundMenuItem<Inspection> { Text = "Edit", BoundObject = inspection };
				BoundMenuItem<Inspection> Delete = new BoundMenuItem<Inspection> { Text = "Delete", BoundObject = inspection, IsDestructive = true };
				Edit.Clicked += openEditPage;
				Delete.Clicked += deleteInspection;
				cell.ContextActions.Add(Edit);
				cell.ContextActions.Add(Delete);

				cells.Add(cell);
			}
			CreateInspectionButton createInspectionButton = new CreateInspectionButton(checklist);
			createInspectionButton.Text = "Start new Inspection";
			createInspectionButton.Clicked += InspectionHelper.CreateInspectionButtonClicked;
			ViewCell createInspectionButtonView = new ViewCell();
			createInspectionButtonView.View = createInspectionButton;

			section.Add(cells);
			section.Add(createInspectionButtonView);
			root.Add(section);
			view.Root = root;

			Content = view;
		}

		public async void openEditPage(object sender, EventArgs e)
		{
			BoundMenuItem<Inspection> button = (BoundMenuItem<Inspection>)sender;
			Inspection inspection = button.BoundObject;
			EditInspectionPage page = new EditInspectionPage(inspection);
			page.CallingPage = this;
			await App.Navigation.PushModalAsync(page);
		}
		public async void deleteInspection(object sender, EventArgs e)
		{
			//Warn that this is permanent.  Ask if they're sure.
			BoundMenuItem<Inspection> button = (BoundMenuItem<Inspection>)sender;
			Inspection inspection = button.BoundObject;
			bool answer = await DisplayAlert("Confirm Deletion", "Are you sure you want to delete "+inspection.Name+"?  All its data and scores will be lost.", "Yes", "No");
			if (!answer)
			{
				return;
			}			
			Inspection.DeleteInspection(inspection);
			if (checklist.Inspections.Contains(inspection))
			{	//This is supposed to be removed in the DeleteInspection method, but there appear to be multiple copies of som
					//objects in memory.
				checklist.Inspections.Remove(inspection);
			}
			ResetInspections();
		}
	}
}
