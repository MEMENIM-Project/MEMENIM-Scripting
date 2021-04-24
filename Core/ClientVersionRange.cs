using System;

namespace Memenim.Scripting.Core
{
    public class ClientVersionRange
    {
        public ClientVersion MinVersion { get; }
        public ClientVersion MaxVersion { get; }

        public bool AnyMinVersion
        {
            get
            {
                return MinVersion == null
                       || MinVersion == ClientVersion.MinValue
                       || MinVersion == ClientVersion.Any;
            }
        }
        public bool AnyMaxVersion
        {
            get
            {
                return MaxVersion == null
                       || MaxVersion == ClientVersion.MaxValue
                       || MaxVersion == ClientVersion.Any;
            }
        }
        public bool AnyVersion
        {
            get
            {
                return AnyMinVersion && AnyMaxVersion;
            }
        }

        public ClientVersionRange(
            ClientVersion minVersion = null, ClientVersion maxVersion = null)
        {
            if (minVersion == null)
                minVersion = ClientVersion.Any;
            if (maxVersion == null)
                maxVersion = ClientVersion.Any;

            if (minVersion > maxVersion)
            {
                MinVersion = maxVersion;
                MaxVersion = minVersion;

                return;
            }

            MinVersion = minVersion;
            MaxVersion = maxVersion;
        }

        public bool IsSatisfied(ClientVersion version)
        {
            if (AnyVersion)
                return true;

            if (AnyMinVersion)
                return version <= MaxVersion;

            if (AnyMaxVersion)
                return MinVersion <= version;

            return MinVersion <= version && version <= MaxVersion;
        }
    }
}
