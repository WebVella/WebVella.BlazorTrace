using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace.Tests;

public partial class WvBlazorTraceServiceTests : BaseTest
{

	[Fact]
	public void OnEnterTestBaseCalls()
	{
		lock (_locker)
		{
			//given
			var options = new WvTraceMethodOptions { };
			var firstRender = true;
			var moduleName = "WebVella.BlazorTrace.Tests";
			var componentName = "TestComponent";
			var componentFullName = moduleName + "." + componentName;
			var methodsList = new List<string>{
				"OnInitialized","OnInitializedAsync",
				"OnParametersSet","OnParametersSetAsync",
				"OnAfterRender","OnAfterRenderAsync",
				"ShouldRender",
				"Dispose","DisposeAsync",
				"OnEnterTestOther","OnEnterTestOtherAsync"
			};

			foreach (var methodName in methodsList)
			{
				var traceId = Guid.NewGuid();
				var instanceTag = Guid.NewGuid().ToString();
				var customData = Guid.NewGuid().ToString();
				//when
				Action action = () => WvBlazorTraceServiceMock.Object.OnEnter(
					component: Component,
					traceId: traceId,
					options: options,
					firstRender: firstRender,
					instanceTag: instanceTag,
					customData: customData,
					methodName: methodName
				);
				var ex = Record.Exception(action);
				Assert.Null(ex);
				//than
				var queue = WvBlazorTraceServiceMock.Object.GetQueue();
				Assert.NotEmpty(queue);
				WvBlazorTraceServiceMock.Object.ForceProcessQueue();
				var (method, trace) = CheckTraceExists(
					moduleDict: WvBlazorTraceServiceMock.Object.GetModuleDict(),
					moduleName: moduleName,
					componentFullName: componentFullName,
					componentName: componentName,
					instanceTag: instanceTag,
					methodName: methodName,
					traceId: traceId
				);
				Assert.NotNull(trace.EnteredOn);
				Assert.NotNull(trace.OnEnterOptions);
				Assert.Null(trace.ExitedOn);
			}
		}
	}

	[Fact]
	public void OnExitTestBaseCalls()
	{
		lock (_locker)
		{
			//given
			var options = new WvTraceMethodOptions { };
			var firstRender = true;
			var moduleName = "WebVella.BlazorTrace.Tests";
			var componentName = "TestComponent";
			var componentFullName = moduleName + "." + componentName;
			var methodsList = new List<string>{
				"OnInitialized","OnInitializedAsync",
				"OnParametersSet","OnParametersSetAsync",
				"OnAfterRender","OnAfterRenderAsync",
				"ShouldRender",
				"Dispose","DisposeAsync",
				"OnEnterTestOther","OnEnterTestOtherAsync"
			};
			foreach (var methodName in methodsList)
			{
				var traceId = Guid.NewGuid();
				var instanceTag = Guid.NewGuid().ToString();
				var customData = Guid.NewGuid().ToString();
				//when
				Action action = () => WvBlazorTraceServiceMock.Object.OnExit(
					component: Component,
					traceId: traceId,
					options: options,
					firstRender: firstRender,
					instanceTag: instanceTag,
					customData: customData,
					methodName: methodName
				);
				var ex = Record.Exception(action);
				Assert.Null(ex);
				//than
				var queue = WvBlazorTraceServiceMock.Object.GetQueue();
				Assert.NotEmpty(queue);
				WvBlazorTraceServiceMock.Object.ForceProcessQueue();
				var (method, trace) = CheckTraceExists(
					moduleDict: WvBlazorTraceServiceMock.Object.GetModuleDict(),
					moduleName: moduleName,
					componentFullName: componentFullName,
					componentName: componentName,
					instanceTag: instanceTag,
					methodName: methodName,
					traceId: traceId
				);
				Assert.Null(trace.EnteredOn);
				Assert.NotNull(trace.ExitedOn);
				Assert.NotNull(trace.OnExitOptions);
			}
		}
	}

	[Fact]
	public void SignalTestBaseCalls()
	{
		lock (_locker)
		{
			//given
			var options = new WvTraceSignalOptions { };
			var moduleName = "WebVella.BlazorTrace.Tests";
			var signalName = "test-signal";
			var componentName = "TestComponent";
			var componentFullName = moduleName + "." + componentName;
			var methodName = "CustomMethod";

			var traceId = Guid.NewGuid();
			var instanceTag = Guid.NewGuid().ToString();
			var customData = Guid.NewGuid().ToString();
			//when
			Action action = () => WvBlazorTraceServiceMock.Object.OnSignal(
				caller: Component,
				signalName: signalName,
				instanceTag: instanceTag,
				customData: customData,
				options: options,
				methodName: methodName
			);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			//than
			var queue = WvBlazorTraceServiceMock.Object.GetQueue();
			Assert.NotEmpty(queue);
			WvBlazorTraceServiceMock.Object.ForceProcessQueue();
			var (method, trace) = CheckSignalTraceExists(
				moduleDict: WvBlazorTraceServiceMock.Object.GetModuleDict(),
				moduleName: moduleName,
				signalName : signalName,
				componentFullName: componentFullName,
				instanceTag: instanceTag,
				methodName: methodName
			);
			Assert.NotNull(trace.SendOn);
			Assert.NotNull(trace.Options);
		}
	}
}
