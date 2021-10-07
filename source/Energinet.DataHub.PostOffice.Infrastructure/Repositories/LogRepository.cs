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
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Energinet.DataHub.PostOffice.Domain.Model.Logging;
using Energinet.DataHub.PostOffice.Domain.Repositories;
using Energinet.DataHub.PostOffice.Infrastructure.Model;
using Energinet.DataHub.PostOffice.Infrastructure.Repositories.Containers;
using Microsoft.Azure.Cosmos;

namespace Energinet.DataHub.PostOffice.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly ILogRepositoryContainer _logRepositoryContainer;

        public LogRepository(ILogRepositoryContainer logRepositoryContainer)
        {
            _logRepositoryContainer = logRepositoryContainer;
        }

        public async Task<string> SaveLogOccurrenceAsync(Log log)
        {
            if (log is null)
                throw new ArgumentNullException(nameof(log));

            var instanceToLog = new CosmosLog(
                log.Id,
                log.EndpointType,
                log.MarketOperator.Value,
                log.ProcessId,
                log.Description);

            if (log.LogReferenceId is not null)
                instanceToLog.LogReferenceId = log.LogReferenceId;

            if (log.ReplyToMarketOperator is not null && log.ReplyToMarketOperator.BundleReference is not null)
            {
                var azureBundleContent = (AzureBlobBundleContent)log.ReplyToMarketOperator.BundleReference;
                instanceToLog.BundleReference = azureBundleContent.ContentPath.AbsoluteUri.ToString();
            }
            else if (log.ReplyToMarketOperator?.BundleError is not null)
            {
                instanceToLog.ErrorReason = log.ReplyToMarketOperator.BundleError.Reason.ToString();
                instanceToLog.FailureDescription = log.ReplyToMarketOperator.BundleError.FailureDescription;
            }

            var response = await _logRepositoryContainer.Container.CreateItemAsync(instanceToLog).ConfigureAwait(false);
            if (response.StatusCode is not HttpStatusCode.Created)
                throw new InvalidOperationException("Could not create document in cosmos");

            return log.Id;
        }
    }
}
