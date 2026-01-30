using Cuity.UI;
using Cuity.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Rendering;

/// <summary>
/// Represent a helper component in the rendering.
/// </summary>
public abstract class RenderComponent: IComponent {
    protected readonly Dictionary<StyleTag, IStyleComponent> m_cache = null!;

    protected int m_entityVersion = 0;
    private volatile bool m_isDirty = true;

    /// <summary>
    /// Name of the <see cref="RenderComponent"/>.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Version of the entity, which target of the <see cref="RenderComponent"/>.
    /// </summary>
    internal int EntityVersion { get => m_entityVersion; set => m_entityVersion = value; }

    /// <summary>
    /// Indicates the component is "dirty".
    /// </summary>
    internal bool IsDirty { get => m_isDirty; set => m_isDirty = value; }

    protected RenderComponent() 
        => m_cache = new Dictionary<StyleTag, IStyleComponent>();

    /// <summary>
    /// Propagate changes on the <see cref="RenderComponent"/>.
    /// </summary>
    public void UpdateChanges() => m_isDirty = true;

    /// <summary>
    /// Render the component to the screen.
    /// </summary>
    /// <param name="buffer">Portion of the screen buffer.</param>
    /// <param name="styles">Styles of the renderer.</param>
    internal abstract void Render(in Canvas buffer, int version, params IEnumerable<IStyleComponent> styles);

    /// <summary>
    /// Cache the required <see cref="IStyleComponent"/>s.
    /// </summary>
    /// <param name="styles">Non-filtered <see cref="IStyleComponent"/>s.</param>
    protected abstract void CacheStyles(IEnumerable<IStyleComponent> styles);
}