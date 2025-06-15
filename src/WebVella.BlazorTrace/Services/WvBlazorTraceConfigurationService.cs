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
		if (_configuration.ExcludeDefaultMethods)
		{
			if (_configuration.TraceExcludeMethodList is null)
				_configuration.TraceExcludeMethodList = new();

			_configuration.TraceExcludeMethodList.AddRange(
				new[]{
					"<",
					"get_",
					"set_"
				}
			);
		}
		if (_configuration.ExcludeDefaultAssemblies)
		{
			if (_configuration.MemoryExcludeAssemblyList is null)
				_configuration.MemoryExcludeAssemblyList = new();

			_configuration.MemoryExcludeAssemblyList.AddRange(
				new[]{
					"Microsoft.AspNetCore"
				}
			);
		}
		if (_configuration.ExcludeDefaultFieldNames)
		{
			if (_configuration.MemoryExcludeFieldNameList is null)
				_configuration.MemoryExcludeFieldNameList = new();

			_configuration.MemoryExcludeFieldNameList.AddRange(
				new[]{
					"k__BackingField"
				}
			);
		}
		if(_configuration.DefaultTraceMethodOptions is null)
			_configuration.DefaultTraceMethodOptions = new();

		if(_configuration.DefaultTraceSignalOptions is null)
			_configuration.DefaultTraceSignalOptions = new();
	}
	public WvBlazorTraceConfiguration GetConfiguraion()
	{
		return _configuration;
	}
}

