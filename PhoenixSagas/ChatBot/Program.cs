using OpenAI;
using OpenAI.Chat;
using System.Net.Http;

namespace ChatBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var api = new OpenAIClient(new OpenAIAuthentication("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", "org-e21S4SkM1mR0fIxbZShlAUNN"));
            
            string generateRoomsPrompt = @"For a MUD style RPG game I need you to build me rooms based on my forthcoming requests. 
                Generate interconnected room descriptions for an abandoned, snowy town setting in JSON format, with Virtual Room IDs starting at 1000 and all other IDs as GUIDs. Each room should include details such as exits that connect to other rooms, objects within the room, and any NPCs or mobs. Ensure each room has a unique GUID and that exits correctly link rooms together, maintaining a coherent flow through the town.

                Format the output as follows in JSON:

                {
                  ""rooms"": [
                    {
                      ""Id"": ""[GUID for the Room]"",
                      ""nameKey"": ""[Room Name]"",
                      ""longDescription"": ""[Detailed Room Description]"",
                      ""shortDescription"": ""[Short Room Description]"",
                      ""exits"": [
                        {
                          ""Id"": ""[GUID for the Exit]"",
                          ""exitTo"": ""[GUID of the Room this Exit Leads to]"",
                          ""nameKey"": ""[Direction or Name of Exit]"",
                          ""longDescription"": ""[Description of Where the Exit Leads]"",
                          ""shortDescription"": ""[Short Exit Description]"",
                          ""isGround"": true/false,
                          ""exitFlags"": [Exit Flags],
                          ""travelDir"": [Direction Code],
                          ""entityVars"": {
                            ""keyDesc"": ""[Key Description]"",
                            ""keyword"": ""[Keywords for Interaction]"",
                            ""original_exit_to"": ""[Original Room GUID Before Any Changes]""
                          },
                          ""parentRoom"": ""[GUID of the Parent Room]""
                        }
                      ],
                      ""objects"": [
                        {
                          ""Id"": ""[GUID for the Object]"",
                          ""name"": ""[Object Name]"",
                          ""description"": ""[Object Description]"",
                          ""interactable"": true/false
                        }
                      ],
                      ""mobs"": [],
                      ""roomId"": ""[GUID for the Room]"",
                      ""vRoomId"": [Virtual Room ID, starting at 1000],
                      ""roomFlags"": [Room Flags]
                    }
                  ]
                }
                ";

            //Console.WriteLine(generateRoomsPrompt);
            string jsonString = @"{""Id"": 1, ""mobs"": [], ""exits"": [{""Id"": 8, ""exitTo"": 1004, ""nameKey"": ""southwest"", ""scripts"": [], ""isGround"": true, ""exitFlags"": 64, ""travelDir"": 5, ""entityVars"": {""keyDesc"": ""New Exit"", ""keyword"": ""southwest"", ""original_exit_to"": ""0""}, ""parentRoom"": 1000, ""longDescription"": ""New Exit"", ""shortDescription"": ""New Exit""}, {""Id"": 39, ""exitTo"": 1001, ""nameKey"": ""north"", ""scripts"": [], ""isGround"": true, ""exitFlags"": 64, ""travelDir"": 0, ""entityVars"": {""keyDesc"": ""New Exit"", ""keyword"": ""north"", ""original_exit_to"": ""1001""}, ""parentRoom"": 1000, ""longDescription"": ""New Exit"", ""shortDescription"": ""New Exit""}], ""roomId"": ""dfea39eb-505b-4238-9f47-08b0f44cac20"", ""nameKey"": ""[A snowy clearing]"", ""objects"": [], ""players"": [], ""scripts"": [], ""vRoomId"": 1000, ""roomFlags"": null, ""longDescription"": ""You're at a clearing in the forest which closes up towards the south. To the north the clearing continues and widens but the entire distance is covered in snow. You notice some man-made structures in that direction."", ""shortDescription"": """"}";

            var messages = new List<Message>
            {
                new Message(Role.System, generateRoomsPrompt),
                new Message(Role.User, "Build me some rooms output in JSON"),
                new Message(Role.Assistant, jsonString),
                new Message(Role.User, "Please generate three interconnected rooms with detailed descriptions, exits that link these rooms, and include at least one interactable object per room. The setting is an abandoned town in a snowy, Alaska-type region, long since abandoned with no obvious reason why. The first room should start with a Virtual Room ID of 1000, and subsequent rooms should have incrementing Virtual Room IDs."),
            };
            var chatRequest = new ChatRequest(messages, "gpt-4-turbo-preview", responseFormat: ChatResponseFormat.Json);
            var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

            foreach (var choice in response.Choices)
            {
                Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice} | Finish Reason: {choice.FinishReason}");
            }

            response.GetUsage();
        }
    }

    public class CustomHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(200);

            return httpClient;
        }
    }
}