using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace.Tests;

public partial class WvBlazorTraceServiceTests : BaseTest
{

	[Fact]
	public void LimitDuration1()
	{
		lock (_locker)
		{
			//given
			var options = new WvTraceMethodOptions { 
				DurationLimitMS = 1
			};
			var firstRender = true;
			var moduleName = "WebVella.BlazorTrace.Tests";
			var componentName = "TestComponent";
			var componentFullName = moduleName + "." + componentName;
			var methodName = "SlowMethod";
			var traceId = Guid.NewGuid();
			var instanceTag = Guid.NewGuid().ToString();
			var callTag = Guid.NewGuid().ToString();
			//when
			WvBlazorTraceServiceMock.Object.OnEnter(
				component: Component,
				traceId: traceId,
				options: options,
				firstRender: firstRender,
				instanceTag: instanceTag,
				callTag: callTag,
				methodName: methodName
			);
			Thread.Sleep(5);
			WvBlazorTraceServiceMock.Object.OnExit(
				component: Component,
				traceId: traceId,
				options: options,
				firstRender: firstRender,
				instanceTag: instanceTag,
				callTag: callTag,
				methodName: methodName
			);
			
			//than
			var queue = WvBlazorTraceServiceMock.Object.GetQueue();
			Assert.NotEmpty(queue);
			WvBlazorTraceServiceMock.Object.ProcessQueueForTests();
			var (method,trace) = CheckTraceExists(
				moduleDict: WvBlazorTraceServiceMock.Object.GetModuleDict(),
				moduleName: moduleName,
				componentFullName: componentFullName,
				componentName: componentName,
				instanceTag: instanceTag,
				methodName: methodName,
				traceId: traceId
			);
			Assert.Single(method.LimitHits);
			var limitHit = method.LimitHits.First();
			Assert.Equal(WvTraceSessionLimitType.Duration,limitHit.Type);
			Assert.Equal(options.DurationLimitMS,limitHit.Limit);
			Assert.Equal(false,limitHit.IsOnEnter);
		}
	}

}
