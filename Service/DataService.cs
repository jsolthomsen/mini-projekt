using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using Data;
using Model;

namespace Service;

public class DataService
{
	private PostContext db { get; }

	public DataService(PostContext db)
	{
		this.db = db;
	}

	/// <summary>
	/// Seeder noget nyt data i databasen hvis det er nødvendigt.
	/// </summary>
	public void SeedData()
	{

		Post post = db.Posts.FirstOrDefault()!;
		if (post == null)
		{
			db.Posts.Add(new Post { User = new User("Thomas"), Content = "This is my first post!", Upvotes = 0, Downvotes = 0 });
			db.Posts.Add(new Post { User = new User("Henrik"), Content = "What a great website", Upvotes = 0, Downvotes = 0});
			db.Posts.Add(new Post { User = new User("Philip"), Content = "How do I make Entity Framework make sense?", Upvotes = 0, Downvotes = 0 });
		}

		db.SaveChanges();
	}


	public List<Post> GetPosts()
	{
		return db.Posts.ToList();
	}
	public Post GetPost(int id)
	{
		return db.Posts.FirstOrDefault(p => p.Id == id);
	}
	public Post CreatePost(User user, string content, string title)
	{
		Post post = new Post { User = user, Content = content, Title = title, Upvotes = 0, Downvotes = 0 };
		db.Posts.Add(post);
		db.SaveChanges();
		return post;
	}
	public Post CreateComment(int id, string content, User user)
	{
		Post post = db.Posts.FirstOrDefault(p => p.Id == id);
		post.Comments.Add(new Comment { Content = content, User = user, Upvotes = 0, Downvotes = 0 });
		db.SaveChanges();
		return post;
	}
	public Post UpvotePost(int id)
	{
		Post post = db.Posts.FirstOrDefault(p => p.Id == id);
		post.Upvotes++;
		db.SaveChanges();
		return post;
	}
	public Post DownvotePost(int id)
	{
		Post post = db.Posts.FirstOrDefault(p => p.Id == id);
		post.Downvotes++;
		db.SaveChanges();
		return post;
	}
	public Comment UpvoteComment(int id, int commentId)
	{
		Post post = db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == id);
		Comment comment = post.Comments.FirstOrDefault(c => c.Id == commentId);
		comment.Upvotes++;
		db.SaveChanges();
		return comment;
	}
	public Comment DownvoteComment(int id, int commentId)
	{
		Post post = db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == id);
		Comment comment = post.Comments.FirstOrDefault(c => c.Id == commentId);
		comment.Downvotes++;
		db.SaveChanges();
		return comment;
	}
}

/*
public Post GetBook(int id) {
    return db.Books.Include(b => b.Author).FirstOrDefault(b => b.BookId == id);
}

public List<Comment> GetAuthors() {
    return db.Posts.ToList();
}

public Comment GetAuthor(int id) {
    return db.Posts.Include(a => a.Books).FirstOrDefault(a => a.AuthorId == id);
}

public string CreateBook(string title, int authorId) {
    Comment comment = db.Posts.FirstOrDefault(a => a.AuthorId == authorId);
    db.Books.Add(new Post { Title = title, Comment = comment });
    db.SaveChanges();
    return "Book created";
}

} */