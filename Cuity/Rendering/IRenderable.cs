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
    /// Render the object to the screen.
    /// </summary>
    /// <param name="buffer">Back-buffer of the renderer.</param>
    public void Render(in ConsoleBuffer buffer);
}
