using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
	[JsonIgnore]
	public bool CanGetMemory { get => IsOnAfterRender; }
	[JsonIgnore]
	public bool IsOnInitialized { get => new List<string> {"OnInitialized","OnInitializedAsync"}.Contains(MethodName ?? string.Empty); }
	[JsonIgnore]
	public bool IsOnParameterSet  { get => new List<string> {"OnParametersSet","OnParametersSetAsync"}.Contains(MethodName ?? string.Empty); }
	[JsonIgnore]
	public bool IsOnAfterRender  { get => new List<string> {"OnAfterRender","OnAfterRenderAsync"}.Contains(MethodName ?? string.Empty); }
	[JsonIgnore]
	public bool IsShouldRender  { get => new List<string> {"ShouldRender"}.Contains(MethodName ?? string.Empty); }
	[JsonIgnore]
	public bool IsDispose  { get => new List<string> {"Dispose","DisposeAsync"}.Contains(MethodName ?? string.Empty); }
	[JsonIgnore]
	public bool IsOther { get => !IsOnInitialized && !IsOnParameterSet && !IsOnAfterRender && !IsShouldRender && !IsDispose; }
}
