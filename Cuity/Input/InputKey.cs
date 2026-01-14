using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Input;

public enum InputKey: byte {
    NONE,
    W,
    A,
    S,
    D
}

public enum InputModifier: byte {
    NONE,
    SHIFT,
    CTRL,
    ALT
}