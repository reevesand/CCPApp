using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp.Models
{
	public class Question
	{
		[PrimaryKey,AutoIncrement]
		public int? Id { get; set; }
		[ForeignKey(typeof(Section))]
		public int? SectionId { get; set; }
		[ManyToOne(CascadeOperations= CascadeOperation.All)]
		public Section section { get; set; }
		[ForeignKey(typeof(SectionPart))]
		public int? SectionPartId { get; set; }
		[ManyToOne(CascadeOperations = CascadeOperation.All)]
		public SectionPart part { get; set; }


		public int Number { get; set; }
		public string Subqualifier { get; set; }
		public bool Critical { get; set; }
		public string CriticalApplication = "";
		public bool InvertScore { get; set; }
		public bool IsLastQuestion = false;
		public bool HasSubItems { get; set; }
		public bool Updated { get; set; }
		public string Text { get; set; }
		public string PrintedText { get; set; }
		public string OldText { get; set; }
		public List<Reference> References = new List<Reference>();

		public override string ToString()
		{
			return Number.ToString() + Subqualifier;
		}
		public bool IsScorable()
		{
			return !HasSubItems;
		}

		public Question()
		{

		}
	}
	/// <summary>
	/// Represents a reference for a question.
	/// </summary>
	public class Reference
	{
		public string Document = string.Empty;
		public string DocumentName = string.Empty;
		public string Bookmark = string.Empty;
		public string Description = string.Empty;
		public Reference() { }
		public Reference(Reference r)
		{
			Document = r.Document;
			DocumentName = r.DocumentName;
			Bookmark = r.Bookmark;
			Description = r.Description;
		}

		public override string ToString()
		{
			return Description;
		}
		public string NameWithoutExt
		{
			get
			{
				return DocumentName.Substring(0, DocumentName.LastIndexOf('.'));
			}
			set
			{
				DocumentName = value + Extension;
			}

		}
		public string Extension
		{
			get
			{
				return DocumentName.Substring(DocumentName.LastIndexOf('.'));
			}
			set
			{
				DocumentName = NameWithoutExt + value;
			}
		}
	}
}
