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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Energinet.DataHub.PostOffice.Domain.Model;
using Energinet.DataHub.PostOffice.Domain.Repositories;
using Energinet.DataHub.PostOffice.Infrastructure.Common;
using Energinet.DataHub.PostOffice.Infrastructure.Documents;
using Energinet.DataHub.PostOffice.Infrastructure.Repositories.Containers;
using Microsoft.Azure.Cosmos;

namespace Energinet.DataHub.PostOffice.Infrastructure.Repositories
{
    public class DataAvailableNotificationRepository : IDataAvailableNotificationRepository
    {
        private readonly IDataAvailableNotificationRepositoryContainer _repositoryContainer;

        public DataAvailableNotificationRepository(IDataAvailableNotificationRepositoryContainer repositoryContainer)
        {
            _repositoryContainer = repositoryContainer;
        }

        public async Task SaveAsync(DataAvailableNotification dataAvailableNotification)
        {
            if (dataAvailableNotification is null)
                throw new ArgumentNullException(nameof(dataAvailableNotification));

            var cosmosDocument = new CosmosDataAvailable
            {
                Uuid = dataAvailableNotification.NotificationId.ToString(),
                Recipient = dataAvailableNotification.Recipient.Gln.Value,
                ContentType = dataAvailableNotification.ContentType.Value,
                Origin = dataAvailableNotification.Origin.ToString(),
                SupportsBundling = dataAvailableNotification.SupportsBundling.Value,
                RelativeWeight = dataAvailableNotification.Weight.Value,
                Acknowledge = false
            };

            await _repositoryContainer.Container
                .CreateItemAsync(cosmosDocument)
                .ConfigureAwait(false);
        }

        public async Task<DataAvailableNotification?> GetNextUnacknowledgedAsync(MarketOperator recipient)
        {
            if (recipient is null)
                throw new ArgumentNullException(nameof(recipient));

            var asLinq = _repositoryContainer
                .Container
                .GetItemLinqQueryable<CosmosDataAvailable>();

            var query =
                from dataAvailable in asLinq
                where
                    dataAvailable.Recipient == recipient.Gln.Value &&
                    !dataAvailable.Acknowledge
                orderby dataAvailable.Timestamp
                select dataAvailable;

            return await ExecuteQueryAsync(query).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<DataAvailableNotification?> GetNextUnacknowledgedForDomainAsync(MarketOperator recipient, DomainOrigin domainOrigin)
        {
            if (recipient is null)
                throw new ArgumentNullException(nameof(recipient));

            var asLinq = _repositoryContainer
                .Container
                .GetItemLinqQueryable<CosmosDataAvailable>();

            var query =
                from dataAvailable in asLinq
                where
                    dataAvailable.Recipient == recipient.Gln.Value &&
                    dataAvailable.Origin == domainOrigin.ToString() &&
                    !dataAvailable.Acknowledge
                orderby dataAvailable.Timestamp
                select dataAvailable;

            return await ExecuteQueryAsync(query).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<DataAvailableNotification>> GetNextUnacknowledgedAsync(MarketOperator recipient, ContentType contentType, Weight weight)
        {
            if (recipient is null)
                throw new ArgumentNullException(nameof(recipient));

            if (contentType is null)
                throw new ArgumentNullException(nameof(contentType));

            var asLinq = _repositoryContainer
                .Container
                .GetItemLinqQueryable<CosmosDataAvailable>();

            var query =
                from dataAvailable in asLinq
                where
                    dataAvailable.Recipient == recipient.Gln.Value &&
                    dataAvailable.ContentType == contentType.Value &&
                    !dataAvailable.Acknowledge
                orderby dataAvailable.Timestamp
                select dataAvailable;

            return await ExecuteQueryAsync(query).ToListAsync().ConfigureAwait(false);
        }

        public async Task AcknowledgeAsync(IEnumerable<Uuid> dataAvailableNotificationUuids)
        {
            if (dataAvailableNotificationUuids is null)
                throw new ArgumentNullException(nameof(dataAvailableNotificationUuids));

            var stringIds = dataAvailableNotificationUuids
                .Select(x => x.ToString())
                .ToList();

            var container = _repositoryContainer.Container;
            var asLinq = container
                .GetItemLinqQueryable<CosmosDataAvailable>();

            var query =
                from dataAvailable in asLinq
                where stringIds.Contains(dataAvailable.Uuid)
                select dataAvailable;

            await foreach (var document in query.AsCosmosIteratorAsync())
            {
                var updatedDocument = document with { Acknowledge = true };
                await container
                    .ReplaceItemAsync(updatedDocument, updatedDocument.Id, new PartitionKey("recipient"))
                    .ConfigureAwait(false);
            }
        }

        private static async IAsyncEnumerable<DataAvailableNotification> ExecuteQueryAsync(IQueryable<CosmosDataAvailable> query)
        {
            await foreach (var document in query.AsCosmosIteratorAsync())
            {
                yield return new DataAvailableNotification(
                    new Uuid(document.Uuid),
                    new MarketOperator(new GlobalLocationNumber(document.Recipient)),
                    new ContentType(document.ContentType),
                    Enum.Parse<DomainOrigin>(document.Origin, true),
                    new SupportsBundling(document.SupportsBundling),
                    new Weight(document.RelativeWeight));
            }
        }
    }
}
