using Kinesis.Processing;
using Kinesis.Rendering;
using Kinesis.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.UI;

/// <summary>
/// Interacts, when a frame was rendered.
/// </summary>
public class OnRenderUpdate: Entity {
    private readonly EntityContext m_context = null!;
    private readonly Island m_island = null!;

    /// <summary>
    /// Callback, when a frame was rendered.
    /// </summary>
    public Action<RenderMessage, PageEntityVisitor> On {
        set {
            InteractionComponent? interaction = base.GetComponent<InteractionComponent>();

            if (interaction == null) {
                interaction = new InteractionComponent(onRender: (message) => SetCallback(value, message), m_context, m_island);
                base.AttachComponent<InteractionComponent>(interaction, isUnique: true);
            }
        }
    }

    /// <summary>
    /// Attached child of the <see cref="OnRenderUpdate"/>.
    /// </summary>
    public Entity Child {
        set {
            ConnectionComponent connection = base.GetComponent<ConnectionComponent>(index: 1)!;
            connection?.Attached = value;

            value.GetComponent<ConnectionComponent>(index: ConnectionComponent.Parent)?.Attached = this;
        }
    }

    public OnRenderUpdate(Island island) {
        base.AttachComponent<ConnectionComponent>(component: new ConnectionComponent() { Direction = ConnectionDir.UP });
        base.AttachComponent<ConnectionComponent>(component: new ConnectionComponent() { Direction = ConnectionDir.DOWN });

        this.m_island = island;
        this.m_context = new EntityContext();
    }

    private void SetCallback(Action<RenderMessage, PageEntityVisitor> func, RenderMessage message) {
        m_context.Reset();
        func(message, new PageEntityVisitor(this, m_context));
        m_context.Lockdown();
    }
}