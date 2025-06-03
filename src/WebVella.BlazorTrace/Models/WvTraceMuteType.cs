using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public enum WvTraceMuteType
{
	[Description("<span>Module</span>")]
	Module = 0,
	[Description("<span>Component</span>")]
	Component = 1,
	[Description("<span>Component Instance</span>")]
	ComponentInstance = 2,
	[Description("<span>Method</span>")]
	Method = 3,
	[Description("<span>Method</span> <span class='wv-mute'>in</span> <span>Module</span>")]
	MethodInModule = 4,
	[Description("<span>Method</span> <span class='wv-mute'>in</span> <span>Component</span>")]
	MethodInComponent = 5,
	[Description("<span>Method</span> <span class='wv-mute'>in</span> <span>Component Instance</span>")]
	MethodInComponentInstance = 6,
	[Description("<span>Signal</span>")]
	Signal = 7,
	[Description("<span>Field</span>")]
	Field = 8,
	[Description("<span>Limit</span>")]
	Limit = 9,
	[Description("<span>Custom Data</span>")]
	CustomData = 10,
	[Description("<span>Not Pinned</span> methods")]
	NotPinnedMethods = 11,
	[Description("<span>Pinned</span> methods")]
	PinnedMethods = 12,
	[Description("<span>Not Pinned</span> signals")]
	NotPinnedSignals = 13,
	[Description("<span>Pinned</span> signals")]
	PinnedSignals = 14,
}
