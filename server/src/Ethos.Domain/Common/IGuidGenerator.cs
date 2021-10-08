using System;

namespace Ethos.Domain.Common
{
    public interface IGuidGenerator
    {
        /// <summary>
        /// Creates a new GUID.
        /// </summary>
        /// <returns>A new GUID.</returns>
        Guid Create();
    }
}
