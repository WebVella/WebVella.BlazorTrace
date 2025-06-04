using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Utility;
namespace WebVella.BlazorTrace.Tests;

public partial class WvBlazorTraceServiceTests : BaseTest
{

	[Fact]
	public void TraceRowGenerationSameSnapshot()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();

			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces,pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			var trRow = CheckTraceRowExists(
				traceRows: result,
				moduleName: ModuleName,
				componentFullName: ComponentFullName,
				componentName: ComponentName,
				instanceTag: InstanceTag,
				methodName: MethodName
			);
			Assert.Single(trRow.TraceList);
			Assert.NotNull(trRow.MethodComparison);
			Assert.Equal(0, trRow.MethodComparison.TraceListChange);
			Assert.Equal(0, trRow.MethodComparison.LastDurationChangeMS);

			Assert.NotNull(trRow.MemoryComparison);
			Assert.Equal(0, trRow.MemoryComparison.LastMemoryChangeBytes);
		}
	}

	[Fact]
	public void TraceRowGenerationSnapshotComparison()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var secondarySN = GetSnapshot();
			secondarySN.Id = Guid.NewGuid();
			secondarySN.Name = "secondary";
			var secondaryMethod = secondarySN.ModuleDict[ModuleName]
				.ComponentDict[ComponentFullName]
				.TaggedInstances[0]
				.OnInitialized;
			secondaryMethod.TraceList.Add(new WvTraceSessionMethodTrace
				{
					TraceId = Guid.NewGuid(),
					OnEnterCustomData = OnEnterCustomData,
					OnExitCustomData = OnEnterCustomData,
					EnteredOn = TimeStamp.AddSeconds(30),
					ExitedOn = TimeStamp2.AddSeconds(30).AddMilliseconds(DelayMS),
					OnEnterFirstRender = FirstRender,
					OnExitFirstRender = FirstRender,
					OnEnterMemoryBytes = MemoryBytes,
					OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = ModuleName,
														FieldName = FieldName,
														Size = MemoryBytes
														}
													},
					OnExitMemoryBytes = MemoryBytes + ExtraMemoryBytes,
					OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = ModuleName,
														FieldName = FieldName,
														Size = MemoryBytes + ExtraMemoryBytes
														}
													},
					OnEnterOptions = Options,
					OnExitOptions = Options
				});
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			var trRow = CheckTraceRowExists(
				traceRows: result,
				moduleName: ModuleName,
				componentFullName: ComponentFullName,
				componentName: ComponentName,
				instanceTag: InstanceTag,
				methodName: MethodName
			);
			Assert.Equal(2, trRow.TraceList.Count);
			Assert.Equal(MemoryBytes + ExtraMemoryBytes, trRow.LastMemoryBytes);
			Assert.Equal(DelayMS * 2, trRow.LastDurationMS);
			Assert.NotNull(trRow.MethodComparison);
			Assert.Equal(1, trRow.MethodComparison.TraceListChange);
			Assert.Equal(DelayMS, trRow.MethodComparison.LastDurationChangeMS);

			Assert.NotNull(trRow.MemoryComparison);
			Assert.Equal(ExtraMemoryBytes, trRow.MemoryComparison.LastMemoryChangeBytes);
		}
	}
}
