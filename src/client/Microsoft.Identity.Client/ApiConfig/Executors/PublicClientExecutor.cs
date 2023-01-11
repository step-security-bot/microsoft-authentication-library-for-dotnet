﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client.ApiConfig.Parameters;
using Microsoft.Identity.Client.Internal;
using Microsoft.Identity.Client.Internal.Requests;
using Microsoft.Identity.Client.Internal.Requests.Silent;
using Microsoft.Identity.Client.UI;

namespace Microsoft.Identity.Client.ApiConfig.Executors
{
    internal class PublicClientExecutor : AbstractExecutor, IPublicClientApplicationExecutor
    {
        private readonly PublicClientApplication _publicClientApplication;

        public PublicClientExecutor(IServiceBundle serviceBundle, PublicClientApplication publicClientApplication)
            : base(serviceBundle)
        {
            _publicClientApplication = publicClientApplication;
        }

        public async Task<AuthenticationResult> ExecuteAsync(
            AcquireTokenCommonParameters commonParameters,
            AcquireTokenInteractiveParameters interactiveParameters,
            CancellationToken cancellationToken)
        {
            var requestContext = CreateRequestContextAndLogVersionInfo(commonParameters.CorrelationId, cancellationToken);

            AuthenticationRequestParameters requestParams = await _publicClientApplication.CreateRequestParametersAsync(
                commonParameters,
                requestContext,
                _publicClientApplication.UserTokenCacheInternal).ConfigureAwait(false);

            requestParams.LoginHint = interactiveParameters.LoginHint;
            requestParams.Account = interactiveParameters.Account;

            InteractiveRequest interactiveRequest =
                new InteractiveRequest(requestParams, interactiveParameters, _publicClientApplication.ServiceBundlePublic);

            return await interactiveRequest.RunAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<AuthenticationResult> ExecuteAsync(
            AcquireTokenCommonParameters commonParameters,
            AcquireTokenWithDeviceCodeParameters deviceCodeParameters,
            CancellationToken cancellationToken)
        {
            var requestContext = CreateRequestContextAndLogVersionInfo(commonParameters.CorrelationId, cancellationToken);

            var requestParams = await _publicClientApplication.CreateRequestParametersAsync(
                commonParameters,
                requestContext,
                _publicClientApplication.UserTokenCacheInternal).ConfigureAwait(false);

            var handler = new DeviceCodeRequest(
                ServiceBundle,
                requestParams,
                deviceCodeParameters);

            return await handler.RunAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<AuthenticationResult> ExecuteAsync(
            AcquireTokenCommonParameters commonParameters,
            AcquireTokenByIntegratedWindowsAuthParameters integratedWindowsAuthParameters,
            CancellationToken cancellationToken)
        {
            var requestContext = CreateRequestContextAndLogVersionInfo(commonParameters.CorrelationId, cancellationToken);

            var requestParams = await _publicClientApplication.CreateRequestParametersAsync(
                commonParameters,
                requestContext,
                _publicClientApplication.UserTokenCacheInternal).ConfigureAwait(false);

            var handler = new IntegratedWindowsAuthRequest(
                _publicClientApplication.ServiceBundlePublic,
                requestParams,
                integratedWindowsAuthParameters);

            return await handler.RunAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<AuthenticationResult> ExecuteAsync(
            AcquireTokenCommonParameters commonParameters,
            AcquireTokenByUsernamePasswordParameters usernamePasswordParameters,
            CancellationToken cancellationToken)
        {
            var requestContext = CreateRequestContextAndLogVersionInfo(commonParameters.CorrelationId, cancellationToken);

            var requestParams = await _publicClientApplication.CreateRequestParametersAsync(
                commonParameters,
                requestContext,
                _publicClientApplication.UserTokenCacheInternal).ConfigureAwait(false);

            var handler = new UsernamePasswordRequest(
                _publicClientApplication.ServiceBundlePublic,
                requestParams,
                usernamePasswordParameters);

            return await handler.RunAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<AuthenticationResult> ExecuteAsync(
            AcquireTokenCommonParameters commonParameters,
            AcquireTokenSilentParameters silentParameters,
            CancellationToken cancellationToken)
        {

            var requestContext = CreateRequestContextAndLogVersionInfo(commonParameters.CorrelationId, cancellationToken);

            var requestParameters = await _publicClientApplication.CreateRequestParametersAsync(
                commonParameters,
                requestContext,
                _publicClientApplication.UserTokenCacheInternal).ConfigureAwait(false);

            requestParameters.SendX5C = silentParameters.SendX5C ?? false;

            bool isBrokerConfigured = requestParameters.AppConfig.IsBrokerEnabled &&
                                      _publicClientApplication.ServiceBundlePublic.PlatformProxyPublic.CanBrokerSupportSilentAuth();

            var handler = new SilentRequest(ServiceBundle, requestParameters, silentParameters, isBrokerConfigured: isBrokerConfigured);

            if (isBrokerConfigured)
            {
                handler.BrokerStrategy = new BrokerSilentStrategy(
                    handler,
                    ServiceBundle,
                    requestParameters,
                    silentParameters,
                    _publicClientApplication.ServiceBundlePublic.PlatformProxyPublic.CreateBroker(
                        _publicClientApplication.ServiceBundlePublic.ConfigPublic, null));
            }

            return await handler.RunAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
