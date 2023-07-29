using k8s.KubeConfigModels;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;
using Voting.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseOrleans((ctx, orleansBuilder) =>
{
    if (ctx.HostingEnvironment.IsDevelopment())
    {
        // During development time, we don't want to have to deal with
        // storage emulators or other dependencies. Just "Hit F5" to run.
        orleansBuilder
            .UseLocalhostClustering()
            .AddMemoryGrainStorage("votes")
            .UseDashboard(options => { });
    }
    else
    {
        //var storageConnectionString = builder.Configuration.GetValue<string>(EnvironmentVariables.AzureStorageConnectionString);
        

        var connectionString =
            builder.Configuration["ORLEANS_AZURE_STORAGE_CONNECTION_STRING"];

        orleansBuilder
            //.ConfigureEndpoints(endpointAddress, siloPort, gatewayPort)
            .Configure<ClusterOptions>(
                options =>
                {
                    options.ClusterId = "PollCluster";
                    options.ServiceId = nameof(PollService);
                }).UseAzureStorageClustering(
            options => options.ConfigureTableServiceClient(connectionString));
        orleansBuilder.AddAzureTableGrainStorage(
            "votes",
            options => options.ConfigureTableServiceClient(connectionString));
        // In Kubernetes, we use environment variables and the pod manifest
        //orleansBuilder.UseKubernetesHosting();

        // Use Redis for clustering & persistence
        //var redisAddress = $"{Environment.GetEnvironmentVariable("REDIS")}:6379";
        //orleansBuilder.UseRedisClustering(options => options.ConnectionString = redisAddress);
        //orleansBuilder.AddRedisGrainStorage("votes", options => options.ConnectionString = redisAddress);
    }
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<PollService>();
builder.Services.AddScoped<DemoService>();
//builder.Services.AddScoped<TranslatorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
