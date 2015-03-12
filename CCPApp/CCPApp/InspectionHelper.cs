using CCPApp.Models;
using CCPApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp
{
	public class InspectionHelper
	{
		public static async void CreateInspectionButtonClicked(object sender, EventArgs e)
		{
			InspectionButton button = (InspectionButton)sender;
			Inspection inspection = button.inspection;
			CreateInspectionPage page = new CreateInspectionPage();
			page.inspection = inspection;
			page.CallingPage = (ChecklistPage)button.ParentView.ParentView;
			await App.Navigation.PushModalAsync(page);
		}
		public static async void SelectInspectionButtonClicked(object sender, EventArgs e)
		{
			InspectionButton button = (InspectionButton)sender;
			Inspection inspection = button.inspection;
			InspectionPage page = new InspectionPage(inspection);
			await App.Navigation.PushAsync(page);
		}

		internal static List<QuestionPage> GenerateQuestionPages(List<Question> questions, Inspection inspection)
		{
			List<QuestionPage> pages = new List<QuestionPage>();
			string MasterQuestionText = string.Empty;
			int MasterQuestionNumber = -2;
			foreach (Question question in questions)
			{
				QuestionPage page = null;
				if (question.HasSubItems)
				{
					MasterQuestionNumber = question.Number;
					MasterQuestionText = question.Text.Trim();
				}
				else if (question.Number == MasterQuestionNumber)
				{
					page = new QuestionPage(question, inspection, MasterQuestionText + "\n" + question.Text.Trim());
				}
				else
				{
					page = new QuestionPage(question, inspection);
				}
				if (page != null)
				{
					pages.Add(page);
				}
			}
			return pages;
		}
	}

	public class InspectionButton : Button
	{
		public InspectionButton(Inspection inspection)
		{
			this.inspection = inspection;
			Text = inspection.Name;
		}
		public Inspection inspection { get; set; }
	}

	public class CreateInspectionPage : ContentPage
	{
		public TableSection tableSection { get; set; }
		public Inspection inspection { get; set; }
		private EntryCell NameCell { get; set; }
		public ChecklistPage CallingPage { get; set; }
		public CreateInspectionPage()
		{
			Title = "Create new Inspection";
			TableView view = new TableView();
			TableRoot root = new TableRoot("Create New Inspection");
			TableSection section = new TableSection();
			Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
			NameCell = new EntryCell
			{
				BindingContext = inspection,
				Text = "Inspection Name:",
			};
			NameCell.SetBinding(EntryCell.TextProperty, "Name");
			ViewCell SaveCell = new ViewCell();
			ViewCell CancelCell = new ViewCell();
			Button saveButton = new Button();
			Button cancelButton = new Button();
			saveButton.Clicked += SaveInspectionClicked;
			cancelButton.Clicked += CancelInspectionClicked;
			saveButton.Text = "Save";
			cancelButton.Text = "Cancel";
			SaveCell.View = saveButton;
			CancelCell.View = cancelButton;

			section.Add(NameCell);
			section.Add(SaveCell);
			section.Add(CancelCell);
			root.Add(section);
			view.Root = root;
			Content = view;
		}

		public async void SaveInspectionClicked(object sender, EventArgs e)
		{
			ChecklistModel checklist = inspection.Checklist;
			inspection.Name = NameCell.Text;
			inspection.ChecklistId = checklist.Id;
			checklist.Inspections.Add(inspection);
			App.database.SaveInspection(inspection);

			/*ViewCell cell = new ViewCell();
			Button button = new InspectionButton(inspection);
			button.Clicked += InspectionHelper.SelectInspectionButtonClicked;
			cell.View = button;
			tableSection.Insert(tableSection.Count - 1, cell);*/

			CallingPage.ResetInspections();

			await App.Navigation.PopModalAsync(true);
		}
		public async void CancelInspectionClicked(object sender, EventArgs e)
		{
			await App.Navigation.PopModalAsync(true);
		}
	}
	public class GoToQuestionButton : Button
	{
		Question question { get; set; }
		InspectionPage inspectionPage { get; set; }
		public GoToQuestionButton(Question question, InspectionPage inspectionPage)
		{
			this.question = question;
			this.inspectionPage = inspectionPage;
			this.Clicked += clickQuestionButton;
		}

		private async static void clickQuestionButton(object sender, EventArgs e)
		{
			GoToQuestionButton button = (GoToQuestionButton)sender;
			ISectionPage sectionPage = button.inspectionPage.SetSectionPage(button.question.section);
			sectionPage.SetSelectedQuestion(button.question);

			await App.Navigation.PopAsync();
		}
	}
}
