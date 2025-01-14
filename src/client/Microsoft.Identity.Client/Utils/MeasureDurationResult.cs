﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Utils
{
    /// <summary>
    /// Structure that holds a <see cref="Task"/> result and duration of the <see cref="Task"/> in milliseconds
    /// </summary>
    internal struct MeasureDurationResult<TResult>
    {
        public MeasureDurationResult(TResult result, long ticks)
        {
            Result = result;
            Milliseconds = ticks / TimeSpan.TicksPerMillisecond;
            Ticks = ticks;
        }

        public TResult Result { get; }

        public long Milliseconds { get; }

        public long Ticks { get; }
    }

    /// <summary>
    /// Structure that holds a duration of the <see cref="Task"/> in milliseconds.
    /// </summary>
    internal struct MeasureDurationResult
    {
        public MeasureDurationResult(long ticks)
        {
            Milliseconds = ticks / TimeSpan.TicksPerMillisecond;
            Ticks = ticks;
        }

        public long Milliseconds { get; }

        public long Ticks { get; }
    }
}
