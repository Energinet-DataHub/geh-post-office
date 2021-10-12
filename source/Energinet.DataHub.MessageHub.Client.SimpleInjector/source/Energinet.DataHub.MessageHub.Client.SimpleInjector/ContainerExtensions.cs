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
using Energinet.DataHub.MessageHub.Client.DataAvailable;
using Energinet.DataHub.MessageHub.Client.Dequeue;
using Energinet.DataHub.MessageHub.Client.Factories;
using Energinet.DataHub.MessageHub.Client.Peek;
using Energinet.DataHub.MessageHub.Client.Storage;
using SimpleInjector;

namespace Energinet.DataHub.MessageHub.Client.SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void AddPostOfficeCommunication(this Container container, DomainConfig config)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            container.RegisterSingleton(() => config);
            container.AddServiceBus(config.ServiceBusConnectionString);
            container.AddApplicationServices();
            container.AddStorageHandler(config.StorageServiceConnectionString);
        }

        private static void AddServiceBus(this Container container, string serviceBusConnectionStringConfigKey)
        {
            container.RegisterSingleton<IServiceBusClientFactory>(() =>
            {
                if (string.IsNullOrEmpty(serviceBusConnectionStringConfigKey))
                {
                    throw new InvalidOperationException(
                        "Please specify a valid ServiceBus in the appSettings.json file or your Azure Functions Settings.");
                }

                return new ServiceBusClientFactory(serviceBusConnectionStringConfigKey);
            });
        }

        private static void AddApplicationServices(this Container container)
        {
            container.Register<IDataAvailableNotificationSender, DataAvailableNotificationSender>(Lifestyle.Singleton);
            container.Register<IRequestBundleParser, RequestBundleParser>(Lifestyle.Singleton);
            container.Register<IResponseBundleParser, ResponseBundleParser>(Lifestyle.Singleton);
            container.Register<IDataBundleResponseSender, DataBundleResponseSender>(Lifestyle.Singleton);
            container.Register<IDequeueNotificationParser, DequeueNotificationParser>(Lifestyle.Singleton);
        }

        private static void AddStorageHandler(this Container container, string storageServiceConnectionStringConfigKey)
        {
            container.RegisterSingleton<IStorageServiceClientFactory>(() =>
            {
                if (string.IsNullOrEmpty(storageServiceConnectionStringConfigKey))
                {
                    throw new InvalidOperationException(
                        "Please specify a valid BlobStorageConnectionString in the appSettings.json file or your Azure Functions Settings.");
                }

                return new StorageServiceClientFactory(storageServiceConnectionStringConfigKey);
            });

            container.Register<IStorageHandler, StorageHandler>(Lifestyle.Singleton);
        }
    }
}
