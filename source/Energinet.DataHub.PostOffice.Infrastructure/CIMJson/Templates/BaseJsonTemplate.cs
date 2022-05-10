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
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Energinet.DataHub.PostOffice.Infrastructure.CIMJson.FluentCimJson.Reader;

namespace Energinet.DataHub.PostOffice.Infrastructure.CIMJson.Templates
{
    internal abstract class BaseJsonTemplate : IDisposable
    {
        private readonly string _jsonRootElementName;
        private CimXmlReader? _reader;
        protected BaseJsonTemplate(string jsonRootElementName)
        {
            _jsonRootElementName = jsonRootElementName;
        }

        public async Task<MemoryStream> ParseXmlAsync(Stream xmlData)
        {
            _reader = new CimXmlReader(xmlData);
            // _reader = XmlReader.Create(xmlData, new XmlReaderSettings()
            // {
            //     Async = true,
            //     CheckCharacters = false,
            //     CloseInput = true,
            //     ConformanceLevel = ConformanceLevel.Auto,
            //     IgnoreWhitespace = true,
            // });
#pragma warning disable CA2007
            await using MemoryStream jsonStream = new();
            await using Utf8JsonWriter jsonWriter = new(
                jsonStream,
                new JsonWriterOptions { Indented = false, SkipValidation = true });
#pragma warning restore CA2007
            jsonWriter.WriteStartObject();
            jsonWriter.WriteStartObject(_jsonRootElementName);
            await GenerateAsync(jsonWriter, _reader).ConfigureAwait(false);
            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
            await jsonWriter.FlushAsync().ConfigureAwait(false);

            return jsonStream;
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }

        protected abstract ValueTask GenerateAsync(Utf8JsonWriter jsonWriter, CimXmlReader reader);
    }
}
