using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvTraceInfo
{
	public string? MethodName { get; set; }
	public string? ComponentFullName { get; set; }
	public string? ComponentName { get; set; }
	public string? Tag { get; set; }
	public string? ModuleName { get; set; }
	public bool CanGetMemory { get => IsOnAfterRender; }
	public bool IsOnInitialized { get => (MethodName ?? string.Empty).StartsWith("OnInitialized"); }
	public bool IsOnParameterSet  { get => (MethodName ?? string.Empty).StartsWith("OnParametersSet"); }
	public bool IsOnAfterRender  { get => (MethodName ?? string.Empty).StartsWith("OnAfterRender"); }
	public bool IsShouldRender  { get => (MethodName ?? string.Empty).StartsWith("ShouldRender"); }
	public bool IsDispose  { get => (MethodName ?? string.Empty).StartsWith("Dispose"); }
	public bool IsOther { get => !IsOnInitialized && !IsOnParameterSet && !IsOnAfterRender && !IsShouldRender && !IsDispose; }
}
