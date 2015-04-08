using CCPApp.Items;
using CCPApp.Models;
using CCPApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class FrontPage : ContentPage
	{
		TableSection checklistSection;
		public List<ChecklistModel> checklists { get; set; }
		ViewCell ResetCell;

		public FrontPage(List<ChecklistModel> inputChecklists)
		{
			this.checklists = inputChecklists;
			ToolbarItem inspectorButton = new ToolbarItem();
			inspectorButton.Text = "Inspectors";
			inspectorButton.Clicked += InspectorHelper.openInspectorsPage;
			ToolbarItems.Add(inspectorButton);
			Button ResetButton = new Button
			{
				Text = "Refresh Checklists",
			};
			ResetButton.Clicked += ResetButton_Clicked;
			ResetCell = new ViewCell
			{
				View = ResetButton
			};

			ResetChecklists();
			Title = "Select a checklist";
		}

		void ResetButton_Clicked(object sender, EventArgs e)
		{
			CheckForChecklists();
		}
		protected override void OnAppearing()
		{
			CheckForChecklists();
			base.OnAppearing();
		}
		internal void CheckForChecklists()
		{
			IEnumerable<string> zipFileNames = DependencyService.Get<IFileManage>().GetAllValidFiles();
			if (zipFileNames.Any())
			{
				List<ChecklistModel> newChecklists = new List<ChecklistModel>();
				foreach (string zipName in zipFileNames)
				{
					string unzippedDirectory = DependencyService.Get<IUnzipHelper>().Unzip(zipName);
					string xmlFile = DependencyService.Get<IFileManage>().GetXmlFile(unzippedDirectory);
					string checklistId = DependencyService.Get<IParseChecklist>().GetChecklistId(xmlFile);
					ChecklistModel model = ChecklistModel.Initialize(xmlFile);
					//move the files to a new folder.
					DependencyService.Get<IFileManage>().MoveDirectoryToPrivate(unzippedDirectory, checklistId);
					//Delete the zip file once we're done with it.
					DependencyService.Get<IFileManage>().DeleteFile(zipName);
					newChecklists.Add(model);
					checklists.Add(model);
				}
				App.database.SaveChecklists(newChecklists);
				ResetChecklists();
			}
		}
		internal void ResetChecklists()
		{
			ActivityIndicator indicator = new ActivityIndicator()
			{
				IsEnabled = true,
				IsRunning = true,
				IsVisible = true,
				Color = Color.Red,
			};
			TableView checklistsView = new TableView();
			TableRoot root = new TableRoot("Select a Checklist");
			TableSection tempChecklistSection = new TableSection();
			checklistsView.Intent = TableIntent.Menu;
			//TableSection inspectorSection = new TableSection();
			List<ViewCell> cells = new List<ViewCell>();

			foreach (ChecklistModel checklist in checklists)
			{
				ChecklistButton button = new ChecklistButton();
				button.Clicked += ChecklistHelper.ChecklistButtonClicked;
				button.Text = checklist.Title;
				button.checklist = checklist;
				button.HorizontalOptions = LayoutOptions.Start;

				ViewCell cell = new ViewCell
				{
					View = button,
				};

				BoundMenuItem<ChecklistModel> Delete = new BoundMenuItem<ChecklistModel> { Text = "Delete", BoundObject = checklist, IsDestructive = true };
				Delete.Clicked += DeleteChecklist;
				cell.ContextActions.Add(Delete);

				cells.Add(cell);
			}
			tempChecklistSection.Add(cells);
			tempChecklistSection.Add(ResetCell);
			root.Add(tempChecklistSection);
			checklistSection = tempChecklistSection;
			checklistsView.Root = root;

			Content = checklistsView;
		}

		public async void DeleteChecklist(object sender, EventArgs e)
		{
			BoundMenuItem<ChecklistModel> button = (BoundMenuItem<ChecklistModel>)sender;
			ChecklistModel checklist = button.BoundObject;
			bool answer = await DisplayAlert("Confirm Deletion", DependencyService.Get<IValuesHelper>().deleteChecklistWarning(checklist.Title), "Yes", "No");
			if (!answer)
			{
				return;
			}
			ChecklistModel.DeleteChecklist(checklist);
			checklists.Remove(checklist);
			ResetChecklists();
		}
	}
}
