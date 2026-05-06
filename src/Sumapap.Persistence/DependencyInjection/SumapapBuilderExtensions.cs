using System;
using System.Collections.Generic;
using System.Text;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.Persistence.DependencyInjection
{
    public static class SumapapBuilderExtensions
    {
        extension(ISumapapBuilder builder)
        {
            /// <summary>
            /// Adds persistence services to the Sumapap builder.
            /// </summary>
            /// <param name="builder">The Sumapap builder.</param>
            /// <returns>The same builder for chaining.</returns>
            public ISumapapBuilder WithRepositories()
            {
                // This method is intentionally left blank.
                // It serves as an entry point for persistence-related registrations
                // which are handled by extension methods in PersistenceBuilderExtensions.
                return builder;
            }
        }
    }
}