using Microsoft.AspNetCore.Mvc;
using SistemaAutoStock.BancoDeDados;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using SistemaAutoStock.ViewModels;
using System.Linq; // Adicione isso para o Select funcionar sem erro

namespace SistemaAutoStock.Controllers
{
    [Authorize(Roles = "Coordenador")]
    public class DashboardController : Controller
    {
        // Removi o [Route("dashboard")] para não conflitar com o padrão do ASP.NET
        public IActionResult Index()
        {
            Estoque o_Estoque = new Estoque();
            DataTable dt = o_Estoque.SelecionarTodos();

            float qtdAcumulada = 0;
            float valorAcumulado = 0;
            int critico = 0;
            int bom = 0;
            List<string> nomes = new List<string>();
            List<float> quantidades = new List<float>();

            foreach (DataRow row in dt.Rows)
            {
                // Tratamento de segurança para evitar erros se o banco vier nulo
                float q = row["quantidade"] != DBNull.Value ? Convert.ToSingle(row["quantidade"]) : 0;
                float v = row["valor"] != DBNull.Value ? Convert.ToSingle(row["valor"]) : 0;

                qtdAcumulada += q;
                valorAcumulado += (q * v);

                if (q <= 5) critico++; else bom++;

                if (nomes.Count < 5)
                {
                    nomes.Add(row["nome_peca"]?.ToString() ?? "Sem Nome");
                    quantidades.Add(q);
                }
            }

            var viewModel = new DashboardViewModel
            {
                TotalItens = dt.Rows.Count,
                QtdTotal = qtdAcumulada,
                ValorTotal = valorAcumulado.ToString("C2", new System.Globalization.CultureInfo("pt-BR")),
                BaixoEstoque = critico,
                EstoqueBom = bom,
                // O .Select aqui precisa do "using System.Linq;" lá no topo
                NomesParaGrafico = string.Join(",", nomes.Select(n => $"'{n}'")),
                ValoresParaGrafico = string.Join(",", quantidades)
            };

            return View(viewModel);
        }
    }
}