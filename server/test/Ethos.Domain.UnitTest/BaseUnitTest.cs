using System;
using Ethos.Domain.Common;

namespace Ethos.Domain.UnitTest
{
    public abstract class BaseUnitTest
    {
        protected IGuidGenerator GuidGenerator { get; }

        protected BaseUnitTest()
        {
            GuidGenerator = new TestGuidGenerator();
        }

        private class TestGuidGenerator : IGuidGenerator
        {
            public Guid Create()
            {
                return Guid.NewGuid();
            }
        }
    }
}
