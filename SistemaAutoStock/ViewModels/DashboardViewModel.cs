namespace SistemaAutoStock.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalItens { get; set; }
        public float QtdTotal { get; set; }
        public string? ValorTotal { get; set; }

        // Grafico de Nivel de Estoque - Quantidade
        public int BaixoEstoque { get; set; }
        public int EstoqueMedio { get; set; }
        public int EstoqueBom { get; set; }

        // Grafico de quantidade x peça
        public string? NomesParaGrafico { get; set; }
        public string? ValoresParaGrafico { get; set; }

        // Grafico por valor x peça
        public string? NomesParaGraficoValor { get; set; }
        public string ValoresTotaisParaGrafico { get; set; }
    }
}