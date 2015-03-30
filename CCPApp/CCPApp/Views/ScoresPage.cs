using CCPApp.Items;
using CCPApp.Models;
using CCPApp.Utilities;
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
			GenericPicker<SectionModel> sectionPicker = new GenericPicker<SectionModel>();
			foreach (SectionModel section in inspection.Checklist.Sections)
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
			double cummulativeScore = ScoringHelper.ScoreInspection(inspection).Item3;
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
			GenericPicker<SectionModel> picker = (GenericPicker<SectionModel>)sender;
			SectionModel section = picker.SelectedItem;
			double sectionScore = ScoringHelper.ScoreSection(section, inspection).Item3;
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
			double partScore = ScoringHelper.ScorePart(part, inspection).Item3;
			partScoreLabel.Text = "Part score: " + (partScore * 100).ToString("0.00") + "%";
			setScoresColor(partScore, partScoreLabel);
		}
		private void setScoresColor(double score, VisualElement element)
		{
			double percentScore = score * 100;
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
	}
	
	public class ScoresLayout : StackLayout
	{
		public void closePage()
		{
			App.Navigation.PopAsync();
		}
	}
}
