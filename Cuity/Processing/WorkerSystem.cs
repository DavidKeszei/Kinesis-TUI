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
internal class WorkerSystem: ISystem {
    private const string DEDICATED_THREAD_NAME = "THREAD::WORKER";
    private static WorkerSystem m_instance = null!;

    private ConcurrentQueue<InputMessage> m_inputs = null!;
    private ConcurrentQueue<Action<InputMessage>> m_targets = null!;

    /// <summary>
    /// Current instance of the <see cref="WorkerSystem"/>.
    /// </summary>
    public static WorkerSystem Instance { get => m_instance; }

    public WorkerSystem() {
        WorkerSystem.m_instance ??= this;

        m_targets = new ConcurrentQueue<Action<InputMessage>>();
        m_inputs = new ConcurrentQueue<InputMessage>();
    }
    
    /// <summary>
    /// Add new <see cref="InputMessage"/> to the workers.
    /// </summary>
    /// <param name="message">The message itself.</param>
    public void AddInputMessage(InputMessage message) => m_inputs.Enqueue(message);

    /// <summary>
    /// Add <paramref name="work"/> to the queueu.
    /// </summary>
    /// <param name="work">Current work item.</param>
    public void AddQueueAction(Action<InputMessage> work) => m_targets.Enqueue(work);

    public void Start() {
        Thread.CurrentThread.Name = DEDICATED_THREAD_NAME;

        while(true) {
            if (!m_inputs.TryDequeue(out InputMessage message)) {
                Thread.Sleep(millisecondsTimeout: 5);
                continue;
            }

            foreach (Action<InputMessage> work in m_targets)
                work(message);
        }
    }
}
