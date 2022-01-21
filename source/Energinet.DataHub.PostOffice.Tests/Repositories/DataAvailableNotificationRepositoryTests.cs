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
using System.Threading.Tasks;
using Energinet.DataHub.PostOffice.Domain.Model;
using Energinet.DataHub.PostOffice.Domain.Repositories;
using Energinet.DataHub.PostOffice.Infrastructure.Repositories;
using Energinet.DataHub.PostOffice.Infrastructure.Repositories.Containers;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Energinet.DataHub.PostOffice.Tests.Repositories
{
    [UnitTest]
    public sealed class DataAvailableNotificationRepositoryTests
    {
        [Fact]
        public async Task ReadCatalogForNextUnacknowledgedAsync_NullRecipient_ThrowsException()
        {
            // Arrange
            var target = CreateTarget();

            // Act + Assert
            await Assert
                .ThrowsAsync<ArgumentNullException>(() =>
                    target.ReadCatalogForNextUnacknowledgedAsync(null!, Array.Empty<DomainOrigin>()))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task ReadCatalogForNextUnacknowledgedAsync_NullDomains_ThrowsException()
        {
            // Arrange
            var target = CreateTarget();

            // Act + Assert
            await Assert
                .ThrowsAsync<ArgumentNullException>(() =>
                    target.ReadCatalogForNextUnacknowledgedAsync(new MarketOperator(null!), null!))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task GetCabinetReaderAsync_NullKey_ThrowsException()
        {
            // Arrange
            var target = CreateTarget();

            // Act + Assert
            await Assert
                .ThrowsAsync<ArgumentNullException>(() => target.GetCabinetReaderAsync(null!))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task AcknowledgeAsync_NullBundle_ThrowsException()
        {
            // Arrange
            var target = CreateTarget();

            // Act + Assert
            await Assert
                .ThrowsAsync<ArgumentNullException>(() => target.AcknowledgeAsync(null!))
                .ConfigureAwait(false);
        }

        private static DataAvailableNotificationRepository CreateTarget()
        {
            var dataAvailableNotificationRepositoryContainer = new Mock<IDataAvailableNotificationRepositoryContainer>();
            var bundleRepositoryContainer = new Mock<IBundleRepositoryContainer>();
            var sequenceNumberRepository = new Mock<ISequenceNumberRepository>();
            return new DataAvailableNotificationRepository(
                bundleRepositoryContainer.Object,
                dataAvailableNotificationRepositoryContainer.Object,
                sequenceNumberRepository.Object);
        }
    }
}
