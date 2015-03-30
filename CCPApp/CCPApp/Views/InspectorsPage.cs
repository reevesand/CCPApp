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
	public class InspectorsPage : ContentPage
	{
		public InspectorsPage()
		{
			Title = "Inspectors";
			ResetInspectors();
		}

		public void ResetInspectors()
		{
			TableView inspectorsView = new TableView();
			TableRoot root = new TableRoot();
			TableSection section = new TableSection();
			List<ViewCell> cells = new List<ViewCell>();

			foreach (Inspector inspector in App.database.LoadAllInspectors())
			{
				Button button = new Button();
				button.Text = inspector.Name;
				ViewCell cell = new ViewCell { View = button };
				BoundMenuItem<Inspector> Edit = new BoundMenuItem<Inspector> { Text = "Edit" , BoundObject = inspector };
				BoundMenuItem<Inspector> Delete = new BoundMenuItem<Inspector> { Text = "Delete", IsDestructive = true, BoundObject = inspector };
				Edit.Clicked += openEditInspectorPage;
				Delete.Clicked += deleteInspector;
				cell.ContextActions.Add(Edit);
				cell.ContextActions.Add(Delete);
				cells.Add(cell);
			}

			Button createInspectorsButton = new Button();
			createInspectorsButton.Text = "New Inspector";
			createInspectorsButton.Clicked += openCreateInspectorPage;
			cells.Add(new ViewCell { View = createInspectorsButton });

			section.Add(cells);
			root.Add(section);
			inspectorsView.Root = root;

			Content = inspectorsView;
		}
		public async void openCreateInspectorPage(object sender, EventArgs e)
		{
			EditInspectorPage page = new EditInspectorPage();
			page.CallingPage = this;

			await App.Navigation.PushModalAsync(page);
		}
		public void deleteInspector(object sender, EventArgs e)
		{
			BoundMenuItem<Inspector> item = (BoundMenuItem<Inspector>)sender;
			Inspector inspector = item.BoundObject;
			Inspector.DeleteInspector(inspector);
			ResetInspectors();
		}
		public async void openEditInspectorPage(object sender, EventArgs e)
		{
			BoundMenuItem<Inspector> item = (BoundMenuItem<Inspector>)sender;
			Inspector inspector = item.BoundObject;
			EditInspectorPage page = new EditInspectorPage(inspector);
			page.CallingPage = this;
			await App.Navigation.PushModalAsync(page);
		}
	}


	public class EditInspectorPage : ContentPage
	{
		public InspectorsPage CallingPage { get; set; }
		private Inspector inspector;
		EntryCell NameCell;

		public EditInspectorPage(Inspector existingInspector = null)
		{
			if (existingInspector == null)
			{
				this.inspector = new Inspector();
				Title = "Create new Inspector";
			}
			else
			{
				this.inspector = existingInspector;
				Title = "Edit Inspector";
			}
			TableView view = new TableView();
			TableRoot root = new TableRoot(Title);
			TableSection section = new TableSection();
			Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
			NameCell = new EntryCell
			{
				BindingContext = inspector,
				Label = "Inspector Name:",
			};
			NameCell.SetBinding(EntryCell.TextProperty, "Name");
			ViewCell SaveCell = new ViewCell();
			ViewCell CancelCell = new ViewCell();
			Button saveButton = new Button();
			Button cancelButton = new Button();
			saveButton.Clicked += SaveInspectorClicked;
			cancelButton.Clicked += CancelInspectorClicked;
			saveButton.Text = "Save";
			cancelButton.Text = "Cancel";
			SaveCell.View = saveButton;
			CancelCell.View = cancelButton;

			section.Add(NameCell);
			section.Add(SaveCell);
			section.Add(CancelCell);
			root.Add(section);
			view.Root = root;
			Content = view;
		}
		public async void SaveInspectorClicked(object sender, EventArgs e)
		{
			inspector.Name = NameCell.Text;
			App.database.SaveInspector(inspector);
			CallingPage.ResetInspectors();

			await App.Navigation.PopModalAsync(true);
		}
		public async void CancelInspectorClicked(object sender, EventArgs e)
		{
			await App.Navigation.PopModalAsync(true);
		}
	}
}
