namespace Model
{
	public class Post
	{
		public long PostId { get; set; }

		public DateTime Date { get; set; }

		public string Text { get; set; }

		public string User { get; set; }
		public int Votes { get; set; } = 0;
		public List<Comment> Comments { get; set; }
	}
}