using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace.Tests;

public class MethodCalls : BaseTest
{
	#region << OnEnter >>

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
				var callTag = Guid.NewGuid().ToString();
				//when
				Action action = () => WvBlazorTraceServiceMock.Object.OnEnter(
					component: Component,
					traceId: traceId,
					options: options,
					firstRender: firstRender,
					instanceTag: instanceTag,
					callTag: callTag,
					methodName: methodName
				);
				var ex = Record.Exception(action);
				Assert.Null(ex);
				//than
				var queue = WvBlazorTraceServiceMock.Object.GetQueue();
				Assert.NotEmpty(queue);
				WvBlazorTraceServiceMock.Object.ProcessQueueForTests();
				var trace = CheckTraceExists(
					moduleDict: WvBlazorTraceServiceMock.Object.GetModuleDict(),
					moduleName: moduleName,
					componentFullName: componentFullName,
					componentName: componentName,
					instanceTag: instanceTag,
					methodName: methodName,
					traceId: traceId
				);
				Assert.NotNull(trace.EnteredOn);
				Assert.Null(trace.ExitedOn);
			}
		}
	}

	#endregion

	#region << OnExit >>
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
				var callTag = Guid.NewGuid().ToString();
				//when
				Action action = () => WvBlazorTraceServiceMock.Object.OnExit(
					component: Component,
					traceId: traceId,
					options: options,
					firstRender: firstRender,
					instanceTag: instanceTag,
					callTag: callTag,
					methodName: methodName
				);
				var ex = Record.Exception(action);
				Assert.Null(ex);
				//than
				var queue = WvBlazorTraceServiceMock.Object.GetQueue();
				Assert.NotEmpty(queue);
				WvBlazorTraceServiceMock.Object.ProcessQueueForTests();
				var trace = CheckTraceExists(
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
			}
		}
	}

	#endregion
}
