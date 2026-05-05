using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System;
using SistemaAutoStock.BancoDeDados;
using SistemaAutoStock.ViewModels;

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

                // Se por acaso o banco ainda mandar null, a gente garante um objeto vazio aqui
                return View("SelecionarView", dtPecas ?? new DataTable());
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro: {ex.Message}";
                // SEMPRE passe um novo DataTable no erro para a View não quebrar
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

                    // 1. Mapeia os campos básicos
                    MapearVMparaModel(o_EstoqueVM, o_Estoque);

                    // 2. Garante o ID para a atualização
                    o_Estoque.id_peca = o_EstoqueVM.IdPeca;

                    // 3. Atribuição direta (Sem Convert ou Parse)
                    // Se o valor for nulo na VM, ele salva como 0 no banco
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
        public IActionResult ExcluirProcessar(EstoqueViewModel o_EstoqueVM)
        {
            try
            {
                Estoque o_Estoque = new Estoque();
                o_Estoque.id_peca = o_EstoqueVM.IdPeca;
                o_Estoque.Excluir();
                TempData["MsgSucesso"] = "Item removido com sucesso!";
            }
            catch (Exception ex) { TempData["MsgErro"] = ex.Message; }
            return RedirectToAction("Selecionar");
        }

        // Método auxiliar para evitar repetição de código
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