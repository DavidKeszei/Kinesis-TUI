using Cuity.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Navigation;

/// <summary>
/// Provides navigation between <see cref="Page"/> instances.
/// </summary>
public interface INavigator: ISystem {

    /// <summary>
    /// Navigate to a <see cref="Page"/> based on the <paramref name="method"/>.
    /// </summary>
    /// <param name="method">Navigation method of the <see cref="Page"/>.</param>
    public void NavigateTo(Func<ISystemProvider, Page> method);

    /// <summary>
    /// Navigate to a <see cref="Page"/> based on the <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Name of the regitered <see cref="Page"/>.</param>
    public void NavigateTo(string name);

    /// <summary>
    /// Navigate back to the previous <see cref="Page"/>. 
    /// </summary>
    public void NavigateBack();
}