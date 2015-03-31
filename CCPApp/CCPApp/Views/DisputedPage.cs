using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class DisputedPage : ContentPage
	{
		public Inspection inspection { get; set; }
		InspectionPage inspectionPage { get; set; }
		public DisputedPage(Inspection inspection, InspectionPage page)
		{
			this.inspection = inspection;
			inspectionPage = page;
			List<ScoredQuestion> disputedQuestions = inspection.scores.Where(s => s.answer == Answer.Disputed).ToList();
			TableView table = new TableView();
			table.Intent = TableIntent.Settings;
			TableSection section = new TableSection();

			foreach (ScoredQuestion score in disputedQuestions)
			{
				Question question = score.question;
				GoToQuestionButton button = new GoToQuestionButton(question,inspectionPage);
				button.Text = question.ToString() + " " + question.Text;
				ViewCell cell = new ViewCell();
				cell.View = button;
				section.Add(cell);
			}
			table.Root.Add(section);
			this.Content = table;
		}
	}
}
