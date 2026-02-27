using Kinesis.Input;
using Kinesis.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.UI.Components;

/// <summary>
/// Represent a interactive component on an <see cref="Entity"/>.
/// </summary>
public class InteractionComponent: IComponent, IStaticType {
    private const string TYPE_NAME = "Interaction";

    /// <summary>
    /// Name of the <see cref="InteractionComponent"/>.
    /// </summary>
    public static string Name { get => TYPE_NAME; }

    /// <summary>
    /// Create a new <see cref="InteractionComponent"/>, which fires every input.
    /// </summary>
    /// <param name="onInput">Callback for the inputs.</param>
    public InteractionComponent(Action<InputMessage> onInput, EntityContext context, Island island) 
        => WorkerSystem.Current.AddCallback(work: onInput, context, island);

    /// <summary>
    /// Create a new <see cref="InteractionComponent"/>, which fires every render frame ends.
    /// </summary>
    /// <param name="onRender">Callback for the end of the frame.</param>
    public InteractionComponent(Action<RenderMessage> onRender, EntityContext context, Island island)
        => WorkerSystem.Current.AddCallback(work: onRender, context, island);

    public bool TypeOf(string type) 
        => TYPE_NAME == type;
}
