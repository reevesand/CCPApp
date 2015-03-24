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
			Inspection newInspection = new Inspection();
			newInspection.Checklist = checklist;
			InspectionButton createInspectionButton = new InspectionButton(newInspection);
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
			EditInspectionPage page = new EditInspectionPage();
			page.inspection = inspection;
			page.CallingPage = (ChecklistPage)button.ParentView.ParentView;
			await App.Navigation.PushModalAsync(page);
		}
		public async void deleteInspection(object sender, EventArgs e)
		{

		}
	}
}
