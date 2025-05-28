using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvSnapshotListItem
{
	public Guid Id { get; set; }
	public string Name { get; set; } = default!;
	public DateTimeOffset CreatedOn { get; set; } = default!;
	public Action OnRemove { get; set; } = default!;
	public Action OnRename { get; set; } = default!;
}

