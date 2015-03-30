using CCPApp.Utilities;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp.Models
{
	/// <summary>
	/// Represents a section in the checklist.
	/// </summary>
	public class SectionModel
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }
		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Question> Questions { get; set; }
		[ForeignKey(typeof(ChecklistModel))]
		public string ChecklistId { get; set; }
		[ManyToOne(CascadeOperations = CascadeOperation.All)]
		public ChecklistModel checklist { get; set; }
		public string Label { get; set; }
		public string Title { get; set; }
		public string ShortTitle { get; set; }
		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<SectionPart> SectionParts { get; set; }
		public string ScoringModel = "YesNo";
		public double availablePoints;
		public double earnedPoints;
		public double percentage;
		public Rating rating;

		public SectionModel()
		{
			Questions = new List<Question>();
			SectionParts = new List<SectionPart>();
		}
		public override string ToString()
		{
			return "Section " + Label + ": " + Title;
		}

		//returns a list of all questions, pulled together from the top level and each part.
		public List<Question> AllQuestions()
		{
			List<Question> questions = new List<Question>();
			questions.AddRange(Questions);
			foreach (SectionPart part in SectionParts)
			{
				questions.AddRange(part.Questions);
			}
			return questions;
		}
		public List<Question> AllScorableQuestions()
		{
			List<Question> questions = AllQuestions();
			questions.RemoveAll(q => !q.IsScorable());
			return questions;
		}
	}
	/// <summary>
	/// Represents a section part in a section.
	/// </summary>
	public class SectionPart
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }
		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Question> Questions { get; set; }
		[ForeignKey(typeof(SectionModel))]
		public int? SectionId { get; set; }
		[ManyToOne(CascadeOperations = CascadeOperation.All)]
		public SectionModel section { get; set; }
		public string Label { get; set; }
		public string Description { get; set; }
		public string ScoringModel = "YesNo";
		public double availablePoints;
		public double earnedPoints;
		public double percentage;
		public Rating rating;

		public SectionPart()
		{
			Questions = new List<Question>();
			Label = string.Empty;
			Description = string.Empty;
		}
		public override string ToString()
		{
			return "Part " + Label + ": " + Description;
		}
	}

	/// <summary>
	/// Points to a section, but also contains information important for reporting,
	/// including the parts to be rendered for this section.
	/// </summary>
	public class ReportSection
	{
		public SectionModel section { get; private set; }
		public List<SectionPart> PartsToRender { get; set; }

		public ReportSection(SectionModel section)
		{
			this.section = section;
			PartsToRender = new List<SectionPart>();
		}

	}
}
