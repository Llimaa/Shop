using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres.")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres.")]
        public string Title { get; set; }

        [MaxLength(1024,ErrorMessage ="O campo deve conter no maximo 2024 Caractéres")]
        public string Description { get; set; }

        [Required(ErrorMessage ="Este campo é obrigátorio")]
        [Range(1, int.MaxValue, ErrorMessage ="O Campo deve ser maior que 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Este campo é obrigátorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Valor invalido")]
        public int CategoryId { get; set; }
            
        public Category Category { get; set; }
    }
}
