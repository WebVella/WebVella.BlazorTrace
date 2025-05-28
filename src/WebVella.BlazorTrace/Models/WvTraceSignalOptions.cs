using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvTraceSignalOptions
{
	/// <summary>
	/// How many calls is OK for this method. Default is 10.
	/// </summary>
	public long CallLimit { get; set; } = 10;

}
