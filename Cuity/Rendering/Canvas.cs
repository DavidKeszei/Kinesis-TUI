using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Rendering;

/// <summary>
/// Represent a portion from the screen.
/// </summary>
public readonly ref struct Canvas {
    private readonly ref ConsoleBuffer m_buffer;
    private readonly (int X, int Y) m_scale = (0, 0);

    private readonly (int X, int Y) m_position = (0, 0);

    /// <summary>
    /// Get a <see cref="vtchar_t"/> reference from the canvas.
    /// </summary>
    /// <param name="x">X coordinate of the reference on the screen.</param>
    /// <param name="y">Y coordinate of the reference on the screen.</param>
    /// <returns>Return a <see cref="vtchar_t"/> reference.</returns>
    public ref vtchar_t this[int x, int y] { get => ref m_buffer[m_position.X + x, m_position.Y + y]; }

    /// <summary>
    /// Scale of the <see cref="Canvas"/>.
    /// </summary>
    public (int X, int Y) Scale { get => m_scale; }

    /// <summary>
    /// Create a new <see cref="Canvas"/> from a <paramref name="buffer"/> based on the <paramref name="scale"/>.
    /// </summary>
    /// <param name="buffer">Buffer of the screen.</param>
    /// <param name="scale">Scale of the <see cref="Canvas"/>.</param>
    internal Canvas(ref ConsoleBuffer buffer, (int X, int Y) scale, (int X, int Y) position) {
        m_buffer = ref buffer;
        m_scale = scale;

        m_position = position;
    }
}
