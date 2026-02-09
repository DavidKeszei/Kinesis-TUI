using Cuity.Input;
using Cuity.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a interactive component on an <see cref="Entity"/>.
/// </summary>
public class InteractionComponent: IComponent {
    private const string NAME_OF = "Interaction";

    public string Name { get => nameof(InteractionComponent); }

    /// <summary>
    /// Create a new <see cref="InteractionComponent"/>, which fires every input.
    /// </summary>
    /// <param name="onInput">Callback for the inputs.</param>
    public InteractionComponent(Action<InputMessage> onInput, Entity target) 
        => WorkerSystem.Current.AddCallback(work: onInput, target);

    /// <summary>
    /// Create a new <see cref="InteractionComponent"/>, which fires every render frame ends.
    /// </summary>
    /// <param name="onRender">Callback for the end of the frame.</param>
    public InteractionComponent(Action<RenderMessage> onRender, Entity target)
        => WorkerSystem.Current.AddCallback(work: onRender, target);
}
