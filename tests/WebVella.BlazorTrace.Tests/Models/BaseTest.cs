using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace.Tests;
public class BaseTest
{
	protected static readonly Lock _locker = new Lock();
	public Mock<IWvBlazorTraceConfigurationService> WvBlazorTraceConfigurationServiceMock;
	public Mock<IJSRuntime> JsRuntimeMock;
	public Mock<WvBlazorTraceService> WvBlazorTraceServiceMock;
	public TestComponent Component;

	public BaseTest()
	{
		this.WvBlazorTraceConfigurationServiceMock = new Mock<IWvBlazorTraceConfigurationService>();
		this.WvBlazorTraceConfigurationServiceMock.Setup(x => x.GetConfiguraion()).Returns(new WvBlazorTraceConfiguration());
		this.JsRuntimeMock = new Mock<IJSRuntime>();
		this.WvBlazorTraceServiceMock = new Mock<WvBlazorTraceService>(
			this.JsRuntimeMock.Object,
			this.WvBlazorTraceConfigurationServiceMock.Object,
			false);
		this.Component = new TestComponent();
	}

	public (WvTraceSessionMethod, WvTraceSessionTrace) CheckTraceExists(
		Dictionary<string, WvTraceSessionModule> moduleDict,
		string moduleName,
		string componentFullName,
		string componentName,
		string instanceTag,
		string methodName,
		Guid? traceId)
	{
		WvTraceSessionMethod? method = null;
		WvTraceSessionTrace? trace = null;

		Assert.Single(moduleDict.Keys);
		Assert.NotNull(moduleDict.Keys.SingleOrDefault(x => x == moduleName));
		var module = moduleDict[moduleName];
		Assert.NotNull(module.ComponentDict.Keys.SingleOrDefault(x => x == componentFullName));
		var component = module.ComponentDict[componentFullName];
		Assert.Equal(componentName, component.Name);
		Assert.NotNull(component.TaggedInstances.SingleOrDefault(x => x.Tag == instanceTag));
		var compInstance = component.TaggedInstances.Single(x => x.Tag == instanceTag);
		var traceInfo = new WvTraceInfo
		{
			TraceId = traceId,
			MethodName = methodName,
			ComponentFullName = componentFullName,
			ComponentName = componentName,
			InstanceTag = instanceTag,
			ModuleName = moduleName
		};
		var traceList = new List<WvTraceSessionTrace>();
		if (traceInfo.IsOnInitialized)
		{
			method = compInstance.OnInitialized;
			Assert.Equal(methodName, method.Name);
			traceList = compInstance.OnInitialized.TraceList;
		}
		else if (traceInfo.IsOnParameterSet)
		{
			method = compInstance.OnParameterSet;
			Assert.Equal(methodName, method.Name);
			traceList = compInstance.OnParameterSet.TraceList;
		}
		else if (traceInfo.IsOnAfterRender)
		{
			method = compInstance.OnAfterRender;
			Assert.Equal(methodName, method.Name);
			traceList = compInstance.OnAfterRender.TraceList;
		}
		else if (traceInfo.IsShouldRender)
		{
			method = compInstance.ShouldRender;
			Assert.Equal(methodName, method.Name);
			traceList = compInstance.ShouldRender.TraceList;
		}
		else if (traceInfo.IsDispose)
		{
			method = compInstance.Dispose;
			Assert.Equal(methodName, method.Name);
			traceList = compInstance.Dispose.TraceList;
		}
		else if (traceInfo.IsOther)
		{
			Assert.NotNull(compInstance.OtherMethods.SingleOrDefault(x => x.Name == methodName));
			method = compInstance.OtherMethods.Single(x => x.Name == methodName);
			traceList = method.TraceList;
		}
		Assert.NotNull(method);
		Assert.NotEmpty(traceList);
		Assert.NotNull(traceList.SingleOrDefault(x => x.TraceId == traceId));
		trace = traceList.Single();

		return (method, trace);
	}

		public (WvTraceSessionSignal, WvTraceSessionSignalTrace) CheckSignalTraceExists(
		Dictionary<string, WvTraceSessionSignal> signalDict,
		string moduleName,
		string signalName,
		string componentFullName,
		string instanceTag,
		string methodName)
	{
		WvTraceSessionSignal? signal = null;
		WvTraceSessionSignalTrace? trace = null;

		Assert.NotNull(signalDict.Keys.SingleOrDefault(x => x == signalName));
		signal = signalDict[signalName];
		Assert.NotEmpty(signal.TraceList);
		trace = signal.TraceList.FirstOrDefault(x => 
			x.ComponentFullName == componentFullName
			&& x.InstanceTag == instanceTag
			&& x.MethodName == methodName
		);
		Assert.NotNull(trace);

		return (signal, trace);
	}

	public WvMethodTraceRow CheckTraceRowExists(
			List<WvMethodTraceRow> traceRows,
			string moduleName,
			string componentFullName,
			string componentName,
			string instanceTag,
			string methodName)
	{
		Assert.NotNull(traceRows);
		Assert.NotNull(traceRows.SingleOrDefault(x => x.Module == moduleName
			&& x.ComponentFullName == componentFullName
			&& x.Component == componentName
			&& x.InstanceTag == instanceTag
			&& x.Method == methodName));

		return traceRows.Single(x => x.Module == moduleName
			&& x.ComponentFullName == componentFullName
			&& x.Component == componentName
			&& x.InstanceTag == instanceTag
			&& x.Method == methodName);
	}
}
