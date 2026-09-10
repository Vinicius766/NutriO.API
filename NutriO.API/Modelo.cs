using Postgrest.Attributes;
using Postgrest.Models;

namespace NutriO.API.Models;

[Table("produtos_info_nutricional")]
public class Produto : BaseModel
{
    [PrimaryKey("produto_id")]
    public int ProdutoId { get; set; }

    [Column("nome_produto")]
    public string? NomeProduto { get; set; }

    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("preco")]
    public decimal Preco { get; set; }

    [Column("categoria")]
    public string? Categoria { get; set; }

    [Column("qtd_estoque")]
    public int QtdEstoque { get; set; }

    [Column("calorias")]
    public decimal Calorias { get; set; }

    [Column("proteinas")]
    public decimal Proteinas { get; set; }

    [Column("carboidratos")]
    public decimal Carboidratos { get; set; }

    [Column("gorduras_totais")]
    public decimal GordurasTotais { get; set; }

    [Column("imagem_url")]
    public string? ImagemUrl { get; set; }
}

[Table("clientes")]
public class Cliente : BaseModel
{
    [PrimaryKey("cpf")]
    public string? Cpf { get; set; }

    [Column("nome")]
    public string? Nome { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("telefone")]
    public string? Telefone { get; set; }

    [Column("objetivo_nutricional")]
    public string? ObjetivoNutricional { get; set; }

    [Column("senha")]
    public string? Senha { get; set; }

    [Column("data_nascimento")]
    public DateTime? DataNascimento { get; set; }
}

[Table("pedidos")]
public class Pedido : BaseModel
{
    [PrimaryKey("pedido_id")]
    public int PedidoId { get; set; }

    [Column("data_hora")]
    public DateTime DataHora { get; set; }

    [Column("valor_total")]
    public decimal ValorTotal { get; set; }

    [Column("status_pedido")]
    public string? StatusPedido { get; set; }

    [Column("forma_pagamento")]
    public string? FormaPagamento { get; set; }

    [Column("fk_cliente_cpf")]
    public string? FkClienteCpf { get; set; }
}

[Table("itens_pedidos")]
public class ItensPedido : BaseModel
{
    [PrimaryKey("item_id")]
    public int ItemId { get; set; }

    [Column("quantidade")]
    public int Quantidade { get; set; }

    [Column("preco_no_momento")]
    public decimal PrecoNoMomento { get; set; }

    [Column("fk_pedidos_id")]
    public int FkPedidosId { get; set; }

    [Column("fk_produto_id")]
    public int FkProdutoId { get; set; }
}