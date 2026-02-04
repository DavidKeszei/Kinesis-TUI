using Cuity.Input;
using Cuity.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Processing;

/// <summary>
/// Represent a bunch of workers for different tasks.
/// </summary>
internal class WorkerSystem: IDynamicSystem {
    private const string DEDICATED_THREAD_NAME = "<Thread> Worker";
    private static WorkerSystem m_instance = null!;

    private readonly ConcurrentQueue<WorkMessage> m_workMessages = null!;
    private readonly ConcurrentQueue<Delegate> m_targets = null!;

    /// <summary>
    /// Indicates behavior of the <see cref="WorkerSystem"/>.
    /// </summary>
    public SystemBehavior Behavior { get => SystemBehavior.DYNAMIC; }

    /// <summary>
    /// Current instance of the <see cref="WorkerSystem"/>.
    /// </summary>
    public static WorkerSystem Current { get => m_instance ??= new WorkerSystem(); }

    public WorkerSystem() {
        m_targets = new ConcurrentQueue<Delegate>();
        m_workMessages = new ConcurrentQueue<WorkMessage>();
    }
    
    /// <summary>
    /// Add new <see cref="InputMessage"/> to the workers.
    /// </summary>
    /// <param name="message">The message itself.</param>
    public void AddInputMessage(InputMessage message) => m_workMessages.Enqueue(new WorkMessage(message));

    /// <summary>
    /// Add new <see cref="RenderMessage"/> to the workers.
    /// </summary>
    /// <param name="message">The message itself.</param>
    public void AddRenderMessage(RenderMessage message) => m_workMessages.Enqueue(new WorkMessage(message));

    /// <summary>
    /// Add <paramref name="work"/> to the queue.
    /// </summary>
    /// <param name="work">Current work item.</param>
    public void AddInputCallback(Action<InputMessage> work) => m_targets.Enqueue(work);

    /// <summary>
    /// Add <paramref name="work"/> to the queue.
    /// </summary>
    /// <param name="work">Current work item.</param>
    public void AddRenderCallback(Action<RenderMessage> work) => m_targets.Enqueue(work);

    public void Run() {
        Thread.CurrentThread.Name = DEDICATED_THREAD_NAME;

        while(true) {
            if (!m_workMessages.TryDequeue(out WorkMessage message)) {
                Thread.Sleep(millisecondsTimeout: 5);
                continue;
            }

            foreach (Delegate work in m_targets) {
                switch (message.Source) {
                    case WorkMessageSource.INPUT:
                        if(work is Action<InputMessage> onInput)
                            onInput(message.Input);

                        break;

                    case WorkMessageSource.RENDERING:
                        if (work is Action<RenderMessage> onRender)
                            onRender(message.Render);

                        break;
                }
            }
        }
    }
}
