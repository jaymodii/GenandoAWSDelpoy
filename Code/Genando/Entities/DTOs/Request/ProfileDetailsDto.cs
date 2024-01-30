using Common.Enums;
using Entities.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Request
{
    public class ProfileDetailsDto : BaseValidationModel<ProfileDetailsDto>
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Headline { get; set; }

        public DateTimeOffset? DOB { get; set; }

        public GenderType? Gender { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Address { get; set; } = null!;

        public IFormFile? Avatar { get; set; }

    }
}
