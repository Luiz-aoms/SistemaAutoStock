using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SistemaAutoStock.BancoDeDados
{
    public class Estoque
    {
        //--------------------------------
        // Propriedades
        //--------------------------------
        public int? id_peca { get; set; }
        public string? nome_peca { get; set; }
        public int? quantidade { get; set; }
        public string? status { get; set; }
        public string? material { get; set; }
        public float? peso { get; set; }
        public float? valor { get; set; }
        public string? tipo { get; set; }
        public bool? registro_ativo { get; set; }

        SqlConnection con;

        //--------------------------------
        // Construtor
        //--------------------------------
        public Estoque()
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

        public DataTable SelecionarTodos()
        {
            try
            {
                string sql = "SELECT * FROM tb_pecas";
                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(sql, con);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);

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

        public void Inserir()
        {
            try
            {
                string sql = @"INSERT INTO tb_pecas (nome_peca, quantidade, status, material, peso, valor, tipo, registro_ativo)
                               VALUES (@nome, @qtd, @status, @material, @peso, @valor, @tipo, 1)";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@nome", nome_peca ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@qtd", quantidade ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@status", status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@material", material ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@peso", peso ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@valor", valor ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@tipo", tipo ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
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

        public void Alterar()
        {
            try
            {
                string sql = @"UPDATE tb_pecas SET nome_peca = @nome, quantidade = @qtd, status = @status, 
                               material = @material, peso = @peso, valor = @valor, tipo = @tipo 
                               WHERE id_peca = @id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id_peca);
                cmd.Parameters.AddWithValue("@nome", nome_peca ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@qtd", quantidade ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@status", status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@material", material ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@peso", peso ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@valor", valor ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@tipo", tipo ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
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

        public void ExcluirParcial()
        {
            try
            {
                string sql = "UPDATE tb_pecas SET registro_ativo = 0 WHERE id_peca = @id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id_peca);

                con.Open();
                cmd.ExecuteNonQuery();
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

        public void Ativar()
        {
            try
            {
                string sql = "UPDATE tb_pecas SET registro_ativo = 1 WHERE id_peca = @id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id_peca);

                con.Open();
                cmd.ExecuteNonQuery();
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

        public void ExcluirTotal()
        {
            try
            {
                string sql = "DELETE FROM tb_pecas WHERE id_peca = @id";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id_peca);

                con.Open();
                cmd.ExecuteNonQuery();
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
    }
}