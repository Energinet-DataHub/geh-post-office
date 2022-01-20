﻿// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Energinet.DataHub.Core.FunctionApp.Common.SimpleInjector;
using SimpleInjector;

namespace Energinet.DataHub.PostOffice.Common.Auth
{
    internal static class AuthRegistrations
    {
        public static void AddAuthentication(this Container container)
        {
            var tenantId = Environment.GetEnvironmentVariable("B2C_TENANT_ID") ?? throw new InvalidOperationException("B2C tenant id not found.");
            var audience = Environment.GetEnvironmentVariable("BACKEND_SERVICE_APP_ID") ?? throw new InvalidOperationException("Backend service app id not found.");

            container.AddJwtTokenSecurity($"https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration", audience);
            container.Register<IMarketOperatorIdentity, MarketOperatorIdentity>(Lifestyle.Scoped);
            container.Register<JwtAuthenticationMiddleware>(Lifestyle.Scoped);
            container.Register<QueryAuthenticationMiddleware>(Lifestyle.Scoped);
        }
    }
}
