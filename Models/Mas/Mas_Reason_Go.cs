﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoWMS.Server.Models.Mas
{
	public class Mas_Reason_Go
	{
		public Int64? Efidx { get; set; }
		public Int32? Efstatus { get; set; }
		public DateTime? Created { get; set; }
		public DateTime? Modified { get; set; }
		public Int64? Innovator { get; set; }
		public string Device { get; set; }
		public string Rescode { get; set; }
		public string Description { get; set; }

	}
}
