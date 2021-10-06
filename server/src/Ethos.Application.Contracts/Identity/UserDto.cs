using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Identity
{
    public class UserDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public Guid Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public IEnumerable<string> Roles { get; set; }
    }
}
