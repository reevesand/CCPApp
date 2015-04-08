using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CCPApp.Models;
using CCPApp.Views;

namespace CCPApp
{
	class ChecklistInfo
	{
		public ChecklistInfo(string name)
		{
			Name = name;
		}
		public string Name { get; private set; }
	}

	class ChecklistHelper
	{
		public static async void ChecklistButtonClicked(object sender, EventArgs e)
		{
			ChecklistButton button = (ChecklistButton)sender;
			ChecklistPage page = new ChecklistPage(button.checklist);
			await App.Navigation.PushAsync(page);
		}
		/// <summary>
		/// Saves the checklist to the database, calling section and question saves recursively.
		/// </summary>
		/// <param name="checklist">The checklist to be saved to the database.</param>
		/*public static void SaveChecklistToDatabase(ChecklistModel checklist)
		{
			DatabaseAccess database = App.database;
			database.SaveChecklist(checklist);
			/*foreach (Section section in checklist.Sections)
			{
				section.checklist = checklist;
				section.ChecklistId = checklist.Id;
				database.SaveSection(section);
				foreach (SectionPart part in section.SectionParts)
				{
					part.section = section;
					part.SectionId = section.Id;
					database.SavePart(part);
					foreach (Question question in part.Questions)
					{
						question.part = part;
						question.SectionPartId = part.Id;
						question.section = section;
						question.SectionId = section.Id;
						database.SaveQuestion(question);
					}
				}
				foreach (Question question in section.Questions)
				{
					question.section = section;
					question.SectionId = section.Id;
					database.SaveQuestion(question);
				}
			}*//*
		}*/
		public static ChecklistModel LoadChecklistDetails(string checklistId)
		{
			DatabaseAccess database = App.database;
			ChecklistModel checklist = database.LoadChecklist(checklistId);
			/*checklist.Inspections = database.LoadInspectionsForChecklist(checklistId).ToList();
			checklist.Sections = database.LoadSectionsForChecklist(checklistId).ToList();
			foreach (Section section in checklist.Sections)
			{
				section.SectionParts = database.LoadPartsForSection(section.Id).ToList();
				if (section.SectionParts.Count == 0)
				{
					section.Questions = database.LoadQuestionsForSection(section.Id).ToList();
				}
				else
				{
					foreach (SectionPart part in section.SectionParts)
					{
						part.Questions = database.LoadQuestionsForPart(part.Id).ToList();
					}
				}
			}*/
			return checklist;
		}
	}

	class ChecklistButton : Button
	{
		public ChecklistModel checklist
		{
			get
			{
				return (ChecklistModel)GetValue(ChecklistProperty);
			}
			set
			{
				SetValue(ChecklistProperty, value);
			}
		}

		public static readonly BindableProperty ChecklistProperty =
			BindableProperty.Create<ChecklistButton, ChecklistModel>
			(p => p.checklist,null);
	}
}
