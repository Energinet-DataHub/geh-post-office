module "sbn_inbound" {
  source              = "git::https://github.com/Energinet-DataHub/green-energy-hub-core.git//terraform/modules/service-bus-namespace?ref=1.0.0"
  name                = "sbn-inbound-${var.project}-${var.organisation}-${var.environment}"
  resource_group_name = data.azurerm_resource_group.postoffice.name
  location            = data.azurerm_resource_group.postoffice.location
  sku                 = "basic"
  tags                = data.azurerm_resource_group.postoffice.tags
}

module "sbq_inbound" {
  source              = "git::https://github.com/Energinet-DataHub/green-energy-hub-core.git//terraform/modules/service-bus-queue?ref=1.0.0"
  name                = "sbq-inbound"
  namespace_name      = module.sbn_inbound.name
  resource_group_name = data.azurerm_resource_group.postoffice.name
  dependencies        = [module.sbn_inbound]
}

module "sbnar_inbound_listener" {
  source                    = "git::https://github.com/Energinet-DataHub/green-energy-hub-core.git//terraform/modules/service-bus-auth-rule?ref=1.0.0"
  name                      = "sbnar-inbound-listener"
  namespace_name            = module.sbn_inbound.name
  queue_name                = module.sbq_inbound.name
  resource_group_name       = data.azurerm_resource_group.postoffice.name
  listen                    = true
  dependencies              = [module.sbq_inbound]
}

module "sbnar_inbound_sender" {
  source                    = "git::https://github.com/Energinet-DataHub/green-energy-hub-core.git//terraform/modules/service-bus-auth-rule?ref=1.0.0"
  name                      = "sbnar-inbound-sender"
  namespace_name            = module.sbn_inbound.name
  queue_name                = module.sbq_inbound.name
  resource_group_name       = data.azurerm_resource_group.postoffice.name
  send                      = true
  dependencies              = [module.sbq_inbound]
}