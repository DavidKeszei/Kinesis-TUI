using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Processing;

public readonly struct RenderMessage {
    private readonly float m_delta = .0f;
    private readonly int m_fps = 0;

    public float DeltaTime { get => m_delta; }

    public int FPS { get => m_fps; }

    internal RenderMessage(float deltaTime, int fps) {
        m_delta = deltaTime;
        m_fps = fps;
    }
}
