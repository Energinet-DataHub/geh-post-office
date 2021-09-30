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

using Azure.Storage.Blobs;

namespace GreenEnergyHub.PostOffice.Communicator.Factories
{
    public sealed class StorageServiceClientFactory : IStorageServiceClientFactory
    {
        private static BlobServiceClient? _blobServiceClient;
        private readonly string _connectionString;

        public StorageServiceClientFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BlobServiceClient Create()
        {
            if (_blobServiceClient is null)
            {
                _blobServiceClient = new BlobServiceClient(_connectionString);
            }

            return _blobServiceClient;
        }
    }
}
