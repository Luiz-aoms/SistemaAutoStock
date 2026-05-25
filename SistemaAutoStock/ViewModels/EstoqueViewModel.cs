using System.ComponentModel.DataAnnotations;

namespace SistemaAutoStock.ViewModels
{
    public class EstoqueViewModel
    {
        public int? IdPeca { get; set; }

        [Required(ErrorMessage = "Digite o NOME da peça/kit.")]
        public string? NomePeca { get; set; }

        [Required(ErrorMessage = "Digite a QUANTIDADE.")]
        public int? Quantidade { get; set; }

        [Required(ErrorMessage = "Digite o STATUS. ex: Ativo/Inativo")]
        public string? Status { get; set; }

        [Required(ErrorMessage = "Digite o MATERIAL da Peça/Kit.")]
        public string? Material { get; set; }

        [Required(ErrorMessage = "Digite o PESO da Peça/Kit.")]
        public float? Peso { get; set; }

        [Required(ErrorMessage = "Digite o VALOR da Peça/Kit.")]
        public float? Valor { get; set; }

        [Required(ErrorMessage = "Informe o Tipo. ex: peça, kit...")]
        public string? Tipo { get; set; }

        public bool RegistroAtivo { get; set; } = true;
    }
}