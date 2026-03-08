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
/// <param name="Island">Container of the changed entities.</param>
internal record WorkTarget(Delegate Action, Island Island);

/// <summary>
/// Represent a bunch of workers for different tasks.
/// </summary>
internal class WorkerSystem: IDynamicSystem {
    private const string DEDICATED_THREAD_NAME = "<Thread> Worker";
    private const string ERR_SYNC_NOT_FOUND = "The synchronization context/state wasn't found.";

    private const int MAX_RENDER_MSG = 120;

    private static WorkerSystem m_instance = null!;

    private readonly ConcurrentQueue<WorkMessage> m_workMessages = null!;
    private readonly ConcurrentQueue<WorkTarget> m_targets = null!;

    private State<WorkerSystemState> m_workSync = null!;
    private int m_renderMsgCount = 0;

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
    /// Add synchronization context/state to the <see cref="WorkerSystem"/>.
    /// </summary>
    /// <param name="sync">Synchronization state of the <see cref="KinesisEngine"/>.</param>
    /// <remarks>Remarks: If the state wasn't set, then the <see cref="WorkerSystem.Run"/> throws <see cref="InvalidOperationException"/> in the first run.</remarks>
    public void AddSyncState(State<WorkerSystemState> sync) => m_workSync ??= sync;
    
    /// <summary>
    /// Add new <see cref="InputMessage"/> to the workers.
    /// </summary>
    /// <param name="message">The message itself.</param>
    public void AddInputMessage(InputMessage message) => m_workMessages.Enqueue(new WorkMessage(message));

    /// <summary>
    /// Add new <see cref="RenderMessage"/> to the workers.
    /// </summary>
    /// <param name="message">The message itself.</param>
    public void AddRenderMessage(RenderMessage message) {
        if (m_renderMsgCount >= MAX_RENDER_MSG)
            return;

        m_workMessages.Enqueue(new WorkMessage(message));
        ++m_renderMsgCount;
    }

    /// <summary>
    /// Add <paramref name="work"/> to the queue.
    /// </summary>
    /// <param name="work">Current work item.</param>
    /// <param name="context">Holds the changed entities.</param>
    public void AddCallback<T>(Action<T> work, Island island) where T: IWorkMessage
        => m_targets.Enqueue(new WorkTarget(work, island));

    public void Run() {
        if (m_workSync == null)
            throw new InvalidOperationException(message: ERR_SYNC_NOT_FOUND);

        Thread.CurrentThread.Name = DEDICATED_THREAD_NAME;

        while(true) {
            WorkMessage message = default!;
            if (m_workSync != WorkerSystemState.OPEN_FOR_PROCESSING || !m_workMessages.TryDequeue(out message)) {
                Thread.Sleep(millisecondsTimeout: 5);
                continue;
            }

            foreach (WorkTarget workUnit in m_targets) {
                if (!workUnit.Island.IsActive)
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

            if (message.Source == WorkMessageSource.RENDERING)
                --m_renderMsgCount;

            m_workSync.Value = WorkerSystemState.WAIT_FOR_RENDERER;
        }
    }
}

/// <summary>
/// Simple state representation between the <see cref="Renderer"/> and <see cref="WorkerSystem"/>.
/// </summary>
public enum WorkerSystemState: byte {
    /// <summary>
    /// Indicates the <see cref="WorkerSystem"/> can process one message from the queue.
    /// </summary>
    OPEN_FOR_PROCESSING,
    /// <summary>
    /// Indicates for the <see cref="WorkerSystem"/> wait to the <see cref="Renderer"/>.
    /// </summary>
    WAIT_FOR_RENDERER
}