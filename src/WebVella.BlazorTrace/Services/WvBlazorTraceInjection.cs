using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace;
public static class WvBlazorTraceInjection
{
	public static IServiceCollection AddBlazorTrace(this IServiceCollection services, 
		WvBlazorTraceConfiguration? configuration = null)
	{
		services.AddSingleton<IWvBlazorTraceConfigurationService>(provider => new WvBlazorTraceConfigurationService(configuration));
		services.AddSingleton<IWvBlazorTraceService,WvBlazorTraceService>();
		return services;
	}
}
