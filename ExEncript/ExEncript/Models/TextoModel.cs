using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExEncript.Models
{
    public class TextoModel
    {
        [Key]
        [Display(Name = "ID:")]
        public int Id { get; set; }
        [Display(Name = "Texto:")]
        [Required(ErrorMessage = "Digite o Texto")]
        public string Mensagem { get; set; }
    }
}