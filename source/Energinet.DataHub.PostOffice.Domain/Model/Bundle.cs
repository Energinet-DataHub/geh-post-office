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
using System.Diagnostics.CodeAnalysis;

namespace Energinet.DataHub.PostOffice.Domain.Model
{
    public sealed class Bundle
    {
        private IBundleContent? _bundleContent;

        public Bundle(
            Uuid bundleId,
            DomainOrigin origin,
            MarketOperator recipient,
            IReadOnlyCollection<Uuid> notificationIds)
        {
            BundleId = bundleId;
            Origin = origin;
            Recipient = recipient;
            NotificationIds = notificationIds;
            ProcessId = new ProcessId(bundleId, recipient);
        }

        public Bundle(
            Uuid bundleId,
            DomainOrigin origin,
            MarketOperator recipient,
            IReadOnlyCollection<Uuid> notificationIds,
            IBundleContent? bundleContent)
        {
            BundleId = bundleId;
            Origin = origin;
            Recipient = recipient;
            NotificationIds = notificationIds;
            _bundleContent = bundleContent;
            ProcessId = new ProcessId(bundleId, recipient);
        }

        public Uuid BundleId { get; }
        public DomainOrigin Origin { get; }
        public MarketOperator Recipient { get; }
        public IReadOnlyCollection<Uuid> NotificationIds { get; }
        public ProcessId ProcessId { get; }
        public bool NotificationsArchived { get; set; }

        public bool TryGetContent([NotNullWhen(true)] out IBundleContent? bundleContent)
        {
            bundleContent = _bundleContent;
            return _bundleContent != null;
        }

        public void AssignContent(IBundleContent bundleContent)
        {
            if (_bundleContent != null)
                throw new InvalidOperationException("Content has already been set.");

            _bundleContent = bundleContent;
        }
    }
}
