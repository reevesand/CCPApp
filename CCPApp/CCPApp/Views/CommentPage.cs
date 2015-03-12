using CCPApp.Items;
using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class CommentPage : ContentPage
	{
		public Inspection inspection;
		Label commentIndicatorLabel;
		Editor subjectTextEditor;
		GenericPicker<CommentType> commentTypePicker;
		GenericPicker<Question> questionPicker;
		Comment existingComment = null;
		Editor commentText;
		//ResizingLayout layout;

		public CommentPage(Inspection inspection, Question initialQuestion)
		{
			this.inspection = inspection;
			ChecklistModel checklist = inspection.Checklist;
			this.Title = "Add/Edit Comment";

			StackLayout layout = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};
			//Choose comment type
			Label chooseCommentTypeLabel = new Label{Text="Choose comment type"};
			commentTypePicker = new GenericPicker<CommentType>();
			commentTypePicker.AddItem(CommentType.Finding);
			commentTypePicker.AddItem(CommentType.Observation);
			commentTypePicker.AddItem(CommentType.Commendable);
			commentTypePicker.SelectedIndexChanged += SelectCommentType;

			//Choose question
			Label chooseQuestionLabel = new Label{Text="Choose question"};
			questionPicker = new GenericPicker<Question>();
			foreach (Question question in checklist.GetAllQuestions())
			{
				questionPicker.AddItem(question);
			}
			questionPicker.SelectedIndexChanged += SelectQuestion;

			//Comment description
			Label commentSubjectLabel = new Label{Text="Comment subject"};
			subjectTextEditor = new Editor();

			//Enter comment
			commentIndicatorLabel = new Label{Text="Comment:"};
			commentText = new Editor();
			//commentText.TextChanged += CommentTextChanged;

			//Choose date
			Label chooseDateLabel = new Label{Text="Date:"};
			DatePicker date = new DatePicker();

			//Save button
			Button saveButton = new Button { Text = "Save Comment" };
			saveButton.Clicked += SaveComment;
			//TODO: choose inspector

			//Perform the setup actions.
			commentTypePicker.SelectedIndex = 0;
			questionPicker.SelectedIndex = questionPicker.TItems.IndexOf(initialQuestion);

			layout.Children.Add(chooseCommentTypeLabel);
			layout.Children.Add(commentTypePicker);
			layout.Children.Add(chooseQuestionLabel);
			layout.Children.Add(questionPicker);
			layout.Children.Add(commentSubjectLabel);
			layout.Children.Add(subjectTextEditor);
			layout.Children.Add(commentIndicatorLabel);
			layout.Children.Add(commentText);
			layout.Children.Add(chooseDateLabel);
			layout.Children.Add(date);
			layout.Children.Add(saveButton);
			this.Content = layout;
		}

		void SaveComment(object sender, EventArgs e)
		{
			if (existingComment == null)
			{
				existingComment = new Comment();
			}
			Question question = questionPicker.SelectedItem;
			existingComment.QuestionId = (int)question.Id;
			existingComment.question = question;
			existingComment.inspection = inspection;
			if (!inspection.comments.Contains(existingComment))
			{
				inspection.comments.Add(existingComment);
			}
			existingComment.CommentText = commentText.Text;
			existingComment.Subject = subjectTextEditor.Text;
			existingComment.type = commentTypePicker.SelectedItem;
			App.database.SaveComment(existingComment);
			Device.BeginInvokeOnMainThread(async () =>
			{
				await App.Navigation.PopAsync();
			});
		}
		private void SelectCommentType(object sender, EventArgs e)
		{
			//Update commentIndicatorLabel
			string commentType = commentTypePicker.Items[commentTypePicker.SelectedIndex];
			commentIndicatorLabel.Text = commentType+":";
			ProcessExistingComment();
		}
		private void SelectQuestion(object sender, EventArgs e)
		{
			Question question = questionPicker.SelectedItem;
			subjectTextEditor.Text = question.Text;
			ProcessExistingComment();
		}
		private void ProcessExistingComment()
		{
			if (questionPicker.SelectedIndex < 0 || questionPicker.SelectedIndex > questionPicker.Items.Count)
			{	//Things aren't initialized properly yet.
				return;
			}
			Question question = questionPicker.SelectedItem;
			List<Comment> commentsForQuestion = inspection.comments.Where(c => c.question == question).ToList();
			CommentType type = commentTypePicker.SelectedItem;
			existingComment = commentsForQuestion.SingleOrDefault(c => c.type == type);
			if (existingComment == null)
			{
				commentText.Text = "";
			}
			else
			{
				commentText.Text = existingComment.CommentText;
				subjectTextEditor.Text = existingComment.Subject;
			}
		}
		//private void CommentTextChanged(object sender, EventArgs e)
		//{
			//layout.OnChildChanged();
		//}
	}
}
