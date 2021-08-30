﻿// // Copyright 2020 Energinet DataHub A/S
// //
// // Licensed under the Apache License, Version 2.0 (the "License2");
// // you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at
// //
// //     http://www.apache.org/licenses/LICENSE-2.0
// //
// // Unless required by applicable law or agreed to in writing, software
// // distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and
// // limitations under the License.
using System;
using System.Linq;
using Energinet.DataHub.PostOffice.Domain.Model;
using Energinet.DataHub.PostOffice.Infrastructure.Entities;

namespace Energinet.DataHub.PostOffice.Infrastructure.Mappers
{
    internal static class BundleMapper
    {
        public static Bundle MapFromDocument(BundleDocument from)
        {
            return new Bundle(
                new Uuid(from.Id),
                from.NotificationsIds.Select(x => new Uuid(x)));
        }

        public static BundleDocument MapToDocument(IBundle from, Recipient recipient)
        {
            return new BundleDocument()
            {
                Recipient = recipient.Value,
                Id = from.Id.Value,
                NotificationsIds = from.NotificationsIds.Select(x => x.Value).ToList(),
                Dequeued = false
            };
        }
    }
}
