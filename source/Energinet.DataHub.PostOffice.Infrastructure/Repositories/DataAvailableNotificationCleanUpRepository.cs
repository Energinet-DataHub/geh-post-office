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

using System.Linq;
using System.Threading.Tasks;
using Energinet.DataHub.PostOffice.Domain.Repositories;
using Energinet.DataHub.PostOffice.Infrastructure.Common;
using Energinet.DataHub.PostOffice.Infrastructure.Documents;
using Energinet.DataHub.PostOffice.Infrastructure.Repositories.Containers;
using Microsoft.Azure.Cosmos;
using NodaTime;

namespace Energinet.DataHub.PostOffice.Infrastructure.Repositories
{
    public sealed class DataAvailableNotificationCleanUpRepository : IDataAvailableNotificationCleanUpRepository
    {
        private const int MaximumCabinetDrawerItemCount = 10000;

        private readonly IDataAvailableNotificationRepositoryContainer _repositoryContainer;

        public DataAvailableNotificationCleanUpRepository(
            IDataAvailableNotificationRepositoryContainer repositoryContainer)
        {
            _repositoryContainer = repositoryContainer;
        }

        public async Task DeleteOldCabinetDrawersAsync()
        {
            var asLinq = _repositoryContainer
                .Cabinet
                .GetItemLinqQueryable<CosmosCabinetDrawer>();

            var nowInIsoUtc = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(-7));
            var deletionTime = (ulong)nowInIsoUtc.ToUnixTimeMilliseconds();

            var query =
                from cabinetDrawer in asLinq
                where
                    cabinetDrawer.Position == MaximumCabinetDrawerItemCount &&
                    cabinetDrawer.TimeStamp < deletionTime
                select cabinetDrawer;

            await foreach (var drawerToDelete in query.AsCosmosIteratorAsync())
            {
                await DeleteDrawerAsync(drawerToDelete).ConfigureAwait(false);
            }
        }

        private async Task DeleteDrawerAsync(CosmosCabinetDrawer drawerToDelete)
        {
            await _repositoryContainer
                 .Cabinet
                 .DeleteItemAsync<CosmosCabinetDrawer>(drawerToDelete.Id, new PartitionKey(drawerToDelete.PartitionKey))
                 .ConfigureAwait(false);
        }
    }
}
