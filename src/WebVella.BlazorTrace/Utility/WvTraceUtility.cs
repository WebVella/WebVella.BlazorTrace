using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace.Utility;
public static partial class WvTraceUtility
{
	public static void ConsoleLog(string message)
	{
#if DEBUG
		Console.WriteLine($"$$$$$$ [{DateTime.Now.ToString("HH:mm:ss:ffff")}] =>{message} ");
#endif
	}
}
