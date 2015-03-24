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
			ToolbarItem exportButton = new ToolbarItem();
			exportButton.Clicked += OpenExportPage;
			exportButton.Text = "Export";
			ToolbarItems.Add(exportButton);
		}
		public static string GeneratePdf(Inspection inspection)
		{
			string fileName = "Report.pdf";
			IGeneratePdf pdfMaker = DependencyService.Get<IGeneratePdf>();
			pdfMaker.Initialize(inspection);
			foreach (Comment comment in inspection.comments)
			{
				pdfMaker.CreateCommentPage(comment);
				if (comment != inspection.comments.Last())
				{
					pdfMaker.NewPage();
				}
			}
			pdfMaker.NewPage();
			foreach (SectionModel section in inspection.Checklist.Sections)
			{
				ReportSection reportSection = new ReportSection(section);
				reportSection.PartsToRender = section.SectionParts;
				pdfMaker.CreateQuestionSection(reportSection);
				if (section != inspection.Checklist.Sections.Last())
				{
					pdfMaker.NewPage();
				}
			}
			pdfMaker.Finish();

			return fileName;
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

			layout.VerticalOptions = LayoutOptions.Center;
			layout.HorizontalOptions = LayoutOptions.Center;
			layout.Children.Add(instructionsLabel);
			layout.Children.Add(fileNameBox);
			layout.Children.Add(saveButton);

			Content = layout;
		}

		public async void SaveReport(object Sender, EventArgs e)
		{
			string destinationFile = fileNameBox.Text;
			if (!destinationFile.EndsWith(".pdf"))
			{
				destinationFile += ".pdf";
			}
			DependencyService.Get<IFileManage>().CopyFileFromPrivateToPublic(filename, destinationFile);

			await App.Navigation.PopModalAsync();
		}
	}
}
