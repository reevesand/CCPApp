using CCPApp.Items;
using CCPApp.Models;
using CCPApp.Views;
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
			List<ChecklistModel> checklists = new List<ChecklistModel>();
			/*
			IEnumerable<string> zipFileNames = DependencyService.Get<IFileManage>().GetAllValidFiles();
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
			}*/
			//database.SaveChecklists(newChecklists);

			checklists.AddRange(database.LoadAllChecklists());

			FrontPage frontPage = new FrontPage(checklists);
			
			MainPage = new NavigationPage(frontPage);
			Navigation = MainPage.Navigation;
		}
	}
}
