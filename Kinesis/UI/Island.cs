using Kinesis.Rendering;
using Kinesis.UI.Components;
using Kinesis.Navigation;

using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.UI;

/// <summary>
/// Represent a segment on the screen.
/// </summary>
public abstract class Island {
    private readonly List<Entity> m_renderSet = null!;
    private int m_currentEntityIndex = 0;

    private bool m_isActive = false;

    /// <summary>
    /// Created <see cref="Entity"/> instance-tree as "list".
    /// </summary>
    internal IReadOnlyList<Entity> Tree { get => m_renderSet; }

    /// <summary>
    /// Indicates the <see cref="Island"/> is active by the <see cref="Renderer"/> and the <see cref="INavigator"/>.
    /// </summary>
    internal bool IsActive { get => m_isActive; set => m_isActive = value; }

    /// <summary>
    /// Implicit conversion between <see cref="Island"/> and <see cref="Entity"/>.
    /// </summary>
    /// <param name="island">Current <see cref="Island"/> instance.</param>
    public static explicit operator Entity?(Island island) => island.Build();

    public Island()
        => m_renderSet = new List<Entity>(capacity: 32);

    /// <summary>
    /// Build the page as <see cref="Entity"/>.
    /// </summary>
    /// <returns>Return <see cref="Entity"/> instance from the current <see cref="Island"/>.</returns>
    protected abstract Entity? Build();

    /// <summary>
    /// Move through the tree and create list from it.
    /// </summary>
    /// <param name="entity">Current target entity of the call.</param>
    internal void CreateRenderSet(Entity? entity = null!, bool isTop = true) {
        if(isTop) entity ??= Build();
        if(entity == null) return;

        bool isRenderable = entity.GetComponent<RenderComponent>() != null;
        if (isRenderable) m_renderSet.Add(entity!);

        int childrenCount = CountOfChild(entity);

        for (int i = 1; i < childrenCount; ++i) {
            ConnectionComponent child = entity!.GetComponent<ConnectionComponent>(i)!;

            if(child.Attached != null) 
                CreateRenderSet(entity: child.Attached, isTop: false);
        }
    }

    private int CountOfChild(Entity? entity) {
        if (entity == null)
            return 0;

        int count = 0;
        foreach (Component component in entity) {
            if (component.TypeOf(ConnectionComponent.Name))
                ++count;
        }

        return count;
    }
}