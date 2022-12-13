﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Platforms.netcore;

namespace Microsoft.Identity.Client.Platforms.netstandard
{
    /// <summary>
    /// NetStandard 2.0 is very similar to Net Core. We could completely remove netcore2.1 however
    /// customers have reported issues with approach.
    /// </summary>
    internal class NetStandardPlatformProxyPublic : NetCorePlatformProxyPublic
    {
        public NetStandardPlatformProxyPublic(ILoggerAdapter logger)
            : base(logger)
        {
        }

        /// <inheritdoc />
        internal override string InternalGetProductName()
        {
            return "MSAL.CoreCLR";
        }
    }
}