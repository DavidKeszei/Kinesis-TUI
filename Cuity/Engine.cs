using Cuity.Input;
using Cuity.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity;

/// <summary>
/// Represent the heart of the library: This connects all systems to one class.
/// </summary>
public sealed class Engine {
    private readonly Renderer m_renderer = null!;
    private readonly InputHandler m_input = null!;
    
    /// <summary>
    /// Create a new <see cref="Engine"/> instance.
    /// </summary>
    public Engine() {
        m_input = new InputHandler();
        m_renderer = new Renderer(x: Console.BufferWidth, y: Console.BufferHeight);
    }

    /// <summary>
    /// Start the <see cref="Engine"/> instance with the systems.
    /// </summary>
    public void Start(CancellationToken token = default) {
        /* Start main parts of the engine on different threads. (Input, Workers)*/
        Task.Run(action: () => m_input.Start(), token);

        while(!token.IsCancellationRequested) {

            /* [Last Step] Render the frame to the screen/terminal window. */
            m_renderer.Render(entities: []);
        }
    }
}
