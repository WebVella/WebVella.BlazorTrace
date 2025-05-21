using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvTraceMethodOptions
{
	/// <summary>
	/// How many milliseconds is an OK for executing this method. Default is 1000.
	/// </summary>
	public long DurationLimit { get; set; } = 1000;

	/// <summary>
	/// How many calls is OK for this method. Default is 10.
	/// </summary>
	public long CallLimit { get; set; } = 10;

	/// <summary>
	/// What is the total maximum memory limit. Default is 2048.
	/// </summary>
	public long MemoryLimitTotalBytes { get; set; } = 2048;

	/// <summary>
	/// What is the maximum memory delta limit between enter and exit. Default is 2048.
	/// </summary>
	public long MemoryLimitDeltaBytes { get; set; } = 2048;
}
