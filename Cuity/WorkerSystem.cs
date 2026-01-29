using Cuity.Input;
using Cuity.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

using WorkTarget = System.Action<float, Cuity.Input.InputMessage>;

namespace Cuity;

/// <summary>
/// Represent a bunch of workers for different tasks.
/// </summary>
internal class WorkerSystem: ISystem {
    private const string DEDICATED_THREAD_NAME = "THREAD::WORKER";

    private readonly Renderer m_renderer = null!;

    private ConcurrentQueue<InputMessage> m_inputs = null!;
    private ConcurrentQueue<WorkTarget> m_targets = null!;

    public WorkerSystem(Renderer renderer) {
        m_renderer = renderer;

        m_targets = new ConcurrentQueue<WorkTarget>();
        m_inputs = new ConcurrentQueue<InputMessage>();
    }
    
    /// <summary>
    /// Add new <see cref="InputMessage"/> to the workers.
    /// </summary>
    /// <param name="message">The message itself.</param>
    public void AddInputMessage(InputMessage message) => m_inputs.Enqueue(message);

    /// <summary>
    /// Add work <paramref name="target"/> to the queueu.
    /// </summary>
    /// <param name="target">Current work item.</param>
    public void AddQueueAction(WorkTarget target) => m_targets.Enqueue(target);

    public void Start() {
        Thread.CurrentThread.Name = DEDICATED_THREAD_NAME;

        while(true) {
            if (!m_inputs.TryDequeue(out InputMessage message)) 
                continue;

            foreach (WorkTarget target in m_targets)
                target(m_renderer.FrameTime, message);
        }
    }
}
