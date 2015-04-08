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
		[ForeignKey(typeof(SectionModel))]
		public int? SectionId { get; set; }
		[ManyToOne(CascadeOperations= CascadeOperation.All)]
		public SectionModel section { get; set; }
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
		public string Remarks { get; set; }
		public string OldRemarks;

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Reference> References { get; set; }

		public override string ToString()
		{
			string prefix;
			if (part != null)
			{
				prefix = part.Label;
			}
			else
			{
				prefix = section.Label;
			}
			return prefix + "-" + numberString;
		}
		public string numberString
		{
			get
			{
				return Number.ToString() + Subqualifier;
			}
		}
		public bool IsScorable()
		{
			return !HasSubItems;
		}

		public Question()
		{
			References = new List<Reference>();
		}

		public string FullString
		{
			get
			{
				return ToString() + " " + Text;
			}
		}
		public Question SelfReference
		{
			get
			{
				return this;
			}
		}
	}
	/// <summary>
	/// Represents a reference for a question.
	/// </summary>
	public class Reference
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }

		[ForeignKey(typeof(Question))]
		public int? QuestionId { get; set; }
		[ManyToOne(CascadeOperations = CascadeOperation.All)]
		public Question question { get; set; }

		public string Document { get; set; }
		public string DocumentName { get; set; }
		public string Bookmark { get; set; }
		public string Description { get; set; }
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
