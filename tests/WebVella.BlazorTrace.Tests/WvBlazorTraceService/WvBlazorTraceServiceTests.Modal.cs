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
			var options = new WvTraceMethodOptions
			{
				DurationLimitMS = 1
			};
			var firstRender = true;
			var moduleName = "WebVella.BlazorTrace.Tests";
			var componentName = "TestComponent";
			var componentFullName = moduleName + "." + componentName;
			var methodName = "OnInitialized";
			var traceId = Guid.NewGuid();
			var instanceTag = Guid.NewGuid().ToString();
			var customData = Guid.NewGuid().ToString();

			var primarySN = new WvSnapshot()
			{
				Id = Guid.NewGuid(),
				CreatedOn = DateTimeOffset.Now,
				Name = "Test",
				ModuleDict = new Dictionary<string, WvTraceSessionModule> {
					{moduleName,new WvTraceSessionModule{
						ComponentDict = new Dictionary<string, WvTraceSessionComponent>{
							{componentFullName, new WvTraceSessionComponent{
								Name = componentName,
								TaggedInstances = new List<WvTraceSessionComponentTaggedInstance>{
									new WvTraceSessionComponentTaggedInstance{
										Tag = instanceTag,
										OnInitialized = new WvTraceSessionMethod{
											Name = methodName,
											TraceList = new List<WvTraceSessionTrace>{
												new WvTraceSessionTrace{
													TraceId = traceId,
													OnEnterCustomData = customData,
													OnExitCustomData = customData,
													EnteredOn = DateTimeOffset.Now,
													ExitedOn = DateTimeOffset.Now.AddMilliseconds(15),
													OnEnterFirstRender = firstRender,
													OnExitFirstRender = firstRender,
													OnEnterMemoryBytes = null,
													OnEnterMemoryInfo = null,
													OnExitMemoryBytes = null,
													OnExitMemoryInfo = null,
													OnEnterOptions = options,
													OnExitOptions = options
												}
											}
										}
									}
								}
							}}
						}
					}}
				}
			};


			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			var trRow = CheckTraceRowExists(
				traceRows: result,
				moduleName: moduleName,
				componentFullName: componentFullName,
				componentName: componentName,
				instanceTag: instanceTag,
				methodName: methodName
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
			var options = new WvTraceMethodOptions
			{
				DurationLimitMS = 1
			};
			var firstRender = true;
			var moduleName = "WebVella.BlazorTrace.Tests";
			var componentName = "TestComponent";
			var componentFullName = moduleName + "." + componentName;
			var methodName = "OnInitialized";
			var traceId = Guid.NewGuid();
			var instanceTag = Guid.NewGuid().ToString();
			var customData = Guid.NewGuid().ToString();
			var timestamp = DateTimeOffset.Now;
			var timestamp2 = DateTimeOffset.Now.AddSeconds(2);
			int delayMS = 200;
			string memoryFieldName = "_test";
			long extraMemoryBytes = 2048;
			var primarySN = new WvSnapshot()
			{
				Id = Guid.NewGuid(),
				CreatedOn = DateTimeOffset.Now,
				Name = "Test",
				ModuleDict = new Dictionary<string, WvTraceSessionModule> {
					{moduleName,new WvTraceSessionModule{
						ComponentDict = new Dictionary<string, WvTraceSessionComponent>{
							{componentFullName, new WvTraceSessionComponent{
								Name = componentName,
								TaggedInstances = new List<WvTraceSessionComponentTaggedInstance>{
									new WvTraceSessionComponentTaggedInstance{
										Tag = instanceTag,
										OnInitialized = new WvTraceSessionMethod{
											Name = methodName,
											TraceList = new List<WvTraceSessionTrace>{
												new WvTraceSessionTrace{
													TraceId = traceId,
													OnEnterCustomData = customData,
													OnExitCustomData = customData,
													EnteredOn = timestamp,
													ExitedOn = timestamp,
													OnEnterFirstRender = firstRender,
													OnExitFirstRender = firstRender,
													OnEnterMemoryBytes = 0,
													OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = moduleName,
														FieldName = memoryFieldName,
														Size = 0
														}
													},
													OnExitMemoryBytes = 0,
													OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = moduleName,
														FieldName = memoryFieldName,
														Size = 0
														}
													},
													OnEnterOptions = options,
													OnExitOptions = options
												}
											}
										}
									}
								}
							}}
						}
					}}
				}
			};

			var secondarySN = new WvSnapshot()
			{
				Id = Guid.NewGuid(),
				CreatedOn = DateTimeOffset.Now,
				Name = "Test 2",
				ModuleDict = new Dictionary<string, WvTraceSessionModule> {
					{moduleName,new WvTraceSessionModule{
						ComponentDict = new Dictionary<string, WvTraceSessionComponent>{
							{componentFullName, new WvTraceSessionComponent{
								Name = componentName,
								TaggedInstances = new List<WvTraceSessionComponentTaggedInstance>{
									new WvTraceSessionComponentTaggedInstance{
										Tag = instanceTag,
										OnInitialized = new WvTraceSessionMethod{
											Name = methodName,
											TraceList = new List<WvTraceSessionTrace>{
												new WvTraceSessionTrace{
													TraceId = traceId,
													OnEnterCustomData = customData,
													OnExitCustomData = customData,
													EnteredOn = timestamp,
													ExitedOn = timestamp,
													OnEnterFirstRender = firstRender,
													OnExitFirstRender = firstRender,
													OnEnterMemoryBytes = 0,
													OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = moduleName,
														FieldName = memoryFieldName,
														Size = 0
														}
													},
													OnExitMemoryBytes = 0,
													OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = moduleName,
														FieldName = memoryFieldName,
														Size = 0
														}
													},
													OnEnterOptions = options,
													OnExitOptions = options
												},
												new WvTraceSessionTrace{
													TraceId = traceId,
													OnEnterCustomData = customData,
													OnExitCustomData = customData,
													EnteredOn = timestamp2,
													ExitedOn = timestamp2.AddMilliseconds(delayMS),
													OnEnterFirstRender = firstRender,
													OnExitFirstRender = firstRender,
													OnEnterMemoryBytes = 0,
													OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = moduleName,
														FieldName = memoryFieldName,
														Size = 0
														}
													},
													OnExitMemoryBytes = extraMemoryBytes,
													OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = moduleName,
														FieldName = memoryFieldName,
														Size = extraMemoryBytes
														}
													},
													OnEnterOptions = options,
													OnExitOptions = options
												}
											}
										}
									}
								}
							}}
						}
					}}
				}
			};


			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(secondarySN);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			var trRow = CheckTraceRowExists(
				traceRows: result,
				moduleName: moduleName,
				componentFullName: componentFullName,
				componentName: componentName,
				instanceTag: instanceTag,
				methodName: methodName
			);
			Assert.Equal(2,trRow.TraceList.Count);
			Assert.Equal(extraMemoryBytes.ToKilobytes(),trRow.LastMemoryKB);
			Assert.Equal(delayMS,trRow.LastDurationMS);
			Assert.NotNull(trRow.MethodComparison);
			Assert.Equal(1, trRow.MethodComparison.TraceListChange);
			Assert.Equal(delayMS, trRow.MethodComparison.LastDurationChangeMS);

			Assert.NotNull(trRow.MemoryComparison);
			Assert.Equal(extraMemoryBytes, trRow.MemoryComparison.LastMemoryChangeBytes);
		}
	}
}
