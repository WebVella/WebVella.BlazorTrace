﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvTraceModalMenuItem
{
	public WvTraceModalMenu Id { get; set; } = WvTraceModalMenu.MethodName;
	public bool IsActive { get; set; } = false;
	public int Counter { get; set; } = 0;
	[JsonIgnore]
	public string CounterHtml
	{
		get
		{
			if (Counter <= 0) return String.Empty;
			return $"<span class='wv-badge'>{(Counter > 99 ? "99+" : Counter)}</span>";
		}
	}
	[JsonIgnore]
	public Action OnClick { get; set; } = default!;
}
