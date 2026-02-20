using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Struct iterator for <see cref="IComponent"/> instances.
/// </summary>
public struct ComponentIterator {
    private List<IComponent> m_components = null!;
    private int m_current = -1;

    public IComponent Current { get => m_components[m_current]; }

    public ComponentIterator(List<IComponent> components)
        => m_components = components;

    public bool MoveNext() {
        if (++m_current < m_components.Count)
            return true;

        return false;
    }
}
