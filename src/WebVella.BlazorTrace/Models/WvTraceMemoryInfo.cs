using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvTraceMemoryInfo
{
	public string Id { get => WvTraceUtility.GetMemoryInfoId(AssemblyName,FieldName); }
	public string FieldName { get; set; } = String.Empty;
	public string AssemblyName { get; set; } = String.Empty;
	public long Size { get; set; } = 0;
}
