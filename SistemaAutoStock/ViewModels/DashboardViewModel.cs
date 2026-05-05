namespace SistemaAutoStock.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalItens { get; set; }
        public float QtdTotal { get; set; }
        public string? ValorTotal { get; set; }
        public int BaixoEstoque { get; set; }
        public int EstoqueBom { get; set; }

        // Dados formatados para o gráfico
        public string? NomesParaGrafico { get; set; }
        public string? ValoresParaGrafico { get; set; }
    }
}
