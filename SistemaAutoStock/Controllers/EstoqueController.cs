using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System;
using SistemaAutoStock.BancoDeDados;
using SistemaAutoStock.ViewModels;
using System.Security.Claims;
using ClosedXML.Excel;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SistemaAutoStock.Controllers  
{
    [Authorize(Roles = "Professor, Coordenador")]
    [Route("estoque")]
    public class EstoqueController : Controller
    {
        [HttpGet("")]
        public IActionResult Selecionar()
        {
            try
            {
                Estoque o_Estoque = new Estoque();
                DataTable dtPecas = o_Estoque.SelecionarTodos();

                return View("SelecionarView", dtPecas ?? new DataTable());
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                return View("SelecionarView", new DataTable());
            }
        }

        [HttpPost("novo")]
        public IActionResult InserirProcessar(EstoqueViewModel o_EstoqueVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Estoque o_Estoque = new Estoque();
                    MapearVMparaModel(o_EstoqueVM, o_Estoque);
                    o_Estoque.Inserir();
                    TempData["MsgSucesso"] = "Item adicionado ao estoque!";
                }
            }
            catch (Exception ex) { TempData["MsgErro"] = ex.Message; }
            return RedirectToAction("Selecionar");
        }

        [HttpPost("editar")]
        public IActionResult AlterarProcessar(EstoqueViewModel o_EstoqueVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Estoque o_Estoque = new Estoque();

                    MapearVMparaModel(o_EstoqueVM, o_Estoque);

                    o_Estoque.id_peca = o_EstoqueVM.IdPeca;

                    o_Estoque.peso = o_EstoqueVM.Peso ?? 0;
                    o_Estoque.valor = o_EstoqueVM.Valor ?? 0;

                    o_Estoque.Alterar();
                    TempData["MsgSucesso"] = "Item atualizado!";
                }
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = "Erro ao salvar: " + ex.Message;
            }   
            return RedirectToAction("Selecionar");
        }

        [HttpPost("excluir")]
        public IActionResult ExcluirTProcessar(EstoqueViewModel o_EstoqueVM)
        {
            try
            {
                Estoque o_Estoque = new Estoque();
                o_Estoque.id_peca = o_EstoqueVM.IdPeca;
                o_Estoque.ExcluirTotal();
                TempData["MsgSucesso"] = "Item removido com sucesso!";
            }
            catch (Exception ex) { TempData["MsgErro"] = "Erro ao excluir peça!"; }
            return RedirectToAction("Selecionar");
        }

        [HttpPost("excluir-parcial")]
        public IActionResult ExcluirParcialProcessar(EstoqueViewModel o_EstoqueVM)
        {
            try
            {
                Estoque o_Estoque = new Estoque();
                o_Estoque.id_peca = o_EstoqueVM.IdPeca;
                o_Estoque.ExcluirParcial();
                TempData["MsgSucesso"] = "Item desativado com sucesso!";
            }
            catch (Exception ex) { TempData["MsgErro"] = ex.Message; }
            return RedirectToAction("Selecionar");
        }

        [HttpPost("ativar")]
        public IActionResult AtivarProcessar(EstoqueViewModel o_EstoqueVM)
        {
            try
            {
                Estoque o_Estoque = new Estoque();
                o_Estoque.id_peca = o_EstoqueVM.IdPeca;
                o_Estoque.Ativar();
                TempData["MsgSucesso"] = "Item reativado com sucesso!";
            }
            catch (Exception ex) { TempData["MsgErro"] = ex.Message; }
            return RedirectToAction("Selecionar");
        }

        [HttpPost("movimentar")]
        public IActionResult MovimentarProcessar(MovimentacaoViewModel o_MovimentacaoVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Instanciamos o DAO logo no começo para poder usar os métodos dele
                    Movimentacao o_Movimentacao = new Movimentacao();

                    // =========================================================================
                    // NOVA VALIDAÇÃO: Bloquear saída maior que o estoque
                    // =========================================================================

                    // 1. Busca a quantidade atual da peça no banco de dados
                    int estoqueAtual = o_Movimentacao.ObterQuantidadeAtualDaPeca(o_MovimentacaoVM.IdPeca);

                    // 2. Verifica se é saída ("S") e se a pessoa pediu mais do que tem
                    // Usei "S" porque vi no seu DAO que é assim que você salva a saída no banco.
                    if (o_MovimentacaoVM.TipoMovimentacao == "S")
                    {
                        if (o_MovimentacaoVM.Quantidade > estoqueAtual)
                        {
                            TempData["MsgErro"] = $"Estoque insuficiente! Você tentou retirar {o_MovimentacaoVM.Quantidade}, mas há apenas {estoqueAtual} no estoque.";
                            return RedirectToAction("Selecionar"); // Devolve para a tela sem salvar nada
                        }
                    }
                    // =========================================================================

                    // Se passou pela validação, preenche os dados e registra no banco
                    o_Movimentacao.id_peca = o_MovimentacaoVM.IdPeca;
                    o_Movimentacao.tipo_movi = o_MovimentacaoVM.TipoMovimentacao;
                    o_Movimentacao.quantidade = o_MovimentacaoVM.Quantidade;
                    o_Movimentacao.observacao = o_MovimentacaoVM.Observacao;

                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    o_Movimentacao.id_usuario = userId;

                    o_Movimentacao.Registrar();

                    TempData["MsgSucesso"] = "Movimentação realizada e estoque atualizado com sucesso!";
                }
                else
                {
                    TempData["MsgErro"] = "Dados informados estão inválidos para efetuar a movimentação.";
                }
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao processar movimentação: {ex.Message}";
            }

            return RedirectToAction("Selecionar");
        }

        [HttpGet("historico")]
        [Authorize(Roles = "Coordenador")]

        public IActionResult Historico()
        {
            try
            {
                Movimentacao o_Movimentacao = new Movimentacao();
                DataTable dtHistorico = o_Movimentacao.SelecionarTodos();

                return View(dtHistorico ?? new DataTable());
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao carregar o histórico: {ex.Message}";
                return View(new DataTable());
            }
        }

        [HttpGet("exportar-historico")]
        public IActionResult ExportarHistoricoExcel()
        {
            try
            {
                Movimentacao o_Movimentacao = new Movimentacao();
                DataTable dt = o_Movimentacao.SelecionarTodos();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Historico de Movimentacoes");

                    worksheet.Cell(1, 1).Value = "Data e Hora";
                    worksheet.Cell(1, 2).Value = "Usuário";
                    worksheet.Cell(1, 3).Value = "Peça";
                    worksheet.Cell(1, 4).Value = "Tipo";
                    worksheet.Cell(1, 5).Value = "Qtd. Anterior";
                    worksheet.Cell(1, 6).Value = "Movimentado";
                    worksheet.Cell(1, 7).Value = "Saldo Final";
                    worksheet.Cell(1, 8).Value = "Observação";

                    var headerRange = worksheet.Range("A1:H1");
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                    int linha = 2;
                    foreach (DataRow row in dt.Rows)
                    {
                        string tipo = row["tipo_movi"].ToString() == "E" ? "Entrada" : "Saída";
                        int qtdAnterior = Convert.ToInt32(row["quantidade_anterior"]);
                        int qtdMovimentada = Convert.ToInt32(row["quantidade"]);
                        int saldoFinal = row["tipo_movi"].ToString() == "E" ? (qtdAnterior + qtdMovimentada) : (qtdAnterior - qtdMovimentada);

                        worksheet.Cell(linha, 1).Value = Convert.ToDateTime(row["data_hora"]).ToString("dd/MM/yyyy HH:mm");
                        worksheet.Cell(linha, 2).Value = row["nome_usuario"]?.ToString();
                        worksheet.Cell(linha, 3).Value = row["nome_peca"].ToString();
                        worksheet.Cell(linha, 4).Value = tipo;
                        worksheet.Cell(linha, 5).Value = qtdAnterior;
                        worksheet.Cell(linha, 6).Value = qtdMovimentada;
                        worksheet.Cell(linha, 7).Value = saldoFinal;
                        worksheet.Cell(linha, 8).Value = row["observacao"]?.ToString();

                        linha++;
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Historico_Estoque_{DateTime.Now:dd-MM-yyyy}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao exportar arquivo: {ex.Message}";
                return RedirectToAction("Historico");
            }
        }

        private void MapearVMparaModel(EstoqueViewModel vm, Estoque model)
        {
            model.nome_peca = vm.NomePeca;
            model.quantidade = vm.Quantidade;
            model.status = vm.Status;
            model.material = vm.Material;
            model.peso = vm.Peso;
            model.valor = vm.Valor;
            model.tipo = vm.Tipo;
        }

        [HttpGet("recomendacoes")]
        public IActionResult Recomendacoes()
        {
            return View();
        }
    }
}