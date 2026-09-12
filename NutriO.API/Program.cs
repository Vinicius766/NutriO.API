// Nutri-O API v2 - Corrigido
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

app.MapGet("/", () => "Nutri-O API v2 rodando!");

// ═══════════════════════════════════════════════════════════
// PRODUTOS
// ═══════════════════════════════════════════════════════════

// GET /produtos - Lista todos
app.MapGet("/produtos", async (Client db) =>
{
    var result = await db.From<Produto>().Get();
    return Results.Ok(result.Models.Select(p => new {
        p.ProdutoId, p.NomeProduto, p.Descricao, p.Preco,
        p.Categoria, p.QtdEstoque, p.Calorias, p.Proteinas,
        p.Carboidratos, p.GordurasTotais, p.ImagemUrl
    }));
});

// GET /produtos/{id} - Busca por ID
app.MapGet("/produtos/{id}", async (Client db, int id) =>
{
    var result = await db.From<Produto>().Where(p => p.ProdutoId == id).Get();
    var p = result.Models.FirstOrDefault();
    if (p == null) return Results.NotFound();
    return Results.Ok(new {
        p.ProdutoId, p.NomeProduto, p.Descricao, p.Preco,
        p.Categoria, p.QtdEstoque, p.Calorias, p.Proteinas,
        p.Carboidratos, p.GordurasTotais, p.ImagemUrl
    });
});

// POST /produtos - Cria novo produto (via supabase-csharp)
app.MapPost("/produtos", async (Client db, Produto produto) =>
{
    var result = await db.From<Produto>().Insert(produto);
    var p = result.Models.FirstOrDefault();
    return Results.Ok(new { p!.ProdutoId, p.NomeProduto, p.Preco });
});

// PUT /produtos/{id} - Atualiza produto
app.MapPut("/produtos/{id}", async (Client db, int id, Produto produto) =>
{
    produto.ProdutoId = id;
    await db.From<Produto>().Where(p => p.ProdutoId == id).Update(produto);
    return Results.Ok(new { id, atualizado = true });
});

// DELETE /produtos/{id} - Deleta produto
app.MapDelete("/produtos/{id}", async (Client db, int id) =>
{
    await db.From<Produto>().Where(p => p.ProdutoId == id).Delete();
    return Results.Ok(new { id, deletado = true });
});

// ═══════════════════════════════════════════════════════════
// CLIENTES
// ═══════════════════════════════════════════════════════════

// GET /clientes - Lista todos
app.MapGet("/clientes", async (Client db) =>
{
    var result = await db.From<Cliente>().Get();
    return Results.Ok(result.Models.Select(c => new {
        c.Cpf, c.Nome, c.Email, c.Telefone,
        c.DataNascimento, c.ObjetivoNutricional
    }));
});

// POST /clientes - Cria novo cliente
app.MapPost("/clientes", async (ClienteRequest req) =>
{
    using var http = new HttpClient();
    http.DefaultRequestHeaders.Add("apikey", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImZ0amtkc2RuYmxtenNuam5qa3BwIiwicm9sZSI6ImFub24iLCJpYXQiOjE3ODc4NDg0OTQsImV4cCI6MjEwMzQyNDQ5NH0.2LlJz97QI6F7NmjSHZEEsQm9UeKARsSJTY9m5z7eBSA");
    http.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImZ0amtkc2RuYmxtenNuam5qa3BwIiwicm9sZSI6ImFub24iLCJpYXQiOjE3ODc4NDg0OTQsImV4cCI6MjEwMzQyNDQ5NH0.2LlJz97QI6F7NmjSHZEEsQm9UeKARsSJTY9m5z7eBSA");

    var json = System.Text.Json.JsonSerializer.Serialize(new {
        cpf = req.Cpf,
        nome = req.Nome,
        email = req.Email,
        telefone = req.Telefone,
        objetivo_nutricional = req.ObjetivoNutricional,
        senha = string.IsNullOrEmpty(req.Senha) ? "123456" : req.Senha
    });

    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    var response = await http.PostAsync("https://ftjkdsdnblmzsnjnjkpp.supabase.co/rest/v1/clientes", content);
    var body = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        return Results.Problem(body);

    return Results.Ok(new { cpf = req.Cpf, nome = req.Nome });
});

// PUT /clientes/{cpf} - Atualiza cliente
app.MapPut("/clientes/{cpf}", async (Client db, string cpf, Cliente cliente) =>
{
    cliente.Cpf = cpf;
    await db.From<Cliente>().Where(c => c.Cpf == cpf).Update(cliente);
    return Results.Ok(new { cpf, atualizado = true });
});

// DELETE /clientes/{cpf} - Deleta cliente
app.MapDelete("/clientes/{cpf}", async (Client db, string cpf) =>
{
    await db.From<Cliente>().Where(c => c.Cpf == cpf).Delete();
    return Results.Ok(new { cpf, deletado = true });
});

// ═══════════════════════════════════════════════════════════
// PEDIDOS
// ═══════════════════════════════════════════════════════════

// GET /pedidos - Lista todos
app.MapGet("/pedidos", async (Client db) =>
{
    var result = await db.From<Pedido>().Get();
    return Results.Ok(result.Models.Select(p => new {
        p.PedidoId, p.DataHora, p.ValorTotal,
        p.StatusPedido, p.FormaPagamento, p.FkClienteCpf
    }));
});

// GET /pedidos/resumo - Lista com nome do cliente
app.MapGet("/pedidos/resumo", async (Client db) =>
{
    var pedidos = await db.From<Pedido>().Get();
    var clientes = await db.From<Cliente>().Get();

    var resultado = pedidos.Models.Select(p => new {
        p.PedidoId,
        p.DataHora,
        p.ValorTotal,
        p.StatusPedido,
        p.FormaPagamento,
        p.FkClienteCpf,
        Cliente = clientes.Models
            .FirstOrDefault(c => c.Cpf == p.FkClienteCpf)?.Nome ?? p.FkClienteCpf
    });

    return Results.Ok(resultado);
});

// POST /pedidos - Cria novo pedido
app.MapPost("/pedidos", async (Client db, Pedido pedido) =>
{
    pedido.DataHora = DateTime.UtcNow;
    var result = await db.From<Pedido>().Insert(pedido);
    var p = result.Models.FirstOrDefault();
    return Results.Ok(new { p!.PedidoId, p.DataHora, p.ValorTotal });
});

// PUT /pedidos/{id}/status - Atualiza status
app.MapPut("/pedidos/{id}/status", async (Client db, int id, StatusUpdate body) =>
{
    var pedido = new Pedido { StatusPedido = body.Status };
    await db.From<Pedido>().Where(p => p.PedidoId == id).Update(pedido);
    return Results.Ok(new { id, novoStatus = body.Status });
});

// DELETE /pedidos/{id} - Deleta pedido + itens
app.MapDelete("/pedidos/{id}", async (Client db, int id) =>
{
    await db.From<ItensPedido>().Where(i => i.FkPedidosId == id).Delete();
    await db.From<Pedido>().Where(p => p.PedidoId == id).Delete();
    return Results.Ok(new { id, deletado = true });
});

// ═══════════════════════════════════════════════════════════
// ITENS PEDIDO
// ═══════════════════════════════════════════════════════════

// POST /itens - Cria item de pedido
app.MapPost("/itens", async (Client db, ItensPedido item) =>
{
    var result = await db.From<ItensPedido>().Insert(item);
    var i = result.Models.FirstOrDefault();
    return Results.Ok(new { i!.ItemId, i.FkPedidosId, i.Quantidade });
});

app.Run();

// ═══════════════════════════════════════════════════════════
// DTOs
// ═══════════════════════════════════════════════════════════
record ClienteRequest(string Cpf, string Nome, string Email, string Telefone, DateTime? DataNascimento, string ObjetivoNutricional, string Senha);
record StatusUpdate(string Status);
