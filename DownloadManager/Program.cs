using DownloadManager;
using Shared;

Console.WriteLine("Enter Download Link : ");
string link = Console.ReadLine();
Console.Clear();

string fileName = UrlHandler.GetFileNameFromUrl(link);
string destination = Path.Combine(Environment.CurrentDirectory, fileName);

int threadNumber = Environment.ProcessorCount;
ProgressCounter.Progress = new double[threadNumber];

Task[] downloadParts = new Task[threadNumber];
for (int i = 0; i < threadNumber; i++)
{
	downloadParts[i] = new DownloadHandler().DownloadFileAsync(link, destination , i, threadNumber);
}

var progressTask = Task.Run(async () =>
{
	do
	{
		for (int i = 0; i < threadNumber; i++)
		{
			Console.SetCursorPosition(0, i);
			Console.WriteLine($"Part {i + 1}: {ProgressCounter.Progress[i]:0.00}%");
		}

		await Task.Delay(500);
	}
	while (!Task.WhenAll(downloadParts).IsCompleted) ;
});

await Task.WhenAll(downloadParts);

for (int i = 0; i < threadNumber; i++)
{
	Console.SetCursorPosition(0, i);
	Console.WriteLine($"Part {i + 1}: {ProgressCounter.Progress[i]:0.00}%         ");
}

Console.WriteLine("Download completed.");


