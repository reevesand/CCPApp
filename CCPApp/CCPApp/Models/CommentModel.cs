using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp.Models
{
	public class Comment
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }

		[ForeignKey(typeof(Inspection))]
		public int? InspectionId { get; set; }
		[ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
		public Inspection inspection { get; set; }

		[ForeignKey(typeof(Question))]
		public int QuestionId { get; set; }
		[ManyToOne(CascadeOperations = CascadeOperation.All)]
		public Question question { get; set; }

		public string Subject { get; set; }
		public string CommentText { get; set; }
		public CommentType type { get; set; }
	}

	public enum CommentType
	{
		Finding,
		Observation,
		Commendable
	}
}
