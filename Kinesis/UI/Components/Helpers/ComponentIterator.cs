using Kinesis.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.UI;

/// <summary>
/// Struct iterator for <typeparamref name="T"/> instances.
/// </summary>
public ref struct ComponentIterator<T> where T: Component {
    private readonly List<T> m_components = null!;
    private int m_current = -1;

    public readonly T Current { get => m_components[m_current]; }

    public ComponentIterator(List<T> components) => m_components = components;

    public bool MoveNext() {
        if (++m_current < m_components.Count)
            return true;

        return false;
    }
}

public readonly ref struct StyleEnumerator {
    private readonly ComponentIterator<Style> m_styles = default!;

    public StyleEnumerator(Entity entity) {
        List<Style> styles = new List<Style>(capacity: 8);

        foreach (Component component in entity) {

            if (component.TypeOf(Style.Name))
                styles.Add((Style)component);
        }

        m_styles = new ComponentIterator<Style>(styles);
    }

    public ComponentIterator<Style> GetEnumerator() => m_styles;
}