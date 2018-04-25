using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB.Models
{
    public partial class User
    {
        // this will be overwritten by codegenerator

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}
