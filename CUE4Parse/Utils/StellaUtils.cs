namespace CUE4Parse.Utils;

public class StellaUtils
{
    public class MarvelRivalsVersion
    {
        private readonly string _version;
        private readonly long _numericVersion;

        public MarvelRivalsVersion(string version)
        {
            _version = version;
            _numericVersion = long.Parse(version.Replace(".", ""));
        }

        public static implicit operator MarvelRivalsVersion(string version) => new(version);

        public static bool operator >(MarvelRivalsVersion a, MarvelRivalsVersion b) => a._numericVersion > b._numericVersion;
        public static bool operator <(MarvelRivalsVersion a, MarvelRivalsVersion b) => a._numericVersion < b._numericVersion;
        public static bool operator >=(MarvelRivalsVersion a, MarvelRivalsVersion b) => a._numericVersion >= b._numericVersion;
        public static bool operator <=(MarvelRivalsVersion a, MarvelRivalsVersion b) => a._numericVersion <= b._numericVersion;
        public static bool operator ==(MarvelRivalsVersion a, MarvelRivalsVersion b) => a._numericVersion == b._numericVersion;
        public static bool operator !=(MarvelRivalsVersion a, MarvelRivalsVersion b) => a._numericVersion != b._numericVersion;

        public override bool Equals(object? obj) => obj is MarvelRivalsVersion version && _numericVersion == version._numericVersion;
        public override int GetHashCode() => _numericVersion.GetHashCode();
        public override string ToString() => _version;
    }

    public static MarvelRivalsVersion Version => new("2.0.0");
}