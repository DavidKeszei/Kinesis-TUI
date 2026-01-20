using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Cuity.Input;

public readonly record struct InputMessage(char Key, InputModifier Modifier, InputAction Action);

public class InputHandler {
    /// <summary>
    /// Minimum time, when we think no input was happened & fire. (50ms)
    /// </summary>
    private const int DEAD_ZONE = 10;

    private const int HOLD_THRESHHOLD = 250;

    private (char Key, InputModifier Modifier, TimeSpan When) m_startInputInfo = ('\0', InputModifier.NONE, TimeSpan.Zero);
    private IInputBackend m_backend = null!;

    public InputHandler() {
        m_backend = RuntimeInformation.IsOSPlatform(osPlatform: OSPlatform.Windows) ? WindowsInputBackend.Init() : null!;
    }

    public void Start() {
        float deadZoneTime = DEAD_ZONE;

        InputMessage message = default;
        InputAction currentAction = InputAction.PRESS;

        while (true) {
            DateTime now = DateTime.UtcNow;

            if (m_backend.HasInput) {
                (char character, InputModifier modifiers) = m_backend.ReadInput();

                if(character == '\0')
                    continue;

                if (m_startInputInfo.Key == character && m_startInputInfo.Modifier == modifiers) {

                    if((now.TimeOfDay - m_startInputInfo.When).TotalMilliseconds >= HOLD_THRESHHOLD) {
                        message = new InputMessage(Key: m_startInputInfo.Key,
                                                          Modifier: m_startInputInfo.Modifier,
                                                          Action: currentAction);

                        if(currentAction != InputAction.HOLD)
                            Console.Out.WriteLineAsync($"<INPUT> Key: {message.Key}, Action: {Enum.GetName<InputAction>(InputAction.HOLD)} - H");
                        currentAction = InputAction.HOLD;
                    }

                    continue;
                }
                else if (m_startInputInfo.Key != character || m_startInputInfo.Modifier != modifiers) {
                    if (m_startInputInfo.When != TimeSpan.Zero) {

                        message = new InputMessage(Key: m_startInputInfo.Key,
                                                   Modifier: m_startInputInfo.Modifier,
                                                   Action: currentAction);

                        deadZoneTime = DEAD_ZONE;
                        Console.Out.WriteLineAsync($"<INPUT> Key: {message.Key}, Action: {Enum.GetName<InputAction>(currentAction)} - F");
                    }

                    m_startInputInfo = (character, modifiers, now.TimeOfDay);
                }

                continue;
            }

            if (deadZoneTime <= 0) {
                if(currentAction != InputAction.HOLD) {
                    message = new InputMessage(Key: m_startInputInfo.Key,
                                               Modifier: m_startInputInfo.Modifier,
                                               Action: currentAction);

                    Console.Out.WriteLineAsync($"<INPUT> Key: {message.Key}, Action: {Enum.GetName<InputAction>(currentAction)} - P");
                }

                m_startInputInfo = ('\0', InputModifier.NONE, TimeSpan.Zero);

                deadZoneTime = DEAD_ZONE;
                currentAction = InputAction.PRESS;
                continue;
            }


            if(m_startInputInfo.When != TimeSpan.Zero) {
                float inputTime = (float)(DateTime.UtcNow - now).TotalMilliseconds;
                deadZoneTime -= inputTime;
            }
        }

    }
}
