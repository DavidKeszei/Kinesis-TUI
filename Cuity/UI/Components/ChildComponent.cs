using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a component, which can hold a <typeparamref name="T"/> instance.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ChildComponent<T>: IComponent where T: CuityObject {
    private const string NAME_OF = "ChildContainer";

    private T m_child = null!;
    private bool m_isUnique = false;

    public string Name { get => NAME_OF; }

    public bool IsUnique { get => m_isUnique; }

    /// <summary>
    /// Attached child object as <typeparamref name="T"/>.
    /// </summary>
    public T Child { get => m_child; set => m_child = value; }
}
