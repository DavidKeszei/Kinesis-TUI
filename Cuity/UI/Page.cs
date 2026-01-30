using Cuity.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Represent an UI page on the screen.
/// </summary>
public abstract class Page {
    private readonly List<Entity> m_renderSet = null!;
    private bool m_pageIsDirty = true;

    /// <summary>
    /// Created <see cref="Entity"/> instance-tree as "list".
    /// </summary>
    internal IEnumerable<Entity> UIElements { get => m_renderSet; }

    public Page()
        => m_renderSet = new List<Entity>(capacity: 32);

    /// <summary>
    /// Build the page as <see cref="Entity"/>.
    /// </summary>
    /// <returns>Return <see cref="Entity"/> instance from the current <see cref="Page"/>.</returns>
    protected abstract Entity? Build();

    /// <summary>
    /// Indicates the whole page is dirty. Most of the time, when new <see cref="Entity"/> added to the tree.
    /// </summary>
    protected void IsDirty() => m_pageIsDirty = true;

    /// <summary>
    /// Move through the tree and create list from it.
    /// </summary>
    /// <param name="entity">Current target entity of the call.</param>
    internal void CreateRenderSet(Entity? entity = null!) {
        if (m_pageIsDirty) {
            entity = Build();
            m_renderSet.Clear();

            m_pageIsDirty = false;
        }

        if (entity == null) return;

        m_renderSet.Add(entity);
        int childrenCount = entity.Count(static x => x.Name == "Child");

        for (int i = 0; i < childrenCount; ++i) {
            Child child = entity.GetComponent<Child>(i)!;
            CreateRenderSet(entity: child.Attached);
        }
    }
}