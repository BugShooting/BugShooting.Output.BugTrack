namespace BS.Output.BugTrack
{

  public class Output: IOutput 
  {
    
    string name;
    string url;
    string fileName;
    string fileFormat;
    int lastEntryID;

    public Output(string name, 
                  string url, 
                  string fileName, 
                  string fileFormat,
                  int lastEntryID)
    {
      this.name = name;
      this.url = url;
      this.fileName = fileName;
      this.fileFormat = fileFormat;
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

    public string FileFormat
    {
      get { return fileFormat; }
    }
    
    public int LastEntryID
    {
      get { return lastEntryID; }
    }

  }
}
