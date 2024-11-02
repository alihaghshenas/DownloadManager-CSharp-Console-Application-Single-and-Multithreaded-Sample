namespace DownloadManager;
public class DownloadHandler
{
	public async Task DownloadFileAsync(string fileUrl, string destinationPath, int threadNumber, int totalThreadNumber, int chunkSize = 8192)
	{
		HttpClient httpClient = new HttpClient();

		HttpResponseMessage responseHeader = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
		var separatedThreadBytes = new ThreadByteCalculator().Calculate(responseHeader.Content.Headers.ContentLength, totalThreadNumber);
		responseHeader.Dispose();

		long? start = (threadNumber == 0) ? 0 : separatedThreadBytes[threadNumber - 1] + 1;
		long? end = separatedThreadBytes[threadNumber];

		var request = new HttpRequestMessage(HttpMethod.Get, fileUrl);
		request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(start, end);

		HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
		response.EnsureSuccessStatusCode();

		using (Stream contentStream = await response.Content.ReadAsStreamAsync())
		using (FileStream fileStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, chunkSize, true))
		{
			fileStream.Seek((long)start!, SeekOrigin.Begin);

			var buffer = new byte[chunkSize];
			int bytesRead;
			long totalRead = 0;

			while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
			{
				await fileStream.WriteAsync(buffer, 0, bytesRead);
				totalRead += bytesRead;

				if (response.Content.Headers.ContentLength.HasValue)
				{
					ProgressCounter.Progress[threadNumber] = (double)totalRead / response.Content.Headers.ContentLength.Value * 100;
				}
			}
		}

		response.Dispose();
	}
}