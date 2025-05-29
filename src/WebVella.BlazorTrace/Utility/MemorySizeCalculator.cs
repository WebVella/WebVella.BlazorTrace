using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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
		measuredObject: component,
		component: component,
		valueLabel: component.GetType().FullName!,
		memoryDetails: memoryDetails,
		configuration: configuration,
		currentDepth: maxDepth,
		maxDepth:maxDepth);
	}

	private static long CalculateSize(
		object measuredObject,
		object component,
		string valueLabel,
		List<WvTraceMemoryInfo> memoryDetails,
		WvBlazorTraceConfiguration configuration,
		int currentDepth,
		int maxDepth,
		HashSet<object>? visited = null)
	{
		// Initialize the set on first call
		visited ??= new HashSet<object>();
		if (measuredObject == null || currentDepth < 0)
			return 0;

		Type type = measuredObject.GetType();
		if (type.Assembly is null) return 0;
		if (configuration.MemoryExcludeAssemblyList.Any(x => type.Assembly.FullName!.StartsWith(x))
			&& !configuration.MemoryIncludeAssemblyList.Any(x => type.Assembly.FullName!.StartsWith(x)))
		{
			return 0;
		}
		if (configuration.MemoryExcludeFieldNameList.Any(x => valueLabel.Contains(x))
			&& !configuration.MemoryIncludeFieldNameList.Any(x => valueLabel.Contains(x)))
		{
			return 0;
		}

		// Skip already processed reference types to prevent infinite loops
		if (!type.IsValueType && visited.Contains(measuredObject))
			return 0;

		// Add ref-type objects to the visited set to avoid cycles
		if (!type.IsValueType)
			visited.Add(measuredObject);

		long size = 0;

		// Process arrays recursively
		if (type.IsArray)
		{
			Array array = (Array)measuredObject;
			for (int i = 0; i < array.Length; i++)
			{
				var element = array.GetValue(i);
				if (element == null) continue;

				if (currentDepth > 0)
					size += CalculateSize(
							measuredObject: element,
							component: component,
							valueLabel: valueLabel,
							memoryDetails: memoryDetails,
							configuration: configuration,
							currentDepth: currentDepth - 1,
							maxDepth: maxDepth,
							visited: visited);
				else
					// Approximate non-primitive elements as a pointer/ref
					size += 8;
			}
		}
		else if (type.IsValueType)
		{
			// Handle primitive value types directly
			if (WvBTIsPrimitiveType(type))
			{
				size += WvBTGetPrimitiveSize(measuredObject);
			}
			else
			{
				// Recursively calculate the size of complex struct fields
				var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				foreach (var field in fields)
				{
					object? fieldValue = field.GetValue(measuredObject);
					if (fieldValue == null) continue;

					if (currentDepth > 0)
						size += CalculateSize(
								measuredObject: fieldValue,
								component: component,
								valueLabel: field.Name,
								memoryDetails: memoryDetails,
								configuration: configuration,
								currentDepth: currentDepth - 1,
								maxDepth: maxDepth,
								visited: visited);
					else
						// Approximate non-primitive struct elements as a pointer/ref
						size += 8;
				}
			}
		}
		else // Reference type but not array
		{
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

			foreach (var field in fields)
			{
				object? fieldValue = field.GetValue(measuredObject);
				if (fieldValue == null) continue;

				if (currentDepth > 0)
					size += CalculateSize(
							measuredObject: fieldValue,
							component: component,
							valueLabel: field.Name,
							memoryDetails: memoryDetails,
							configuration: configuration,
							currentDepth: currentDepth - 1,
							maxDepth: maxDepth,
							visited: visited);
				else
					// Approximate non-primitive elements as a pointer/ref
					size += 8;
			}
		}

		// Remove ref-type objects from the set after processing
		if (!type.IsValueType && measuredObject != null)
			visited.Remove(measuredObject);

		//Exclude the component itself from the log
		//maxDepth is the component, maxDepth-1 are the props of it
		if (size > 0 && currentDepth == maxDepth-1)
		{
			var memInfoId = WvTraceUtility.WvBTGetMemoryInfoId(type.Assembly.FullName!, valueLabel);
			var memIndex = memoryDetails.FindIndex(x => x.Id == memInfoId);
			if (memIndex < 0)
			{
				memoryDetails.Add(new WvTraceMemoryInfo
				{
					FieldName = valueLabel,
					AssemblyName = type.Assembly.GetName().Name ?? "unknown",
					TypeName = type.Name,
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

	private static bool WvBTIsPrimitiveType(Type type)
	{
		return type.IsPrimitive ||
			   type == typeof(string) ||
			   type == typeof(decimal);
	}

	private static long WvBTGetPrimitiveSize(object value)
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
