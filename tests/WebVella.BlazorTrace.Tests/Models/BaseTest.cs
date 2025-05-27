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

	public WvTraceSessionTrace CheckTraceExists(
		Dictionary<string, WvTraceSessionModule> moduleDict,
		string moduleName,
		string componentFullName,
		string componentName,
		string instanceTag,
		string methodName,
		Guid? traceId)
	{
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
			Assert.Equal(methodName, compInstance.OnInitialized.Name);
			traceList = compInstance.OnInitialized.TraceList;
		}
		else if (traceInfo.IsOnParameterSet)
		{
			Assert.Equal(methodName, compInstance.OnParameterSet.Name);
			traceList = compInstance.OnParameterSet.TraceList;
		}
		else if (traceInfo.IsOnAfterRender)
		{
			Assert.Equal(methodName, compInstance.OnAfterRender.Name);
			traceList = compInstance.OnAfterRender.TraceList;
		}
		else if (traceInfo.IsShouldRender)
		{
			Assert.Equal(methodName, compInstance.ShouldRender.Name);
			traceList = compInstance.ShouldRender.TraceList;
		}
		else if (traceInfo.IsDispose)
		{
			Assert.Equal(methodName, compInstance.Dispose.Name);
			traceList = compInstance.Dispose.TraceList;
		}
		else if (traceInfo.IsOther)
		{
			Assert.NotNull(compInstance.OtherMethods.SingleOrDefault(x => x.Name == methodName));
			traceList = compInstance.OtherMethods.Single(x => x.Name == methodName).TraceList;
		}

		Assert.NotEmpty(traceList);
		Assert.NotNull(traceList.SingleOrDefault(x => x.TraceId == traceId));
		return traceList.Single();
	}
}
