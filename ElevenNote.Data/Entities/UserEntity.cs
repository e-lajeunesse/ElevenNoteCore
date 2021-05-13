using System;
using System.ComponentModel.DataAnnotations;

namespace ElevenNote.Data.Entities
{
    public class UserEntity
    {
        [Key]
        public int Id {get;set;}

        [Required]
        public string Email {get;set;}

        [Required]
        public string UserName {get;set;}

        [Required]
        public string Password {get;set;}
        public string FirstName {get;set;}
        public string LastName {get;set;}

        [Required]
        public DateTime DateCreated {get;set;}
        
    }
}