using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.UI.Components;

/// <summary>
/// Represent a component, which contains the transform of a <see cref="Entity"/>.
/// </summary>
public class Transform: IComponent, IStaticType {
    private const string TYPE_NAME = "Transform";

    private Vec2 m_position = Vec2.Zero;
    private Vec2 m_oldPosition = Vec2.Zero;

    private Vec2 m_scale = Vec2.Zero;
    private Vec2 m_oldScale = Vec2.Zero;

    /// <summary>
    /// Name of the component.
    /// </summary>
    public static string Name { get => TYPE_NAME; }

    /// <summary>
    /// Position of the current <see cref="Transform"/> instance.
    /// </summary>
    public Vec2 Position { 
        get => m_position;
        set {
            m_oldPosition = m_position;
            m_position = value;
        }
    }

    /// <summary>
    /// Scale of the current <see cref="Transform"/> instance.
    /// </summary>
    public Vec2 Scale { 
        get => m_scale;
        set {
            m_oldScale = m_scale;
            m_scale = value;
        }
    }

    public bool TypeOf(string type) => TYPE_NAME == type;

    /// <summary>
    /// Old position of the <see cref="Transform"/> component.
    /// </summary>
    internal Vec2 OldPosition { get => m_oldPosition; }

    /// <summary>
    /// Old scale of the <see cref="Transform"/> component.
    /// </summary>
    internal Vec2 OldScale { get => m_oldScale; }
}