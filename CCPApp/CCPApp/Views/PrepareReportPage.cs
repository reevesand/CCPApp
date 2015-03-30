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

			Grid grid = new Grid
			{
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

			Content = grid;
		}
	}
}
