using Aplication;
using Aplication.Configuration;
using ApiBet.OpenApi;
using Infraestructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddApiDocumentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOptions<OddsSyncOptions>()
    .Bind(builder.Configuration.GetSection(OddsSyncOptions.SectionName))
    .Validate(options => options.Limit is >= 1 and <= 200, "OddsSync:Limit must be between 1 and 200.")
    .Validate(options => options.MaxOffset >= options.Offset, "OddsSync:MaxOffset must be greater than or equal to Offset.")
    .Validate(options => options.OffsetIncrement >= 1, "OddsSync:OffsetIncrement must be at least 1.")
    .Validate(options => options.DelaySeconds >= 0, "OddsSync:DelaySeconds must be zero or greater.")
    .Validate(options => options.GetSportSlugs().Count > 0, "OddsSync:Sports must contain at least one sport.")
    .ValidateOnStart();
builder.Services.AddOptions<OddsFeedOptions>()
    .Bind(builder.Configuration.GetSection(OddsFeedOptions.SectionName))
    .Validate(options => options.Limit is >= 1 and <= 200, "OddsFeed:Limit must be between 1 and 200.")
    .Validate(options => options.MaxPages >= 1, "OddsFeed:MaxPages must be at least 1.")
    .Validate(options => options.DelaySeconds >= 0, "OddsFeed:DelaySeconds must be zero or greater.")
    .Validate(options => options.Offset >= 0, "OddsFeed:Offset must be zero or greater.")
    .ValidateOnStart();
builder.Services.AddOptions<OddsDeltaOptions>()
    .Bind(builder.Configuration.GetSection(OddsDeltaOptions.SectionName))
    .Validate(options => options.Limit is >= 1 and <= 500, "OddsDelta:Limit must be between 1 and 500.")
    .Validate(options => options.MaxPages >= 1, "OddsDelta:MaxPages must be at least 1.")
    .Validate(options => options.DelaySeconds >= 0, "OddsDelta:DelaySeconds must be zero or greater.")
    .Validate(options => options.Offset >= 0, "OddsDelta:Offset must be zero or greater.")
    .ValidateOnStart();
builder.Services.AddOptions<ArbitrageFeedOptions>()
    .Bind(builder.Configuration.GetSection(ArbitrageFeedOptions.SectionName))
    .Validate(options => options.Limit is >= 1 and <= 200, "ArbitrageFeed:Limit must be between 1 and 200.")
    .Validate(options => options.Offset >= 0, "ArbitrageFeed:Offset must be zero or greater.")
    .Validate(options => options.MaxPages >= 1, "ArbitrageFeed:MaxPages must be at least 1.")
    .Validate(options => options.DelaySeconds >= 0, "ArbitrageFeed:DelaySeconds must be zero or greater.")
    .Validate(
        options => !options.MinProfit.HasValue || options.MinProfit.Value >= 0,
        "ArbitrageFeed:MinProfit must be zero or greater.")
    .Validate(options => options.RetentionMinutes >= 0, "ArbitrageFeed:RetentionMinutes must be zero or greater.")
    .ValidateOnStart();
builder.Services.Configure<ValueModelOptions>(
    builder.Configuration.GetSection(ValueModelOptions.SectionName));

var app = builder.Build();

app.UseApiDocumentation();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();
app.Run();
