using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp.Models
{
	public class Inspector
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }

		public string Name { get; set; }

		[ManyToMany(typeof(InspectorInspections),CascadeOperations=CascadeOperation.All)]
		public List<Inspection> inspections { get; set; }

		public override string ToString()
		{
			return Name;
		}
		public Inspector()
		{
			inspections = new List<Inspection>();
		}
	}


	public class InspectorInspections
	{
		[ForeignKey(typeof(Inspector))]
		public int? InspectorId { get; set; }

		[ForeignKey(typeof(Inspection))]
		public int? InspectionId { get; set; }
	}
}
