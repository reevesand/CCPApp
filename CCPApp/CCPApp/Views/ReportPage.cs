using CCPApp.Models;
using CCPApp.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Views
{
	public class ReportPage : PdfPage
	{
		public ReportPage(string fileName)
			:base(fileName,1)
		{
			useTemp = true;
			ToolbarItem exportButton = new ToolbarItem();
			exportButton.Clicked += OpenExportPage;
			exportButton.Text = "Export";
			ToolbarItems.Add(exportButton);
		}
		public static string GeneratePdf(Inspection inspection, ReportOptionsModel model = null)
		{
			if (model == null)
			{
				model = new ReportOptionsModel();
			}
			IGeneratePdf pdfMaker = DependencyService.Get<IGeneratePdf>();
			string fileName = pdfMaker.Initialize(inspection);
			int numCommentPages = 0;
			//draw comment pages
			if (model.Comments)
			{
				foreach (Comment comment in inspection.comments)
				{
					numCommentPages++;
					pdfMaker.CreateCommentPage(comment);
					if (comment != inspection.comments.Last())
					{
						pdfMaker.NewPage();
					}
				}
			}
			//draw question listing
			List<ReportSection> reportSections = PrepareInspectionForScoring(inspection);
			if (model.Questions)
			{
				pdfMaker.NewPageIfNotEmpty();
				foreach (ReportSection section in reportSections)
				{
					pdfMaker.CreateQuestionSection(section);
					if (section != reportSections.Last())
					{
						pdfMaker.NewPage();
					}
				}
			}

			//draw section totals
			if (model.Totals)
			{
				pdfMaker.NewPageIfNotEmpty();
				pdfMaker.CreateSectionTotals();
			}

			//draw checklist structure
			if (model.Structure)
			{
				pdfMaker.NewPageIfNotEmpty();
				pdfMaker.CreateStructure();
			}

			//draw scoresheet
			if (model.ScoreSheet)
			{
				pdfMaker.NewPageIfNotEmpty();
				pdfMaker.CreateScoreSheet();
			}

			//draw scores graph
			if (model.GraphSheet)
			{
				pdfMaker.NewPageIfNotEmpty();
				pdfMaker.CreateScoreGraph();
			}
			pdfMaker.Finish();
			pdfMaker.StampFooter(numCommentPages);

			return fileName;
		}

		private static List<ReportSection> PrepareInspectionForScoring(Inspection inspection)
		{
			List<ReportSection> reportSections = new List<ReportSection>();
			double sumTotalAvailablePoints = 0;
			double sumTotalEarnedPoints = 0;
			bool anyUnacceptables = false;
			foreach (SectionModel section in inspection.Checklist.Sections)
			{
				//We can have a commendable section even with unacceptable parts.
				//bool anyUnacceptableParts = false;
				if (section.SectionParts.Any())
				{
					double sumSectionAvailablePoints = 0;
					double sumSectionEarnedPoints = 0;
					foreach (SectionPart part in section.SectionParts)
					{
						Tuple<double, double, double> partScores = ScoringHelper.ScorePart(part, inspection);
						part.availablePoints = partScores.Item1;
						part.earnedPoints = partScores.Item2;
						part.percentage = partScores.Item3;
						if (part.availablePoints > 0)
						{
							double percentScore = part.percentage * 100;
							if (percentScore < inspection.Checklist.ScoreThresholdSatisfactory)
							{
								part.rating = Rating.Unacceptable;
								//anyUnacceptableParts = true;
							}
							else if (percentScore < inspection.Checklist.ScoreThresholdCommendable)
							{
								part.rating = Rating.Satisfactory;
							}
							else
							{
								part.rating = Rating.Commendable;
							}
						}
						sumSectionAvailablePoints += partScores.Item1;
						sumSectionEarnedPoints += partScores.Item2;
					}
					section.availablePoints = sumSectionAvailablePoints;
					section.earnedPoints = sumSectionEarnedPoints;
					if (sumSectionAvailablePoints > 0)
					{
						section.percentage = sumSectionEarnedPoints / sumSectionAvailablePoints;
					}
					else
					{
						section.percentage = 0;
					}
				}
				else
				{
					Tuple<double, double, double> sectionScores = ScoringHelper.ScoreSection(section, inspection);
					section.availablePoints = sectionScores.Item1;
					section.earnedPoints = sectionScores.Item2;
					section.percentage = sectionScores.Item3;
				}

				if (section.availablePoints > 0)
				{
					double percentScore = section.percentage * 100;
					if (percentScore < inspection.Checklist.ScoreThresholdSatisfactory)
					{
						anyUnacceptables = true;
						section.rating = Rating.Unacceptable;
					}
					else if (percentScore < inspection.Checklist.ScoreThresholdCommendable /*|| anyUnacceptableParts*/)
					{ section.rating = Rating.Satisfactory; }
					else
					{ section.rating = Rating.Commendable; }
				}
				else
				{
					section.rating = Rating.None;
				}

				sumTotalAvailablePoints += section.availablePoints;
				sumTotalEarnedPoints += section.earnedPoints;
				if (true)	//if we're supposed to render this section
				{
					ReportSection reportSection = new ReportSection(section);
					reportSection.PartsToRender = section.SectionParts;
					reportSections.Add(reportSection);

				}
			}
			inspection.availablePoints = sumTotalAvailablePoints;
			inspection.earnedPoints = sumTotalEarnedPoints;
			if (sumTotalAvailablePoints > 0)
			{
				inspection.percentage = sumTotalEarnedPoints / sumTotalAvailablePoints;
				double percentScore = inspection.percentage * 100;
				if (percentScore < inspection.Checklist.ScoreThresholdSatisfactory)
				{
					inspection.rating = Rating.Unacceptable;
				}
				else if (percentScore < inspection.Checklist.ScoreThresholdCommendable || anyUnacceptables)
				{
					inspection.rating = Rating.Satisfactory;
				}
				else
				{
					inspection.rating = Rating.Commendable;
				}
			}
			else
			{
				inspection.percentage = 0;
				inspection.rating = Rating.None;
			}
			return reportSections;
		}

		public async void OpenExportPage(object Sender, EventArgs e)
		{
			ExportPage page = new ExportPage(FileName);
			await App.Navigation.PushModalAsync(page);
		}
	}
	public class ExportPage : ContentPage
	{
		string filename;
		Entry fileNameBox = new Entry();
		public ExportPage(string filename)
		{
			Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
			this.filename = filename;
			StackLayout layout = new StackLayout();
			Label instructionsLabel = new Label();
			instructionsLabel.Text = DependencyService.Get<IValuesHelper>().exportInstructions();
			Button saveButton = new Button();
			saveButton.Text = "Save Report";
			saveButton.Clicked += SaveReport;
			Button cancelButton = new Button();
			cancelButton.Text = "Cancel";
			cancelButton.Clicked += (async (object sender, EventArgs e) =>
			{
				await App.Navigation.PopModalAsync();
			});

			layout.VerticalOptions = LayoutOptions.Center;
			layout.HorizontalOptions = LayoutOptions.Center;
			layout.Children.Add(instructionsLabel);
			layout.Children.Add(fileNameBox);
			layout.Children.Add(saveButton);
			layout.Children.Add(cancelButton);

			Content = layout;
		}

		public async void SaveReport(object Sender, EventArgs e)
		{
			string destinationFile = fileNameBox.Text;
			if (destinationFile == null)
			{
				destinationFile = string.Empty;
			}
			if (!destinationFile.EndsWith(".pdf"))
			{
				destinationFile += ".pdf";
			}
			DependencyService.Get<IFileManage>().CopyFileFromTempToPublic(filename, destinationFile);
			//DependencyService.Get<IFileManage>().DeleteTempFile(filename);

			await App.Navigation.PopModalAsync();
		}

		
	}
}
