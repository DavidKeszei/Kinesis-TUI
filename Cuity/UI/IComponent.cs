using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Indicates a component for an object.
/// </summary>
public interface IComponent {

    /// <summary>
    /// Name of the component.
    /// </summary>
    public string Name { get; }
}
