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

The last parameter is the image name

Options | Description
---------|----------
 `-d, --detached` | Run container in background and print container ID
`--rm` | Automatically remove the container when it exits 
`-p, --publish list` | Publish a container's port(s) to the host
`-v` | Volume

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
