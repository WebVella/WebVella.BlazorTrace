using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Utility;
namespace WebVella.BlazorTrace.Tests;

public partial class WvBlazorTraceServiceTests : BaseTest
{
	#region << Module >>
	[Fact]
	public void TraceRowGenerationMuteTraceModule()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.Module,
				Module = ModuleName,
			});

			//when
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}
	#endregion
	#region << Component >>
	[Fact]
	public void TraceRowGenerationMuteTraceComponent()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.Component,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
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
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.ComponentInstance,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				InstanceTag = InstanceTag,
			});

			//when
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	#endregion
	#region << Method >>
	[Fact]
	public void TraceRowGenerationMuteTraceMethod()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.Method,
				Method = MethodName
			});

			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
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
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.MethodInModule,
				Module = ModuleName,
				Method = MethodName
			});

			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
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
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.MethodInComponent,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				Method = MethodName
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
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
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.MethodInComponentInstance,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				InstanceTag = InstanceTag,
				Method = MethodName
			});

			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}
	#endregion
	#region << Field >>
	[Fact]
	public void TraceRowGenerationMuteTraceField()
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
				EnteredOn = TimeStamp,
				ExitedOn = TimeStamp2,
				OnEnterFirstRender = FirstRender,
				OnExitFirstRender = FirstRender,
				OnEnterMemoryBytes = MemoryBytes,
				OnExitMemoryBytes = MemoryBytes + ExtraMemoryBytes,
				OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes
														}
													},
				OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes + ExtraMemoryBytes
														}
													},
				OnEnterOptions = MethodOptions,
				OnExitOptions = MethodOptions
			});
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.NotEqual(0, result[0].LastMemoryBytes);
			Assert.NotEmpty(result[0].MemoryComparison.Fields);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.Field,
				Field = FieldName,
			});

			action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.Equal(0, result[0].LastMemoryBytes);
			Assert.Empty(result[0].MemoryComparison.Fields);
		}
	}
	[Fact]
	public void TraceRowGenerationMuteTraceFieldInModule()
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
				EnteredOn = TimeStamp,
				ExitedOn = TimeStamp2,
				OnEnterFirstRender = FirstRender,
				OnExitFirstRender = FirstRender,
				OnEnterMemoryBytes = MemoryBytes,
				OnExitMemoryBytes = MemoryBytes + ExtraMemoryBytes,
				OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes
														}
													},
				OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes + ExtraMemoryBytes
														}
													},
				OnEnterOptions = MethodOptions,
				OnExitOptions = MethodOptions
			});
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();
			var mutedTraces = new List<WvTraceMute>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.NotEqual(0, result[0].LastMemoryBytes);
			Assert.NotEmpty(result[0].MemoryComparison.Fields);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.FieldInModule,
				Module = ModuleName,
				Field = FieldName
			});

			action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.Equal(0, result[0].LastMemoryBytes);
			Assert.Empty(result[0].MemoryComparison.Fields);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceFieldInComponent()
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
				EnteredOn = TimeStamp,
				ExitedOn = TimeStamp2,
				OnEnterFirstRender = FirstRender,
				OnExitFirstRender = FirstRender,
				OnEnterMemoryBytes = MemoryBytes,
				OnExitMemoryBytes = MemoryBytes + ExtraMemoryBytes,
				OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes
														}
													},
				OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes + ExtraMemoryBytes
														}
													},
				OnEnterOptions = MethodOptions,
				OnExitOptions = MethodOptions
			});
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.NotEqual(0, result[0].LastMemoryBytes);
			Assert.NotEmpty(result[0].MemoryComparison.Fields);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.FieldInComponent,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				Field = FieldName
			});

			action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.Equal(0, result[0].LastMemoryBytes);
			Assert.Empty(result[0].MemoryComparison.Fields);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceFieldInComponentInstance()
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
				EnteredOn = TimeStamp,
				ExitedOn = TimeStamp2,
				OnEnterFirstRender = FirstRender,
				OnExitFirstRender = FirstRender,
				OnEnterMemoryBytes = MemoryBytes,
				OnExitMemoryBytes = MemoryBytes + ExtraMemoryBytes,
				OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes
														}
													},
				OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes + ExtraMemoryBytes
														}
													},
				OnEnterOptions = MethodOptions,
				OnExitOptions = MethodOptions
			});
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.NotEqual(0, result[0].LastMemoryBytes);
			Assert.NotEmpty(result[0].MemoryComparison.Fields);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.FieldInComponentInstance,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				InstanceTag = InstanceTag,
				Field = FieldName
			});

			action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.Equal(0, result[0].LastMemoryBytes);
			Assert.Empty(result[0].MemoryComparison.Fields);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceFieldInAssembly()
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
				EnteredOn = TimeStamp,
				ExitedOn = TimeStamp2,
				OnEnterFirstRender = FirstRender,
				OnExitFirstRender = FirstRender,
				OnEnterMemoryBytes = MemoryBytes,
				OnExitMemoryBytes = MemoryBytes + ExtraMemoryBytes,
				OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes
														}
													},
				OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes + ExtraMemoryBytes
														}
													},
				OnEnterOptions = MethodOptions,
				OnExitOptions = MethodOptions
			});
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.NotEqual(0, result[0].LastMemoryBytes);
			Assert.NotEmpty(result[0].MemoryComparison.Fields);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.FieldInAssembly,
				Assembly = AssemblyName,
				Field = FieldName
			});
			action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.Equal(0, result[0].LastMemoryBytes);
			Assert.Empty(result[0].MemoryComparison.Fields);
		}
	}

	#endregion
	#region << Assembly >>
	[Fact]
	public void TraceRowGenerationMuteTraceAssembly()
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
				EnteredOn = TimeStamp,
				ExitedOn = TimeStamp2,
				OnEnterFirstRender = FirstRender,
				OnExitFirstRender = FirstRender,
				OnEnterMemoryBytes = MemoryBytes,
				OnExitMemoryBytes = MemoryBytes + ExtraMemoryBytes,
				OnEnterMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes
														}
													},
				OnExitMemoryInfo = new List<WvTraceMemoryInfo>{
														new WvTraceMemoryInfo{
														AssemblyName = AssemblyName,
														FieldName = FieldName,
														Size = MemoryBytes + ExtraMemoryBytes
														}
													},
				OnEnterOptions = MethodOptions,
				OnExitOptions = MethodOptions
			});
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.NotEqual(0, result[0].LastMemoryBytes);
			Assert.NotEmpty(result[0].MemoryComparison.Fields);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.Assembly,
				Assembly = AssemblyName,
			});

			action = () => result = primarySN.GenerateMethodTraceRows(secondarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.Equal(0, result[0].LastMemoryBytes);
			Assert.Empty(result[0].MemoryComparison.Fields);
		}
	}
	#endregion
	#region << Custom Data >>
	[Fact]
	public void TraceRowGenerationMuteTraceOnEnterCustomData()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.OnEnterCustomData,
				OnEnterCustomData = OnEnterCustomData,
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}
	[Fact]
	public void TraceRowGenerationMuteTraceOnEnterCustomDataInModule()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.OnEnterCustomDataInModule,
				OnEnterCustomData = OnEnterCustomData,
				Module = ModuleName,
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceOnEnterCustomDataInComponent()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.OnEnterCustomDataInComponent,
				OnEnterCustomData = OnEnterCustomData,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceOnEnterCustomDataInComponentInstance()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute 
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.OnEnterCustomDataInComponentInstance,
				OnEnterCustomData = OnEnterCustomData,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				InstanceTag = InstanceTag,
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceOnExitCustomData()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.OnExitCustomData,
				OnExitCustomData = OnExitCustomData,
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}
	[Fact]
	public void TraceRowGenerationMuteTraceOnExitCustomDataInModule()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();
	
			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.OnExitCustomDataInModule,
				OnExitCustomData = OnExitCustomData,
				Module = ModuleName,
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceOnExitCustomDataInComponent()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.OnExitCustomDataInComponent,
				OnExitCustomData = OnExitCustomData,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceOnExitCustomDataInComponentInstance()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();
			
			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);
			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.OnExitCustomDataInComponentInstance,
				OnExitCustomData = OnExitCustomData,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				InstanceTag = InstanceTag,
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}
	#endregion
	#region << Limit >>
	[Fact]
	public void TraceRowGenerationMuteTraceLimit()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var result = new List<WvMethodTraceRow>();
			var pins = new List<string>();

			//No Mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.NotEmpty(result[0].LimitHits);

			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.Limit,
				LimitType = WvTraceSessionLimitType.CallCount
			});

			//when
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.Empty(result[0].LimitHits);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceLimitInModule()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.NotEmpty(result[0].LimitHits);
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.LimitInModule,
				Module = ModuleName,
				LimitType = WvTraceSessionLimitType.CallCount
			});

			//when
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.Empty(result[0].LimitHits);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceLimitInComponent()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.NotEmpty(result[0].LimitHits);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.LimitInComponent,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				LimitType = WvTraceSessionLimitType.CallCount
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.Empty(result[0].LimitHits);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceLimitInComponentInstance()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvMethodTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.NotEmpty(result[0].LimitHits);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.LimitInComponentInstance,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				InstanceTag = InstanceTag,
				LimitType = WvTraceSessionLimitType.CallCount
			});
			action = () => result = primarySN.GenerateMethodTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Single(result);
			Assert.Empty(result[0].LimitHits);
		}
	}
	#endregion
	#region << Signal >>
	[Fact]
	public void TraceRowGenerationMuteTraceSignal()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvSignalTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateSignalTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.Signal,
				Signal = SignalName
			});

			action = () => result = primarySN.GenerateSignalTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceSignalInModule()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvSignalTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateSignalTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.SignalInModule,
				Module = ModuleName,
				Signal = SignalName
			});

			action = () => result = primarySN.GenerateSignalTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceSignalInComponent()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvSignalTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateSignalTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.SignalInComponent,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				Signal = SignalName
			});
			action = () => result = primarySN.GenerateSignalTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}

	[Fact]
	public void TraceRowGenerationMuteTraceSignalInComponentInstance()
	{
		lock (_locker)
		{
			//given
			var primarySN = GetSnapshot();
			var mutedTraces = new List<WvTraceMute>();
			var pins = new List<string>();
			var result = new List<WvSignalTraceRow>();

			//Without mute
			Action action = () => result = primarySN.GenerateSignalTraceRows(primarySN, mutedTraces, pins);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.NotEmpty(result);

			//With mute
			mutedTraces.Add(new WvTraceMute
			{
				Type = WvTraceMuteType.SignalInComponentInstance,
				ComponentFullName = ComponentFullName,
				ComponentName = ComponentName,
				InstanceTag = InstanceTag,
				Signal = SignalName
			});

			action = () => result = primarySN.GenerateSignalTraceRows(primarySN, mutedTraces, pins);
			ex = Record.Exception(action);
			Assert.Null(ex);
			Assert.Empty(result);
		}
	}
	#endregion
}
