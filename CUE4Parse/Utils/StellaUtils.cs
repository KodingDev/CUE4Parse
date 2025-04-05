using System;
using System.Linq;

namespace CUE4Parse.Utils;

public class StellaUtils
{
    public class MarvelRivalsVersion
    {
        private readonly string _version;
        private readonly int[] _components;

        public MarvelRivalsVersion(string version)
        {
            _version = version;
            var parts = version.Split('.');
            _components = new int[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i], out _components[i]))
                {
                    throw new ArgumentException($"Version component '{parts[i]}' must be a valid integer", nameof(version));
                }
            }
        }

        public static implicit operator MarvelRivalsVersion(string version) => new(version);

        public static bool operator >(MarvelRivalsVersion a, MarvelRivalsVersion b) => Compare(a, b) > 0;
        public static bool operator <(MarvelRivalsVersion a, MarvelRivalsVersion b) => Compare(a, b) < 0;
        public static bool operator >=(MarvelRivalsVersion a, MarvelRivalsVersion b) => Compare(a, b) >= 0;
        public static bool operator <=(MarvelRivalsVersion a, MarvelRivalsVersion b) => Compare(a, b) <= 0;
        public static bool operator ==(MarvelRivalsVersion a, MarvelRivalsVersion b) => Compare(a, b) == 0;
        public static bool operator !=(MarvelRivalsVersion a, MarvelRivalsVersion b) => Compare(a, b) != 0;

        private static int Compare(MarvelRivalsVersion a, MarvelRivalsVersion b)
        {
            int maxLength = Math.Max(a._components.Length, b._components.Length);

            for (int i = 0; i < maxLength; i++)
            {
                int aValue = i < a._components.Length ? a._components[i] : 0;
                int bValue = i < b._components.Length ? b._components[i] : 0;

                if (aValue != bValue)
                    return aValue.CompareTo(bValue);
            }

            return 0;
        }

        public override bool Equals(object? obj) =>
            obj is MarvelRivalsVersion version && this == version;

        public override int GetHashCode() =>
            _components.Aggregate(17, (hash, component) => hash * 23 + component);

        public override string ToString() => _version;
    }

    public static MarvelRivalsVersion Version { get; set; } = new ("2.0.0");
}
