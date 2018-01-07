using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Forms;
using BS.Plugin.V3.Output;
using BS.Plugin.V3.Common;
using BS.Plugin.V3.Utilities;

namespace BugShooting.Output.BugTrack
{
  public class OutputPlugin : OutputPlugin<Output>
  {

    protected override string Name
    {
      get { return "BUGtrack"; }
    }

    protected override Image Image64
    {
      get  { return Properties.Resources.logo_64; }
    }

    protected override Image Image16
    {
      get { return Properties.Resources.logo_16 ; }
    }

    protected override bool Editable
    {
      get { return true; }
    }

    protected override string Description
    {
      get { return "Attach screenshots to BUGtrack entries."; }
    }
    
    protected override Output CreateOutput(IWin32Window Owner)
    {
      
      Output output = new Output(Name, 
                                 String.Empty, 
                                 "Screenshot",
                                 String.Empty, 
                                 1);

      return EditOutput(Owner, output);

    }

    protected override Output EditOutput(IWin32Window Owner, Output Output)
    {

      Edit edit = new Edit(Output);

      var ownerHelper = new System.Windows.Interop.WindowInteropHelper(edit);
      ownerHelper.Owner = Owner.Handle;
      
      if (edit.ShowDialog() == true) {

        return new Output(edit.OutputName,
                          edit.Url,
                          edit.FileName,
                          edit.FileFormat,
                          Output.LastEntryID);
      }
      else
      {
        return null; 
      }

    }

    protected override OutputValues SerializeOutput(Output Output)
    {

      OutputValues outputValues = new OutputValues();

      outputValues.Add("Name", Output.Name);
      outputValues.Add("Url", Output.Url);
      outputValues.Add("FileName", Output.FileName);
      outputValues.Add("FileFormat", Output.FileFormat);
      outputValues.Add("LastEntryID", Output.LastEntryID.ToString());

      return outputValues;
      
    }

    protected override Output DeserializeOutput(OutputValues OutputValues)
    {

      return new Output(OutputValues["Name", this.Name],
                        OutputValues["Url", ""], 
                        OutputValues["FileName", "Screenshot"], 
                        OutputValues["FileFormat", ""],
                        Convert.ToInt32(OutputValues["LastEntryID", "1"]));

    }

    protected override async Task<SendResult> Send(IWin32Window Owner, Output Output, ImageData ImageData)
    {

      try
      {

        string fileName = AttributeHelper.ReplaceAttributes(Output.FileName, ImageData);

        // Show send window
        Send send = new Send(Output.Url, Output.LastEntryID, fileName);

        var ownerHelper = new System.Windows.Interop.WindowInteropHelper(send);
        ownerHelper.Owner = Owner.Handle;

        if (!send.ShowDialog() == true)
        {
          return new SendResult(Result.Canceled);
        }

        string fullFileName = string.Format("{0}.{1}", send.FileName, FileHelper.GetFileExtension(Output.FileFormat));
        string mimeType = FileHelper.GetMimeType(Output.FileFormat);
        byte[] fileBytes = FileHelper.GetFileBytes(Output.FileFormat, ImageData);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/upload.ashx", Output.Url));

        string boundary = string.Format("----------{0}", DateTime.Now.Ticks.ToString("x"));

        request.CookieContainer = new CookieContainer();
        request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
        request.Method = "POST";

        StringBuilder postData = new StringBuilder();
        postData.AppendFormat("--{0}", boundary);
        postData.AppendLine();
        postData.AppendFormat("Content-Disposition: form-data; name=\"fileupload\"; filename=\"{0}\"\n", fullFileName);
        postData.AppendFormat("Content-Type: {0}\n",  mimeType);
        postData.AppendLine();

        byte[] postDataBytes = Encoding.UTF8.GetBytes(postData.ToString());
        byte[] boundaryBytes = Encoding.ASCII.GetBytes(string.Format("\n--{0}\n", boundary));

        request.ContentLength = postDataBytes.Length + fileBytes.Length + boundaryBytes.Length;
        
        using (Stream requestStream = await request.GetRequestStreamAsync())
        {
          await requestStream.WriteAsync(postDataBytes, 0, postDataBytes.Length);
          await requestStream.WriteAsync(fileBytes, 0, fileBytes.Length);
          await requestStream.WriteAsync(boundaryBytes, 0, boundaryBytes.Length);
        }
        
        using (HttpWebResponse response =  (HttpWebResponse)await request.GetResponseAsync())
        {

          if (response.StatusCode != HttpStatusCode.OK)
          {
            return new SendResult(Result.Failed, response.StatusDescription);
          }

          string fileID = null;
          using (Stream responseStream = response.GetResponseStream())
          {
            using (StreamReader reader = new StreamReader(responseStream))
            {
              fileID = reader.ReadToEnd();
            }
          }

          if (send.CreateNewEntry)
          {
            WebHelper.OpenUrl(string.Format("{0}/openbug.aspx?files={1}", Output.Url, fileID));
            return new SendResult(Result.Success);
          }
          else
          {
            WebHelper.OpenUrl(string.Format("{0}/editbug.aspx?id={1}&files={2}", Output.Url, send.EntryID, fileID));
            return new SendResult(Result.Success,
                                  new Output(Output.Name,
                                            Output.Url,
                                            Output.FileName,
                                            Output.FileFormat,
                                            send.EntryID));
          }
                  
        }

      }
      catch (Exception ex)
      {
        return new SendResult(Result.Failed, ex.Message);
      }

    }
      
  }
}
