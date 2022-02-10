using Adai.DbContext.Attributes;

namespace Adai.DbContext.Test.Models
{
	/// <summary>
	/// User
	/// </summary>
	[Table("User")]
	internal class User
	{
		[Column("Id")]
		public int Id { get; set; }

		[Column("Username")]
		public string Username { get; set; } = "";

		[Column("Nickname")]
		public string? Nickname { get; set; }

		[Column("Avatar")]
		public string? Avatar { get; set; }

		[Column("FirstName")]
		public string? FirstName { get; set; }

		[Column("LastName")]
		public string? LastName { get; set; }

		[Column("Email")]
		public string? Email { get; set; }

		[Column("Mobile")]
		public string? Mobile { get; set; }

		[Column("Tel")]
		public string? Tel { get; set; }

		[Column("Status")]
		public int Status { get; set; }

		[Column("CreateTime")]
		public DateTime CreateTime { get; set; }

		[Column("UpdateTime")]
		public DateTime? UpdateTime { get; set; }

		[Column("Note")]
		public string? Note { get; set; }

		[Column("E1")]
		public int? E1 { get; set; }

		[Column("E2")]
		public decimal? E2 { get; set; }

		[Column("E3")]
		public DateTime? E3 { get; set; }

		[Column("E4")]
		public string? E4 { get; set; }
	}
}
