using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class UnansweredPage : ContentPage
	{
		public Inspection inspection { get; set; }
		InspectionPage inspectionPage { get; set; }
		public UnansweredPage(Inspection inspection, InspectionPage page)
		{
			this.inspection = inspection;
			inspectionPage = page;
			List<Question> questions = new List<Question>();
			foreach (Section section in inspection.Checklist.Sections)
			{
				questions.AddRange(section.AllScorableQuestions());
			}
			List<ScoredQuestion> scoredQuestions = inspection.scores;
			foreach (ScoredQuestion score in scoredQuestions)
			{
				questions.RemoveAll(q => q.Id == score.QuestionId);
			}
			TableView table = new TableView();
			TableSection tableSection = new TableSection();

			foreach (Question question in questions)
			{
				GoToQuestionButton button = new GoToQuestionButton(question, inspectionPage);
				button.Text = question.ToString() + " " + question.Text;
				ViewCell cell = new ViewCell();
				cell.View = button;
				tableSection.Add(cell);
			}
			table.Root.Add(tableSection);
			this.Content = table;
		}
	}
}
