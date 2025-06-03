using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Utility;
namespace WebVella.BlazorTrace.Tests;

public partial class WvBlazorTraceServiceTests : BaseTest
{

	[Fact]
	public void TraceRowGenerationMuteTraceModule()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();

			var mutedTraces = new List<WvTraceMute>();
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.Module,
				Module = ModuleName,
			});
			var pins = new List<string>();
			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceComponent()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();

			var mutedTraces = new List<WvTraceMute>();
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.Component,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
			});
			var pins = new List<string>();
			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceComponentInstance()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();

			var mutedTraces = new List<WvTraceMute>();
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.ComponentInstance,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				InstanceTag = InstanceTag,
			});
			var pins = new List<string>();
			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceMethod()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();

			var mutedTraces = new List<WvTraceMute>();
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.Method,
				Method = MethodName
			});
			var pins = new List<string>();
			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceMethodInModule()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();

			var mutedTraces = new List<WvTraceMute>();
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.MethodInModule,
				Module = ModuleName,
				Method = MethodName
			});
			var pins = new List<string>();
			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceMethodInComponent()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();

			var mutedTraces = new List<WvTraceMute>();
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.MethodInComponent,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				Method = MethodName
			});
			var pins = new List<string>();
			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceMethodInComponentInstance()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();

			var mutedTraces = new List<WvTraceMute>();
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.MethodInComponentInstance,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				InstanceTag = InstanceTag,
				Method = MethodName
			});
			var pins = new List<string>();
			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceField()
	{
		lock (_locker)
		{
			//given
			long extraMemoryBytes = 2048;
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
				OnEnterCustomData = CustomData,
				OnExitCustomData = CustomData,
				EnteredOn = TimeStamp.AddSeconds(30),
				ExitedOn = TimeStamp2.AddSeconds(30).AddMilliseconds(DelayMS),
				OnEnterFirstRender = FirstRender,
				OnExitFirstRender = FirstRender,
				OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = ModuleName,
														FieldName = FieldName,
														Size = 0
														}
													},
				OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = ModuleName,
														FieldName = FieldName,
														Size = extraMemoryBytes
														}
													},
				OnEnterOptions = Options,
				OnExitOptions = Options
			});
			var mutedTraces = new List<WvTraceMute>();
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.Field,
				Field = FieldName,
			});
			var pins = new List<string>();
			//when
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.Equal(0, result[0].LastMemoryBytes);
			Assert.Empty(result[0].MemoryComparison.Fields);
		}
	}
}
