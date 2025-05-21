using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace;
public static class WvBlazorTraceInjection
{
	public static IServiceCollection AddBlazorTrace(this IServiceCollection services)
	{
		services.AddScoped<IWvBlazorTraceService,WvBlazorTraceService>();
		return services;
	}
}
