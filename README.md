# Catalog REST API

REST API based on .NET 5 with MongoDB running on Docker Container

## Steps

### Installing MongoDB driver

```sh
$ dotnet add package MongoDB.Driver
```

### Start the docker container

```sh
$ docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo
```

Adding environment variables

```sh
$ docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongoadmin -e MONGO_INITDB_ROOT_PASSWORD=Pass#word1 mongo
```

Adding network

```sh
$ docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongoadmin -e MONGO_INITDB_ROOT_PASSWORD=Pass#word1 --network=net5tutorial mongo
```

The last parameter is the image name

Options | Description
---------|----------
 `-d, --detached` | Run container in background and print container ID
`--rm` | Automatically remove the container when it exits 
`-p, --publish list` | Publish a container's port(s) to the host
`-v` | Volume
`-e` | Ennvironment variables

###

MongoDB Settings

Add this group to `appsettings.json`, and other settings environment as well.

```json
  "MongoDBSettings": {
    "Host": "localhost",
    "Port": "27017"
  }
```

### Injecting Mongo Service

In `Startup.cs`
```
public void ConfigureServices(IServiceCollection services)
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Bson serialization
        BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

        // Injecting MongoClient
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var settings = Configuration.GetSection(nameof(MongoDBSettings)).Get<MongoDBSettings>();
            return new MongoClient(settings.ConnectionString);
        });

        //services.AddSingleton<IItemsRepository, InMemItemsRepository>();
        services.AddSingleton<IItemsRepository, MongoDBItemsRepository>();
        
        //...
    }
}
```

## Secrets Managements

**Configuration source** hold informations like Host and Port for connecting to the DB.
We can use for that `appsettings.json` to store informations, although it's not advised to store password and similar sensitive information in any project file.

A better way is to use _command line arguments_ or _environment variables_ or some _cloud sources_ to "inject" these required informations into our .

Here we are going to use **Secret Manager** that is built-in into .NET

### Authentication in MongoDB

We will create a new volume with authentication in our MongoDB, since this isn't a production environment.

```sh
docker stop mongo
docker volume ls
docker volume rm <volume-name>
```

We spin off our new mongo docker container, this time passing the username and password as environment variables (see above).

```sh
// this will add an id into your Project file.
$ dotnet user-secrets init

// follow the convention of your `appsettings.json`
$ dotnet user-secrets set MongoDBSettings:Password Pass#word1 
```

This auto-generated code from `Program.cs` automatically inject that password from the user-secrets for us. More specifically the `CreateDefaultBuilder`

```cs
  public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(webBuilder =>
          {
              webBuilder.UseStartup<Startup>();
          });
```

## Health checks

Is our ReST API alive?
Can it reach the DB?
Is it healthy?

We implement a **Health check endpoint**.
This is not only useful for developers involved in the project but also for an `Orchestrator`.

in `Startup.cs` we add

```cs
public void ConfigureServices(IServiceCollection services)
{
  //...
  // add this line
  services.AddHealthChecks();
}

// and 
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
  ...
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        // add this line bellow
        endpoints.MapHealthChecks("/health");
    });
}
```

Now we can get query the health

```sh
{Host}:{Port}/health
```

To get more details about MongoDB health we can use a Nugget package for that

This is an open-source project
<https://www.nuget.org/packages/AspNetCore.HealthChecks.MongoDb>

```sh
$ dotnet add package AspNetCore.HealthChecks.MongoDb
```

```cs
MongoDBSettings mongoDBSettings = 
      Configuration.GetSection(nameof(MongoDBSettings))
                   .Get<MongoDBSettings>();

// use this extension method
services.AddHealthChecks()
            .AddMongoDb(mongoDBSettings.ConnectionString, name: "mongodb",
                        failureStatus: null, tags: null, timeout: TimeSpan.FromSeconds(3));
```

### Common Health endpoints

`/health/ready` Is the service ready to receive request?
`/health/live` Is the service alive?

This would be ok for now, still if you want a more detailed report back, this is how we could do it

```cs
app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        // checks the DB
        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions{
            Predicate = (check) => check.Tags.Contains("ready"),
            ResponseWriter = async(context, report) =>
            {
                var result = JsonSerializer.Serialize(
                    new {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                            duration = entry.Value.Duration.ToString()                                
                        })
                    }
                );

                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(result);
            }
        });

        // checks the REST API itself
        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions{
            Predicate = (_) => false
        });
    });
```

For other HealthCheck providers check out <https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks>

## Docker

Let's use our REST API with Docker.

To simplify this task we are going to use only **HTTP** (and not **HTTPS**).
To disable this automatic redirection - `"https://localhost:5001` into `http://localhost:5000"` - we just add an extra check in `Startup.cs` method `Configure`. We add the following check to `UseHttpsRedirection()`.

When we run inside a Docker container the ASP environment change the environment to Production.

```cs
    if (env.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }
```

To facilitate the creation of Docker files, we will use the Microsoft's official Docker extension (ms-azuretools.vscode-docker).

On the Visual Studio command pallete, type `Docker: Add Docker Files to Workspace` and follow the steps.
Note that we want to use `Linux` if we were running docker on WSL. And for this project we only want port `80`.


### Building

```sh
$ docker build -t catalog:v1 .
```

Since we also need a docker container for our MongoDB, we would need a network to join them.

```sh
$ docker network create net5tutorial
```

Running our RESP API - ASP core images run into port 80
We also want to override some settings in the environment variables

```sh
$ docker run -it --rm -p 8080:80 -e MongoDBSettings:Host=mongo -e MongoDBSettings:Password=Pass#word1 --network=net5tutorial catalog:v1
```

### Pushing it to DockerHub

To push our Docker image to DockerHub

```sh
$ docker login
# retagging
$ docker tag catalog:v1 daltonlima/catalog:v1
$ docker push daltonlima/catalog:v1
```

### Pulling our image

```sh
$ docker pull daltonlima/catalog:v1
```

or just pull and run
```sh
$ docker run -it --rm -p 8080:80 -e MongoDBSettings:Host=mongo -e MongoDBSettings:Password=Pass#word1 --network=net5tutorial daltonlima/catalog:v1
```

## Kubernetes

* Why a container orchestrator is needed
* What is Kubernetes and which are its basic components
* How Kubernetes enables resilient distributed systems
* How to stand up a basic Kubernetes cluster in your box
* How to deploy your REST API (and MongoDB) to Kubernetes
* How to scale a Kubernetes deployment

