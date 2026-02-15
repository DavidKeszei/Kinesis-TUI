using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Processing;

public readonly record struct RenderMessage {
    private readonly float m_delta = .0f;
    private readonly int m_fps = 0;

    private readonly Vec2 m_scale = Vec2.Zero;

    public float DeltaTime { get => m_delta; }

    public int FPS { get => m_fps; }

    public Vec2 Scale { get => m_scale; }

    internal RenderMessage(float deltaTime, int fps, Vec2 scale) {
        m_delta = deltaTime;
        m_fps = fps;

        m_scale = scale;
    }
}
