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
    public sealed class ServiceBusClientFactory : IServiceBusClientFactory
    {
        private readonly string _connectionString;
        private readonly IMessageBusFactory _messageBusFactory;

        public ServiceBusClientFactory(string connectionString, IMessageBusFactory messageBusFactory)
        {
            _connectionString = connectionString;
            _messageBusFactory = messageBusFactory;
        }

        public ISenderMessageBus CreateSender(string queueOrTopicName)
        {
            return _messageBusFactory.GetSenderClient(_connectionString, queueOrTopicName);
        }

        public Task<AzureSessionReceiverServiceBus> CreateSessionReceiverAsync(string queueOrTopicName, string sessionId)
        {
            return _messageBusFactory.GetSessionReceiverClientAsync(_connectionString, queueOrTopicName, sessionId);
        }
    }
}
