namespace SistemaAutoStock.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalItens { get; set; }
        public float QtdTotal { get; set; }
        public string? ValorTotal { get; set; }
        public int BaixoEstoque { get; set; }

        public int EstoqueMedio { get; set; }
        public int EstoqueBom { get; set; }

        // Dados para o gráfico de QUANTIDADE (Azul)
        public string? NomesParaGrafico { get; set; }
        public string? ValoresParaGrafico { get; set; }

        // Dados para o gráfico de VALOR PATRIMONIAL (Verde)
        public string? NomesParaGraficoValor { get; set; }
        public string ValoresTotaisParaGrafico { get; set; }
    }
}