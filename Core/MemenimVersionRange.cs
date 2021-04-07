using System;

namespace Memenim.Scripting.Core
{
    public class MemenimVersionRange
    {
        public MemenimVersion MinVersion { get; }
        public MemenimVersion MaxVersion { get; }

        public bool AnyMinVersion
        {
            get
            {
                return MinVersion == null
                       || MinVersion == MemenimVersion.MinValue
                       || MinVersion == MemenimVersion.Any;
            }
        }
        public bool AnyMaxVersion
        {
            get
            {
                return MaxVersion == null
                       || MaxVersion == MemenimVersion.MaxValue
                       || MaxVersion == MemenimVersion.Any;
            }
        }
        public bool AnyVersion
        {
            get
            {
                return AnyMinVersion && AnyMaxVersion;
            }
        }

        public MemenimVersionRange(
            MemenimVersion minVersion = null, MemenimVersion maxVersion = null)
        {
            if (minVersion == null)
                minVersion = MemenimVersion.Any;
            if (maxVersion == null)
                maxVersion = MemenimVersion.Any;

            if (minVersion > maxVersion)
            {
                MinVersion = maxVersion;
                MaxVersion = minVersion;

                return;
            }

            MinVersion = minVersion;
            MaxVersion = maxVersion;
        }

        public bool IsSatisfied(MemenimVersion version)
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
