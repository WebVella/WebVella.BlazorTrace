using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvTraceModalMenuItem
{
	public WvTraceModalMenu Id { get; set; } = WvTraceModalMenu.MethodName;
	public bool IsActive { get; set; } = false;
	public Action OnClick { get; set; }
}
