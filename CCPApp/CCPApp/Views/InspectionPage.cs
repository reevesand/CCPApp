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
					Children.Add(page);
				}
				else
				{
					SectionNoPartsPage page = new SectionNoPartsPage(section, inspection);
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
		private void ClickChecklistInfo()
		{
			DisplayAlert("Checklist Information", "You are viewing the checklist\n"+inspection.Checklist.Title, "OK");
		}
		private async void ClickActionsButton(object sender, EventArgs e)
		{
			string action = await DisplayActionSheet("Inspection Actions", "Cancel", null, "Generate Report", "Disputed Questions", "Unanswered Questions", "View Scores","Generate Outbriefing", "Checklist Information","Cancel");
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
			case "Checklist Information":
				ClickChecklistInfo();
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
		/// <summary>
		/// Checks to see if the icon needs to be updated and, if so, updates it.
		/// </summary>
		/// <param name="answered">True if a question has just been answered, false if one has just been cleared</param>
		void UpdateIcon(bool answered);
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
			if (section.AllScorableQuestions().Count == inspection.scores.Count(s => s.question.section == section))
			{
				Icon = "Checkmark2.png";
			}
			else
			{
				Icon = "TabIconGreenNoBG.png";
			}
		}
		public void Initialize()
		{
			if (!initialized)
			{
				initialized = true;
				foreach (SectionPart part in section.SectionParts)
				{
					PartPage page = new PartPage(part, inspection,this);
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
		public void UpdateIcon(bool answered)
		{
			((PartPage)CurrentPage).UpdateIcon(answered);
			if (!answered)
			{
				Icon = "TabIconGreenNoBG.png";
				return;
			}
			if (section.AllScorableQuestions().Count == inspection.scores.Count(s => s.question.section == section))
			{
				Icon = "Checkmark2.png";
			}
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
			if (section.AllScorableQuestions().Count == inspection.scores.Count(s => s.question.section == section))
			{
				Icon = "Checkmark2.png";
			}
			else
			{
				Icon = "TabIconGreenNoBG.png";
			}
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
				
				List<QuestionPage> pages = InspectionHelper.GenerateQuestionPages(section.Questions, inspection,this);
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
		public void UpdateIcon(bool answered)
		{
			if (!answered)
			{
				//They cleared one.  Clearly the checklist is still in progress.  Currently we only have two states.
				//Set icon to in progress if it's not already.
				Icon = "TabIconGreenNoBG.png";
				return;
			}
			if (Children.Cast<QuestionPage>().All(p => p.HasScore))
			{
				Icon = "Checkmark2.png";
			}
			//set icon to done if it's not already.
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
		public PartPage(SectionPart part, Inspection inspection, SectionWithPartsPage sectionPage)
		{
			this.part = part;
			this.inspection = inspection;
			Title = "Part " + part.Label;
			Icon = "TabIconGreenNoBG.png";
			List<QuestionPage> pages = InspectionHelper.GenerateQuestionPages(part.Questions, inspection, sectionPage);
			foreach (ContentPage page in pages)
			{
				Children.Add(page);
			}
			UpdateIcon(true);
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
		public void UpdateIcon(bool answered)
		{
			if (!answered)
			{
				Icon = "TabIconGreenNoBG.png";
			}
			if (Children.Cast<QuestionPage>().All(p => p.HasScore))
			{
				Icon = "Checkmark2.png";
			}
		}
	}
}
