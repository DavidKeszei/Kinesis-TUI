using Cuity.Input;
using Cuity.Navigation;
using Cuity.Processing;
using Cuity.Rendering;
using Cuity.UI;
using Cuity.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity;

/// <summary>
/// Represent the heart of the library: This connects all systems to one class.
/// </summary>
public sealed class Engine: ISystemProvider {
    private readonly Renderer m_renderer = null!;
    private readonly InputSystem m_input = null!;

    private readonly WorkerSystem m_worker = null!;
    private readonly NavigationSystem m_navigator = null!;

    private readonly List<SystemInvocationInfo> m_customSystems = null!;
    
    /// <summary>
    /// Create a new <see cref="Engine"/> instance.
    /// </summary>
    public Engine() {
        m_renderer = new Renderer(x: Console.BufferWidth, y: Console.BufferHeight);
        m_input = new InputSystem();

        m_worker = WorkerSystem.Current;
        m_navigator = new NavigationSystem(provider: this);

        m_customSystems = new List<SystemInvocationInfo>();
    }

    public T? GetSystem<T>() where T: class, ISystem {
        for (int i = 0; i < m_customSystems.Count; ++i) {
            if (m_customSystems[i].When == SystemInvocationTime.NEVER && m_customSystems[i].System is T) {

                if (m_customSystems[i].System == null!)
                    m_customSystems[i] = m_customSystems[i] with { System = m_customSystems[i].Creation(this) };

                return m_customSystems[i].System as T;
            }
        }

        return default!;
    }

    /// <summary>
    /// Add a system to the engine.
    /// </summary>
    /// <param name="action">The system itself.</param>
    /// <param name="when">Time of the invocation of the system.</param>
    public void AddSystem(SystemInvocationTime when, Func<ISystemProvider, ISystem> action)
        => m_customSystems.Add(item: new SystemInvocationInfo(action, null, when));

    /// <summary>
    /// Register a named route with a <paramref name="pageCreation"/> method.
    /// </summary>
    /// <param name="name">Name of the route.</param>
    /// <param name="pageCreation">Creation method of the <see cref="Page"/>.</param>
    /// <returns>Return <see langword="true"/>, if the route is successfully registered. Otherwise return <see langword="false"/>.</returns>
    public bool AddPage<T>(string name, Func<ISystemProvider, T> pageCreation) where T: Page
        => m_navigator.Register(name, pageCreation);

    /// <summary>
    /// Start the <see cref="Engine"/> instance with the systems.
    /// </summary>
    public void Start(CancellationToken token = default) {
        /* Run the starter systems. */
        Run(invocation: SystemInvocationTime.ON_BEGIN);

        /* Start main parts of the engine on different threads. (Input, Workers) */
        _ = Task.Run(action: () => m_worker.Run(), token);
        _ = Task.Run(action: () => m_input.Run(), token);

        while(!token.IsCancellationRequested) {
            /* Render the frame to the screen/terminal window. */
            m_renderer.Render(entities: m_navigator.Current?.Tree ?? []);
            m_worker.AddRenderMessage(new RenderMessage(m_renderer.FrameTime, (int)m_renderer.FPS));
        }

        /* Run the shutdown systems. */
        Run(invocation: SystemInvocationTime.ON_END);
    }

    private void Run(SystemInvocationTime invocation) {
        ISystem system = null!;

        foreach (SystemInvocationInfo systemInfo in m_customSystems.Where(x => x.When == invocation)) {
            system = systemInfo.System ?? systemInfo.Creation(this);

            if (system.Behavior == SystemBehavior.DYNAMIC && systemInfo.System is IDynamicSystem dynamic)
                dynamic.Run();
        }
    }
}