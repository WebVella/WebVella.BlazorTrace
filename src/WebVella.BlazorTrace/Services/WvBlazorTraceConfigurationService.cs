using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace;
public partial interface IWvBlazorTraceConfigurationService
{
	WvBlazorTraceConfiguration GetConfiguraion();
}
public partial class WvBlazorTraceConfigurationService : IWvBlazorTraceConfigurationService
{
	private readonly WvBlazorTraceConfiguration _configuration = default!;
	public WvBlazorTraceConfigurationService(WvBlazorTraceConfiguration? config)
	{
		this._configuration = config is not null ? config : new();
		if (_configuration.ExcludeDefaultFrameworkAssemblies)
		{
			if (_configuration.MemoryTraceExcludeAssemblyStartWithList is null)
				_configuration.MemoryTraceExcludeAssemblyStartWithList = new();

			_configuration.MemoryTraceExcludeAssemblyStartWithList.AddRange(
				new[]{
					"Microsoft.AspNetCore"
				}
			);
		}
		if (_configuration.ExcludeDefaultFieldNames)
		{
			if (_configuration.MemoryTraceExcludeFieldNameContainsFromList is null)
				_configuration.MemoryTraceExcludeFieldNameContainsFromList = new();

			_configuration.MemoryTraceExcludeFieldNameContainsFromList.AddRange(
				new[]{
					"k__BackingField"
				}
			);
		}
	}
	public WvBlazorTraceConfiguration GetConfiguraion()
	{
		return _configuration;
	}
}

