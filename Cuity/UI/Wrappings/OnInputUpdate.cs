using Cuity.Input;
using Cuity.Processing;
using Cuity.Rendering;
using Cuity.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Interacts, when input was happened on the standard IO. 
/// </summary>
public class OnInputUpdate: Entity {
    private readonly EntityChangeContext m_context = null!;

    /// <summary>
    /// Callback, when the input was happened.
    /// </summary>
    public Action<InputMessage, PageEntityVisitor> On {
        set {
            InteractionComponent? interaction = base.GetComponent<InteractionComponent>();

            if (interaction == null) {
                interaction = new InteractionComponent(onInput: (message) => SetCallback(value, message), m_context!);
                base.AttachComponent<InteractionComponent>(interaction, isUnique: true);
            }
        }
    }

    /// <summary>
    /// Attached child of the <see cref="OnInputUpdate"/>.
    /// </summary>
    public Entity Child {
        set {
            ConnectionComponent connection = base.GetComponent<ConnectionComponent>()!;
            connection.Attached = value;
        }
    }

    public OnInputUpdate() {
        base.AttachComponent<ConnectionComponent>(new ConnectionComponent() { Direction = ConnectionDir.UP   });
        base.AttachComponent<ConnectionComponent>(new ConnectionComponent() { Direction = ConnectionDir.DOWN });

        this.m_context = new EntityChangeContext();
    }

    private void SetCallback(Action<InputMessage, PageEntityVisitor> func, InputMessage message) {
        m_context.Reset();
        func(message, new PageEntityVisitor(this, m_context));
        m_context.Lockdown();
    }
}