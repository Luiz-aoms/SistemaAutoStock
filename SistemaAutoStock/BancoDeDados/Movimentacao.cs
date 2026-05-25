using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SistemaAutoStock.BancoDeDados
{
    public class Movimentacao
    {
        // Propriedades espelhando a tabela de movimentação
        public int? id_movi { get; set; }
        public string? tipo_movi { get; set; } // 'E' para Entrada, 'S' para Saída
        public DateTime? data_hora { get; set; }
        public int? quantidade { get; set; }
        public int? id_peca { get; set; }
        public int? quantidade_anterior { get; set; }
        public string? id_usuario { get; set; }
        public string? observacao { get; set; }

        SqlConnection con;

        //--------------------------------
        // Construtor
        //--------------------------------
        public Movimentacao()
        {
            try
            {
                IConfigurationRoot o_Config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(@".\appsettings.json")
                   .Build();

                string strConexao = o_Config.GetConnectionString(@"Default");
                con = new SqlConnection(strConexao);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //--------------------------------
        // Métodos
        //--------------------------------

        public void Registrar()
        {
            try
            {
                con.Open();

                // Inicia a transação: garante que as duas tabelas sejam atualizadas juntas
                using (SqlTransaction transacao = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Descobrir a quantidade atual da peça antes de movimentar
                        int qtdAnterior = 0;
                        string sqlBusca = "SELECT quantidade FROM tb_pecas WHERE id_peca = @IdPeca";

                        using (SqlCommand cmdBusca = new SqlCommand(sqlBusca, con, transacao))
                        {
                            cmdBusca.Parameters.AddWithValue("@IdPeca", id_peca);
                            object result = cmdBusca.ExecuteScalar();

                            if (result != null && result != DBNull.Value)
                                qtdAnterior = Convert.ToInt32(result);
                        }

                        // Alimenta a propriedade para ser usada no INSERT logo abaixo
                        quantidade_anterior = qtdAnterior;

                        // 2. MATEMÁTICA: Atualizar o estoque na tabela principal (tb_pecas)
                        string sqlAtualizaEstoque = "";

                        if (tipo_movi == "E")
                        {
                            sqlAtualizaEstoque = "UPDATE tb_pecas SET quantidade = quantidade + @Quantidade WHERE id_peca = @IdPeca";
                        }
                        else if (tipo_movi == "S")
                        {
                            sqlAtualizaEstoque = "UPDATE tb_pecas SET quantidade = quantidade - @Quantidade WHERE id_peca = @IdPeca";
                        }

                        using (SqlCommand cmdEstoque = new SqlCommand(sqlAtualizaEstoque, con, transacao))
                        {
                            cmdEstoque.Parameters.AddWithValue("@Quantidade", quantidade);
                            cmdEstoque.Parameters.AddWithValue("@IdPeca", id_peca);
                            cmdEstoque.ExecuteNonQuery();
                        }

                        // 3. Inserir o registro na tabela de movimentação
                        string sqlInsert = @"INSERT INTO tb_movimentacao 
                            (tipo_movi, data_hora, quantidade, id_peca, quantidade_anterior, id_usuario, observacao) 
                            VALUES (@TipoMovi, GETDATE(), @Quantidade, @IdPeca, @QtdAnterior, @IdUsuario, @Observacao)";

                        using (SqlCommand cmdInsert = new SqlCommand(sqlInsert, con, transacao))
                        {
                            cmdInsert.Parameters.AddWithValue("@TipoMovi", tipo_movi);
                            cmdInsert.Parameters.AddWithValue("@Quantidade", quantidade);
                            cmdInsert.Parameters.AddWithValue("@IdPeca", id_peca);
                            cmdInsert.Parameters.AddWithValue("@QtdAnterior", quantidade_anterior);
                            cmdInsert.Parameters.AddWithValue("@IdUsuario", id_usuario);

                            // Campos que podem ser nulos
                            cmdInsert.Parameters.AddWithValue("@Observacao", string.IsNullOrEmpty(observacao) ? (object)DBNull.Value : observacao);

                            cmdInsert.ExecuteNonQuery();
                        }

                        // Se tudo deu certo, efetiva as mudanças no banco!
                        transacao.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Se deu qualquer erro no caminho, desfaz tudo
                        transacao.Rollback();
                        throw new Exception("Erro durante a transação: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                // Garante que a conexão será fechada independente de erro ou sucesso
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
        public DataTable SelecionarTodos()
        {
            try
            {
                // Trazemos os dados da movimentação e o nome da peça usando INNER JOIN
                string cmdSQL = @"
                    SELECT 
                    m.id_movi, 
                    m.tipo_movi, 
                    m.data_hora, 
                    m.quantidade, 
                    m.quantidade_anterior, 
                    m.observacao,   
                    p.nome_peca,
                    u.UserName AS nome_usuario -- Pegando o nome do usuário do Identity
                    FROM tb_movimentacao m
                    INNER JOIN tb_pecas p ON m.id_peca = p.id_peca
                    LEFT JOIN AspNetUsers u ON m.id_usuario = u.Id
                    ORDER BY m.data_hora DESC";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                return dtPesquisa;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }


        public DataTable SelecionarHistoricoPorPeca()
        {
            try
            {
                // Busca o extrato de movimentações de uma peça específica, da mais recente pra mais antiga
                string cmdSQL = "SELECT * FROM tb_movimentacao WHERE id_peca = @IdPeca ORDER BY data_hora DESC";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@IdPeca", id_peca);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                int qtdLinhasAfetadas = o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                if (qtdLinhasAfetadas > 0) return dtPesquisa;
                else return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}