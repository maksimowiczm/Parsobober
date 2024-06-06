using System.Collections;

namespace Parsobober.Pkb.Relations.Dto;

public interface IPkbDto
{
    // forgive and forget
    public static IEnumerable<IPkbDto> Boolean(bool value) => new BooleanPkbDto(value);

    private class BooleanPkbDto(bool value) : IEnumerable<IPkbDto>
    {
        private readonly BooleanEnumerator _enumerator = new(value);

        public IEnumerator<IPkbDto> GetEnumerator() => _enumerator;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private class BooleanEnumerator(bool value) : IEnumerator<IPkbDto>
    {
        public bool MoveNext() => value;

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public IPkbDto Current => throw new NotImplementedException();

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}