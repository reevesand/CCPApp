using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace CCPApp.Models
{
	public class ChecklistModel
	{
		[PrimaryKey]
		public string Id { get; set; }

		public string Title { get; set; }
		public string Description;
		public string ContactName;
		public string ContactPosition;
		public string ContactAddress;
		public string ContactCityState;
		public string ContactZip;
		public string DocumentDirectory;
		public string SplashBackground;
		public string SplashForegroundColor;
		public string Logo;
		public string Logo2;
		public string LogoLocation;
		public string LogoLocation2;
		public string FontFamily;
		public string ResumeFormName;

		public int ScoreThresholdCommendable { get; set; }
		public int ScoreThresholdSatisfactory { get; set; }

		public bool UserInvert;

		public ChecklistModel SelfReference
		{
			get
			{
				return this;
			}
		}

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Inspection> Inspections { get; set; }
		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Section> Sections { get; set; }

		public ChecklistModel()
		{
			Inspections = new List<Inspection>();
			Sections = new List<Section>();
		}
		public List<Question> GetAllQuestions()
		{
			List<Question> questions = new List<Question>();
			foreach (Section section in Sections)
			{
				questions.AddRange(section.AllQuestions());
			}
			return questions;
		}
		/*public void loadInspections()
		{
			Inspections = App.database.LoadInspectionsForChecklist(this.Id).ToList();
			foreach (Inspection inspection in Inspections)
			{
				inspection.Checklist = this;
			}
		}*/
		public static ChecklistModel Initialize(string configFileName)
		{
			ChecklistModel model = new ChecklistModel();
			DependencyService.Get<IParseChecklist>().Parse(model, configFileName);
			return model;
		}
	}

	public class Threshold
	{
		public int Commendable { get; set; }
		public int Satisfactory { get; set; }

		public Threshold() { }
		public Threshold(int commendable, int satisfactory)
		{
			Commendable = commendable;
			Satisfactory = satisfactory;
		}

	}
}
