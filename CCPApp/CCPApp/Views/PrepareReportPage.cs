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

		public PrepareReportPage()
		{
			Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
			Title = "Report Options";
			TableView table = new TableView();
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
			Content = table;

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
	}
	internal class ReportOptionsModel
	{
		public bool Comemnts { get; set; }
		public bool Questions { get; set; }
		public bool Structure { get; set; }
		public bool Totals { get; set; }
		public bool ScoreSheet { get; set; }
		public bool GraphSheet { get; set; }
	}
}
