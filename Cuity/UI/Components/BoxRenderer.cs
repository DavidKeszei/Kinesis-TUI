using Cuity.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a component, which can drawing a box to the screen.
/// </summary>
public class BoxRenderer: IRenderable {
    private const string NAME_OF = "BoxRenderer";

    private RGB m_backgroundColor = RGB.White;
    private bool m_isDirty = true;

    /// <summary>
    /// Name of the <see cref="BoxRenderer"/>.
    /// </summary>
    public string Name { get => NAME_OF; }

    /// <summary>
    /// Background color of the <see cref="BoxRenderer"/>.
    /// </summary>
    public RGB Background { get => m_backgroundColor; set => m_backgroundColor = value; }

    /// <summary>
    /// Indicates the <see cref="BoxRenderer"/> is "dirty".
    /// </summary>
    public bool IsDirty { get => m_isDirty; set => m_isDirty = value; }

    /// <summary>
    /// Render a box to the specific <paramref name="buffer"/> area.
    /// </summary>
    /// <param name="buffer"></param>
    public void Render(in Canvas buffer) {
        for (int x = 0; x < buffer.Scale.X; ++x) {

            for (int y = 0; y < buffer.Scale.Y; ++y) {
                ref vt_char ch = ref buffer[x, y];

                ch.Color = m_backgroundColor;
                ch.Character = ' ';
            }
        }
    }
}
