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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Energinet.DataHub.PostOffice.Application.Commands;
using Energinet.DataHub.PostOffice.Domain.Model;
using Energinet.DataHub.PostOffice.Domain.Services;
using MediatR;

namespace Energinet.DataHub.PostOffice.Application.Handlers
{
    public class PeekHandler : IRequestHandler<PeekCommand, PeekResponse>
    {
        private readonly IMarketOperatorDataDomainService _marketOperatorDataDomainService;

        public PeekHandler(IMarketOperatorDataDomainService marketOperatorDataDomainService)
        {
            _marketOperatorDataDomainService = marketOperatorDataDomainService;
        }

        public async Task<PeekResponse> Handle(PeekCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var bundle = await _marketOperatorDataDomainService
                .GetNextUnacknowledgedAsync(new MarketOperator(new GlobalLocationNumber(request.Recipient)), new Uuid(request.BundleId))
                .ConfigureAwait(false);

            return bundle != null && bundle.TryGetContent(out var bundleContent)
                ? new PeekResponse(true, await bundleContent.OpenAsync().ConfigureAwait(false))
                : new PeekResponse(false, Stream.Null);
        }
    }
}
