using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.Processing;

public interface IWorkMessage {

    public abstract static WorkMessageSource Target { get; }
}
