using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp.Models
{
	public class Inspector : IdModel
	{
		[PrimaryKey, AutoIncrement]
		public override int? Id { get; set; }

		public string Name { get; set; }

		[ManyToMany(typeof(InspectorInspections),CascadeOperations=CascadeOperation.All)]
		public List<Inspection> inspections { get; set; }

		public override string ToString()
		{
			return Name;
		}
		/*public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(Inspector))
			{
				return false;
			}
			else
			{
				return (Id == ((Inspector)obj).Id && Id != null);
			}
		}
		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}*/
		public Inspector()
		{
			inspections = new List<Inspection>();
		}

		public static void DeleteInspector(Inspector inspector)
		{
			foreach (Inspection inspection in inspector.inspections)
			{
				if (inspection.inspectors.Contains(inspector))
				{
					inspection.inspectors.Remove(inspector);
				}
			}
			App.database.DeleteInspector(inspector);
		}
		public Inspector SelfReference
		{
			get
			{
				return this;
			}
		}
		static Inspector _null = null;
		public static Inspector Null
		{
			get
			{
				if (_null == null)
				{
					_null = new Inspector();
					_null.Name = string.Empty;
				}
				return _null;
			}
		}
	}


	public class InspectorInspections
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }

		[ForeignKey(typeof(Inspector))]
		public int? InspectorId { get; set; }

		[ForeignKey(typeof(Inspection))]
		public int? InspectionId { get; set; }
	}
}
