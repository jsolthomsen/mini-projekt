using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;

using Model;
using Data;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Sætter CORS så API'en kan bruges fra andre domæner
var AllowSomeStuff = "_AllowSomeStuff";
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: AllowSomeStuff, builder =>
	{
		builder.AllowAnyOrigin()
			   .AllowAnyHeader()
			   .AllowAnyMethod();
	});
});

// Tilføj DbContext factory som service.
builder.Services.AddDbContext<PostContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// Tilføj DataService så den kan bruges i endpoints
builder.Services.AddScoped<DataService>();

// Dette kode kan bruges til at fjerne "cykler" i JSON objekterne.

builder.Services.Configure<JsonOptions>(options =>
{
	// Her kan man fjerne fejl der opstår, når man returnerer JSON med objekter,
	// der refererer til hinanden i en cykel.
	// (altså dobbelrettede associeringer)
	options.SerializerOptions.ReferenceHandler =
		System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


var app = builder.Build();

// Seed data hvis nødvendigt.
using (var scope = app.Services.CreateScope())
{
	var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
	dataService.SeedData(); // Fylder data på, hvis databasen er tom. Ellers ikke.
}

app.UseHttpsRedirection();
app.UseCors(AllowSomeStuff);

// Middlware der kører før hver request. Sætter ContentType for alle responses til "JSON".
app.Use(async (context, next) =>
{
	context.Response.ContentType = "application/json; charset=utf-8";
	await next(context);
});


// DataService fås via "Dependency Injection" (DI)
app.MapGet("/", (DataService service) =>
{
	return new { message = "Hello World!" };
});


app.MapGet("/api/posts", (DataService service) =>
{
	return service.GetPosts();
});

app.MapGet("/api/posts/{id}", (DataService service, int id) =>
{
	return service.GetPost(id);
});

app.MapPost("/api/posts", (DataService service, Post post) =>
{
	service.CreatePost(post.User, post.Content, post.Title);
	return new { message = "Post created" };
});

app.MapPost("/api/posts/{id}/comments", (DataService service, int id, Comment comment) =>
	service.CreateComment(id, comment.Content));

app.MapPut("/api/posts/{id}/upvote/", (DataService service, int id) => 
	service.UpvotePost(id));

app.MapPut("/api/posts/{id}/downvote", (DataService service, int id) => 
	service.DownvotePost(id));

app.MapPut("/api/posts/{id}/comments/{commentId}/Upvote", (DataService service, int id, int commentId) =>
	service.UpvoteComment(id, commentId));


app.MapPut("/api/posts/{id}/comments/{commentId}/Downvote", (DataService service, int id, int commentId) =>
	service.DownvoteComment(id, commentId));;

app.Run();