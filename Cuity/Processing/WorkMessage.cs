using Cuity.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Cuity.Processing;

[StructLayout(layoutKind: LayoutKind.Explicit)]
internal readonly struct WorkMessage {
    [FieldOffset(0)] private readonly InputMessage m_input = default!;
    [FieldOffset(0)] private readonly RenderMessage m_renderMessage = default!;

    [FieldOffset(8)] private readonly WorkMessageSource m_source = WorkMessageSource.INPUT;

    public WorkMessageSource Source { get => m_source; }

    public InputMessage Input { get => m_input; }

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
