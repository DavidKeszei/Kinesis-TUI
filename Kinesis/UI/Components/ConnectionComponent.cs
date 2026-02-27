using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Kinesis.UI.Components;

/// <summary>
/// Represent a component, which can hold a <typeparamref name="T"/> instance.
/// </summary>
public class ConnectionComponent: IComponent, IStaticType {
    private const string TYPE_NAME = "ConnectionComponent";

    private Entity m_child = null!;
    private ConnectionDir m_direction = ConnectionDir.DOWN;

    /// <summary>
    /// Name of the component.
    /// </summary>
    public static string Name { get => TYPE_NAME; }

    /// <summary>
    /// Index of the parent on every <see cref="Entity"/> instance.
    /// </summary>
    public static int ParentOf { get => 0; }

    /// <summary>
    /// Next <see cref="Entity"/> instance from this <see cref="Entity"/>.
    /// </summary>
    public Entity Attached { get => m_child; set => m_child = value; }

    /// <summary>
    /// Direction of the current connection in the hierarchy.
    /// </summary>
    public ConnectionDir Direction { get => m_direction; init => m_direction = value; }

    public bool TypeOf(string type) => TYPE_NAME == type;
}

public enum ConnectionDir: byte {
    UP,
    DOWN
}