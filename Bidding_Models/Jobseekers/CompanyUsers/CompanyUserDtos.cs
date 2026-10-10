using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.Jobseekers.CompanyUsers
{
    public class CompanyUserListItemDto
    {
        public int ProfileId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CompanyUsersPageDto
    {
        public System.Collections.Generic.List<CompanyUserListItemDto> Rows { get; set; }
            = new System.Collections.Generic.List<CompanyUserListItemDto>();
        public int Total { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class CompanyUserDistributableRoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsTemplate { get; set; }
        public int? UserGroupId { get; set; }
        public string UserGroupCode { get; set; }
        public string UserGroupNameEn { get; set; }
        public string UserGroupNameAr { get; set; }
    }

    public class RegisterCompanyUserRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(32)]
        public string Mobile { get; set; }

        /// <summary>Identity Access role to distribute (template for linked groups, or org custom role).</summary>
        [Required]
        [MaxLength(450)]
        public string RoleId { get; set; }
    }

    public class RegisterCompanyUserResultDto
    {
        public int ProfileId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
    }

    public class UpdateCompanyUserRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(32)]
        public string Mobile { get; set; }

        [Required]
        [MaxLength(450)]
        public string RoleId { get; set; }
    }
}
