using System;
using System.Collections.Generic;

namespace Ethos.Query.Projections
{
    public class UserProjection : IProjection
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public List<string> Roles { get; set; }
    }
}
