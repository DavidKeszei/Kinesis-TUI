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
    private readonly InputSystem m_input = null!;

    private readonly WorkerSystem m_worker = null!;
    private readonly List<(ISystem System, SystemInvocation Invocation)> m_customSystems = null!;
    
    /// <summary>
    /// Create a new <see cref="Engine"/> instance.
    /// </summary>
    public Engine() {
        m_renderer = new Renderer(x: Console.BufferWidth, y: Console.BufferHeight);
        m_customSystems = new List<(ISystem System, SystemInvocation Invocation)>();

        m_worker = new WorkerSystem(renderer: m_renderer);
        m_input = new InputSystem(workers: m_worker);
    }

    /// <summary>
    /// Add a system to the engine.
    /// </summary>
    /// <typeparam name="T">Implementer type of the <see cref="ISystem"/>.</typeparam>
    /// <param name="system">The system itself.</param>
    /// <param name="when">Time of the invokaction of the system.</param>
    public void AddSystem<T>(T system, SystemInvocation when) where T: ISystem 
        => m_customSystems.Add(item: (system, when));

#if DEBUG

    public void AddAction(Action<float, InputMessage> action)
        => m_worker.AddQueueAction(action);

#endif

    /// <summary>
    /// Start the <see cref="Engine"/> instance with the systems.
    /// </summary>
    public void Start(CancellationToken token = default) {
        /* Run the starter systems. */
        Run(invocation: SystemInvocation.ON_BEGIN);

        /* Start main parts of the engine on different threads. (Input, Workers)*/
        _ = Task.Run(action: () => m_input.Start(), token);
        _ = Task.Run(action: () => m_worker.Start(), token);

        while(!token.IsCancellationRequested) {
            /* Render the frame to the screen/terminal window. */
            m_renderer.Render(entities: []);
        }

        /* Run the shutdown systems. */
        Run(invocation: SystemInvocation.ON_END);
    }

    private void Run(SystemInvocation invocation) {
        foreach (ISystem system in m_customSystems.Where(x => x.Invocation == invocation).Select(y => y.System))
            system.Start();
    }
}