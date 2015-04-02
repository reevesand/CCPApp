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
	public class PrepareReportPage : ContentPage
	{
		Inspection inspection;
		ReportOptionsModel model = new ReportOptionsModel();
		public PrepareReportPage(Inspection inspection)
		{
			this.inspection = inspection;
			Padding = new Thickness(10, 0, 10, 5);
			Title = "Report Options";
			StackLayout layout = new StackLayout();
			layout.VerticalOptions = LayoutOptions.Center;
			layout.HorizontalOptions = LayoutOptions.End;

			Label CommentLabel = new Label { Text = "Comments" };
			Switch CommentSwitch = new Switch { IsToggled = true, BindingContext = model };
			CommentSwitch.SetBinding(Switch.IsToggledProperty, "Comments");
			StackLayout CommentLayout = new StackLayout { Children = { CommentLabel, CommentSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };
			//BothSidesLayout CommentLayout = new BothSidesLayout(CommentLabel, CommentSwitch);

			Label QuestionsLabel = new Label { Text = "Questions List" };
			Switch QuestionsSwitch = new Switch { IsToggled = true, BindingContext = model };
			QuestionsSwitch.SetBinding(Switch.IsToggledProperty, "Questions");
			StackLayout QuestionsLayout = new StackLayout { Children = { QuestionsLabel, QuestionsSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

			Label StructureLabel = new Label { Text = "Checklist Structure" };
			Switch StructureSwitch = new Switch { IsToggled = true, BindingContext = model };
			StructureSwitch.SetBinding(Switch.IsToggledProperty, "Structure");
			StackLayout StructureLayout = new StackLayout { Children = { StructureLabel, StructureSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

			Label TotalsLabel = new Label { Text = "Section Totals" };
			Switch TotalsSwitch = new Switch { IsToggled = true, BindingContext = model };
			TotalsSwitch.SetBinding(Switch.IsToggledProperty, "Totals");
			StackLayout TotalsLayout = new StackLayout { Children = { TotalsLabel, TotalsSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

			Label ScoreSheetLabel = new Label { Text = "Score Sheet" };
			Switch ScoreSheetSwitch = new Switch { IsToggled = true, BindingContext = model };
			ScoreSheetSwitch.SetBinding(Switch.IsToggledProperty, "ScoreSheet");
			StackLayout ScoreSheetLayout = new StackLayout { Children = { ScoreSheetLabel, ScoreSheetSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

			Label GraphLabel = new Label { Text = "Graph Sheet" };
			Switch GraphSwitch = new Switch { IsToggled = true, BindingContext = model };
			GraphSwitch.SetBinding(Switch.IsToggledProperty, "GraphSheet");
			StackLayout GraphLayout = new StackLayout { Children = { GraphLabel, GraphSwitch }, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

			Button GenerateButton = new Button { Text = "Generate Report" };
			GenerateButton.Clicked += GenerateButtonClicked;

			layout.Children.Add(CommentLayout);
			layout.Children.Add(QuestionsLayout);
			layout.Children.Add(StructureLayout);
			layout.Children.Add(TotalsLayout);
			layout.Children.Add(ScoreSheetLayout);
			layout.Children.Add(GraphLayout);
			layout.Children.Add(GenerateButton);

			Content = layout;

			/*TableView table = new TableView();
			TableRoot root = new TableRoot();
			TableSection section = new TableSection();

			List<Cell> cells = new List<Cell>();

			SwitchCell CommentsSwitch = new SwitchCell
			{
				Text = "Comments",
				On = true,
			};
			cells.Add(CommentsSwitch);
			SwitchCell QuestionsSwitch = new SwitchCell
			{
				Text = "Questions",
				On = true,
			};
			cells.Add(QuestionsSwitch);
			SwitchCell StructureSwitch = new SwitchCell
			{
				Text = "Checklist Structure",
				On = true,
			};
			cells.Add(StructureSwitch);
			SwitchCell TotalsSwitch = new SwitchCell
			{
				Text = "Section Totals",
				On = true,
			};
			cells.Add(TotalsSwitch);
			SwitchCell ScoreSheetSwitch = new SwitchCell
			{
				Text = "Score Sheet",
				On = true,
			};
			cells.Add(ScoreSheetSwitch);
			SwitchCell GraphSwitch = new SwitchCell
			{
				Text = "Graph Sheet",
				On = true,
			};
			cells.Add(GraphSwitch);
			ViewCell Generate = new ViewCell
			{
				View = new Button
				{
					Text = "Generate Report",
				},
			};
			cells.Add(Generate);

			section.Add(cells);
			root.Add(section);
			table.Root = root;
			Content = table;*/

			/*Grid grid = new Grid
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				ColumnDefinitions =
				{
					new ColumnDefinition{Width = GridLength.Auto},
					new ColumnDefinition{Width = GridLength.Auto}
				}
			};
			Switch QuestionsSwitch = new Switch
			{
				IsToggled = true,				
			};
			Label QuestionsLabel = new Label
			{
				Text = "Questions"
			};

			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.Children.Add(QuestionsLabel,0,0);
			grid.Children.Add(QuestionsSwitch,1,0);

			Content = grid;*/
		}

		public void GenerateButtonClicked(object sender, EventArgs e)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				string generatedReport = ReportPage.GeneratePdf(inspection,model);
				ReportPage page = new ReportPage(generatedReport);
				await App.Navigation.PushAsync(page);
			});
		}
	}
}
