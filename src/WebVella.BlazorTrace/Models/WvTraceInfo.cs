using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvTraceInfo
{
	public Guid? TraceId { get; set; }
	public string? MethodName { get; set; }
	public string? ComponentFullName { get; set; }
	public string? ComponentName { get; set; }
	public string? InstanceTag { get; set; }
	public string? ModuleName { get; set; }
	public bool CanGetMemory { get => IsOnAfterRender; }
	public bool IsOnInitialized { get => new List<string> {"OnInitialized","OnInitializedAsync"}.Contains(MethodName ?? string.Empty); }
	public bool IsOnParameterSet  { get => new List<string> {"OnParametersSet","OnParametersSetAsync"}.Contains(MethodName ?? string.Empty); }
	public bool IsOnAfterRender  { get => new List<string> {"OnAfterRender","OnAfterRenderAsync"}.Contains(MethodName ?? string.Empty); }
	public bool IsShouldRender  { get => new List<string> {"ShouldRender"}.Contains(MethodName ?? string.Empty); }
	public bool IsDispose  { get => new List<string> {"Dispose","DisposeAsync"}.Contains(MethodName ?? string.Empty); }
	public bool IsOther { get => !IsOnInitialized && !IsOnParameterSet && !IsOnAfterRender && !IsShouldRender && !IsDispose; }
}
