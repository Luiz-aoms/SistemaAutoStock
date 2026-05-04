using System.Data;
using System.Data.SqlClient;

namespace SistemaAutoStock.BancoDeDados
{
    public class Estoque
    {
        public int? id_peca { get; set; }
        public string? nome_peca { get; set; }
        public int? quantidade { get; set; }
        public string? status { get; set; }
        public string? material { get; set; }
        public float? peso { get; set; }
        public float? valor { get; set; }
        public string? tipo { get; set; }

        SqlConnection con;

        //--------------------------------
        // Construtor
        //--------------------------------
        public Estoque()
        {
            try
            {
                // Ler o arquivo de config
                IConfigurationRoot o_Config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(@".\appsettings.json")
                   .Build();

                string strConexao = o_Config.GetConnectionString(@"Default");

                // Prepara a conexão com o BD
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

        public void Inserir()
        {
            try
            {
                string cmdSQL = "Insert Into tb_pecas " +
                    "(nome_peca, quantidade, status, material, peso, valor, tipo) " +
                    "Values(@NomePeca, @Quantidade, @Status, @Material, @Peso, @Valor, @Tipo)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@NomePeca", nome_peca);
                cmd.Parameters.AddWithValue("@Quantidade", quantidade);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Material", material);
                cmd.Parameters.AddWithValue("@Peso", peso);
                cmd.Parameters.AddWithValue("@Valor", valor);
                cmd.Parameters.AddWithValue("@Tipo", tipo);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Alterar()
        {
            try
            {
                // Prepara o comando SQL para atualizar a peça pelo ID
                string cmdSQL = "Update tb_pecas Set nome_peca = @NomePeca, quantidade = @Quantidade, " +
                                "status = @Status, material = @Material, peso = @Peso, valor = @Valor, tipo = @Tipo " +
                                "Where id_peca = @IdPeca";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                // Passa os parâmetros da classe para o SQL
                cmd.Parameters.AddWithValue("@IdPeca", id_peca);
                cmd.Parameters.AddWithValue("@NomePeca", nome_peca);
                cmd.Parameters.AddWithValue("@Quantidade", quantidade);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Material", material);
                cmd.Parameters.AddWithValue("@Peso", peso);
                cmd.Parameters.AddWithValue("@Valor", valor);
                cmd.Parameters.AddWithValue("@Tipo", tipo);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Excluir()
        {
            try
            {
                // Deleta a peça com base no ID
                string cmdSQL = "Delete From tb_pecas Where id_peca = @IdPeca";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                cmd.Parameters.AddWithValue("@IdPeca", id_peca);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable SelecionarTodos()
        {
            try
            {
                // Busca todas as peças
                string cmdSQL = "SELECT * FROM tb_pecas ORDER BY id_peca";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                int qtdLinhasAfetadas = o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                if (qtdLinhasAfetadas > 0)
                {
                    return dtPesquisa;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable SelecionarPorID()
        {
            try
            {
                // Busca uma peça específica pelo ID
                string cmdSQL = "SELECT * FROM tb_pecas WHERE id_peca = @IdPeca";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);

                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@IdPeca", id_peca);

                con.Open();
                DataTable dtPesquisa = new DataTable();
                int qtdLinhasAfetadas = o_DataAdapter.Fill(dtPesquisa);
                con.Close();

                if (qtdLinhasAfetadas > 0)
                {
                    return dtPesquisa;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}