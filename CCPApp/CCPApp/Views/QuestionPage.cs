using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class QuestionPage : ContentPage
	{
		public Question question;
		Inspection inspection;
		ScoredQuestion score;
		Label existingAnswerLabel = new Label();
		public QuestionPage(Question question, Inspection inspection, string textOverride = null)
		{
			this.question = question;
			this.inspection = inspection;
			StackLayout layout = new StackLayout
			{
				Padding = new Thickness(20,0),
				Spacing = 0,
				VerticalOptions = LayoutOptions.Center,
			};

			//Question number
			Label questionNumberlabel = new Label
			{
				Text = "Question "+question.ToString(),
				HorizontalOptions = LayoutOptions.Center
			};
			layout.Children.Add(questionNumberlabel);

			//Question text
			Label questionTextLabel = new Label();
			if (textOverride == null)
			{
				questionTextLabel.Text = question.Text.Trim();
			}
			else
			{
				questionTextLabel.Text = textOverride;
			}
			layout.Children.Add(questionTextLabel);

			//Answer
			score = App.database.LoadScoreForQuestion(inspection, question);
			if (score != null) {
				existingAnswerLabel.Text = "Answer: " + score.answer.ToString();
			}
			else
			{
				existingAnswerLabel.Text = "";
			}
			layout.Children.Add(existingAnswerLabel);

			//Add Edit Comment Button
			Button commentButton = new Button();
			commentButton.Text = "Add/Edit Comment For Question";
			commentButton.Clicked += openCommentPage;
			layout.Children.Add(commentButton);

			//References button
			//TODO change to be better and stuff
			Button referenceButton = new Button();
			referenceButton.Text = "Reference Page";
			referenceButton.Clicked += openReferencePage;
			layout.Children.Add(referenceButton);

			//Answer buttons
			List<AnswerButton> answerButtons = new List<AnswerButton>();
			foreach (Answer answer in Enum.GetValues(typeof(Answer)))
			{
				AnswerButton button = new AnswerButton(answer);
				button.Text = answer.ToString();
				button.Clicked += AnswerQuestion;
				layout.Children.Add(button);
			}

			//Clear scores button
			Button clearScoresButton = new Button
			{
				Text = "Clear Scores"
			};
			clearScoresButton.Clicked += clearScores;
			layout.Children.Add(clearScoresButton);

			Content = layout;
		}

		private void AnswerQuestion(object sender, EventArgs e)
		{
			AnswerButton button = (AnswerButton)sender;
			if (score == null)
			{
				score = new ScoredQuestion();
			}
			score.QuestionId = (int)question.Id;
			score.question = question;
			score.inspection = inspection;
			if (!inspection.scores.Contains(score))
			{
				inspection.scores.Add(score);
			}
			score.answer = button.answer;
			App.database.SaveScore(score);
			existingAnswerLabel.Text = "Answer: " + score.answer.ToString();
		}
		private void clearScores(object sender, EventArgs e)
		{
			if (score != null)
			{
				inspection.scores.Remove(score);
				App.database.DeleteScore(score);
			}
			existingAnswerLabel.Text = "";
		}
		private void openCommentPage(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				CommentPage page = new CommentPage(inspection, question);
				await App.Navigation.PushAsync(page);
			});
		}
		private void openReferencePage(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				ReferencePage page = new ReferencePage();
				await App.Navigation.PushAsync(page);
			});
		}
	}
	internal class AnswerButton : Button
	{
		public Answer answer;
		public AnswerButton(Answer answer)
		{
			this.answer = answer;
		}
	}
}
