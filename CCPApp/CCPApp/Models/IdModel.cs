using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp.Models
{
	public abstract class IdModel
	{
		public abstract int? Id { get; set; }

		public override bool Equals(object obj)
		{
			if (obj.GetType() != this.GetType())
			{
				return false;
			}
			return this.Id.Equals(((IdModel)obj).Id);
		}
		public override int GetHashCode()
		{
			if (Id == null)
			{
				return 0;
			}
			return Id.GetHashCode();
		}
	}
}
