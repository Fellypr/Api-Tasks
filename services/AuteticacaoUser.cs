using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ListTaskApi.services
{
    public class AuteticacaoUserProp
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "O nome do usuário é obrigatório")]
        [MinLength(3, ErrorMessage = "O nome do usuário deve ter no mínimo 3 caracteres")]
        [JsonPropertyName("name")]
        public string NameUser { get; set; }
    }
    public class Login
    {
        [Required(ErrorMessage = "O email do usuário é obrigatório")]
        [JsonPropertyName("emailDoUsuario")]
        public string EmailDoUsuario { get; set; }
        [Required(ErrorMessage = "A senha do usuário é obrigatória")]
        [JsonPropertyName("senhaDoUsuario")]
        public string SenhaDoUsuario { get; set; }
    }
}