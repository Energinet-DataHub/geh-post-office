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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Energinet.DataHub.PostOffice.Common.Extensions
{
    internal static class ServiceCollectionValidationExtension
    {
        private static readonly Type _ruleSetDefinitionType = typeof(RuleCollection<>);
        private static readonly Type _propertyRuleType = typeof(PropertyRule<>);

        internal static IServiceCollection DiscoverValidation(this IServiceCollection serviceCollection, Assembly[] targetAssemblies)
        {
            if (serviceCollection is null)
                throw new ArgumentNullException(nameof(serviceCollection));
            if (targetAssemblies is null)
                throw new ArgumentNullException(nameof(targetAssemblies));

            var allTypes = targetAssemblies.SelectMany(a => a.GetTypes());

            foreach (var type in allTypes)
            {
                // Check if the type is a ruleset and add it
                if (TryGetRuleSetDefinition(type, out var ruleSetServiceDescriptor)) serviceCollection.Add(ruleSetServiceDescriptor);

                // Check if the type is a ruleset and configure the rule engine to support it
                if (TryGetRuleEngineServiceDescriptor(type, out var ruleEngineServiceDescriptor)) serviceCollection.Add(ruleEngineServiceDescriptor);

                // Check if the type is a property rule and add it
                if (TryGetPropertyRuleServiceDescriptor(type, out var propertyRuleServiceDescriptor)) serviceCollection.Add(propertyRuleServiceDescriptor);
            }

            // Add our delegate as a singleton
            serviceCollection.AddSingleton<ServiceProviderDelegate>(sp => sp.GetService!);

            return serviceCollection;
        }

        private static bool TryGetRuleSetDefinition(Type type, [NotNullWhen(true)] out ServiceDescriptor? serviceDescriptor)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            serviceDescriptor = GetServiceDescriptor(type, _ruleSetDefinitionType, CreateSingletonGeneric);

            return serviceDescriptor != null;
        }

        private static bool TryGetPropertyRuleServiceDescriptor(Type type, [NotNullWhen(true)] out ServiceDescriptor? serviceDescriptor)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            serviceDescriptor = GetServiceDescriptor(type, _propertyRuleType, CreateSingletonImplementationType);

            return serviceDescriptor != null;
        }

        private static bool TryGetRuleEngineServiceDescriptor(Type type, [NotNullWhen(true)] out ServiceDescriptor? serviceDescriptor)
        {
            static ServiceDescriptor Creator(Type baseType, Type messageType, Type implementationType)
            {
                return ServiceDescriptor.Singleton(typeof(IRuleEngine<>).MakeGenericType(messageType), typeof(FluentHybridRuleEngine<>).MakeGenericType(messageType));
            }

            if (type is null)
                throw new ArgumentNullException(nameof(type));
            serviceDescriptor = GetServiceDescriptor(type, _ruleSetDefinitionType, Creator);

            return serviceDescriptor != null;
        }

        private static ServiceDescriptor? GetServiceDescriptor(Type type, Type baseType, Func<Type, Type, Type, ServiceDescriptor> creator)
        {
            var baseTypeIsGenericType = type.BaseType?.IsGenericType ?? false;
            if (!baseTypeIsGenericType)
                return null;

            var isRuleSetDefinition = type.BaseType?.GetGenericTypeDefinition().IsAssignableFrom(baseType) ?? false;
            if (!isRuleSetDefinition)
                return null;

            var genericType = type.BaseType?.GetGenericArguments().FirstOrDefault();
            return genericType is null
                ? null
                : creator(baseType, genericType, type);
        }

        private static ServiceDescriptor CreateSingletonGeneric(Type baseType, Type genericType, Type implementationType)
        {
            var serviceType = baseType.MakeGenericType(genericType);
            return ServiceDescriptor.Singleton(serviceType, implementationType);
        }

        private static ServiceDescriptor CreateSingletonImplementationType(Type baseType, Type genericType, Type implementationType)
        {
            return ServiceDescriptor.Singleton(implementationType, implementationType);
        }
    }
}
