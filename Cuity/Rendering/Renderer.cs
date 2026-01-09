using Cuity.UI;
using Cuity.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Rendering;

/// <summary>
/// Represent the rendering engine of the library.
/// </summary>
public class Renderer {
    private const float MAX_FPS = 16.6f;

    private readonly List<CuityEntity> m_entites = null!;
    private ConsoleBuffer m_frontBuffer = default!;

    private ConsoleBuffer m_backBuffer = default!;
    private (int X, int Y) m_scale = (0, 0);

    private float m_currentFrameTime = .0f;

    /// <summary>
    /// Current scale of the screen.
    /// </summary>
    public (int X, int Y) Scale { get => m_scale; }

    /// <summary>
    /// Current frame/second value by the engine from the frame time.
    /// </summary>
    public float CurrentFPS { get => 1000 / m_currentFrameTime; }

    /// <summary>
    /// Create a new <see cref="Renderer"/> instance with specific <paramref name="x"/> and <paramref name="y"/> scale.
    /// </summary>
    /// <param name="x">Width of the renderer.</param>
    /// <param name="y">Height of the renderer.</param>
    public Renderer(int x, int y) {
        m_frontBuffer = new ConsoleBuffer(x, y);
        m_backBuffer = new ConsoleBuffer(x, y);

        m_scale = (x, y);
        m_entites = new List<CuityEntity>();
    }

    /// <summary>
    /// Start the rendering process of the application.
    /// </summary>
    public void Render() {
        Console.CursorVisible = false;

        while (true) {
            DateTime start = DateTime.Now;

            foreach (CuityEntity entity in m_entites) {
                Transform transform = entity.GetComponent<Transform>()!;
                IRenderable renderLogic = entity.GetComponent<IRenderable>()!;

                if (renderLogic.IsDirty) {
                    Canvas canvas = ConsoleBuffer.Slice(buffer: ref m_backBuffer, transform.Position, transform.Scale);
                    renderLogic.Render(buffer: in canvas);

                    renderLogic.IsDirty = false;
                }
            }

            Diffing();
            float frameTime = (float)(DateTime.Now - start).TotalMilliseconds;

            if (frameTime < MAX_FPS)
                Thread.Sleep(millisecondsTimeout: (int)(MAX_FPS - frameTime));

            Console.Out.Write("\x1b[0m");
        }
    }

#if DEBUG

    public void AddEntity(CuityEntity entity) => m_entites.Add(entity);

#endif

    private void Diffing() {
        for (int x = 0; x < m_scale.X; ++x) {
            for (int y = 0; y < m_scale.Y; ++y) {
                ref vt_char ch = ref m_frontBuffer[x, y];

                if (!ch.Equals(m_backBuffer[x, y])) {
                    Console.SetCursorPosition(x, y);
                    Console.Out.Write(value: m_backBuffer[x, y].ToVT100Str());
                }
            }
        }

        m_frontBuffer.Copy(in m_backBuffer);
    }
}
