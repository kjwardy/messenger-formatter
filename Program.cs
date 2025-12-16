using Messenger_data;
using System.Text.Json;

class Program
{
  static async Task Main()
  {
    // string currentDirectory = Directory.GetCurrentDirectory();
    // string filepath = Path.Combine(currentDirectory, "/test-data/message_1.json");

    // Get a list of all JSON files in the target directory
    // string targetDirPath = "test-data";
    string[] targetFiles = Directory.GetFiles("test-data", "*.json");

    Console.WriteLine("---------------------------------------------");
    Console.WriteLine(targetFiles.Count() + " JSON files identified in target dir");
    Console.WriteLine("Attempting to read in raw Messenger data...");
    Console.WriteLine("---------------------------------------------");

    try
    {
      int fileCounter = 0;
      Messenger_Data confirmed_data = new();
      foreach (string jsonFile in targetFiles)
      {
        Console.WriteLine("Reading: " + jsonFile + "...");
        fileCounter++;

        string raw_json = File.ReadAllText(jsonFile);
        Messenger_Data raw_data = JsonSerializer.Deserialize<Messenger_Data>(raw_json);

        if (fileCounter == 1)
        {
          // First pass, add entire raw_data object to confirmed_data
          confirmed_data = raw_data;
        }
        else
        {
          // Second or more pass, just add messages to confirmed_data list
          confirmed_data.messages.AddRange(raw_data.messages);
        }
        Console.WriteLine("Read success.");
      }

      Console.WriteLine("---------------------------------------------");
      Console.WriteLine("Successfully read " + confirmed_data.messages.Count + " messages!");
      Console.WriteLine("---------------------------------------------");

      // Generate metrics on the imported messages
      GenerateMetrics(confirmed_data);
      // Generate JSON output and dump export file to disk
      //OutputData(confirmed_data);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
  }

  public static void GenerateMetrics(Messenger_Data data)
  {
    int total_participants = data.participants.Count();
    int total_messages = data.messages.Count();
    int total_reactions = 0;
    //int total_unsent_messages = 0;
    long earliest_timestamp = 0;
    long latest_timestamp = 99999999999999999; // Lol @ this jank

    // Loop through data to generate metrics
    try
    {
      Console.WriteLine("Processing messages and generating metrics...");
      Console.WriteLine("---------------------------------------------");
      int msgCounter = 0;
      foreach (message msg in data.messages)
      {
        try
        {
          var participant = data.participants.FirstOrDefault(p => p.name == msg.sender_name);
          participant.messages_sent += 1;
        }
        catch
        {
          // Participant is no longer in the chat or is the first instance of the Meta AI
          var unknown_participant = new participant();
          unknown_participant.name = msg.sender_name;
          unknown_participant.messages_sent = 0;

          data.participants.Add(unknown_participant);
        }

        if (msg.reactions != null)
        {
          foreach (var reaction in msg.reactions)
          {
            total_reactions += 1;
          }
        }

        if (msg.timestamp_ms > earliest_timestamp) { earliest_timestamp = msg.timestamp_ms; }
        if (msg.timestamp_ms < latest_timestamp) { latest_timestamp = msg.timestamp_ms; }

        // End-user status update
        msgCounter += 1;
        if (msgCounter % 1000 == 0)
        {
          Console.WriteLine("Messages processed: " + msgCounter + ", remaining: " + (total_messages - msgCounter));
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }

    // Change epoch_ms timestamps to human readable
    // DateTimeOffset earliest_dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(earliest_timestamp);
    // DateTimeOffset latest_dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(latest_timestamp);
    // DateTime earliest_dateTime = earliest_dateTimeOffset.DateTime;
    // DateTime latest_dateTime = latest_dateTimeOffset.DateTime;

    // Print generated metrics
    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("              Generated Metrics              ");
    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("Members in the chat: " + total_participants);
    Console.WriteLine("Chat messages processed: " + total_messages);
    Console.WriteLine("Message emoji reactions: " + total_reactions);
    Console.WriteLine("Last message timestamp: " + earliest_timestamp);
    Console.WriteLine("First message timestamp: " + latest_timestamp);
    Console.WriteLine("---------------------------------------------");
    // Messages sent per participant
    foreach (var participant in data.participants)
    {
      Console.WriteLine(participant.name + " sent " + participant.messages_sent + " messages");
    }
    Console.WriteLine("---------------------------------------------");
  }

  public static void OutputData(Messenger_Data data)
  {
    string output_filepath = "output.json";

    using (StreamWriter output = new StreamWriter(output_filepath))
    {
      foreach (message msg in data.messages)
      {
        string json = JsonSerializer.Serialize(msg);
        output.WriteLine(json);
      }
    }
  }
}