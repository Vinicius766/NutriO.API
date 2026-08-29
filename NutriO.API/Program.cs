using Supabase;
using NutriO.API.Models;

var builder = WebApplication.CreateBuilder(args);

var supabaseUrl = "https://ftjkdsdnblmzsnjnjkpp.supabase.co";
var supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImZ0amtkc2RuYmxtenNuam5qa3BwIiwicm9sZSI6ImFub24iLCJpYXQiOjE3ODc4NDg0OTQsImV4cCI6MjEwMzQyNDQ5NH0.2LlJz97QI6F7NmjSHZEEsQm9UeKARsSJTY9m5z7eBSA";

var supabase = new Client(supabaseUrl, supabaseKey);
await supabase.InitializeAsync();

builder.Services.AddSingleton(supabase);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapGet("/", () => "Nutri-O API rodando!");

// ─── PRODUTOS ───────────────────────────────────────────
app.MapGet("/produtos", async (Client db) =>
{
    var result = await db.From<Produto>().Get();
    return Results.Ok(result.Models.Select(p => new {
        p.ProdutoId,
        p.NomeProduto,
        p.Descricao,
        p.Preco,
        p.Categoria,
        p.QtdEstoque,
        p.Calorias,
        p.Proteinas,
        p.Carboidratos,
        p.GordurasTotais,
        p.ImagemUrl
    }));
});

app.MapGet("/produtos/{id}", async (Client db, int id) =>
{
    var result = await db.From<Produto>().Where(p => p.ProdutoId == id).Get();
    var p = result.Models.FirstOrDefault();
    if (p == null) return Results.NotFound();
    return Results.Ok(new
    {
        p.ProdutoId,
        p.NomeProduto,
        p.Descricao,
        p.Preco,
        p.Categoria,
        p.QtdEstoque,
        p.Calorias,
        p.Proteinas,
        p.Carboidratos,
        p.GordurasTotais,
        p.ImagemUrl
    });
});

app.MapPost("/produtos", async (Client db, Produto produto) =>
{
    var result = await db.From<Produto>().Insert(produto);
    var p = result.Models.FirstOrDefault();
    return Results.Ok(new { p!.ProdutoId, p.NomeProduto, p.Preco });
});

app.MapPut("/produtos/{id}", async (Client db, int id, Produto produto) =>
{
    produto.ProdutoId = id;
    var result = await db.From<Produto>().Where(p => p.ProdutoId == id).Update(produto);
    return Results.Ok(new { id, atualizado = true });
});

app.MapDelete("/produtos/{id}", async (Client db, int id) =>
{
    await db.From<Produto>().Where(p => p.ProdutoId == id).Delete();
    return Results.Ok(new { id, deletado = true });
});

// ─── CLIENTES ───────────────────────────────────────────
app.MapGet("/clientes", async (Client db) =>
{
    var result = await db.From<Cliente>().Get();
    return Results.Ok(result.Models.Select(c => new {
        c.Cpf,
        c.Nome,
        c.Email,
        c.Telefone,
        c.DataNascimento,
        c.ObjetivoNutricional
    }));
});

app.MapPost("/clientes", async (Client db, Cliente cliente) =>
{
    if (string.IsNullOrEmpty(cliente.Senha)) cliente.Senha = "123456";
    var result = await db.From<Cliente>().Insert(cliente);
    var c = result.Models.FirstOrDefault();
    return Results.Ok(new { c!.Cpf, c.Nome, c.Email });
});

app.MapPut("/clientes/{cpf}", async (Client db, string cpf, Cliente cliente) =>
{
    cliente.Cpf = cpf;
    await db.From<Cliente>().Where(c => c.Cpf == cpf).Update(cliente);
    return Results.Ok(new { cpf, atualizado = true });
});

app.MapDelete("/clientes/{cpf}", async (Client db, string cpf) =>
{
    await db.From<Cliente>().Where(c => c.Cpf == cpf).Delete();
    return Results.Ok(new { cpf, deletado = true });
});

// ─── PEDIDOS ────────────────────────────────────────────
app.MapGet("/pedidos", async (Client db) =>
{
    var result = await db.From<Pedido>().Get();
    return Results.Ok(result.Models.Select(p => new {
        p.PedidoId,
        p.DataHora,
        p.ValorTotal,
        p.StatusPedido,
        p.FormaPagamento,
        p.FkClienteCpf
    }));
});

app.MapPost("/pedidos", async (Client db, Pedido pedido) =>
{
    pedido.DataHora = DateTime.UtcNow;
    var result = await db.From<Pedido>().Insert(pedido);
    var p = result.Models.FirstOrDefault();
    return Results.Ok(new { p!.PedidoId, p.DataHora, p.ValorTotal });
});

app.MapPut("/pedidos/{id}/status", async (Client db, int id, StatusUpdate body) =>
{
    var pedido = new Pedido { StatusPedido = body.Status };
    await db.From<Pedido>().Where(p => p.PedidoId == id).Update(pedido);
    return Results.Ok(new { id, novoStatus = body.Status });
});

app.MapDelete("/pedidos/{id}", async (Client db, int id) =>
{
    await db.From<ItensPedido>().Where(i => i.FkPedidosId == id).Delete();
    await db.From<Pedido>().Where(p => p.PedidoId == id).Delete();
    return Results.Ok(new { id, deletado = true });
});

// ─── ITENS PEDIDO ───────────────────────────────────────
app.MapPost("/itens", async (Client db, ItensPedido item) =>
{
    var result = await db.From<ItensPedido>().Insert(item);
    var i = result.Models.FirstOrDefault();
    return Results.Ok(new { i!.ItemId, i.FkPedidosId, i.Quantidade });
});

app.Run();

record StatusUpdate(string Status);