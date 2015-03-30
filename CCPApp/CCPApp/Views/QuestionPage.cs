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
		Editor remarksBox;
		Label existingAnswerLabel = new Label();
		public QuestionPage(Question question, Inspection inspection, string textOverride = null, List<Reference> extraReferences = null)
		{
			this.question = question;
			this.inspection = inspection;
			StackLayout layout = new StackLayout
			{
				Padding = new Thickness(20,0),
				Spacing = 0,
				VerticalOptions = LayoutOptions.Center,
			};

			Label SectionLabel = new Label
			{
				Text = "Section "+question.section.Label+": "+question.section.Title,
				HorizontalOptions = LayoutOptions.Center,
			};
			layout.Children.Add(SectionLabel);
			if (question.SectionPartId != null)
			{
				Label PartLabel = new Label
				{
					Text = "Part "+question.part.Label+": "+question.part.Description,
					HorizontalOptions = LayoutOptions.Center,
				};
				layout.Children.Add(PartLabel);
			}

			//Question number
			Label questionNumberlabel = new Label
			{
				Text = "Question "+question.numberString,
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
			score = inspection.GetScoreForQuestion(question);
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

			//References buttons
			List<Reference> references = question.References;	
			if (extraReferences != null)
			{		//Creates a copy of the list so we aren't adding to the original.
				references = references.ToList();
				references.AddRange(extraReferences);	
			}
			foreach (Reference reference in references)
			{
				ReferenceButton referenceButton = new ReferenceButton(reference);
				referenceButton.folderName = inspection.ChecklistId;
				layout.Children.Add(referenceButton);
			}

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

			//Remarks label
			Label remarksLabel = new Label();
			remarksLabel.Text = "Remarks:";
			layout.Children.Add(remarksLabel);
			//Remarks box
			remarksBox = new Editor();
			remarksBox.Text = question.Remarks;
			remarksBox.HeightRequest = 50;
			question.OldRemarks = question.Remarks;
			remarksBox.TextChanged += SaveRemarksText;
			layout.Children.Add(remarksBox);

			ScrollView scroll = new ScrollView();
			scroll.Content = layout;

			Content = scroll;
		}
		protected async void SaveRemarksText(object Sender, EventArgs e)
		{
			await Task.Run(() => question.Remarks = remarksBox.Text);
		}
		protected override void OnDisappearing()
		{
			if (question.Remarks != question.OldRemarks)
			{
				App.database.SaveQuestion(question);
			}
			base.OnDisappearing();
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
				string referenceName = inspection.ChecklistId + "/" + question.References.First().DocumentName;
				string bookmark = question.References.First().Bookmark;
				int pageNumber = 1;
				if (bookmark != string.Empty)
				{
					pageNumber = int.Parse(bookmark);
				}
				ReferencePage page = new ReferencePage(referenceName, pageNumber);
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
	internal class ReferenceButton : Button
	{
		public Reference reference { get; set; }
		public string folderName { get; set; }
		public ReferenceButton(Reference reference)
		{
			this.reference = reference;
			this.Text = reference.Description;
			this.Clicked += openReferencePage;
		}

		void openReferencePage(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				string referenceName = folderName + "/" + reference.DocumentName;
				string bookmark = reference.Bookmark;
				int pageNumber = 1;
				if (bookmark != string.Empty)
				{
					pageNumber = int.Parse(bookmark);
				}
				ReferencePage page = new ReferencePage(referenceName, pageNumber);
				await App.Navigation.PushAsync(page);
			});
		}
	}
}
