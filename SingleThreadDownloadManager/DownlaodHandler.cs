namespace SingleThreadDownloadManager;
public class DownlaodHandler
{
	public async Task DownloadFileAsync(string fileUrl, string destination, int chunkSize = 8192)
	{
		using (HttpResponseMessage response = await new HttpClient().GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
		{
			response.EnsureSuccessStatusCode();

			using (Stream contentStream = await response.Content.ReadAsStreamAsync())
			{
				using (FileStream fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.Write, chunkSize,true)) 
				{
					byte[] buffer = new byte[chunkSize];
					int bytesRead;
					long totalRead = 0;
					long? totalBytes = response.Content.Headers.ContentLength.Value;

					while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0) 
					{
						await fileStream.WriteAsync(buffer,0,bytesRead);
						totalRead += bytesRead;

						if (totalBytes.HasValue)
						{
							decimal progress = ((decimal)totalRead / (decimal)totalBytes * 100);
							Console.SetCursorPosition(0, Console.CursorTop);
							Console.Write($"Progress: {progress}%");
						}
					}
				}
			}
		}

	}
}
