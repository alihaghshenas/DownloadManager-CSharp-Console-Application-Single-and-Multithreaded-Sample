using Shared;
using SingleThreadDownloadManager;

Console.WriteLine("Enter Download Link : ");
string link = Console.ReadLine();
Console.Clear();

string fileName = UrlHandler.GetFileNameFromUrl(link);
string destination = Path.Combine(Environment.CurrentDirectory,fileName);

await new DownlaodHandler().DownloadFileAsync(link,destination);

Console.WriteLine("Downlaod Completed!");

