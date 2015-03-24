using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class FrontPage : ContentPage
	{
		public List<ChecklistModel> checklists { get; set; }
		public FrontPage(List<ChecklistModel> inputChecklists)
		{
			this.checklists = inputChecklists;
			TableView checklistsView = new TableView();
			TableRoot root = new TableRoot("Select a Checklist");
			TableSection checklistSection = new TableSection();
			//TableSection inspectorSection = new TableSection();
			List<ViewCell> cells = new List<ViewCell>();

			foreach (ChecklistModel checklist in checklists)
			{
				ChecklistButton button = new ChecklistButton();
				button.Clicked += ChecklistHelper.ChecklistButtonClicked;
				button.Text = checklist.Title;
				button.checklist = checklist;

				ViewCell cell = new ViewCell
				{
					View = button
				};
				cells.Add(cell);
			}
			ToolbarItem inspectorButton = new ToolbarItem();
			inspectorButton.Text = "Inspectors";
			inspectorButton.Clicked += openInspectorsPage;
			ToolbarItems.Add(inspectorButton);

			//Button inspectorsButton = new Button();
			//inspectorSection.Title = "Inspectors";
			//inspectorsButton.Clicked += openInspectorsPage;

			checklistSection.Add(cells);
			root.Add(checklistSection);
			//root.Add(inspectorSection);
			checklistsView.Root = root;

			Content = checklistsView;
			Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			Title = "Select a checklist";
		}

		public async void openInspectorsPage(object sender, EventArgs e)
		{
			ToolbarItem button = (ToolbarItem)sender;
			InspectorsPage page = new InspectorsPage();
			await App.Navigation.PushAsync(page);
		}
	}
}
