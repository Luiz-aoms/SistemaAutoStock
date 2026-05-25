using System.ComponentModel.DataAnnotations;

namespace SistemaAutoStock.ViewModels
{
    public class MovimentacaoViewModel
    {
        [Required(ErrorMessage = "O ID da peça é obrigatório.")]
        public int IdPeca { get; set; }

        [Required(ErrorMessage = "Selecione o tipo de movimentação (Entrada ou Saída).")]
        public string TipoMovimentacao { get; set; } 

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(1, 9999, ErrorMessage = "A quantidade deve ser de pelo menos 1 unidade.")]
        public int Quantidade { get; set; }

        public string Observacao { get; set; }
    }
}