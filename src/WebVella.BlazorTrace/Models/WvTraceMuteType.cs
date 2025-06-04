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
	Method = 10,
	[Description("<span>Method</span> <span class='wv-mute'>in</span> <span>Module</span>")]
	MethodInModule = 11,
	[Description("<span>Method</span> <span class='wv-mute'>in</span> <span>Component</span>")]
	MethodInComponent = 12,
	[Description("<span>Method</span> <span class='wv-mute'>in</span> <span>Component Instance</span>")]
	MethodInComponentInstance = 13,
	[Description("<span>Signal</span>")]
	Signal = 20,
	[Description("<span>Signal</span> <span class='wv-mute'>in</span> <span>Module</span>")]
	SignalInModule = 21,
	[Description("<span>Signal</span> <span class='wv-mute'>in</span> <span>Component</span>")]
	SignalInComponent = 22,
	[Description("<span>Signal</span> <span class='wv-mute'>in</span> <span>Component Instance</span>")]
	SignalInComponentInstance = 23,
	[Description("<span>Field</span>")]
	Field = 30,
	[Description("<span>Field</span> <span class='wv-mute'>in</span> <span>Module</span>")]
	FieldInModule = 31,
	[Description("<span>Field</span> <span class='wv-mute'>in</span> <span>Component</span>")]
	FieldInComponent = 32,
	[Description("<span>Field</span> <span class='wv-mute'>in</span> <span>Component Instance</span>")]
	FieldInComponentInstance = 33,
	[Description("<span>Field</span> <span class='wv-mute'>in</span> <span>Assembly</span>")]
	FieldInAssembly = 34,
	[Description("<span>Assembly</span>")]
	Assembly = 35,
	[Description("<span>Limit</span>")]
	Limit = 40,
	[Description("<span>Limit</span> <span class='wv-mute'>in</span> <span>Module</span>")]
	LimitInModule = 41,
	[Description("<span>Limit</span> <span class='wv-mute'>in</span> <span>Component</span>")]
	LimitInComponent = 42,
	[Description("<span>Limit</span> <span class='wv-mute'>in</span> <span>Component Instance</span>")]
	LimitInComponentInstance = 43,
	[Description("<span>OnEnter Custom Data</span>")]
	OnEnterCustomData = 50,
	[Description("<span>OnEnter Custom Data</span> <span class='wv-mute'>in</span> <span>Module</span>")]
	OnEnterCustomDataInModule = 51,
	[Description("<span>OnEnter Custom Data</span> <span class='wv-mute'>in</span> <span>Component</span>")]
	OnEnterCustomDataInComponent = 52,
	[Description("<span>OnEnter Custom Data</span> <span class='wv-mute'>in</span> <span>Component Instance</span>")]
	OnEnterCustomDataInComponentInstance = 53,
	[Description("<span>OnExit Custom Data</span>")]
	OnExitCustomData = 54,
	[Description("<span>OnExit Custom Data</span> <span class='wv-mute'>in</span> <span>Module</span>")]
	OnExitCustomDataInModule = 55,
	[Description("<span>OnExit Custom Data</span> <span class='wv-mute'>in</span> <span>Component</span>")]
	OnExitCustomDataInComponent = 56,
	[Description("<span>OnExit Custom Data</span> <span class='wv-mute'>in</span> <span>Component Instance</span>")]
	OnExitCustomDataInComponentInstance = 57,
	[Description("<span>Not Pinned</span> methods")]
	NotPinnedMethods = 60,
	[Description("<span>Pinned</span> methods")]
	PinnedMethods = 61,
	[Description("<span>Not Pinned</span> signals")]
	NotPinnedSignals = 62,
	[Description("<span>Pinned</span> signals")]
	PinnedSignals = 63,
}
