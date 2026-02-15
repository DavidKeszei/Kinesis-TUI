using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Indicates a component for an object.
/// </summary>
public interface IComponent {

    /// <summary>
    /// Check if the component static type name is equal with <paramref name="type"/>.
    /// </summary>
    /// <param name="type">Type name of the component.</param>
    /// <returns>Return <see langword="true"/>, if the <paramref name="type"/> is equal with the underlying name. Otherwise return <see langword="false"/>.</returns>
    public bool IsType(string type);
}
