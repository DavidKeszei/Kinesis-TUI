using Cuity.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Represent a local visitor on the entity-tree to the bottom.
/// </summary>
public readonly ref struct PageEntityVisitor {
    private readonly Entity? m_pivot = null!;

    internal PageEntityVisitor(Entity? pivot) {
        m_pivot = pivot;
    }

    public T? Visit<T>(string name) where T: Entity {
        if (string.IsNullOrEmpty(name) || m_pivot == null) return null!;
        else if (IsSequenceEqual(m_pivot.Name, name) && m_pivot is T) return m_pivot as T;

        Entity? result = RecursiveVisit(current: m_pivot, name);
        return result is T ? result as T : null!;
    }

    private Entity? RecursiveVisit(Entity? current, string name) {
        if (current == null) return null!;

        int childrenCount = current.Count(static x => x is ConnectionComponent);
        for (int i = 0; i < childrenCount; ++i) {
            Entity? child = current.GetComponent<ConnectionComponent>(i)?.Next;

            if (child != null && !string.IsNullOrEmpty(child.Name) && IsSequenceEqual(child.Name, name))
                return child;
        }

        return null!;
    }

    private bool IsSequenceEqual(ReadOnlySpan<char> left, ReadOnlySpan<char> right) {
        if (left.Length != right.Length) return false;

        for (int i = 0; i < left.Length; ++i)
            if (left[i] != right[i])
                return false;

        return true;
    }
}
