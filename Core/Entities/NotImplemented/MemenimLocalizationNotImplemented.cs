using System;

namespace Memenim.Scripting.Core.Entities.NotImplemented
{
    public sealed class MemenimLocalizationNotImplemented : MemenimLocalizationBase
    {
        public override string GetLocalized(string key)
        {
            throw new NotImplementedException();
        }
        public override TOut GetLocalized<TOut>(string key)
        {
            throw new NotImplementedException();
        }

        public override string TryGetLocalized(string key)
        {
            throw new NotImplementedException();
        }
        public override TOut TryGetLocalized<TOut>(string key)
        {
            throw new NotImplementedException();
        }
    }
}
