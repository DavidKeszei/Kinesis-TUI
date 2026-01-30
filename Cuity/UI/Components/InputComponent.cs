using Cuity.Input;
using Cuity.Processing;

using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a component, which run a given task based on the input.
/// </summary>
public class InputComponent: IComponent {
    private const string NAME_OF = "InputComponent";

    private readonly InputAction m_targetType = InputAction.PRESS;
    private readonly char m_key = '\0';

    /// <summary>
    /// Name of the <see cref="InputComponent"/>
    /// </summary>
    public string Name { get => NAME_OF; }

    public InputComponent(Action<InputMessage> work) 
            => WorkerSystem.Instance.AddQueueAction(work);
}
