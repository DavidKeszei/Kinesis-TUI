using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Processing;

public readonly record struct RenderMessage {
    private readonly float m_delta = .0f;
    private readonly int m_fps = 0;

    private readonly (int X, int Y) m_scale = (0, 0);

    public float DeltaTime { get => m_delta; }

    public int FPS { get => m_fps; }

    public (int X, int Y) Scale { get => m_scale; }

    internal RenderMessage(float deltaTime, int fps, (int X, int Y) scale) {
        m_delta = deltaTime;
        m_fps = fps;

        m_scale = scale;
    }
}
