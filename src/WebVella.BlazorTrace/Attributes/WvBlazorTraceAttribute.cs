using MethodDecorator.Fody.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;
using WebVella.BlazorTrace;

[module: WvBlazorTrace]
namespace WebVella.BlazorTrace;
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Module)]
public class WvBlazorTraceAttribute : Attribute, IMethodDecorator
{
	private ComponentBase? _targetInstance = null;
	private MethodBase _method = default!;
	private bool _isAsync = false;

	public void Init(object instance, MethodBase method, object[] args)
	{
		if (instance is not null && instance.GetType().InheritsClass(typeof(ComponentBase)))
			_targetInstance = (instance as ComponentBase)!;

		_method = method;
		_isAsync = method.IsDefined(typeof(AsyncStateMachineAttribute), false);
	}

	public void OnEntry()
	{
		var traceService = WvBlazorTraceService.GetScopedService();
		if (traceService is not null && _targetInstance is not null)
		{
			traceService.OnEnter(_targetInstance, methodName: _method.Name);
		}
	}

	public void OnExit()
	{
		var traceService = WvBlazorTraceService.GetScopedService();
		if (!_isAsync && traceService is not null && _targetInstance is not null)
		{
			traceService.OnExit(_targetInstance, methodName: _method.Name);
		}
	}

	public async Task OnTaskContinuation(Task task)
	{
		await task; // Ensure the task completes before proceeding

		var traceService = WvBlazorTraceService.GetScopedService();
		if (traceService is not null && _targetInstance is not null)
		{
			traceService.OnExit(_targetInstance, methodName: _method.Name);
		}
	}

	public void OnException(Exception exception)
	{
		// do nothing
	}
}