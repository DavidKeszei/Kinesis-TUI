using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity;

/// <summary>
/// Represent a vector in the 2D space.
/// </summary>
public struct Vec2 {
    private float m_x = 0;
    private float m_y = 0;

    /// <summary>
    /// Value of the X axis.
    /// </summary>
    public float X { readonly get => m_x; set => m_x = value; }

    /// <summary>
    /// Value of the Y axis.
    /// </summary>
    public float Y { readonly get => m_y; set => m_y = value; }

    /// <summary>
    /// Represents a (0, 0) vector.
    /// </summary>
    public static Vec2 Zero { get => new Vec2(x: 0, y: 0); }

    public Vec2(float x, float y) {
        m_x = x;
        m_y = y;
    }
}
