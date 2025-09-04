using Demo.MartenDb.Projections;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMarten(_ =>
{
    var options = new StoreOptions();

    options.Events.DatabaseSchemaName = "DemoMartenDb";
    options.DatabaseSchemaName = "DemoMartenDb";
    options.Connection(builder.Configuration.GetConnectionString("DemoMartenDb") ??
                       throw new InvalidOperationException());

    options.UseSystemTextJsonForSerialization(EnumStorage.AsString);

    //options.Events.TenancyStyle = TenancyStyle.Conjoined;
    //options.Policies.AllDocumentsAreMultiTenanted();

    options.Events.StreamIdentity = StreamIdentity.AsGuid;
    options.Events.MetadataConfig.HeadersEnabled = true;
    options.Events.MetadataConfig.CausationIdEnabled = true;
    options.Events.MetadataConfig.CorrelationIdEnabled = true;

    options.Projections.Errors.SkipApplyErrors = false;
    options.Projections.Errors.SkipSerializationErrors = false;
    options.Projections.Errors.SkipUnknownEvents = false;

    options.Projections.Add<ActiveOrdersProjection>(ProjectionLifecycle.Inline); // Inline: Ocorre na mesma transação em que foi capturada
    return options;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
