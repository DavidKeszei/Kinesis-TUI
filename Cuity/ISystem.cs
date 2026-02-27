using Kinesis.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis;

/// <summary>
/// Provides runnable entry point for a <see cref="ISystem"/>.
/// </summary>
public interface ISystem {
    
    /// <summary>
    /// Indicates behavior the <see cref="ISystem"/> instance.
    /// </summary>
    public SystemBehavior Behavior { get; }
}

public interface IDynamicSystem: ISystem {
    /// <summary>
    /// Start the current <see cref="ISystem"/> instance.
    /// </summary>
    public void Run();
}

/// <summary>
/// Indicates, when a <see cref="ISystem"/> instance runs.
/// </summary>
public enum SystemInvocationTime: byte {
    ON_BEGIN,
    ON_END,
    ON_CALL
}

public enum SystemBehavior: byte {
    DYNAMIC,
    STATIC
}

internal readonly record struct SystemInvocationInfo(Func<ISystemProvider, ISystem> Creation, ISystem? System, SystemInvocationTime When);