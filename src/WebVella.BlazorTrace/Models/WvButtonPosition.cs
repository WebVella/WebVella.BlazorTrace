using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public enum WvButtonPosition
{
	[Description("wv-trace-right-top")]
	RightTop = 0,
	[Description("wv-trace-right-center")]
	RightCenter = 1,
	[Description("wv-trace-right-bottom")]
	RightBottom = 2,
	[Description("wv-trace-bottom-right")]
	BottomRight = 3,
	[Description("wv-trace-bottom-center")]
	BottomCenter = 4,
	[Description("wv-trace-bottom-left")]
	BottomLeft = 5,
	[Description("wv-trace-left-top")]
	LeftTop = 6,
	[Description("wv-trace-left-center")]
	LeftCenter = 7,
	[Description("wv-trace-left-bottom")]
	LeftBottom = 8,
	[Description("wv-trace-top-left")]
	TopLeft = 9,
	[Description("wv-trace-top-center")]
	TopCenter = 10,
	[Description("wv-trace-top-right")]
	TopRight = 11
}

