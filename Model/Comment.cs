namespace Model
{
	public class Comment
	{
		public long CommentId { get; set; }

		public string Text { get; set; }

		public string User { get; set; }
		public int Votes { get; set; } = 0;

		public DateTime Date { get; set; }
	}
}