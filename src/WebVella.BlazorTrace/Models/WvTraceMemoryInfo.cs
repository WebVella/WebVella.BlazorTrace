using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvTraceMemoryInfo
{
	[JsonIgnore]
	public string Id { get => WvTraceUtility.WvBTGetMemoryInfoId(AssemblyName,FieldName); }
	public string FieldName { get; set; } = String.Empty;
	public string TypeName { get; set; } = String.Empty;
	public string AssemblyName { get; set; } = String.Empty;
	public long Size { get; set; } = 0;
}
