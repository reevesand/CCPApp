using CCPApp.Items;
using CCPApp.Models;
using CCPApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp
{
	public class InspectionHelper
	{
		public static async void CreateInspectionButtonClicked(object sender, EventArgs e)
		{
			CreateInspectionButton button = (CreateInspectionButton)sender;
			ChecklistModel checklist = button.checklist;
			EditInspectionPage page = new EditInspectionPage(null,checklist);
			page.CallingPage = (ChecklistPage)button.ParentView.ParentView;
			await App.Navigation.PushModalAsync(page);
		}
		public static void SelectInspectionButtonClicked(object sender, EventArgs e)
		{
			InspectionButton button = (InspectionButton)sender;
			Inspection inspection = button.inspection;
			Device.BeginInvokeOnMainThread(async () =>
			{
				InspectionPage page = new InspectionPage(inspection);
				await App.Navigation.PushAsync(page);
			});
		}

		internal static List<QuestionPage> GenerateQuestionPages(List<Question> questions, Inspection inspection)
		{
			List<QuestionPage> pages = new List<QuestionPage>();
			string MasterQuestionText = string.Empty;
			List<Reference> MasterQuestionReferences = new List<Reference>();
			int MasterQuestionNumber = -2;
			foreach (Question question in questions)
			{
				QuestionPage page = null;
				if (question.HasSubItems)
				{
					MasterQuestionNumber = question.Number;
					MasterQuestionText = question.Text.Trim();
					MasterQuestionReferences = question.References;
				}
				else if (question.Number == MasterQuestionNumber)
				{
					page = new QuestionPage(question, inspection, MasterQuestionText + "\n" + question.Text.Trim(),MasterQuestionReferences);
				}
				else
				{
					page = new QuestionPage(question, inspection);
				}
				if (page != null)
				{
					pages.Add(page);
				}
			}
			return pages;
		}
	}

	public class InspectionButton : Button
	{
		public InspectionButton(Inspection inspection)
		{
			this.inspection = inspection;
			Text = inspection.Name;
		}
		public Inspection inspection { get; set; }
	}
	public class CreateInspectionButton : Button
	{
		public ChecklistModel checklist { get; set; }
		public CreateInspectionButton(ChecklistModel checklist)
		{
			this.checklist = checklist;
		}
	}

	public class EditInspectionPage : ContentPage
	{
		public TableSection tableSection { get; set; }
		public Inspection inspection { get; set; }
		public ChecklistPage CallingPage { get; set; }
		public GenericPicker<Inspector> inspectorPicker { get; set; }
		public EditInspectionPage(Inspection existingInspection = null, ChecklistModel checklist = null)
		{
			if (existingInspection == null)
			{
				inspection = new Inspection();
				inspection.Checklist = checklist;
				inspection.ChecklistId = checklist.Id;
				Title = "Create new Inspection";
			}
			else
			{
				inspection = existingInspection;
				Title = "Edit Inspection";
			}
			TableView view = new TableView();
			TableRoot root = new TableRoot("Edit Inspection");
			TableSection section = new TableSection();
			Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
			EntryCell NameCell = new EntryCell
			{
				BindingContext = inspection,
				Label = "Inspection Name:",
			};
			NameCell.SetBinding(EntryCell.TextProperty, "Name");

			EntryCell OrganizationCell = new EntryCell
			{
				BindingContext = inspection,
				Label = "Organization:",
			};
			OrganizationCell.SetBinding(EntryCell.TextProperty,"Organization");

			inspectorPicker = new GenericPicker<Inspector>();
			List<Inspector> allInspectors = App.database.LoadAllInspectors();
			foreach (Inspector inspector in allInspectors)
			{
				inspectorPicker.AddItem(inspector);
			}
			if (inspection.inspectors.Any())
			{	//TODO update to deal with multiple inspectors
				try
				{
					inspectorPicker.SelectedItem = inspection.inspectors.First();
				}
				catch (KeyNotFoundException){}
			}
			ViewCell inspectorCell = new ViewCell { View = inspectorPicker };

			ViewCell SaveCell = new ViewCell();
			ViewCell CancelCell = new ViewCell();
			Button saveButton = new Button();
			Button cancelButton = new Button();
			saveButton.Clicked += SaveInspectionClicked;
			cancelButton.Clicked += CancelInspectionClicked;
			saveButton.Text = "Save";
			cancelButton.Text = "Cancel";
			SaveCell.View = saveButton;
			CancelCell.View = cancelButton;

			section.Add(NameCell);
			section.Add(OrganizationCell);
			section.Add(inspectorCell);
			section.Add(SaveCell);
			section.Add(CancelCell);
			root.Add(section);
			view.Root = root;
			Content = view;
		}

		public async void SaveInspectionClicked(object sender, EventArgs e)
		{
			ChecklistModel checklist = inspection.Checklist;
			//inspection.Name = NameCell.Text;
			inspection.ChecklistId = checklist.Id;
			if (!checklist.Inspections.Contains(inspection))
			{
				checklist.Inspections.Add(inspection);
			}
			if (inspectorPicker.SelectedIndex >= 0)
			{
				inspection.inspectors.Add(inspectorPicker.SelectedItem);
			}
			App.database.SaveInspection(inspection);

			CallingPage.ResetInspections();

			await App.Navigation.PopModalAsync(true);
		}
		public async void CancelInspectionClicked(object sender, EventArgs e)
		{
			await App.Navigation.PopModalAsync(true);
		}
	}
	public class GoToQuestionButton : Button
	{
		//Question question { get; set; }
		InspectionPage inspectionPage { get; set; }
		public GoToQuestionButton(Question question, InspectionPage inspectionPage)
		{
			this.question = question;
			this.inspectionPage = inspectionPage;
			this.Clicked += clickQuestionButton;
		}
		public GoToQuestionButton(InspectionPage inspectionPage)
		{
			this.inspectionPage = inspectionPage;
			this.Clicked += clickQuestionButton;
		}

		private async static void clickQuestionButton(object sender, EventArgs e)
		{
			GoToQuestionButton button = (GoToQuestionButton)sender;
			ISectionPage sectionPage = button.inspectionPage.SetSectionPage(button.question.section);
			sectionPage.SetSelectedQuestion(button.question);

			await App.Navigation.PopAsync();
		}

		public Question question
		{
			get
			{
				return (Question)GetValue(QuestionProperty);
			}
			set
			{
				SetValue(QuestionProperty, value);
			}
		}

		public static readonly BindableProperty QuestionProperty =
			BindableProperty.Create<GoToQuestionButton, Question>
			(p => p.question, null);
	}
}
