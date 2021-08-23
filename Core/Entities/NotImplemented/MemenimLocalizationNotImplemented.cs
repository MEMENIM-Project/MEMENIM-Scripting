using System;

namespace Memenim.Scripting.Core.Entities.NotImplemented
{
    public sealed class MemenimLocalizationNotImplemented : MemenimLocalizationBase
    {
        public override string GetLocalized(
            string key)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetLocalized(
            string key, out string value)
        {
            throw new NotImplementedException();
        }
    }
}
