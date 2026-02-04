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
    private const int MAX_STACK_BUFFER_LEN = 16_384;
    private const float MAX_FPS = 16.6f;

    private ConsoleBuffer m_frontBuffer = default!;
    private ConsoleBuffer m_backBuffer = default!;

    private StreamWriter m_output = null!;
    private (int X, int Y) m_scale = (0, 0);

    private static float m_currentFrameTime = .0f;


    /// <summary>
    /// Current scale of the screen.
    /// </summary>
    public (int X, int Y) Scale { get => m_scale; }

    /// <summary>
    /// Current frame/second value by the engine from the frame time.
    /// </summary>
    public float FPS { get => 1000f / m_currentFrameTime; }

    /// <summary>
    /// Indicates the elapsed time between two frames. Sometimes call this as "delta-time".
    /// </summary>
    public float FrameTime { get => m_currentFrameTime; }

    /// <summary>
    /// Create a new <see cref="Renderer"/> instance with specific <paramref name="x"/> and <paramref name="y"/> scale.
    /// </summary>
    /// <param name="x">Width of the renderer.</param>
    /// <param name="y">Height of the renderer.</param>
    public Renderer(int x, int y) {
        m_frontBuffer = new ConsoleBuffer(x, y);
        m_backBuffer = new ConsoleBuffer(x, y);

        m_scale = (x, y);
        m_output = new StreamWriter(stream: Console.OpenStandardOutput());

        m_output.AutoFlush = false;

        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;
    }

    /// <summary>
    /// Render one frame to the screen.
    /// </summary>
    /// <param name="entities">Renderable entities of the <see cref="Renderer"/>.</param>
    public void Render(IReadOnlyList<Entity> entities) {
        DateTime start = DateTime.Now;

        for(int i = 0; i < entities.Count; ++i) {
            Transform transform = entities[i].GetComponent<Transform>()!;

            RenderComponent? renderLogic = entities[i].GetComponent<RenderComponent>();
            ConnectionComponent? child = entities[i].GetComponent<ConnectionComponent>();

            if(renderLogic != null && renderLogic.IsDirty) {
                Clear(canvas: ConsoleBuffer.Slice(ref m_backBuffer, transform.OldPosition, transform.OldScale), child != null ? child.Previous : null!);
                Canvas canvas = ConsoleBuffer.Slice(buffer: ref m_backBuffer, transform.Position, transform.Scale);

                renderLogic.Render(buffer: in canvas, version: entities[i].Version, styles: entities[i].ResolveStyles());
                renderLogic.IsDirty = false;
            }
        }

        Diffing();
        m_currentFrameTime = (float)(DateTime.Now - start).TotalMilliseconds;

        if(m_currentFrameTime < MAX_FPS)
            Thread.Sleep(millisecondsTimeout: (int)(MAX_FPS - m_currentFrameTime));
    }

    /// <summary>
    /// Check every "pixel" for changed behavior.
    /// </summary>
    private void Diffing() {
        VT100StringBuilder builder = new VT100StringBuilder(buffer: stackalloc char[MAX_STACK_BUFFER_LEN]);
        int written = 0;

        for (int x = 0; x < m_scale.X; ++x) {
            for (int y = 0; y < m_scale.Y; ++y) {

                ref vt_char ch = ref m_frontBuffer[x, y];
                ref vt_char b_ch = ref m_backBuffer[x, y];

                if (!ch.Equals(b_ch) || m_currentFrameTime == .0f) {

                    written += builder.WritePosition(x, y)
                                      .WriteFontStyles(flags: b_ch.Styles)
                                        .WriteColor(color: b_ch.Background, isBackground: true)
                                        .WriteColor(color: b_ch.Foreground, isBackground: false)
                                      .WriteCharacter(value: b_ch.Character)
                                           .Build(destination: m_output);
                }

                if(MAX_STACK_BUFFER_LEN - written < VT100StringBuilder.MAX_COMMAND_LEN) {
                    m_output.Flush();

                    builder.Clear();
                    written = 0;
                }
            }
        }

        m_output.Flush();
        m_frontBuffer.Copy(from: in m_backBuffer);
    }

    private void Clear(in Canvas canvas, Entity entity) {
        if (canvas.Scale.X == 0 || canvas.Scale.Y == 0)
            return;

        RGB bg = GetParentBG(entity);

        for (int x = 0; x < canvas.Scale.X; ++x) {
            for (int y = 0; y < canvas.Scale.Y; ++y) {
                ref vt_char px = ref canvas[x, y];
                px.Clear();

                px.Background = bg;
            }
        }
    }

    private RGB GetParentBG(Entity entity) {
        RGB rgb = RGB.Black;
        if (entity == null) return rgb;

        foreach (IStyleComponent style in entity.ResolveStyles()) {
            if (style.Tag == StyleTag.BACKGROUND)
                return (style as Style<RGB>)!.Value;
        }

        ConnectionComponent? child = entity.GetComponent<ConnectionComponent>();

        if (child == null) return RGB.Black;
        else rgb = GetParentBG(child.Previous);

        return rgb;
    }
}
