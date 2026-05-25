using Microsoft.AspNetCore.Mvc;
using SistemaAutoStock.BancoDeDados;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using SistemaAutoStock.ViewModels;
using System.Linq;
using System.Collections.Generic;

namespace SistemaAutoStock.Controllers
{
    [Authorize(Roles = "Coordenador")]
    public class DashboardController : Controller
    {
        [Route("dashboard")]
        public IActionResult Index()
        {
            Estoque o_Estoque = new Estoque();
            DataTable dt = o_Estoque.SelecionarTodos();

            float qtdAcumulada = 0;
            float valorAcumulado = 0;
            int critico = 0, medio = 0, bom = 0;

            var todosOsItens = new List<(string Nome, float Quantidade, float ValorTotal)>();

            foreach (DataRow row in dt.Rows)
            {
                float q = row["quantidade"] != DBNull.Value ? Convert.ToSingle(row["quantidade"]) : 0;
                float v = row["valor"] != DBNull.Value ? Convert.ToSingle(row["valor"]) : 0;
                float totalItem = q * v;

                qtdAcumulada += q;
                valorAcumulado += totalItem;

                if (q <= 5) critico++; else if (q <= 10) medio++; else bom++;

                string nome = row["nome_peca"]?.ToString() ?? "Sem Nome";
                todosOsItens.Add((nome, q, totalItem));
            }

            var top5Qtd = todosOsItens.OrderByDescending(x => x.Quantidade).Take(5).ToList();

            var top5Valor = todosOsItens.OrderByDescending(x => x.ValorTotal).Take(5).ToList();

            var viewModel = new DashboardViewModel
            {
                TotalItens = dt.Rows.Count,
                QtdTotal = qtdAcumulada,
                ValorTotal = valorAcumulado.ToString("C2", new System.Globalization.CultureInfo("pt-BR")),
                BaixoEstoque = critico,
                EstoqueMedio = medio,
                EstoqueBom = bom,

                NomesParaGrafico = string.Join(",", top5Qtd.Select(x => $"'{x.Nome}'")),
                ValoresParaGrafico = string.Join(",", top5Qtd.Select(x => x.Quantidade.ToString(System.Globalization.CultureInfo.InvariantCulture))),

                NomesParaGraficoValor = string.Join(",", top5Valor.Select(x => $"'{x.Nome}'")),
                ValoresTotaisParaGrafico = string.Join(",", top5Valor.Select(x => x.ValorTotal.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)))
            };

            return View(viewModel);
        }
    }
}