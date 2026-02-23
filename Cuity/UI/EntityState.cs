using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Provides simple enumeration values for <see cref="Entity"/> state tracking.
/// </summary>
internal enum EntityState : byte {
    LOCKED,
    FREE,
}