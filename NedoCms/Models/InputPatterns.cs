namespace NedoCms.Models
{
	public static class InputPatterns
	{
		public const string Text = @"[^<>&\|\\\[\]\{\}\$~#%\*]*";

		public const string Route = @"^[a-zA-Z0-9]+$";

		// Simplified RFC 2822 with big letters.
		public const string Email = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";

		public const string Phone = @"^([0-9\(\)\/\+ \-]*)$";

		public const string NoHtml = @"^((?!</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>).*)$";

		public const string Password = @"[^<>]*";
	}
}