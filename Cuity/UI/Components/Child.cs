using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a component, which can hold a <typeparamref name="T"/> instance.
/// </summary>
/// <typeparam name="T">A <see cref="CuityEntity"/> as child.</typeparam>
public class Child<T>: IComponent where T: CuityEntity {
    private const string NAME_OF = "ChildContainer";
    private T m_child = null!;

    /// <summary>
    /// Name of the component.
    /// </summary>
    public string Name { get => NAME_OF; }

    /// <summary>
    /// Attached child object as <typeparamref name="T"/>.
    /// </summary>
    public T Attached { get => m_child; set => m_child = value; }
}