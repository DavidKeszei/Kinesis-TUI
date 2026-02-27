using Kinesis.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.UI;

/// <summary>
/// Represent a local visitor on the entity-tree to the bottom.
/// </summary>
public readonly ref struct PageEntityVisitor {
    private readonly Entity? m_pivot = null!;
    private readonly EntityContext m_ctx = null!;

    internal PageEntityVisitor(Entity? pivot, EntityContext changeContext) {
        m_ctx = changeContext;
        m_pivot = pivot;
    }

    /// <summary>
    /// Visit a specific <typeparamref name="T"/> entity in the tree.
    /// </summary>
    /// <typeparam name="T">Type of the entity.</typeparam>
    /// <param name="name">Unique name of the entity.</param>
    /// <param name="track">Indicates the visited entity must be updated after the visit. If this <see langword="false"/>, then any change is not propagated.</param>
    /// <returns>Return a entity as <typeparamref name="T"/>. If not in the tree, then return <see langword="null"/>.</returns>
    public T? Visit<T>(string name, bool track = true) where T: Entity {
        if (string.IsNullOrEmpty(name) || m_pivot == null) return null!;
        else if (IsSequenceEqual(m_pivot.Name, name) && m_pivot is T ret) return ret;

        Entity? result = RecursiveVisit(current: m_pivot, name);
        if (result != null && track) m_ctx.Add(result);

        return (T?)result;
    }

    private Entity? RecursiveVisit(Entity? current, string name) {
        if (current == null) return null!;

        int childrenCount = CountOfChild(current);
        for (int i = 1; i < childrenCount; ++i) {
            Entity? child = current.GetComponent<ConnectionComponent>(index: i)?.Attached;

            if (child != null) {
                if (!string.IsNullOrEmpty(child.Name) && IsSequenceEqual(child.Name, name)) return child;
                else {
                    child = RecursiveVisit(child, name);

                    if (child != null)
                        return child;
                }
            }
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

    private int CountOfChild(Entity entity) {
        if(entity == null) return 0;
        int count = 0;

        foreach(IComponent component in entity) {
            if (component.TypeOf(type: ConnectionComponent.Name))
                ++count;
        }

        return count;
    }
}
