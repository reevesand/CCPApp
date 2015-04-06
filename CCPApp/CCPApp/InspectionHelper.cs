using CCPApp.Items;
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
			CreateInspectionButton button = (CreateInspectionButton)sender;
			ChecklistModel checklist = button.checklist;
			EditInspectionPage page = new EditInspectionPage(null,checklist);
			page.CallingPage = (ChecklistPage)button.ParentView.ParentView;
			await App.Navigation.PushAsync(page);
		}
		public static void SelectInspectionButtonClicked(object sender, EventArgs e)
		{
			InspectionButton button = (InspectionButton)sender;
			Inspection inspection = button.inspection;
			Device.BeginInvokeOnMainThread(async () =>
			{
				InspectionPage page = new InspectionPage(inspection);
				await App.Navigation.PushAsync(page);
			});
		}

		internal static List<QuestionPage> GenerateQuestionPages(List<Question> questions, Inspection inspection)
		{
			List<QuestionPage> pages = new List<QuestionPage>();
			string MasterQuestionText = string.Empty;
			List<Reference> MasterQuestionReferences = new List<Reference>();
			int MasterQuestionNumber = -2;
			foreach (Question question in questions)
			{
				QuestionPage page = null;
				if (question.HasSubItems)
				{
					MasterQuestionNumber = question.Number;
					MasterQuestionText = question.Text.Trim();
					MasterQuestionReferences = question.References;
				}
				else if (question.Number == MasterQuestionNumber)
				{
					page = new QuestionPage(question, inspection, MasterQuestionText + "\n" + question.Text.Trim(),MasterQuestionReferences);
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
	public class CreateInspectionButton : Button
	{
		public ChecklistModel checklist { get; set; }
		public CreateInspectionButton(ChecklistModel checklist)
		{
			this.checklist = checklist;
		}
	}

	
	public class GoToQuestionButton : Button
	{
		//Question question { get; set; }
		InspectionPage inspectionPage { get; set; }
		public GoToQuestionButton(Question question, InspectionPage inspectionPage)
		{
			this.question = question;
			this.inspectionPage = inspectionPage;
			this.Clicked += clickQuestionButton;
		}
		public GoToQuestionButton(InspectionPage inspectionPage)
		{
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
		
		public Question question
		{
			get
			{
				return (Question)GetValue(QuestionProperty);
			}
			set
			{
				SetValue(QuestionProperty, value);
			}
		}

		public static readonly BindableProperty QuestionProperty =
			BindableProperty.Create<GoToQuestionButton, Question>
			(p => p.question, null);
	}
}
