using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace.Tests;
public class BaseTest
{
	protected static readonly AsyncLock _locker = new AsyncLock();
	public Mock<IWvBlazorTraceConfigurationService> WvBlazorTraceConfigurationServiceMock;
	public Mock<IJSRuntime> JsRuntimeMock;
	public Mock<WvBlazorTraceService> WvBlazorTraceServiceMock;
	public TestComponent Component;

	public WvTraceMethodOptions MethodOptions { get; set; } = new();
	public WvTraceSignalOptions SignalOptions { get; set; } = new();
	public string AssemblyName { get; set; } = "Microsoft.System";
	public string ModuleName { get; set; } = "WebVella.BlazorTrace.Tests";
	public string ComponentName { get; set; } = "TestComponent";
	public string ComponentFullName { get => ModuleName + "." + ComponentName; }
	public string MethodName { get; set; } = "OnInitialized";
	public string SignalName { get; set; } = "test-signal";
	public Guid TraceId { get; set; } = Guid.NewGuid();
	public string InstanceTag { get; set; } = Guid.NewGuid().ToString();
	public string OnEnterCustomData { get; set; } = Guid.NewGuid().ToString();
	public string OnExitCustomData { get; set; } = Guid.NewGuid().ToString();
	public string SignalCustomData { get; set; } = Guid.NewGuid().ToString();
	public string FieldName { get; set; } = "_test";
	public long MemoryBytes { get; set; } = 48;
	public long ExtraMemoryBytes { get; set; } = 2048;
	public int DelayMS { get; set; } = 200;
	public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;
	public DateTimeOffset TimeStamp2 { get => TimeStamp.AddMilliseconds(DelayMS); }

	public BaseTest()
	{
		this.WvBlazorTraceConfigurationServiceMock = new Mock<IWvBlazorTraceConfigurationService>();
		this.WvBlazorTraceConfigurationServiceMock.Setup(x => x.GetConfiguraion()).Returns(new WvBlazorTraceConfiguration());
		this.JsRuntimeMock = new Mock<IJSRuntime>();
		this.WvBlazorTraceServiceMock = new Mock<WvBlazorTraceService>(
			this.WvBlazorTraceConfigurationServiceMock.Object,
			false);
		this.Component = new TestComponent();
	}

	public (WvTraceSessionMethod, WvTraceSessionMethodTrace) CheckTraceExists(
		Dictionary<string, WvTraceSessionModule> moduleDict,
		string moduleName,
		string componentFullName,
		string componentName,
		string instanceTag,
		string methodName,
		Guid? traceId)
	{
		WvTraceSessionMethod? method = null;
		WvTraceSessionMethodTrace? trace = null;

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
		var traceList = new List<WvTraceSessionMethodTrace>();
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

	public WvSnapshot GetSnapshot()
	{
		MethodOptions = new WvTraceMethodOptions
		{
			CallLimit = 0
		};
		SignalOptions = new WvTraceSignalOptions
		{
			CallLimit = 0
		};

		var snapshot = new WvSnapshot()
		{
			Id = Guid.NewGuid(),
			CreatedOn = DateTimeOffset.Now,
			Name = "Snapshot",
			ModuleDict = new Dictionary<string, WvTraceSessionModule> {
					{ModuleName,new WvTraceSessionModule{
						ComponentDict = new Dictionary<string, WvTraceSessionComponent>{
							{ComponentFullName, new WvTraceSessionComponent{
								Name = ComponentName,
								TaggedInstances = new List<WvTraceSessionComponentTaggedInstance>{
									new WvTraceSessionComponentTaggedInstance{
										Tag = InstanceTag,
										OnInitialized = new WvTraceSessionMethod{
											Name = MethodName,
											TraceList = new List<WvTraceSessionMethodTrace>{
												new WvTraceSessionMethodTrace{
													TraceId = TraceId,
													OnEnterCustomData = OnEnterCustomData,
													OnExitCustomData = OnExitCustomData,
													EnteredOn = TimeStamp,
													ExitedOn = TimeStamp2,
													OnEnterMemoryBytes = MemoryBytes,
													OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes
														}
													},
													OnExitMemoryBytes = MemoryBytes,
													OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes
														}
													},
													OnEnterOptions = MethodOptions,
													OnExitOptions = MethodOptions
												}
											}
										}
									}
								}
							}}
						}
					}}
				},
			SignalDict = new Dictionary<string, WvTraceSessionSignal>{
					{SignalName, new WvTraceSessionSignal{
						TraceList = new List<WvTraceSessionSignalTrace>{
							new WvTraceSessionSignalTrace{
								SendOn = TimeStamp,
								ModuleName = ModuleName,
								ComponentName = ComponentName,
								ComponentFullName = ComponentFullName,
								InstanceTag = InstanceTag,
								MethodName = MethodName,
								CustomData = SignalCustomData,
								Options = SignalOptions
							}
						}
					}}
				}
		};
		return snapshot;
	}

}
