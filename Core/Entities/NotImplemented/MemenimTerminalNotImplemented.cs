using System;

namespace Memenim.Scripting.Core.Entities.NotImplemented
{
    public sealed class MemenimTerminalNotImplemented : MemenimTerminalBase
    {
        public override void WriteLine(string text)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
