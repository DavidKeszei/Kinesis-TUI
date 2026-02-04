using Cuity.Rendering;
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

    /// <summary>
    /// Created <see cref="Entity"/> instance-tree as "list".
    /// </summary>
    internal IReadOnlyList<Entity> Tree { get => m_renderSet; }

    public Page()
        => m_renderSet = new List<Entity>(capacity: 32);

    /// <summary>
    /// Build the page as <see cref="Entity"/>.
    /// </summary>
    /// <returns>Return <see cref="Entity"/> instance from the current <see cref="Page"/>.</returns>
    protected abstract Entity? Build();

    /// <summary>
    /// Move through the tree and create list from it.
    /// </summary>
    /// <param name="entity">Current target entity of the call.</param>
    internal void CreateRenderSet(Entity? entity = null!) {
        entity ??= Build();

        m_renderSet.Add(entity!);
        int childrenCount = entity!.Count(static x => x.Name == "ConnectionComponent");

        for (int i = 0; i < childrenCount; ++i) {
            ConnectionComponent child = entity!.GetComponent<ConnectionComponent>(i)!;
            CreateRenderSet(entity: child.Next);
        }
    }
}