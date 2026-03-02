using Kinesis.Input;
using Kinesis.Rendering;
using Kinesis.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.Processing;

/// <summary>
/// Represent smallest unit of work.
/// </summary>
/// <param name="Action">Callback of the work.</param>
/// <param name="Context">The context, which holds all changes on the entities.</param>
/// <param name="Island">Container of the changed entities.</param>
internal record WorkTarget(Delegate Action, EntityContext Context, Island Island);

/// <summary>
/// Represent a bunch of workers for different tasks.
/// </summary>
internal class WorkerSystem: IDynamicSystem {
    private const string DEDICATED_THREAD_NAME = "<Thread> Worker";
    private static WorkerSystem m_instance = null!;

    private readonly ConcurrentQueue<WorkMessage> m_workMessages = null!;
    private readonly ConcurrentQueue<WorkTarget> m_targets = null!;

    /// <summary>
    /// Indicates behavior of the <see cref="WorkerSystem"/>.
    /// </summary>
    public SystemBehavior Behavior { get => SystemBehavior.DYNAMIC; }

    /// <summary>
    /// Current instance of the <see cref="WorkerSystem"/>.
    /// </summary>
    public static WorkerSystem Current { get => m_instance ??= new WorkerSystem(); }

    public WorkerSystem() {
        m_targets = new ConcurrentQueue<WorkTarget>();
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
    /// <param name="context">Holds the changed entities.</param>
    public void AddCallback<T>(Action<T> work, EntityContext context, Island island) where T: IWorkMessage
        => m_targets.Enqueue(new WorkTarget(work, context, island));

    public void Run() {
        Thread.CurrentThread.Name = DEDICATED_THREAD_NAME;

        while(true) {
            if (!m_workMessages.TryDequeue(out WorkMessage message)) {
                Thread.Sleep(millisecondsTimeout: 5);
                continue;
            }

            foreach (WorkTarget workUnit in m_targets) {
                if (!workUnit.Island.IsActive || workUnit.Context.IsLocked())
                    continue;

                switch (message.Source) {
                    case WorkMessageSource.INPUT:
                        if(workUnit.Action is Action<InputMessage> onInput)
                            onInput(message.Input);

                        break;

                    case WorkMessageSource.RENDERING:
                        if (workUnit.Action is Action<RenderMessage> onRender)
                            onRender(message.Render);

                        break;
                }
            }
        }
    }
}