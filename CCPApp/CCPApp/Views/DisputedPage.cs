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
			IEnumerable<Question> questions = inspection.scores.Where(s => s.answer == Answer.Disputed).Select(s => s.question);

			ListView view = new ListView();
			view.ItemsSource = questions;
			view.ItemTemplate = new DataTemplate(() =>
			{
				GoToQuestionButton button = new GoToQuestionButton(inspectionPage);
				button.SetBinding(Button.TextProperty, "FullString");
				button.SetBinding(GoToQuestionButton.QuestionProperty, "SelfReference");
				ViewCell cell = new ViewCell();
				cell.View = button;
				return cell;
			});
			Content = view;

			//TableView table = new TableView();
			//table.Intent = TableIntent.Settings;
			//TableSection section = new TableSection();

			/*foreach (ScoredQuestion score in disputedQuestions)
			{
				Question question = score.question;
				GoToQuestionButton button = new GoToQuestionButton(question,inspectionPage);
				button.Text = question.ToString() + " " + question.Text;
				ViewCell cell = new ViewCell();
				cell.View = button;
				section.Add(cell);
			}*/
			//table.Root.Add(section);
			//this.Content = table;
		}
	}
}
