using Cuity.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Cuity.Processing;

/// <summary>
/// Represent a message in the <see cref="WorkerSystem"/>.
/// </summary>
[StructLayout(layoutKind: LayoutKind.Explicit)]
internal readonly struct WorkMessage {
    [FieldOffset(0)] private readonly InputMessage m_input = default!;
    [FieldOffset(0)] private readonly RenderMessage m_renderMessage = default!;

    [FieldOffset(16)] private readonly WorkMessageSource m_source = WorkMessageSource.INPUT;

    /// <summary>
    /// Source of the message.
    /// </summary>
    public WorkMessageSource Source { get => m_source; }

    /// <summary>
    /// Input massage from the <see cref="InputSystem"/>.
    /// </summary>
    public InputMessage Input { get => m_input; }

    /// <summary>
    /// Render message from the <see cref="Rendering.Renderer"/>.
    /// </summary>
    public RenderMessage Render { get => m_renderMessage; }

    public WorkMessage(InputMessage input) {
        m_input = input;
        m_source = WorkMessageSource.INPUT;
    }

    public WorkMessage(RenderMessage render) {
        m_renderMessage = render;
        m_source = WorkMessageSource.RENDERING;
    }
}

internal enum WorkMessageSource: byte {
    INPUT,
    RENDERING
}
