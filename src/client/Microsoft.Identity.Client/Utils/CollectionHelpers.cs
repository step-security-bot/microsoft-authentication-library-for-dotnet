﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

#if HAVE_METHOD_IMPL_ATTRIBUTE
using System.Runtime.CompilerServices;
#endif

namespace Microsoft.Identity.Client.Utils
{
    internal static class CollectionHelpers
    {
#if HAVE_METHOD_IMPL_ATTRIBUTE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IReadOnlyList<T> GetEmptyReadOnlyList<T>()
        {
#if NET45
            return new List<T>();
#else
            return Array.Empty<T>();
#endif
        }

#if HAVE_METHOD_IMPL_ATTRIBUTE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static List<T> GetEmptyList<T>()
        {
            return new List<T>();
        }

        public static IDictionary<TKey, TValue> GetEmptyDictionary<TKey, TValue>()
        {
#if NET
            return System.Collections.Immutable.ImmutableDictionary<TKey, TValue>.Empty;
#else
            return new Dictionary<TKey, TValue>();
#endif
        }

        private static Dictionary<string, string> s_emptyROStringDictionary = new Dictionary<string, string>();
        public static IReadOnlyDictionary<string, string> EmptyROStringDictionary
        {
            get
            {
#if NET
                return System.Collections.Immutable.ImmutableDictionary<string, string>.Empty;
#else
            return s_emptyROStringDictionary;
#endif
            }
        }
    }
}
