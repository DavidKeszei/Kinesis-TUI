using Cuity.Input;
using Cuity.Rendering;
using Cuity.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Interacts, when input was happened on the standard IO. 
/// </summary>
/// <typeparam name="T">Type of the child.</typeparam>
public class InputDetector<T>: Entity where T: Entity {

    /// <summary>
    /// Callback, when the input was happened.
    /// </summary>
    public Action<InputMessage, T> OnInput {
        set {
            InteractionComponent? interaction = base.GetComponent<InteractionComponent>();

            if (interaction == null) {
                interaction = new InteractionComponent(onInput: (message) => {
                    Entity child = base.GetComponent<ConnectionComponent>()!.Next;
                    RenderComponent? render = child.GetComponent<RenderComponent>();

                    value(message, (T)child);
                    render?.UpdateChanges();
                });

                base.AttachComponent<InteractionComponent>(interaction, isUnique: true);
            }
        }
    }

    /// <summary>
    /// Attached child of the <see cref="InputDetector{T}"/>.
    /// </summary>
    public Entity Child {
        set {
            ConnectionComponent? connection = base.GetComponent<ConnectionComponent>();
            connection?.Next = value;

            connection = value.GetComponent<ConnectionComponent>();
            connection?.Previous = this;
        }
    }

    public InputDetector() 
        => base.AttachComponent<ConnectionComponent>(new ConnectionComponent(), isUnique: true);
}
