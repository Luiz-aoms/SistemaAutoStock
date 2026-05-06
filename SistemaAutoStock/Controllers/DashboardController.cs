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
        public IActionResult Index()
        {
            Estoque o_Estoque = new Estoque();
            DataTable dt = o_Estoque.SelecionarTodos();

            float qtdAcumulada = 0;
            float valorAcumulado = 0;
            int critico = 0;
            int medio = 0;
            int bom = 0;

            // 1. Criamos uma lista temporária para guardar TODOS os itens e podermos ordená-los
            var todosOsItens = new List<(string Nome, float Quantidade, float ValorTotal)>();

            foreach (DataRow row in dt.Rows)
            {
                float q = row["quantidade"] != DBNull.Value ? Convert.ToSingle(row["quantidade"]) : 0;
                float v = row["valor"] != DBNull.Value ? Convert.ToSingle(row["valor"]) : 0;

                qtdAcumulada += q;
                valorAcumulado += (q * v);

                if (q <= 5) critico++; else if (q <= 10) medio++; else bom++;

                string nome = row["nome_peca"]?.ToString() ?? "Sem Nome";

                // Guardamos os dados desta peça na nossa lista temporária
                todosOsItens.Add((nome, q, q * v));
            }

            // Ordenamos pela Quantidade (do maior para o menor) e pegamos os 5 primeiros
            var top5Itens = todosOsItens.OrderByDescending(item => item.Quantidade).Take(5).ToList();

            var viewModel = new DashboardViewModel
            {
                TotalItens = dt.Rows.Count,
                QtdTotal = qtdAcumulada,
                ValorTotal = valorAcumulado.ToString("C2", new System.Globalization.CultureInfo("pt-BR")),
                BaixoEstoque = critico,
                EstoqueMedio = medio,
                EstoqueBom = bom,

                // 3. Extraímos apenas os nomes, quantidades e valores já ordenados!
                NomesParaGrafico = string.Join(",", top5Itens.Select(x => $"'{x.Nome}'")),
                ValoresParaGrafico = string.Join(",", top5Itens.Select(x => x.Quantidade.ToString(System.Globalization.CultureInfo.InvariantCulture))),
                ValoresTotaisParaGrafico = string.Join(",", top5Itens.Select(x => x.ValorTotal.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)))
            };

            return View(viewModel);
        }
    }
}