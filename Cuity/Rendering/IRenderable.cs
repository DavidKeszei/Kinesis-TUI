using Cuity.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Rendering;

/// <summary>
/// Provides rendering function to draw object to the screen.
/// </summary>
public interface IRenderable: IComponent {

    /// <summary>
    /// Indicates the <see cref="IRenderable"/> component is "dirty".
    /// </summary>
    public bool IsDirty { get; internal set; }

    /// <summary>
    /// Render the object to the screen.
    /// </summary>
    /// <param name="buffer">Back-buffer of the renderer.</param>
    public void Render(in Canvas buffer);
}
