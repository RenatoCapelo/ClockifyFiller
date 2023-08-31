// See https://aka.ms/new-console-template for more information
using ClockifyFiller;
using RestSharp;
using RestSharp.Authenticators;
using Microsoft.Extensions.Configuration;
using System;




IConfiguration config = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", true, true)
      .AddJsonFile("appsettings.development.json",true, true)
      .Build();

string API_KEY = config["API_KEY"] ?? string.Empty;
if(API_KEY == string.Empty)
{
    Console.WriteLine("Tem de preencher a API_KEY no ficheiro appsettings.json");
    Console.WriteLine("Pressione Enter para fechar");
    Console.ReadLine();
    return;
}    


const string URL = "https://api.clockify.me/api/v1";

string WORKSPACE_ID = config["WORKSPACE_ID"] ?? string.Empty;
if (API_KEY == string.Empty)
{
    Console.WriteLine("Tem de preencher o WORKSPACE_ID no ficheiro appsettings.json");
    Console.WriteLine("Pressione Enter para fechar");
    Console.ReadLine();
    return;
}


Console.WriteLine("Insira a sua hora de entrada (manhã hora UTC)");
int hourEntry = int.Parse(Console.ReadLine() ?? "");
Console.WriteLine("Insira a sua hora de saida (manhã hora UTC)");
int hourStartLunch = int.Parse(Console.ReadLine() ?? "");

Console.WriteLine("Insira a sua hora de entrada (tarde hora UTC)");
int hourEndLunch = int.Parse(Console.ReadLine() ?? "");

Console.WriteLine("Insira a sua hora de saida (tarde hora UTC)");
int hourExit = int.Parse(Console.ReadLine() ?? "");

Console.WriteLine("Insira o ano: ");
int yearSelected = int.Parse(Console.ReadLine() ?? "");
Console.WriteLine("Insira o mês: ");
int monthSelected = int.Parse(Console.ReadLine() ??"");

List<int> daysToExclude = new List<int>();

while (true)
{
    Console.WriteLine("Existe algum dia que queira excluir?");
    Console.WriteLine("Escreva 0 caso não tenha nenhum");

    int dayToExclude = int.Parse(Console.ReadLine()??"");
    if (dayToExclude == 0)
        break;
    daysToExclude.Add(dayToExclude);
}

List<DateTime> dateTimes = new List<DateTime>();
bool isInCurrentMonth = true;
DateTime dateTime = new(yearSelected, monthSelected, 1);
while (isInCurrentMonth)
{
    if(dateTime.DayOfWeek != DayOfWeek.Saturday && dateTime.DayOfWeek != DayOfWeek.Sunday)
        dateTimes.Add(dateTime);
    dateTime = dateTime.AddDays(1);
    isInCurrentMonth = monthSelected == dateTime.Month;
}

dateTimes = dateTimes.FindAll(dt => !daysToExclude.Exists(x => x == dt.Day));

RestClient restClient = new RestClient(URL);
restClient.AddDefaultHeader("x-api-key", API_KEY);

RestRequest requestProjects = new RestRequest($"workspaces/{WORKSPACE_ID}/projects");

var projects = restClient.Get<List<Projects>>(requestProjects).OrderBy(x => x.clientName).ToList();

foreach (var date in dateTimes)
{
    Console.Clear();

    Console.WriteLine($"{date.ToString("dd/MM/yyyy")}\n");

    for (int i = 0; i < projects?.Count; i++)
    {
        var project = projects[i];
        Console.WriteLine(i + ") " + project.clientName + " - " + project.name);
    }

    Console.WriteLine("\nSelecione uma opção: ");
    string input = Console.ReadLine() ?? "";

    if (!int.TryParse(input, out int IdSelected))
    {
        Console.WriteLine("Input Invalido");
        return;
    }

    if (IdSelected < 0 && IdSelected > projects?.Count)
    {
        Console.WriteLine("Input fora de range");
        return;
    }

    var projectSelected = projects![IdSelected];

    var restRequestMorning = new RestRequest($"workspaces/{WORKSPACE_ID}/time-entries");

    DateTime dateStartMorning = new DateTime(date.Year, date.Month, date.Day, hourEntry, 0, 0);
    DateTime dateEndMorning = new DateTime(date.Year, date.Month, date.Day, hourStartLunch, 0, 0);

    DateTime dateStartEvening = new DateTime(date.Year, date.Month, date.Day, hourEndLunch, 0, 0);
    DateTime dateEndEvening = new DateTime(date.Year, date.Month, date.Day, hourExit, 0, 0);

    restRequestMorning.AddBody(new TimeEntryToPost()
    {
        projectId = projectSelected.id,
        start = dateStartMorning.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture),
        end = dateEndMorning.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture),
    }) ;

    restClient.Post(restRequestMorning);

    await Task.Delay(500);

    var restRequestEvening = new RestRequest($"workspaces/{WORKSPACE_ID}/time-entries");

    restRequestEvening.AddBody(new TimeEntryToPost()
    {
        projectId = projectSelected.id,
        start = dateStartEvening.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture),
        end = dateEndEvening.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture),
    });

    restClient.Post(restRequestEvening);

    await Task.Delay(500);

}

