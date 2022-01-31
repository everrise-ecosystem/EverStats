// Copyright (c) EverRise Pte Ltd. All rights reserved.

namespace System;

public static partial class MemoryExtensions
{
    public static SpanSplitEnumerator<char> Split(this ReadOnlySpan<char> span, char separator)
        => new SpanSplitEnumerator<char>(span, separator);

    public ref struct SpanSplitEnumerator<T>
#nullable disable // to enable use with both T and T? for reference types due to IEquatable<T> being invariant
            where T : IEquatable<T>
#nullable restore
    {
        private readonly ReadOnlySpan<char> _span;
        private readonly char _separatorChar;
        private int _start;
        private bool _started;
        private bool _ended;
        private Range _current;

        public SpanSplitEnumerator<T> GetEnumerator() => this;

        public Range Current
        {
            get
            {
                if (!_started || _ended)
                {
                    Throw();
                }

                return _current;

                static void Throw()
                {
                    throw new InvalidOperationException();
                }
            }

        }

        internal SpanSplitEnumerator(ReadOnlySpan<char> span, char separator) : this()
        {
            _span = span;
            _separatorChar = separator;
        }

        public bool MoveNext()
        {
            _started = true;

            if (_start > _span.Length)
            {
                _ended = true;
                return false;
            }

            ReadOnlySpan<char> slice = _start == 0
                ? _span
                : _span.Slice(_start);

            int end = _start;
            if (slice.Length > 0)
            {
                int index = slice.IndexOf(_separatorChar);

                if (index == -1)
                {
                    index = slice.Length;
                }

                end += index;
            }

            _current = new Range(_start, end);
            _start = end + 1;

            return true;
        }
    }
}
