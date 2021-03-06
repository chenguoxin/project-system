﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;

using Microsoft.VisualStudio.ProjectSystem.Properties;

using NuGet.SolutionRestoreManager;

namespace Microsoft.VisualStudio.ProjectSystem.VS.NuGet
{
    /// <summary>
    ///     Contains builder methods for creating <see cref="IVsProjectRestoreInfo"/> instances.
    /// </summary>
    internal static class RestoreBuilder
    {
        /// <summary>
        ///     Converts an immutable dictionary of rule snapshot data into an <see cref="IVsProjectRestoreInfo"/> instance.
        /// </summary>
        public static IVsProjectRestoreInfo ToProjectRestoreInfo(IImmutableDictionary<string, IProjectRuleSnapshot> update)
        {
            Requires.NotNull(update, nameof(update));

            IImmutableDictionary<string, string> properties = update[NuGetRestore.SchemaName].Properties;

            IVsTargetFrameworkInfo frameworkInfo = new TargetFrameworkInfo(
                properties[NuGetRestore.TargetFrameworkMonikerProperty],
                ToReferenceItems(update[ProjectReference.SchemaName].Items),
                ToReferenceItems(update[PackageReference.SchemaName].Items),
                ToProjectProperties(properties));

            return new ProjectRestoreInfo(
                properties[NuGetRestore.MSBuildProjectExtensionsPathProperty],
                properties[NuGetRestore.TargetFrameworksProperty],
                new TargetFrameworks(new[] { frameworkInfo }),
                ToReferenceItems(update[DotNetCliToolReference.SchemaName].Items));
        }

        private static IVsProjectProperties ToProjectProperties(IImmutableDictionary<string, string> properties)
        {
            return new ProjectProperties(properties.Select(v => new ProjectProperty(v.Key, v.Value)));
        }

        private static IVsReferenceItems ToReferenceItems(IImmutableDictionary<string, IImmutableDictionary<string, string>> items)
        {
            return new ReferenceItems(items.Select(item => ToReferenceItem(item.Key, item.Value)));
        }

        private static IVsReferenceItem ToReferenceItem(string name, IImmutableDictionary<string, string> metadata)
        {
            return new ReferenceItem(name, ToReferenceProperties(metadata));
        }

        private static IVsReferenceProperties ToReferenceProperties(IImmutableDictionary<string, string> metadata)
        {
            return new ReferenceProperties(metadata.Select(property => new ReferenceProperty(property.Key, property.Value)));
        }
    }
}
