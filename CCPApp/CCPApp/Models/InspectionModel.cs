using SQLite.Net.Attributes;
using SQLite.Net;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp.Models
{
	public class Inspection
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

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

		public Inspection()
		{
			scores = new List<ScoredQuestion>();
			comments = new List<Comment>();
			inspectors = new List<Inspector>();
		}
	}	
}
