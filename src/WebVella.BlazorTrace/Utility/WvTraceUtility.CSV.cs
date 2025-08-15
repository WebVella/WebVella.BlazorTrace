using WebVella.BlazorTrace.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace WebVella.BlazorTrace.Utility;
public static partial class WvTraceUtility
{

	public static string ExportToCsv(List<WvMethodTraceRow> rows)
	{
		if (rows is null || rows.Count == 0)
			return String.Empty;

		var csvRows = new List<WvMethodTraceCSVRow>();
		rows.ForEach(x=> csvRows.AddRange(x.GetAsCsvRow()));

		var sb = new StringBuilder();
		var header = new List<string>{ 
			"Module",
			"Component",
			"ComponentFullName",
			"InstanceTag",
			"Method",
			"TraceId",
			"EnteredOn",
			"ExitedOn",
			"DurationMs",
			"OnEnterMemory",
			"OnExitMemory",
			"MemoryDelta",
			"OnEnterCustomData",
			"OnExitCustomData"
		};

		sb.AppendLine(string.Join(",", header));
		foreach (var item in csvRows)
		{
			var values = new List<string>();
			values.Add(EscapeCsvValue(item.Module));
			values.Add(EscapeCsvValue(item.Component));
			values.Add(EscapeCsvValue(item.ComponentFullName));
			values.Add(EscapeCsvValue(item.InstanceTag));
			values.Add(EscapeCsvValue(item.Method));
			values.Add(EscapeCsvValue(item.TraceId));
			values.Add(EscapeCsvValue(item.EnteredOn));
			values.Add(EscapeCsvValue(item.ExitedOn));
			values.Add(EscapeCsvValue(item.DurationMs));
			values.Add(EscapeCsvValue(item.OnEnterMemory));
			values.Add(EscapeCsvValue(item.OnExitMemory));
			values.Add(EscapeCsvValue(item.MemoryDelta));
			values.Add(EscapeCsvValue(item.OnEnterCustomData));
			values.Add(EscapeCsvValue(item.OnExitCustomData));

			sb.AppendLine(string.Join(",", values));
		}

		

		return sb.ToString();
	}

	public static string ExportToCsv(List<WvSignalTraceRow> rows)
	{
if (rows is null || rows.Count == 0)
			return String.Empty;

		var csvRows = new List<WvSignalTraceCSVRow>();
		rows.ForEach(x=> csvRows.AddRange(x.GetAsCsvRow()));

		var sb = new StringBuilder();
		var header = new List<string>{ 
			"Signal",
			"SendOn",
			"Module",
			"Component",
			"ComponentFullName",
			"InstanceTag",
			"Method",
			"CustomData"
		};

		sb.AppendLine(string.Join(",", header));
		foreach (var item in csvRows)
		{
			var values = new List<string>();
			values.Add(EscapeCsvValue(item.SignalName));
			values.Add(EscapeCsvValue(item.SendOn));
			values.Add(EscapeCsvValue(item.ModuleName));
			values.Add(EscapeCsvValue(item.ComponentName));
			values.Add(EscapeCsvValue(item.ComponentFullName));
			values.Add(EscapeCsvValue(item.InstanceTag));
			values.Add(EscapeCsvValue(item.MethodName));
			values.Add(EscapeCsvValue(item.CustomData));

			sb.AppendLine(string.Join(",", values));
		}

		

		return sb.ToString();
	}

	private static string EscapeCsvValue(string? value)
	{
		if (string.IsNullOrEmpty(value))
			return string.Empty;

		// If value contains comma, quote, or newline, escape it
		if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
		{
			// Escape quotes by doubling them
			var escaped = value.Replace("\"", "\"\"");
			return $"\"{escaped}\"";
		}

		return value;
	}

}
