﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Identity.Client.Internal;
using Microsoft.Identity.Client.Utils;

namespace Microsoft.Identity.Client.ManagedIdentity
{
    internal class AppServiceManagedIdentitySource : ManagedIdentitySource
    {
        // MSI Constants. Docs for MSI are available here https://docs.microsoft.com/azure/app-service/overview-managed-identity
        protected string AppServiceMsiApiVersion = "2019-08-01";
        protected string SecretHeaderName = "X-IDENTITY-HEADER";
        protected string ClientIdHeaderName = "client_id";

        private const string MsiEndpointInvalidUriError = "The environment variable MSI_ENDPOINT contains an invalid Uri.";

        private readonly Uri _endpoint;
        private readonly string _secret;
        private readonly string _clientId;
        private readonly string _resourceId;

        public static ManagedIdentitySource TryCreate(RequestContext requestContext)
        {
            var msiSecret = EnvironmentVariables.IdentityHeader;
            return TryValidateEnvVars(EnvironmentVariables.IdentityEndpoint, msiSecret, out Uri endpointUri)
                ? new AppServiceManagedIdentitySource(requestContext, endpointUri, msiSecret)
                : null;
        }

        protected static bool TryValidateEnvVars(string msiEndpoint, string secret, out Uri endpointUri)
        {
            endpointUri = null;

            // if BOTH the env vars endpoint and secret values are null, this MSI provider is unavailable.
            // Also validate that IdentityServerThumbprint is null or empty to differentiate from Service Fabric.
            if (string.IsNullOrEmpty(msiEndpoint) || string.IsNullOrEmpty(secret))
            {
                return false;
            }

            try
            {
                endpointUri = new Uri(msiEndpoint);
            }
            catch (FormatException ex)
            {
                throw new MsalClientException(MsalError.AuthenticationFailed, MsiEndpointInvalidUriError, ex);
            }

            return true;
        }

        protected AppServiceManagedIdentitySource(RequestContext requestContext, Uri endpoint, string secret) : base(requestContext)
        {
            _endpoint = endpoint;
            _secret = secret;
        }

        protected override ManagedIdentityRequest CreateRequest(string[] scopes)
        {
            // covert the scopes to a resource string
            string resource = ScopeUtilities.ScopesToResource(scopes);

            ManagedIdentityRequest request = new ManagedIdentityRequest(System.Net.Http.HttpMethod.Get, _endpoint);
            IDictionary<string, string> queryParams = new Dictionary<string, string>();

            request.Headers.Add(SecretHeaderName, _secret);
            queryParams["api-version"] = AppServiceMsiApiVersion;
            queryParams["resource"] = resource;

            if (!string.IsNullOrEmpty(_clientId))
            {
                queryParams[ClientIdHeaderName] = _clientId;
            }

            if (!string.IsNullOrEmpty(_resourceId))
            {
                queryParams[MsiConstants.ManagedIdentityResourceId] = _resourceId;
            }

            request.UriBuilder.AppendQueryParameters(queryParams);
            return request;
        }
    }
}