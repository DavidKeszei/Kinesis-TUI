using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a component, which contains the transform of a <see cref="Entity"/>.
/// </summary>
public class Transform: IComponent {
    private (float X, float Y) m_position = (0, 0);
    private (float X, float Y) m_oldPosition = (0, 0);

    private (float X, float Y) m_scale = (0, 0);
    private (float X, float Y) m_oldScale = (0, 0);

    /// <summary>
    /// Name of the component.
    /// </summary>
    public string Name { get => nameof(Transform); }

    /// <summary>
    /// Position of the current <see cref="Transform"/> instance.
    /// </summary>
    public (float X, float Y) Position { 
        get => m_position;
        set {
            m_oldPosition = m_position;
            m_position = value;
        }
    }

    /// <summary>
    /// Scale of the current <see cref="Transform"/> instance.
    /// </summary>
    public (float X, float Y) Scale { 
        get => m_scale;
        set {
            m_oldScale = m_scale;
            m_scale = value;
        }
    }

    /// <summary>
    /// Old position of the <see cref="Transform"/> component.
    /// </summary>
    internal (float X, float Y) OldPosition { get => m_oldPosition; }

    /// <summary>
    /// Old scale of the <see cref="Transform"/> component.
    /// </summary>
    internal (float X, float Y) OldScale { get => m_oldScale; }

    /// <summary>
    /// Convert the specific <paramref name="tuple"/> to integer representation.
    /// </summary>
    /// <param name="tuple">Target tuple.</param>
    /// <returns>Return the <paramref name="tuple"/> as <see cref="int"/>.</returns>
    internal static (int X, int Y) Toi32((float X, float Y) tuple)
        => ((int)tuple.X, (int)tuple.Y);
}