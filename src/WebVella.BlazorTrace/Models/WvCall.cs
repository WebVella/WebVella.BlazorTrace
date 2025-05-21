using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvCall
{
	public DateTimeOffset? EnteredOn { get; set; }
	public DateTimeOffset? ExitedOn { get; set; }
	public long DurationMs
	{
		get
		{
			if (EnteredOn is null || ExitedOn is null) return 0;
			if(EnteredOn.Value > ExitedOn.Value) return 0;
			return (ExitedOn.Value - EnteredOn.Value).Milliseconds;
		}
	}
}

