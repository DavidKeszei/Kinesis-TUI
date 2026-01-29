using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a component, which can hold a <typeparamref name="T"/> instance.
/// </summary>
/// <typeparam name="T">A <see cref="Entity"/> as child.</typeparam>
public class Child: IComponent {
    private const string NAME_OF = "ChildContainer";
    private Entity m_child = null!;

    /// <summary>
    /// Name of the component.
    /// </summary>
    public string Name { get => NAME_OF; }

    /// <summary>
    /// Attached child object as <typeparamref name="T"/>.
    /// </summary>
    public Entity Attached { get => m_child; set => m_child = value; }
}