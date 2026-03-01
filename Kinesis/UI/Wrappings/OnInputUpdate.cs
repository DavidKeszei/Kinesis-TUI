using Kinesis.Input;
using Kinesis.Processing;
using Kinesis.Rendering;
using Kinesis.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.UI;

/// <summary>
/// Interacts, when input was happened on the standard IO. 
/// </summary>
public class OnInputUpdate: Entity {
    private readonly EntityContext m_context = null!;
    private readonly Island m_island = null!;

    /// <summary>
    /// Callback, when the input was happened.
    /// </summary>
    public Action<InputMessage, PageEntityVisitor> On {
        set {
            InteractionComponent? interaction = base.GetComponent<InteractionComponent>();

            if (interaction == null) {
                interaction = new InteractionComponent(onInput: (message) => SetCallback(value, message), m_context!, m_island);
                base.AttachComponent<InteractionComponent>(interaction, isUnique: true);
            }
        }
    }

    /// <summary>
    /// Attached child of the <see cref="OnInputUpdate"/>.
    /// </summary>
    public Entity Child {
        set {
            ConnectionComponent connection = base.GetComponent<ConnectionComponent>(index: 1)!;
            connection.Attached = value;

            value.GetComponent<ConnectionComponent>(index: ConnectionComponent.Parent)!.Attached = this;
        }
    }

    public OnInputUpdate(Island island) {
        base.AttachComponent<ConnectionComponent>(new ConnectionComponent() { Direction = ConnectionDir.UP   });
        base.AttachComponent<ConnectionComponent>(new ConnectionComponent() { Direction = ConnectionDir.DOWN });

        this.m_island = island;
        this.m_context = new EntityContext();
    }

    private void SetCallback(Action<InputMessage, PageEntityVisitor> func, InputMessage message) {
        m_context.Reset();
        func(message, new PageEntityVisitor(this, m_context));
        m_context.Lockdown();
    }
}