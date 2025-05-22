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
	WvBlazorTraceConfiguration GetOptions();
}
public partial class WvBlazorTraceConfigurationService : IWvBlazorTraceConfigurationService
{
	private readonly WvBlazorTraceConfiguration _configuration = default!;
	public WvBlazorTraceConfigurationService(WvBlazorTraceConfiguration? config)
	{
		this._configuration = config is not null ? config : new();
		if (_configuration.ExcludedFrameworkAssembliesByDefault)
		{
			if(_configuration.MemoryTraceExcludedAssemblyStartWithList is null)
				_configuration.MemoryTraceExcludedAssemblyStartWithList = new();

			_configuration.MemoryTraceExcludedAssemblyStartWithList.Add("Microsoft.AspNetCore");
			_configuration.MemoryTraceExcludedAssemblyStartWithList.Add("WebVella.BlazorTrace");
		}
	}
	public WvBlazorTraceConfiguration GetOptions()
	{
		return _configuration;
	}
}

