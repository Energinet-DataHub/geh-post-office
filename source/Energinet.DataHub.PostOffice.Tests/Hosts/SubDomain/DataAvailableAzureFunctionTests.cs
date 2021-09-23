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
using Energinet.DataHub.PostOffice.Application.Commands;
using Energinet.DataHub.PostOffice.EntryPoint.SubDomain.Functions;
using Energinet.DataHub.PostOffice.EntryPoint.SubDomain.Parsing;
using Energinet.DataHub.PostOffice.Infrastructure.Mappers;
using Energinet.DataHub.PostOffice.Tests.Common;
using Google.Protobuf;
using MediatR;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Energinet.DataHub.PostOffice.Tests.Hosts.SubDomain
{
    [UnitTest]
    public sealed class DataAvailableAzureFunctionTests
    {
        [Fact]
        public async Task Run_ValidInput_CallsMediator()
        {
            // Arrange
            var mockedMediator = new Mock<IMediator>();
            var mockedFunctionContext = new MockedFunctionContext();

            var contractParser = new DataAvailableContractParser(new DataAvailableMapper());
            var target = new DataAvailableInbox(mockedMediator.Object, contractParser);

            var protobufMessage = CreateProtoContract();

            // Act
            await target.RunAsync(protobufMessage.ToByteArray(), mockedFunctionContext).ConfigureAwait(false);

            // Assert
            mockedMediator.Verify(mediator => mediator.Send(It.IsAny<DataAvailableNotificationCommand>(), default));
        }

        private static GreenEnergyHub.PostOffice.Communicator.Contracts.DataAvailableNotificationContract CreateProtoContract()
        {
            return new()
            {
                UUID = "SomeGuid",
                Recipient = "recipient",
                MessageType = "DataAvailable",
                Origin = "origin",
                SupportsBundling = false,
                RelativeWeight = 1,
            };
        }

        //[Fact]
        //public void Run_InvalidInput_IsHandled()
        //{
        //    // TODO: Error handling not defined.
        //}

        //[Fact]
        //public void Run_HandlerException_IsHandled()
        //{
        //    // TODO: Error handling not defined.
        //}
    }
}
