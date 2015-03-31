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
		public FrontPage(List<ChecklistModel> inputChecklists)
		{
			this.checklists = inputChecklists;
			ToolbarItem inspectorButton = new ToolbarItem();
			inspectorButton.Text = "Inspectors";
			inspectorButton.Clicked += openInspectorsPage;
			ToolbarItems.Add(inspectorButton);

			ResetChecklists();
			Title = "Select a checklist";
		}
		protected override void OnAppearing()
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
			base.OnAppearing();
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
			root.Add(tempChecklistSection);
			checklistSection = tempChecklistSection;
			checklistsView.Root = root;

			Content = checklistsView;
		}

		public async void openInspectorsPage(object sender, EventArgs e)
		{
			ToolbarItem button = (ToolbarItem)sender;
			InspectorsPage page = new InspectorsPage();
			await App.Navigation.PushAsync(page);
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
