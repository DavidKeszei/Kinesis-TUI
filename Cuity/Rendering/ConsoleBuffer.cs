using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Rendering;

/// <summary>
/// Represent a <see cref="ConsoleBuffer"/> on the screen.
/// </summary>
internal readonly struct ConsoleBuffer {
    private readonly vt_char[,] m_buffer = null!;
    private readonly (int X, int Y) m_dimension = (-1, -1);

    /// <summary>
    /// Dimension of the buffer.
    /// </summary>
    public (int X, int Y) Dimension { get => m_dimension; }

    /// <summary>
    /// Get a <see cref="vt_char"/> reference from the buffer.
    /// </summary>
    /// <param name="x">X position of the reference.</param>
    /// <param name="y">Y position of the reference.</param>
    /// <returns>Return a <see cref="vt_char"/> reference.</returns>
    public ref vt_char this[int x, int y] => ref m_buffer[x, y];

    /// <summary>
    /// Create new <see cref="ConsoleBuffer"/> with specific dimension.
    /// </summary>
    /// <param name="x">Width of the buffer.</param>
    /// <param name="y">Height of the buffer.</param>
    public ConsoleBuffer(int x, int y) {
        m_dimension = (x, y);
        m_buffer = new vt_char[x, y];
    }

    /// <summary>
    /// Copy <paramref name="buffer"/> to this buffer.
    /// </summary>
    /// <param name="buffer">Source buffer.</param>
    public void Copy(in ConsoleBuffer buffer) {
        for (int x = 0; x < m_dimension.X; ++x) {
            for (int y = 0; y < m_dimension.Y; ++y) {
                this[x, y] = buffer[x, y];
            }
        }
    }

    /// <summary>
    /// Create a slice from the current <see cref="ConsoleBuffer"/>.
    /// </summary>
    /// <param name="buffer">Source of the buffer.</param>
    /// <param name="from">Absolute index of the buffer.</param>
    /// <param name="scale">Scale of the buffer.</param>
    /// <returns>Return a <see cref="Canvas"/> instance.</returns>
    public static Canvas Slice(ref ConsoleBuffer buffer, (int X, int Y) from, (int X, int Y) scale) {
        if (from.X < 0) scale.X += from.X;
        else if (from.X + scale.X >= buffer.Dimension.X) scale.X -= (from.X + scale.X) % buffer.Dimension.X;

        if (from.Y < 0) scale.Y += from.Y;
        else if (from.Y + scale.Y >= buffer.Dimension.Y) scale.Y -= (from.Y + scale.Y) % buffer.Dimension.Y;

        from.X = int.Clamp(from.X, 0, buffer.Dimension.X - 1);
        from.Y = int.Clamp(from.Y, 0, buffer.Dimension.Y - 1);

        return new Canvas(ref buffer, scale, from);
    }
}
