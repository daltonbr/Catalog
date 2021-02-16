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

```sh
$ docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo -e MONGO_INITDB_ROOT_USERNAME=mongoadmin -e MONGO_INITDB_ROOT_PASSWORD=Pass#word1 mongo
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