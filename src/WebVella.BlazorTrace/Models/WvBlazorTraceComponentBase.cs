using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvBlazorTraceComponentBase : ComponentBase
{
	public bool IsRenderLockEnabled { get; private set; } = false;
	public Guid CurrentRenderLock { get; private set; } = Guid.Empty;
	public Guid OldRenderLock { get; private set; } = Guid.Empty;

	protected void EnableRenderLock() => IsRenderLockEnabled = true;
	protected void DisableRenderLock() => IsRenderLockEnabled = false;
	protected void RegenRenderLock() => CurrentRenderLock = Guid.NewGuid();
	protected override void OnParametersSet()
	{
		if (IsRenderLockEnabled) RegenRenderLock();
	}

	protected override bool ShouldRender()
	{
		if (!IsRenderLockEnabled) return true;
		if (CurrentRenderLock == OldRenderLock) return false;
		OldRenderLock = CurrentRenderLock;
		return true;
	}
}

