using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WEB.Models
{
    public class UserDTO
    {
        [Required]
        public Guid Id { get; set; }

        // this will be overwritten by codegenerator

        public string Email { get; set; }

        public IList<Guid> RoleIds { get; set; }

    }

    public partial class ModelFactory
    {
        public UserDTO Create(User user)
        {
            if (user == null) return null;

            var roleIds = new List<Guid>();
            foreach (var role in user.Roles)
                roleIds.Add(role.RoleId);

            var userDTO = new UserDTO();

            userDTO.Id = user.Id;
            userDTO.Email = user.Email;
            userDTO.RoleIds = roleIds;

            return userDTO;
        }

        public void Hydrate(User user, UserDTO userDTO)
        {
            user.UserName = userDTO.Email;
            user.Email = userDTO.Email;
        }
    }
}
