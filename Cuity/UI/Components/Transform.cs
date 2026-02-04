using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a component, which contains the transform of a <see cref="Entity"/>.
/// </summary>
public class Transform: IComponent {
    private const string NAME_OF = "Transform";

    private (int X, int Y) m_position = (0, 0);
    private (int X, int Y) m_oldPosition = (0, 0);

    private (int X, int Y) m_scale = (0, 0);
    private (int X, int Y) m_oldScale = (0, 0);

    /// <summary>
    /// Name of the component.
    /// </summary>
    public string Name { get => NAME_OF; }

    /// <summary>
    /// Position of the current <see cref="Transform"/> instance.
    /// </summary>
    public (int X, int Y) Position { 
        get => m_position;
        set {
            m_oldPosition = m_position;
            m_position = value;
        }
    }

    /// <summary>
    /// Scale of the current <see cref="Transform"/> instance.
    /// </summary>
    public (int X, int Y) Scale { 
        get => m_scale;
        set {
            m_oldScale = m_scale;
            m_scale = value;
        }
    }

    /// <summary>
    /// Old position of the <see cref="Transform"/> component.
    /// </summary>
    internal (int X, int Y) OldPosition { get => m_oldPosition; }

    /// <summary>
    /// Old scale of the <see cref="Transform"/> component.
    /// </summary>
    internal (int X, int Y) OldScale { get => m_oldScale; }
}
