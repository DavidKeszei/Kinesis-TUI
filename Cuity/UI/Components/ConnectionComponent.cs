using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a component, which can hold a <typeparamref name="T"/> instance.
/// </summary>
public class ConnectionComponent: IComponent, IStaticType {
    private const string TYPE_NAME = "ConnectionComponent";

    private Entity m_child = null!;
    private Entity m_parent = null!;

    /// <summary>
    /// Name of the component.
    /// </summary>
    public static string Name { get => TYPE_NAME; }

    /// <summary>
    /// Next <see cref="Entity"/> instance from this <see cref="Entity"/>.
    /// </summary>
    public Entity Next { get => m_child; set => m_child = value; }

    /// <summary>
    /// Previous <see cref="Entity"/> in the UI tree.
    /// </summary>
    internal Entity Previous { get => m_parent; set => m_parent = value; }

    public bool IsType(string type) => TYPE_NAME == type;
}