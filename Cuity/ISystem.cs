using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity;

/// <summary>
/// Provides start point for a system.
/// </summary>
public interface ISystem {
    
    /// <summary>
    /// Start the current <see cref="ISystem"/> instance.
    /// </summary>
    public void Start();
}

/// <summary>
/// Indicates, when a <see cref="ISystem"/> instance runs.
/// </summary>
public enum SystemInvocation: byte {
    ON_BEGIN,
    ON_END
}
