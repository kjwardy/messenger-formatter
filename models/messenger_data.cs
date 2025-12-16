using System.Collections.Generic;

namespace Messenger_data
{
  public class Messenger_Data
  {
    public List<participant> participants { get; set; }
    public List<message> messages { get; set; }
    public string title { get; set; }
    public bool is_still_participant { get; set; }
    public string thread_type { get; set; }
    public string thread_path { get; set; }
    public List<string> magic_words { get; set; }
    public image image { get; set; }
  }

  public class participant
  {
    public string name { get; set; }
    public int messages_sent { get; set; }
  }

  public class message
  {
    public string sender_name { get; set; }
    public long timestamp_ms { get; set; }
    public string content { get; set; }
    public List<media> photos { get; set; }
    public List<media> gifs { get; set; }
    public List<media> videos { get; set; }
    public shared share { get; set; }
    public List<media> audio_files { get; set; }
    public List<reaction> reactions { get; set; }
    public string type { get; set; }
    public bool is_unsent { get; set; }
    public bool is_unsent_image_by_messenger_kid_parent { get; set; }
    public bool is_geoblocked_for_viewer { get; set; }
    public string Topic { get; set; }

  }

  public class shared
  {
    public string link { get; set; }
    public string share_text { get; set; }
  }

  public class image
  {
    public string uri { get; set; }
    public long creation_timestamp { get; set; }
  }

  public class media
  {
    public string uri { get; set; }
    public long creation_timestamp { get; set; }
  }

  public class reaction
  {
    public string emoji { get; set; }
    public string actor { get; set; }
  }
}