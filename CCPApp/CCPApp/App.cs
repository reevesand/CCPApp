using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace CCPApp
{
	public class App : Application
	{
		public static INavigation Navigation;
		public static DatabaseAccess database = new DatabaseAccess();
		public App()
		{
			ListView checklistsView = new ListView();
			List<ChecklistModel> checklists = new List<ChecklistModel>();
			///*
			IEnumerable<string> xmlFileNames = DependencyService.Get<IFileManage>().GetAllValidFiles();
			List<ChecklistModel> newChecklists = new List<ChecklistModel>();
			foreach (string fileName in xmlFileNames)
			{
				string checklistId = DependencyService.Get<IParseChecklist>().GetChecklistId(fileName);
				ChecklistModel model;
				if (!database.ChecklistExists(checklistId))
				{
					model = ChecklistModel.Initialize(fileName);
					newChecklists.Add(model);					
				}
				else
				{
					model = ChecklistHelper.LoadChecklistDetails(checklistId);
					//model.loadInspections();
				}
				checklists.Add(model);
			}
			database.SaveChecklists(newChecklists);
			//*/
			/*
			ChecklistModel fakeChecklist = new ChecklistModel();
			fakeChecklist.Title = "Sample Checklist";
			fakeChecklist.Id = "FakeID";
			checklists.Add(fakeChecklist);
			*/
			checklistsView.ItemsSource = checklists;
			checklistsView.ItemTemplate = new DataTemplate(() =>
			{
				ChecklistButton button = new ChecklistButton();
				button.Clicked += ChecklistHelper.ChecklistButtonClicked;
				button.SetBinding(Button.TextProperty, "Title");
				button.SetBinding(ChecklistButton.ChecklistProperty, "SelfReference");
				
				ViewCell cell = new ViewCell
				{
					View = button
				};

				return cell;
			});
			MainPage = new NavigationPage(new ContentPage
			{
				Content = checklistsView,
				Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5),
				//Padding = new Thickness(0,0,0,0),
				Title = "Select a checklist"
			});
			Navigation = MainPage.Navigation;
		}
	}
}
