using SQLite.Net.Attributes;
using SQLite.Net;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCPApp.Utilities;

namespace CCPApp.Models
{
	public class Inspection
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }

		[ForeignKey(typeof(ChecklistModel))]
		public string ChecklistId { get; set; }
		[ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
		public ChecklistModel Checklist { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<ScoredQuestion> scores { get; set; }

		public string Name { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Comment> comments { get; set; }

		[ManyToMany(typeof(InspectorInspections),CascadeOperations=CascadeOperation.All)]
		public List<Inspector> inspectors { get; set; }

		public double availablePoints;
		public double earnedPoints;
		public double percentage;
		public Rating rating;

		public Inspection()
		{
			scores = new List<ScoredQuestion>();
			comments = new List<Comment>();
			inspectors = new List<Inspector>();
		}
		/// <summary>
		/// Removes the inspection.  Note: this does not delete the inspection from a checklist in memory.
		/// That has to be handled by the calling code.
		/// </summary>
		/// <param name="inspection"></param>
		public static void DeleteInspection(Inspection inspection)
		{
			//Before calling: warn them that this is permanent.

			//delete all inspector linkages from memory (not the inspectors themselves)
			foreach (Inspector inspector in inspection.inspectors)
			{
				if (inspector.inspections.Contains(inspection))
				{
					inspector.inspections.Remove(inspection);
				}
			}
			//delete inspection and children from DB.
			App.database.DeleteInspection(inspection);


			//After calling: Call reset inspections on the checklist page
		}

		public ScoredQuestion GetScoreForQuestion(Question question)
		{
			return scores.SingleOrDefault(s => s.QuestionId == question.Id);
		}
	}	
}
