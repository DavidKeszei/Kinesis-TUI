using Cuity.Input;
using Cuity.Navigation;
using Cuity.Processing;
using Cuity.Rendering;
using Cuity.UI;
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

    private readonly NavigationSystem m_navigator = null!;
    
    /// <summary>
    /// Create a new <see cref="Engine"/> instance.
    /// </summary>
    public Engine() {
        m_renderer = new Renderer(x: Console.BufferWidth, y: Console.BufferHeight);
        m_customSystems = new List<(ISystem System, SystemInvocation Invocation)>();

        m_input = new InputSystem();
        m_worker = new WorkerSystem();

        m_navigator = new NavigationSystem();
    }

    /// <summary>
    /// Add a system to the engine.
    /// </summary>
    /// <typeparam name="T">Implementer type of the <see cref="ISystem"/>.</typeparam>
    /// <param name="system">The system itself.</param>
    /// <param name="when">Time of the invokaction of the system.</param>
    public void AddSystem<T>(T system, SystemInvocation when) where T: ISystem 
        => m_customSystems.Add(item: (system, when));

    /// <summary>
    /// Register a named route with a <paramref name="pageCreation"/> method.
    /// </summary>
    /// <param name="route">Name of the route.</param>
    /// <param name="pageCreation">Creation method of the <see cref="Page"/>.</param>
    /// <returns>Return <see langword="true"/>, if the route is successfully registered. Otherwiese return <see langword="false"/>.</returns>
    public bool RegisterRoute(string route, Func<Page> pageCreation)
        => m_navigator.Register(route, pageCreation);

    /// <summary>
    /// Start the <see cref="Engine"/> instance with the systems.
    /// </summary>
    public void Start(CancellationToken token = default) {
        /* Run the starter systems. */
        Run(invocation: SystemInvocation.ON_BEGIN);

        /* Start main parts of the engine on different threads. (Input, Workers) */
        _ = Task.Run(action: () => m_worker.Start(), token);
        _ = Task.Run(action: () => m_input.Start(), token);

        while(!token.IsCancellationRequested) {
            /* Render the frame to the screen/terminal window. */
            m_renderer.Render(entities: m_navigator.Current.UIElements);
        }

        /* Run the shutdown systems. */
        Run(invocation: SystemInvocation.ON_END);
    }

    private void Run(SystemInvocation invocation) {
        foreach (ISystem system in m_customSystems.Where(x => x.Invocation == invocation).Select(y => y.System))
            system.Start();
    }
}