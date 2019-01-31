#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("Slack message received.");
    
    string slackListen = req.Query["text"];

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic dataX = JsonConvert.DeserializeObject(requestBody);
    slackListen = slackListen ?? dataX?.slackListen;

    log.LogInformation($"{dataX?.challenge}");

    log.LogInformation($"Request body 1: {requestBody}");

    if (dataX?.challenge != null) {
        return (ActionResult)new OkObjectResult($"{dataX["challenge"]}");
    }

    string points_col = "8809649655834500";
    string ss_cell = string.Empty;
    string house_str = "";
    string to_post = string.Empty;
    Match points = Match.Empty;




  if (dataX["event"]["text"] != null) {
 


    string slackEvent = dataX["event"]["text"];

    Match house =  Regex.Match(slackEvent.ToLower(), @"(gryffindor)|(hufflepuff)|(ravenclaw)|(slytherin)");
    points =  Regex.Match(slackEvent.ToLower(), @"^(\d*)|(?!\s)\d*");
    Match mesOper =  Regex.Match(slackEvent.ToLower(), @"(for)|(to)|(add)|(grant)|(give)|(assign)|(added)|(granted)|(given)|(assigned)|(adds)|(grants)|(gives)|(assigns)|(take)|(taken)|(takes)|(from)|(remove)|(removes)|(removed)|(subtract)|(subtracts)|(subtracted)");
    
    log.LogInformation($"Points: {points}");
    log.LogInformation($"Operator: {mesOper}");
    log.LogInformation($"House: {house}");
    log.LogInformation($"mesOper Length: {dataX["event"]["text"]}");
    
    string mesOperStr = mesOper.Value;
    string botOper = string.Empty;
    
    house_str = house.Value;
    ss_cell = string.Empty;


    if (mesOperStr.Contains("for")||mesOperStr.Contains("to")||mesOperStr.Contains("add")||mesOperStr.Contains("added")||mesOperStr.Contains("adds")||mesOperStr.Contains("grant")||mesOperStr.Contains("granted")||mesOperStr.Contains("grants")||mesOperStr.Contains("give")||mesOperStr.Contains("given")||mesOperStr.Contains("gives")||mesOperStr.Contains("assign")||mesOperStr.Contains("assigned")||mesOperStr.Contains("assigns")) {
      botOper = "granted to";
    } else {
      botOper = "taken from";
    }


    if(house.ToString().Length != 0){
        to_post = $"{points} points {botOper} {house}!";
   } else {
        to_post = "Expelliarmus!";
    }


log.LogInformation($"ss_cell: {ss_cell}");



HttpWebRequest reqY = (HttpWebRequest)WebRequest.Create("https://api.smartsheet.com/2.0/sheets/6219962109978500/");
reqY.Method = "GET";
reqY.ContentType = "application/json";
reqY.Headers.Add("Authorization", "Bearer ewnmhvfgdvee3trgffiy3pvicp");
HttpWebResponse res_y = (HttpWebResponse)reqY.GetResponse();

log.LogInformation($"res_y: {res_y}");

return new OkObjectResult(res_y);

}





  if (dataX?["rows"][1]["cells"][2]["value"] != null) {

        if (house_str == "Gryffindor")
        {
            ss_cell = "0";
        }
        else if (house_str == "Hufflepuff")
        {
            ss_cell = "1";
        }
        else if (house_str == "Ravenclaw")
        {
            ss_cell = "2";
        }
        else
        {
            ss_cell = "3";
        }


    int prev_value = 0;
    int new_value = 0;
    int points_int = Int32.Parse(points.Value);
    

    prev_value = dataX?["rows"][$"{ss_cell}"]["cells"][2]["value"];

    new_value = prev_value + points_int;

//return (ActionResult)new OkObjectResult($"New Value: {new_value}");

log.LogInformation($"Old value: {prev_value}");
log.LogInformation($"New value: {new_value}");


HttpWebRequest reqX = (HttpWebRequest)WebRequest.Create("https://ios-slack.azurewebsites.net/api/HttpTrigger4?code=8tGkPNOOLiUlZK0XZRUIRASO5U3msH7Tyk4vH0R4GCaDRXItBbYenA==");
reqX.Method = "POST";
reqX.ContentType = "application/json";
Stream stream = reqX.GetRequestStream();
string json = $"{{ \"text\": \"{to_post}\" }}";
byte[] buffer = Encoding.UTF8.GetBytes(json);
stream.Write(buffer,0, buffer.Length);
HttpWebResponse res = (HttpWebResponse)reqX.GetResponse();

return new OkObjectResult(res);


  }

 //  return dataX?["rows"][1]["cells"][2]["value"] != null
 //      ? (ActionResult)new OkObjectResult("worked")
 //       : new BadRequestObjectResult("Please pass a name on the query string or in the request body");


} 
