using CCPApp.Items;
using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class ScoresPage : ContentPage
	{
		public Inspection inspection;
		Label sectionScoreLabel;
		Label partScoreLabel;
		GenericPicker<SectionPart> partPicker;
		Threshold scoresThreshold;
		public ScoresPage(Inspection inspection, Question initialQuestion)
		{
			this.inspection = inspection;
			ChecklistModel checklist = inspection.Checklist;
			scoresThreshold = new Threshold(checklist.ScoreThresholdCommendable, checklist.ScoreThresholdSatisfactory);
			Padding = new Thickness(0, Device.OnPlatform(20,0,0), 0, 0);

			this.Title = "Scores";
			ScoresLayout layout = new ScoresLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};
			Label label = new Label
			{
				Text = "Choose section and part to see scores"
			};
			//add all the other stuff.
			GenericPicker<Section> sectionPicker = new GenericPicker<Section>();
			foreach (Section section in inspection.Checklist.Sections)
			{
				sectionPicker.AddItem(section);
			}
			partPicker = new GenericPicker<SectionPart>();
			foreach (SectionPart part in initialQuestion.section.SectionParts)
			{
				partPicker.AddItem(part);
			}
			Button backButton = new Button
			{
				Text = "Back"
			};
			sectionScoreLabel = new Label
			{
				Text = "Section label",
				TextColor = Color.White
			};
			partScoreLabel = new Label
			{
				Text = "Part label",
				TextColor = Color.White
			};
			backButton.Clicked += BackButtonClicked;
			layout.Children.Add(label);
			layout.Children.Add(sectionPicker);
			layout.Children.Add(sectionScoreLabel);
			layout.Children.Add(partPicker);
			layout.Children.Add(partScoreLabel);
			float cummulativeScore = ScoreInspection(inspection);
			Label cummulativeScoreLabel = new Label
			{
				Text = "Cummulative Score: " + (cummulativeScore * 100).ToString("0.00") + "%",
				TextColor = Color.White
			};
			setScoresColor(cummulativeScore, cummulativeScoreLabel);
			layout.Children.Add(cummulativeScoreLabel);
			layout.Children.Add(backButton);

			this.Content = layout;

			sectionPicker.SelectedIndexChanged += ChangeSection;
			sectionPicker.SelectedIndex = sectionPicker.TItems.IndexOf(initialQuestion.section);
			partPicker.SelectedIndexChanged += ChangePart;
			if (initialQuestion.part != null)
			{
				partPicker.SelectedIndex = partPicker.TItems.IndexOf(initialQuestion.part);
			}
		}
		public void closePage()
		{
			App.Navigation.PopAsync();
		}
		private void ChangeSection(object sender, EventArgs e)
		{
			GenericPicker<Section> picker = (GenericPicker<Section>)sender;
			Section section = picker.SelectedItem;
			float sectionScore = ScoreSection(section, inspection);
			sectionScoreLabel.Text = "Section score: " + (sectionScore * 100).ToString("0.00") + "%";
			setScoresColor(sectionScore, sectionScoreLabel);
			if (section.SectionParts.Count == 0)
			{
				partPicker.IsVisible = false;
				partScoreLabel.IsVisible = false;
			}
			else
			{
				partPicker.SelectedIndexChanged -= ChangePart;
				partPicker.IsVisible = true;
				partScoreLabel.IsVisible = true;
				partPicker.ClearItems();
				foreach (SectionPart part in section.SectionParts)
				{
					partPicker.AddItem(part);
				}
				partPicker.SelectedIndexChanged += ChangePart;
				partPicker.SelectedIndex = 0;
			}
		}
		private void ChangePart(object sender, EventArgs e)
		{
			GenericPicker<SectionPart> picker = (GenericPicker<SectionPart>)sender;
			if (picker.SelectedIndex >= picker.Items.Count || picker.SelectedIndex < 0)
			{
				//this is a fix for a bug where the event listener is still listening even though it's been unassigned.
				return;
			}
			SectionPart part = picker.SelectedItem;
			float partScore = ScorePart(part, inspection);
			partScoreLabel.Text = "Part score: " + (partScore * 100).ToString("0.00") + "%";
			setScoresColor(partScore, partScoreLabel);
		}
		private void setScoresColor(float score, VisualElement element)
		{
			float percentScore = score * 100;
			if (percentScore >= scoresThreshold.Commendable)
			{
				element.BackgroundColor = Color.Blue;
			}
			else if (percentScore >= scoresThreshold.Satisfactory)
			{
				element.BackgroundColor = Color.Green;
			}
			else
			{
				element.BackgroundColor = Color.Red;
			}
		}

		private static async void BackButtonClicked(object sender, EventArgs e){
			await App.Navigation.PopAsync();
		}

		private static float ScoreSection(Section section, Inspection inspection)
		{
			//int TotalQuestionsInSection = section.Questions.Count(q => !q.HasSubItems);
			List<ScoredQuestion> scores = inspection.scores;
			List<ScoredQuestion> RelevantScores = scores.Where(score => score.question.SectionId == section.Id && score.question.IsScorable()).ToList();
			float availablePoints = RelevantScores.Count(score => score.answer == Answer.Yes || score.answer == Answer.No);
			if (availablePoints == 0)
			{
				return 0;
			}
			float scoredPoints = RelevantScores.Count(score => score.answer == Answer.Yes);
			//int TotalQuestionsInSection = scores.Where(score => score.question.section == section && !score.question.HasSubItems);
			return scoredPoints / availablePoints;
		}
		private static float ScorePart(SectionPart part, Inspection inspection)
		{
			List<ScoredQuestion> scores = App.database.LoadScoresForInspection(inspection);
			List<ScoredQuestion> RelevantScores = scores.Where(score => score.question.SectionPartId == part.Id && score.question.IsScorable()).ToList();
			float availablePoints = RelevantScores.Count(score => score.answer == Answer.Yes || score.answer == Answer.No);
			if (availablePoints == 0)
			{
				return 0;
			}
			float scoredPoints = RelevantScores.Count(score => score.answer == Answer.Yes);
			//int TotalQuestionsInSection = scores.Where(score => score.question.section == section && !score.question.HasSubItems);
			return scoredPoints / availablePoints;
		}
		private static float ScoreInspection(Inspection inspection)
		{
			List<ScoredQuestion> scores = App.database.LoadScoresForInspection(inspection);
			List<ScoredQuestion> RelevantScores = scores.Where(score => score.question.IsScorable()).ToList();

			float availablePoints = RelevantScores.Count(score => score.answer == Answer.Yes || score.answer == Answer.No);
			if (availablePoints == 0)
			{
				return 0;
			}
			float scoredPoints = RelevantScores.Count(score => score.answer == Answer.Yes);
			return scoredPoints / availablePoints;
		}
	}
	
	public class ScoresLayout : StackLayout
	{
		public void closePage()
		{
			App.Navigation.PopAsync();
		}
	}
}
