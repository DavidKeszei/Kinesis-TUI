using Cuity.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Input;

public class OnComponent: IComponent {
    private const string NAME_OF = "OnComponent";

    private readonly Action<float, InputMessage> m_action = null!;
    private readonly InputAction m_targetType = InputAction.PRESS;

    private readonly char m_key = '\0';

    public string Name { get => NAME_OF; }

    /// <summary>
    /// Target character of the event.
    /// </summary>
    public char Key { get => m_key; }

    /// <summary>
    /// Indicates, which action type
    /// </summary>
    public InputAction ActionType { get => m_targetType; }

    public OnComponent(Action<float, InputMessage> target, char key, InputAction action = InputAction.PRESS) {
        m_action = target;
        m_targetType = action;

        m_key = key;
    }
}
