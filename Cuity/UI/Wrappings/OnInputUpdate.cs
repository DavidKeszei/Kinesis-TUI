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
/// <typeparam name="T">Type of the child.</typeparam>
public class OnInputUpdate<T>: Entity where T: Entity {

    /// <summary>
    /// Callback, when the input was happened.
    /// </summary>
    public Action<InputMessage, PageEntityVisitor> OnInput {
        set {
            InteractionComponent? interaction = base.GetComponent<InteractionComponent>();

            if (interaction == null) {
                Entity? child = base.GetComponent<ConnectionComponent>()?.Next;

                interaction = new InteractionComponent(onInput: (message) => {
                    RenderComponent? render = child?.GetComponent<RenderComponent>();

                    value(message, new PageEntityVisitor(this));
                    render?.IsDirty = true;

                    child!.State = EntityState.LOCKED;
                }, child!);

                base.AttachComponent<InteractionComponent>(interaction, isUnique: true);
            }
        }
    }

    /// <summary>
    /// Attached child of the <see cref="OnRenderUpdate{T}"/>.
    /// </summary>
    public Entity Child {
        set {
            ConnectionComponent? connection = base.GetComponent<ConnectionComponent>();
            connection?.Next = value;

            if ((connection = value.GetComponent<ConnectionComponent>()) == null) {
                connection = new ConnectionComponent();
                value.AttachComponent<ConnectionComponent>();
            }

            connection.Previous = this;
        }
    }

    public OnInputUpdate() 
        => base.AttachComponent<ConnectionComponent>(new ConnectionComponent(), isUnique: true);
}

/// <summary>
/// Interacts, when input was happened on the standard IO. 
/// </summary>
/// <typeparam name="T">Type of the child.</typeparam>
public class OnRenderUpdate<T>: Entity where T : Entity {

    /// <summary>
    /// Callback, when the input was happened.
    /// </summary>
    public Action<RenderMessage, PageEntityVisitor> OnRender {
        set {
            InteractionComponent? interaction = base.GetComponent<InteractionComponent>();

            if (interaction == null) {
                Entity? child = base.GetComponent<ConnectionComponent>()?.Next;

                interaction = new InteractionComponent(onRender: (message) => {
                    child = base.GetComponent<ConnectionComponent>()?.Next;
                    RenderComponent? render = child?.GetComponent<RenderComponent>();

                    value(message, new PageEntityVisitor(this));
                    render?.IsDirty = true;

                    child!.State = EntityState.LOCKED;
                }, child!);

                base.AttachComponent<InteractionComponent>(interaction, isUnique: true);
            }
        }
    }

    /// <summary>
    /// Attached child of the <see cref="OnRenderUpdate{T}"/>.
    /// </summary>
    public Entity Child {
        set {
            ConnectionComponent? connection = base.GetComponent<ConnectionComponent>();
            connection?.Next = value;

            if ((connection = value.GetComponent<ConnectionComponent>()) == null) {
                connection = new ConnectionComponent();
                value.AttachComponent<ConnectionComponent>();
            }

            connection.Previous = this;
        }
    }

    public OnRenderUpdate()
        => base.AttachComponent<ConnectionComponent>(new ConnectionComponent(), isUnique: true);
}
