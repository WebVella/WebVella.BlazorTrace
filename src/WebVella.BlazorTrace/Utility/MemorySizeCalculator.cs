using System;
using System.Collections.Generic;
using System.Reflection;
using WebVella.BlazorTrace;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Utility;

public class MemorySizeCalculator
{
	/// <summary>
	/// Calculates the memory size of a Blazor component based only on its fields.
	/// </summary>
	/// <typeparam name="T">The type of the component.</typeparam>
	/// <param name="component">The instance of the Blazor component to analyze.</param>
	/// <param name="maxDepth">Maximum depth for recursive field exploration (prevents infinite loops).</param>
	public static long CalculateComponentMemorySize<T>(T component,
		List<WvTraceMemoryInfo> memoryDetails,
		WvBlazorTraceConfiguration configuration,
		int maxDepth = 5)
	{
		if (component == null)
			throw new ArgumentNullException(nameof(component));

		return CalculateSize(
		obj: component,
		valueLabel: component.GetType().FullName!,
		memoryDetails: memoryDetails,
		configuration: configuration,
		currentDepth: maxDepth);
	}

	private static long CalculateSize(object obj,
		string valueLabel,
		List<WvTraceMemoryInfo> memoryDetails,
		WvBlazorTraceConfiguration configuration,
		int currentDepth, HashSet<object> visited = null)
	{
		// Initialize the set on first call
		visited ??= new HashSet<object>();

		if (obj == null || currentDepth < 0)
			return 0;

		Type type = obj.GetType();
		if (type.Assembly is null) return 0;
		if (configuration.MemoryTraceExcludedAssemblyStartWithList.Any(x => type.Assembly.FullName!.StartsWith(x))
			&& !configuration.MemoryTraceIncludedAssemblyStartWithList.Any(x => type.Assembly.FullName!.StartsWith(x)))
		{
			return 0;
		}

		// Skip already processed reference types to prevent infinite loops
		if (!type.IsValueType && visited.Contains(obj))
			return 0;

		// Add ref-type objects to the visited set to avoid cycles
		if (!type.IsValueType)
			visited.Add(obj);

		long size = 0;

		// Process arrays recursively
		if (type.IsArray)
		{
			Array array = (Array)obj;
			for (int i = 0; i < array.Length; i++)
			{
				var element = array.GetValue(i);
				if (element == null) continue;

				if (currentDepth > 0)
					size += CalculateSize(
							obj: element,
							valueLabel: valueLabel,
							memoryDetails: memoryDetails,
							configuration: configuration,
							currentDepth: currentDepth - 1,
							visited: visited);
				else
					// Approximate non-primitive elements as a pointer/ref
					size += 8;
			}
		}
		else if (type.IsValueType)
		{
			// Handle primitive value types directly
			if (IsPrimitiveType(type))
				size += GetPrimitiveSize(obj);
			else
			{
				// Recursively calculate the size of complex struct fields
				var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (var field in fields)
				{
					object fieldValue = field.GetValue(obj);
					if (fieldValue == null) continue;

					if (currentDepth > 0)
						size += CalculateSize(
								obj: fieldValue,
								valueLabel: field.Name,
								memoryDetails: memoryDetails,
								configuration: configuration,
								currentDepth: currentDepth - 1,
								visited: visited);
					else
						// Approximate non-primitive struct elements as a pointer/ref
						size += 8;
				}
			}
		}
		else // Reference type but not array
		{
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (var field in fields)
			{
				object fieldValue = field.GetValue(obj);
				if (fieldValue == null) continue;

				if (currentDepth > 0)
					size += CalculateSize(
							obj: fieldValue,
							valueLabel: field.Name,
							memoryDetails: memoryDetails,
							configuration: configuration,
							currentDepth: currentDepth - 1,
							visited: visited);
				else
					// Approximate non-primitive elements as a pointer/ref
					size += 8;
			}
		}

		// Remove ref-type objects from the set after processing
		if (!type.IsValueType && obj != null)
			visited.Remove(obj);

		if (size > 0)
		{
			var memInfoId = WvTraceUtility.GetMemoryInfoId(type.Assembly.FullName!, valueLabel);
			var memIndex = memoryDetails.FindIndex(x => x.Id == memInfoId);
			if (memIndex < 0)
			{
				memoryDetails.Add(new WvTraceMemoryInfo
				{
					FieldName = valueLabel,
					AssemblyName = type.Assembly.GetName().Name ?? "unknown",
					Size = size,
				});
			}
			else
			{
				memoryDetails[memIndex].Size += size;
			}
		}

		return size;
	}

	private static bool IsPrimitiveType(Type type)
	{
		return type.IsPrimitive ||
			   type == typeof(string) ||
			   type == typeof(decimal);
	}

	private static long GetPrimitiveSize(object value)
	{
		Type t = value.GetType();

		if (t == typeof(bool)) return 1;
		else if (t == typeof(byte) || t == typeof(sbyte)) return 1;
		else if (t == typeof(char)) return 2;
		else if (t == typeof(short) || t == typeof(ushort)) return 2;
		else if (t == typeof(int) || t == typeof(uint) || t == typeof(float)) return 4;
		else if (t == typeof(long) || t == typeof(ulong) || t == typeof(double)) return 8;
		else if (t == typeof(decimal)) return 16; // 128 bits
		else if (t == typeof(string))
			return ((string)value).Length * 2 + 16; // Approximation for string memory including header

		throw new InvalidOperationException($"Unsupported primitive type: {t.FullName}");
	}
}
