using BS.Plugin.V3.Output;
using System;

namespace BugShooting.Output.BugTrack
{

  public class Output: IOutput 
  {
    
    string name;
    string url;
    string fileName;
    Guid fileFormatID;
    int lastEntryID;

    public Output(string name, 
                  string url, 
                  string fileName,
                  Guid fileFormatID,
                  int lastEntryID)
    {
      this.name = name;
      this.url = url;
      this.fileName = fileName;
      this.fileFormatID = fileFormatID;
      this.lastEntryID = lastEntryID;
    }
    
    public string Name
    {
      get { return name; }
    }

    public string Information
    {
      get { return url; }
    }

    public string Url
    {
      get { return url; }
    }
          
    public string FileName
    {
      get { return fileName; }
    }

    public Guid FileFormatID
    {
      get { return fileFormatID; }
    }

    public int LastEntryID
    {
      get { return lastEntryID; }
    }

  }
}
