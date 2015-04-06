using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	/// <summary>
	/// This class contains the design for the meat of the program, the answering of questions.
	/// </summary>
	public class InspectionPage : TabbedPage
	{
		private Inspection inspection;		
		public InspectionPage(Inspection inspection)
		{
			this.inspection = inspection;

			/*ToolbarItem scoreButton = new ToolbarItem();
			scoreButton.Text = "Scores";
			//scoreButton.Icon = "ScoresIcon.png";
			scoreButton.Clicked += ClickScoresButton;

			ToolbarItem unansweredButton = new ToolbarItem();
			unansweredButton.Text = "Unanswered";
			unansweredButton.Clicked += ClickUnansweredButton;
			ToolbarItem disputedButton = new ToolbarItem();
			disputedButton.Text = "Disputed";
			disputedButton.Clicked += ClickDisputedButton;

			ToolbarItem reportButton = new ToolbarItem();
			reportButton.Text = "Report";
			reportButton.Clicked += ClickReportButton;*/

			ToolbarItem actionsButton = new ToolbarItem();
			actionsButton.Text = "Actions";
			actionsButton.Clicked += ClickActionsButton;

			//ToolbarItems.Add(scoreButton);
			//ToolbarItems.Add(unansweredButton);
			//ToolbarItems.Add(disputedButton);
			//ToolbarItems.Add(reportButton);
			ToolbarItems.Add(actionsButton);
			ChecklistModel checklist = inspection.Checklist;
			foreach (SectionModel section in checklist.Sections)
			{
				if (section.SectionParts.Count > 0)
				{
					SectionWithPartsPage page = new SectionWithPartsPage(section, inspection);
					page.Icon = "TabIconGreenNoBG.png";
					Children.Add(page);
				}
				else
				{
					SectionNoPartsPage page = new SectionNoPartsPage(section, inspection);
					page.Icon = "TabIconGreenNoBG.png";
					Children.Add(page);
				}
			}
			if (inspection.GetLastViewedQuestion() == null)
			{
				inspection.SetLastViewedQuestion(checklist.Sections.First().AllScorableQuestions().First());
			}
			this.CurrentPageChanged += this.PageChanged;
			Question targetQuestion = inspection.GetLastViewedQuestion();
			ISectionPage targetPage = SetSectionPage(targetQuestion.section);
			targetPage.Initialize();
			targetPage.SetSelectedQuestion(targetQuestion);
		}
		private void PageChanged(object sender, EventArgs e)
		{
			if (CurrentPage == null)
			{	//clicking "More" can yield a null page.
				return;
			}
			ISectionPage page = (ISectionPage)CurrentPage;
			page.Initialize();
			inspection.SetLastViewedQuestion(page.GetCurrentQuestion());
		}

		private async void ClickScoresButton()
		{
			Question question = ((ISectionPage)CurrentPage).GetCurrentQuestion();
			ScoresPage page = new ScoresPage(inspection, question);
			await App.Navigation.PushAsync(page);
		}
		private void ClickUnansweredButton()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				UnansweredPage page = new UnansweredPage(inspection, this);
				await App.Navigation.PushAsync(page);
			});
		}
		private void ClickDisputedButton()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				DisputedPage page = new DisputedPage(inspection, this);
				await App.Navigation.PushAsync(page);
			});
		}
		private void ClickReportButton()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				//Pop up a loading page if this proves to be slow.
				//string generatedReport = ReportPage.GeneratePdf(inspection);
				//ReportPage page = new ReportPage(generatedReport);
				PrepareReportPage page = new PrepareReportPage(inspection);
				await App.Navigation.PushAsync(page);
			});
		}
		private void ClickOutbriefingButton()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				OutbriefingPage page = new OutbriefingPage(inspection);
				await App.Navigation.PushAsync(page);
			});
		}
		private async void ClickActionsButton(object sender, EventArgs e)
		{
			string action = await DisplayActionSheet("Inspection Actions", "Cancel", null, "Generate Report", "Disputed Questions", "Unanswered Questions", "View Scores","Generate Outbriefing","Cancel");
			switch (action)
			{
			case "Generate Report":
				ClickReportButton();
				break;
			case "Disputed Questions":
				ClickDisputedButton();
				break;
			case "Unanswered Questions":
				ClickUnansweredButton();
				break;
			case "View Scores":
				ClickScoresButton();
				break;
			case "Generate Outbriefing":
				ClickOutbriefingButton();
				break;
			}
		}

		public ISectionPage SetSectionPage(SectionModel section)
		{
			this.CurrentPage = this.Children.Single(s => ((ISectionPage)s).GetCurrentSection().Id == section.Id);
			return (ISectionPage)CurrentPage;
		}

	}
	public interface ISectionPage
	{
		void Initialize();
		SectionModel GetCurrentSection();
		Question GetCurrentQuestion();
		void SetSelectedQuestion(Question question);
	}
	internal class SectionWithPartsPage : TabbedPage, ISectionPage
	{
		SectionModel section;
		bool initialized = false;
		Inspection inspection;
		public SectionWithPartsPage(SectionModel section, Inspection inspection)
		{
			this.section = section;
			this.inspection = inspection;
			Title = section.ShortTitle;
			Icon = "TabIconGreenNoBG.png";
			/*foreach (SectionPart part in section.SectionParts)
			{
				PartPage page = new PartPage(part);
				Children.Add(page);
			}*/
		}
		public void Initialize()
		{
			if (!initialized)
			{
				initialized = true;
				foreach (SectionPart part in section.SectionParts)
				{
					PartPage page = new PartPage(part, inspection);
					Children.Add(page);
				}
			}
		}
		public Question GetCurrentQuestion()
		{
			return ((QuestionPage)((PartPage)CurrentPage).CurrentPage).question;
		}
		public void SetSelectedQuestion(Question question)
		{
			this.CurrentPage = this.Children.Single(p => ((PartPage)p).GetPart().Id == question.part.Id);
			PartPage partPage = (PartPage)this.CurrentPage;
			partPage.CurrentPage = partPage.Children.Single(q => ((QuestionPage)q).question.Id == question.Id);
		}
		public SectionModel GetCurrentSection()
		{
			return section;
		}
		protected override void OnCurrentPageChanged()
		{
			inspection.SetLastViewedQuestion(GetCurrentQuestion());
			base.OnCurrentPageChanged();
		}
	}
	internal class SectionNoPartsPage : CarouselPage, ISectionPage
	{
		SectionModel section;
		bool initialized = false;
		Inspection inspection;
		public SectionNoPartsPage(SectionModel section, Inspection inspection)
		{
			this.section = section;
			this.inspection = inspection;
			Title = section.ShortTitle;
			Icon = "TabIconGreenNoBG.png";
			/*foreach (Question question in section.Questions)
			{
				QuestionPage page = new QuestionPage(question);
				Children.Add(page);
			}*/
		}
		public void Initialize()
		{
			if (!initialized)
			{
				initialized = true;
				
				List<QuestionPage> pages = InspectionHelper.GenerateQuestionPages(section.Questions, inspection);
				foreach (ContentPage page in pages)
				{
					Children.Add(page);
				}
			}
		}
		public Question GetCurrentQuestion()
		{
			return ((QuestionPage)CurrentPage).question;
		}
		public void SetSelectedQuestion(Question question)
		{
			this.CurrentPage = this.Children.Single(q => ((QuestionPage)q).question.Id == question.Id);
		}
		public SectionModel GetCurrentSection()
		{
			return section;
		}
		protected override void OnCurrentPageChanged()
		{
			inspection.SetLastViewedQuestion(GetCurrentQuestion());
			base.OnCurrentPageChanged();
		}
	}

	internal class PartPage : CarouselPage
	{
		SectionPart part;
		Inspection inspection;
		public PartPage(SectionPart part, Inspection inspection)
		{
			this.part = part;
			this.inspection = inspection;
			Title = "Part " + part.Label;
			this.Icon = "TabIconGreenNoBG.png";
			List<QuestionPage> pages = InspectionHelper.GenerateQuestionPages(part.Questions, inspection);
			foreach (ContentPage page in pages)
			{
				Children.Add(page);
			}
			this.CurrentPageChanged += PartPage_CurrentPageChanged;
		}
		public SectionPart GetPart()
		{
			return part;
		}
		protected void PartPage_CurrentPageChanged(object sender, EventArgs e)
		{
			inspection.SetLastViewedQuestion(((QuestionPage)CurrentPage).question);
		}
	}
}
