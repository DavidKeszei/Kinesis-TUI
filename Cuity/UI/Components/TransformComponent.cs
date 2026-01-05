using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI.Components;

public class TransformComponent: IComponent {
    private const string NAME_OF = "Transform";

    private (int X, int Y) m_position = (0, 0);
    private (int X, int Y) m_scale = (0, 0);

    private bool m_isUnique = true;

    public string Name { get => NAME_OF; }

    /// <summary>
    /// Position of the current <see cref="TransformComponent"/> instance.
    /// </summary>
    public (int X, int Y) Position { get => m_position; set => m_position = value; }

    /// <summary>
    /// Scale of the current <see cref="TransformComponent"/> instance.
    /// </summary>
    public (int X, int Y) Scale { get => m_scale; set => m_scale = value; }

    public bool IsUnique { get => m_isUnique; }
}
