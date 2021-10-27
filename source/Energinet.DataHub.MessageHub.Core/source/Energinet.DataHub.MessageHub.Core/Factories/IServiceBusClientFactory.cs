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

using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Energinet.DataHub.MessageHub.Core.Factories
{
    /// <summary>
    /// Factory creating a new <see cref="ServiceBusSender"/> and <see cref="ServiceBusSessionReceiver"/>
    /// </summary>
    public interface IServiceBusClientFactory
    {
        /// <summary>
        /// Create a new <see cref="ServiceBusSender"/>
        /// </summary>
        /// <returns><see cref="ServiceBusSender"/></returns>
        public ISenderMessageBus CreateSender(string queueOrTopicName);

        /// <summary>
        /// Creates a new ServiceBusSessionReceiver
        /// </summary>
        /// <param name="queueOrTopicName"></param>
        /// <param name="sessionId"></param>
        public Task<AzureSessionReceiverServiceBus> CreateSessionReceiverAsync(string queueOrTopicName, string sessionId);
    }
}
