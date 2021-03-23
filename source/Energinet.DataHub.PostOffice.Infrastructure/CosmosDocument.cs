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

using Newtonsoft.Json;

namespace Energinet.DataHub.PostOffice.Infrastructure
{
    #nullable disable
    public class CosmosDocument
    {
        public CosmosDocument()
        {
            Id = System.Guid.NewGuid().ToString();
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "recipient")]
        public string Recipient { get; set; }

        [JsonProperty(PropertyName = "creationDate")]
        public System.DateTimeOffset CreationDate { get; set; }

        [JsonProperty(PropertyName = "content")]
        public object Content { get; set; }

        [JsonProperty(PropertyName = "bundle")]
        public string Bundle { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
    }
    #nullable restore
}
