using Cuity.Input;
using Cuity.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a interactive component on an <see cref="Entity"/>.
/// </summary>
public class InteractionComponent: IComponent, IStaticType {
    private const string TYPE_NAME = "Interaction";

    public static string Name { get => TYPE_NAME; }

    /// <summary>
    /// Create a new <see cref="InteractionComponent"/>, which fires every input.
    /// </summary>
    /// <param name="onInput">Callback for the inputs.</param>
    public InteractionComponent(Action<InputMessage> onInput, EntityChangeContext context) 
        => WorkerSystem.Current.AddCallback(work: onInput, context);

    /// <summary>
    /// Create a new <see cref="InteractionComponent"/>, which fires every render frame ends.
    /// </summary>
    /// <param name="onRender">Callback for the end of the frame.</param>
    public InteractionComponent(Action<RenderMessage> onRender, EntityChangeContext context)
        => WorkerSystem.Current.AddCallback(work: onRender, context);

    public bool TypeOf(string type) => TYPE_NAME == type;
}
