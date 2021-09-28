# Copyright 2020 Energinet DataHub A/S
#
# Licensed under the Apache License, Version 2.0 (the "License2");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
module "sbq_marketroles" {
  source              = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//service-bus-queue?ref=1.8.0"
  name                = "sbq-marketroles"
  namespace_name      = module.sbn_marketoperator.name
  resource_group_name = data.azurerm_resource_group.postoffice.name
  requires_session    = true
  dependencies        = [module.sbn_marketoperator]
}

module "sbq_marketroles_reply" {
  source              = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//service-bus-queue?ref=1.8.0"
  name                = "sbq-marketroles-reply"
  namespace_name      = module.sbn_marketoperator.name
  resource_group_name = data.azurerm_resource_group.postoffice.name
  requires_session    = true
  dependencies        = [module.sbn_marketoperator]
}