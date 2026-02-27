using Kinesis.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.UI;

/// <summary>
/// Represent a holder of changes on <see cref="Entity"/> instances.
/// </summary>
public class EntityContext {
    private readonly HashSet<Entity> m_diff = null!;
    private readonly Entity[] m_changes = null!;

    private int m_count = 0;

    public EntityContext(int capacity = 32) {
        m_changes = new Entity[capacity];
        m_diff = new HashSet<Entity>(capacity);
    }

    /// <summary>
    /// Reset the context to default value.
    /// </summary>
    public void Reset() {
        m_diff.Clear();
        m_count = 0;
    }

    /// <summary>
    /// Add an <see cref="Entity"/> to the context.
    /// </summary>
    /// <param name="entity">Target of the change.</param>
    public void Add(Entity? entity) {
        if (entity == null || !m_diff.Add(entity))
            return;

        m_changes[m_count++] = entity;
    }

    /// <summary>
    /// Check if any <see cref="Entity"/> is not processed by the <see cref="Renderer"/>.
    /// </summary>
    /// <returns>Return <see langword="true"/>, if any <see cref="Entity"/> not processed by the <see cref="Renderer"/>. Otherwise return <see langword="false"/>.</returns>
    public bool IsLocked() {
        for (int i = 0; i < m_count; ++i) {
            if (m_changes[i].State == EntityState.LOCKED)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Lock down <see cref="Entity"/> instances for the rendering.
    /// </summary>
    public void Lockdown() {
        for (int i = m_count - 1; i > -1; --i) {
            m_changes[i].State = EntityState.LOCKED;
            m_changes[i].GetComponent<RenderComponent>()?.IsDirty = true;
        }
    }
}
