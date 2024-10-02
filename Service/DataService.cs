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
			db.Posts.Add(new Post { User = "Mads", Text = "This is my first post!", Date = DateTime.Now });
			db.Posts.Add(new Post { User = "Henrik", Text = "What a great website", Date = DateTime.Today });
			db.Posts.Add(new Post { User = "Philip", Text = "How do I make Entity Framework make sense?", Date = DateTime.Today });
		}

		db.SaveChanges();
	}


	public List<Post> GetPosts()
	{
		return db.Posts.Include(b => b.Text).ToList();
	}
	public Post GetPost(int id)
	{
		return db.Posts.Include(b => b.Text).FirstOrDefault(b => b.PostId == id);
	}
	public Post CreatePost(string user, string text)
	{
		Post post = new Post { User = user, Text = text, Date = DateTime.Now };
		db.Posts.Add(post);
		db.SaveChanges();
		return post;
	}
	public Post CreateComment(int id, string text, string user)
	{
		Post post = db.Posts.Include(b => b.Comments).FirstOrDefault(b => b.PostId == id);
		post.Comments.Add(new Comment { Text = text, User = user, Date = DateTime.Now });
		db.SaveChanges();
		return post;
	}
	public Post UpvotePost(int id)
	{
		Post post = db.Posts.FirstOrDefault(b => b.PostId == id);
		post.Votes++;
		db.SaveChanges();
		return post;
	}
	public Post DownvotePost(int id)
	{
		Post post = db.Posts.FirstOrDefault(b => b.PostId == id);
		post.Votes--;
		db.SaveChanges();
		return post;
	}
	public Comment UpvoteComment(int id, int commentId)
	{
		Post post = db.Posts.Include(b => b.Comments).FirstOrDefault(b => b.PostId == id);
		Comment comment = post.Comments.FirstOrDefault(c => c.CommentId == commentId);
		comment.Votes++;
		db.SaveChanges();
		return comment;
	}
	public Comment DownvoteComment(int id, int commentId)
	{
		Post post = db.Posts.Include(b => b.Comments).FirstOrDefault(b => b.PostId == id);
		Comment comment = post.Comments.FirstOrDefault(c => c.CommentId == commentId);
		comment.Votes--;
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