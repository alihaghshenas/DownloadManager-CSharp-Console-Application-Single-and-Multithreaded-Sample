namespace Shared;
public static class UrlHandler
{
	public static string GetFileNameFromUrl(string fileUrl)
	{
		Uri uri = new Uri(fileUrl);
		return Path.GetFileName(uri.LocalPath);
	}
}
