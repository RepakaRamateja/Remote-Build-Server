///////////////////////////////////////////////////////////////////////////////////////////////////
//CoreBuilder.cs - Demonstrate Core Builder operations                                           //
// ver 2.0                                                                                       //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                       //
// Application: CSE681 Project 4-Remote Build Server                                             //
// Environment: C# console                                                                       //
///////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 *  Demonstrates operations like handling build requests sent from Mother builder,
 *  requesting for files from Mock Repository,Communicating with Comm messages and files by using WCF,
 *  generating dll files and test request and sending files to test harness and logs to mock repository
 * 
 * Public Interface
 * ----------------
 *   CoreBuilder builder = new CoreBuilder(port,mockport)  // corebuilder constructor which takes two arguments CoreBuilder port number and Mockrepository port number
 *   builder.handlebuildrequest(string filename);//function responsbible for handling BuildRequest
 *   builder.sendReadyMessage(string tosend);//function responsible for sending ready messages to MotherBuilder
 *   builder.handlebuilds(CommMessage msg);//function responsible for handling builds by sending file requests to the Mock Repository
 *   builder.startchildprocess();//function that is responsible for creating a thread which keeps on listening for incoming messages
 *   builder.deleteallfiles();//contains logic to delete all files in a directory
 *   builder.deletetempfiles();//contains logic to delete temparary files
 *   builder.HandlerThreadProc();//function that keeps on listening for incoming messages
 *   builder.generatetestrequest();//used for generating the test request
 *   builder.generatedllfiles();//contains the core logic to generate dll files using process class
 *   builder.sendlogtorepository();//function that handles sending build logs to the repository
 *   builder.createlogfile();//function for appending the output of build to a log file
 *   builder.showRecievedXml();//function used to display the recieved xml content
 *   builder.handlebuildrequest();//function responsbible for handling BuildRequest
 *   builder.sourcefilelist();//function which returns source file list
 *   builder.processbuildrequest();//Contains core logic to process build request
 *   builder.sendReadyMessage();//function responsible for sending ready messages to MotherBuilder
 *   builder.handlebuilds();//function responsible for handling builds by sending file requests to the Mock Repository
 * 
 * Required Files:
 * ---------------
 * CoreBuilder.cs , MPCommService.cs, IMPCommService.cs
 * 
 * Build Process
 * -------------
 * csc CoreBuilder.cs MPCommService.cs  IMPCommService.cs
 * devenv CoreBuilder.csproj 
 * 
 * Maintenance History:
 * --------------------
 * ver 2.0 : 6th December 2017
 * - added functionality for generating dll files 
 * - sending generated dll files and test request to Mock test harness
 * 
 * ver 1.0 : 26 Oct 2017
 * - first release
 * 
 */
using Build_Request;
using MessagePassingComm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestHarnessMessages;
using Utilities;

namespace CoreBuilder
{
  //class that has operations related to core builder like handlebuildrequest ,sendreadymessages etc
  public class CoreBuilder
  {
    string myAddress { get; set; } = null;
    string mockrepoaddress { get; set; } = null;
    string harnessaddress { get; set; } = null;
    string Mockclientaddress { get; set; } = null;
    Comm comm;
    Thread msgHandler;
    private static int startport { get; set; } = 0;
    string builderId = null;
    //Dictionary<string, string> dict = null;
    List<string> filelist = null;
    string buildRequestContent = null;

    //<----------Constructor which takes Corebuilder port number and the mock Repository Port number------>
    public CoreBuilder(int startport, int mockport, int harnessport, int mockclientport)
    {
      comm = new Comm("http://localhost", startport);
      myAddress = "http://localhost:" + startport.ToString() + "/IMessagePassingComm";
      mockrepoaddress = "http://localhost:" + mockport.ToString() + "/IMessagePassingComm";
      harnessaddress = "http://localhost:" + harnessport.ToString() + "/IMessagePassingComm";
      Mockclientaddress = "http://localhost:" + mockclientport.ToString() + "/IMessagePassingComm";
      builderId = "Builder" + startport.ToString();
      filelist = new List<string>();
      bool exists = System.IO.Directory.Exists("../../../CoreBuilder/Builderstorage/" + builderId);
      if (!exists)
        System.IO.Directory.CreateDirectory("../../../CoreBuilder/Builderstorage/" + builderId);
      deleteallfiles("../../../CoreBuilder/Builderstorage/" + builderId);
      Console.WriteLine("\n");
      Console.WriteLine("The child builder directory is" + "../../../CoreBuilder/Builderstorage/" + builderId);
      Console.WriteLine("\n");
    }

    //<----------contains logic to delete all files in a directory--------------------------------->
    public void deleteallfiles(string path)
    {
      string[] picList = Directory.GetFiles(path, "*.*");
      foreach (string f in picList)
      {
        File.Delete(f);
      }
    }

    //<----------contains logic to delete temparary files--------------------------------->
    public void deletetempfiles(string path)
    {
      string[] picList = Directory.GetFiles(path, "*.dll");
      foreach (string f in picList)
      {
        File.Delete(f);
      }
    }

    //<--------------- function that keeps on listening for incoming messages---------------> 
    private void HandlerThreadProc()
    {
      while (true)
      {
        CommMessage msg = comm.getMessage();
        if (msg.command == "Are u ready?????")
        {
          Console.WriteLine("Message recieved from:  " + msg.from);
          msg.show();
          sendReadyMessage(msg.from);
        }
        if (msg.command == "BuildRequestFileName")
        {
          Console.WriteLine("\n");
          Console.WriteLine(" Build Request Message recieved from:  " + msg.from);
          bool retstatus = handlebuilds(msg);
          if (retstatus)
            sendReadyMessage(msg.from);
        }
        if (msg.command == "filesent")
        {
          Console.WriteLine("\nMessage recieved from Mock Repository\n");
          msg.show();
          showRecievedXml(msg);
        }
        if (msg.command == "quit")
        {
          Console.WriteLine(" Message recieved from:  " + msg.from); Console.WriteLine("\n");
          msg.show();
          Console.WriteLine("shutting down the process");
          Console.WriteLine("\n");
          Console.WriteLine("Now you cant use the process again");
          Console.WriteLine("\n");
          break;
        }
        if (msg.command == "filelistsent")
        {
          Dictionary<string, string> dict = handlebuildrequest(msg);
          Console.WriteLine(" Message recieved from:  " + msg.from); Console.WriteLine("\n");
          msg.show(); Console.WriteLine("\n");
          Console.WriteLine("\n creating dll file ......\n");
          List<string> files = generatedllfiles(filelist, dict, ServiceEnvironment.fileStorage + "/" + builderId, msg);
          generatetestrequest(files, msg);
        }
      }
    }

    //<----------------------------------used for generating the test request------------------------------------>
    public void generatetestrequest(List<string> outputfilelist, CommMessage msg)
    {
      BuildRequest newRequest = buildRequestContent.FromXml<BuildRequest>();
      TestRequest testRequest = new TestRequest();
      testRequest.author = "Ramteja Repaka";
      List<string> testDriverList = new List<string>();
      foreach (BuildItem item in newRequest.Builds)
      {
        TestElement element = new TestElement();
        element.testName = item.builddesc;
        string testdrivername = null;
        foreach (file f in item.driver)
        {
          String s = f.name;
          s = s.Replace(".cs", ".dll");
          element.addDriver(s);
          testdrivername = s;
        }
        foreach (file f in item.sourcefiles)
        {
          String s = f.name;
          s = s.Replace(".cs", ".dll");
          element.addCode(s);
        }
        if (outputfilelist.Contains(testdrivername))
        {
          testRequest.tests.Add(element);
          testDriverList.Add(testdrivername);
        }
      }
      string testxml = testRequest.ToXml();
      string filename = msg.filename;
      filename = filename.Replace("BuildRequest", "TestRequest");
      File.WriteAllText(ServiceEnvironment.fileStorage + "/" + builderId + "/" + filename, testxml);
      Console.WriteLine("TestRequest saved to BuilderStorage\n");
      sendfilestoharness(testDriverList, filename);
    }

    //<------------- sends Test request and dll files to Mock test harness---------------------->
    private void sendfilestoharness(List<string> driverlist, string filename)
    {
      Console.WriteLine("\nSending the Test Request and dll files to the Mock Test harness  DLLRepository Folder \n");
      string source = "../../../CoreBuilder/Builderstorage/" + builderId;
      string temp = builderId;
      temp = temp.Replace("Builder", "");
      temp = temp + "Files";
      string destination = "../../../MockTestHarness/DLLRepository/" + temp;
      this.comm.transferfile(filename, source, destination, harnessaddress);
      foreach (string str in driverlist)
      {
        this.comm.transferfile(str, source, destination, harnessaddress);
      }
      Console.WriteLine("\nDll Files and Test Request sent to " + destination); Console.WriteLine("\n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "executedll";
      csndMsg.author = "RamaTejaRepaka";
      csndMsg.to = harnessaddress;
      csndMsg.from = myAddress;
      csndMsg.filename = filename;
      csndMsg.builderId = temp;
      csndMsg.arguments = driverlist;
      Console.WriteLine("Sending the request to Mock Test Harness");
      Console.WriteLine("\n");
      this.comm.postMessage(csndMsg);
      csndMsg.show(); Console.WriteLine("\n");

    }

    //<-------------------contains the core logic to generate dll files using process class---------------------->
    public List<string> generatedllfiles(List<string> Files, Dictionary<string, string> dict, string dir, CommMessage msg)
    {
      string filecontent = "";
      filecontent = filecontent + "\n\n############################## start of the Build request ##############################\n\n"; List<string> outputfilelist = new List<string>(); deletetempfiles(dir);
      foreach (string file in Files)
      {
        Process first = new Process(); string dllfile = file; dllfile = dllfile.Replace(".cs", ".dll");
        try
        {
          string temp = null;
          if (file.Contains("Driver"))
          {
            temp = file + "   " + dict[file]; var frameworkPath = RuntimeEnvironment.GetRuntimeDirectory();
            var cscPath = Path.Combine(frameworkPath, "csc.exe"); first.StartInfo.FileName = cscPath;
            first.StartInfo.Arguments = "/warn:4  /target:library " + temp; first.StartInfo.UseShellExecute = false;
            first.StartInfo.WorkingDirectory = dir; first.StartInfo.RedirectStandardOutput = true; first.StartInfo.RedirectStandardError = true; first.Start();
            string output = first.StandardOutput.ReadToEnd(); string error = first.StandardError.ReadToEnd();
            if (output != null)
            {
              string[] tempFiles = Directory.GetFiles(dir, "*.dll");
              for (int i = 0; i < tempFiles.Length; ++i)
              {
                tempFiles[i] = Path.GetFileName(tempFiles[i]);
              }
              if (tempFiles.Contains(dllfile))
              {
                outputfilelist.Add(dllfile); Console.WriteLine("Build Successful For command : " + "  csc  /target:library  " + temp); Console.WriteLine("\n"); Console.WriteLine(" Generated-------->" + dllfile + "  in  " + dir); Console.WriteLine("\n");
              }
              else
              {
                Console.WriteLine("BUILDNOTSUCCESSFULL:" + temp); Console.WriteLine("\n"); Console.WriteLine(output);
              }
            }
            if (error != null)
              Console.WriteLine(error);
            filecontent = createlogfile(output, error, temp, dir, dllfile, filecontent); first.Close();
          }
        }
        catch (Exception e)
        {
          Console.WriteLine("The Exception caught is " + e.Message);
        }
      }
      filecontent = filecontent + "\n\n"; filecontent = filecontent + "\n\n ############################### end of the Build request ########################## \n\n";
      using (StreamWriter sw = File.AppendText(dir + "/" + builderId + ".log"))
      {
        sw.Write(filecontent);
      }
      sendlogtorepository(filecontent, builderId, msg); filecontent = null;
      return outputfilelist;
    }

    //<-------------------- function that handles sending build logs to the repository ----------------------->
    void sendlogtorepository(string filecontent, string filename, CommMessage msg)
    {
      Console.WriteLine("\n sending the logs for the build request file " + msg.filename);
      Console.WriteLine("\n\n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "buildlog";
      csndMsg.author = "RamaTejaRepaka";
      csndMsg.to = mockrepoaddress;
      csndMsg.from = myAddress;
      csndMsg.filename = msg.filename;
      csndMsg.builderId = builderId;
      csndMsg.filecontent = filecontent;
      Console.WriteLine("\n");
      this.comm.postMessage(csndMsg);
      csndMsg.show(); Console.WriteLine("\n\n");
    }

    //<--------- function for appending the output of build to a log file ----------------->
    string createlogfile(string output, string error, string tempcommand, string dir, string dllfile, string filecontent)
    {
      filecontent = filecontent + "\n\n";
      filecontent = filecontent + "\n\n ######################### log start for the Test driver ########################### \n\n";
      filecontent = filecontent + "\n\n";
      filecontent = filecontent + "\n the command and output that is executed is shown below\n";
      filecontent = filecontent + "the time stamp is " + DateTime.Now.ToFileTime() + "\n\n";

      filecontent = filecontent + "\n  csc  /target:library  " + tempcommand;
      if (error != null)
      {
        filecontent = filecontent + "\n\n";
        filecontent = filecontent + error;
        filecontent = filecontent + "\n\n";
      }

      if (output != null)
      {
        filecontent = filecontent + "\n\n";
        filecontent = filecontent + "Generated-------->" + dllfile + "  in  " + dir;
        filecontent = filecontent + "\n\n";
      }

      filecontent = filecontent + "\n\n ############################# log end for the Test driver ##################################### \n\n";
      return filecontent;
    }

    //<---------- function used to display the recieved xml content------------------------->
    void showRecievedXml(CommMessage msg)
    {
      Console.WriteLine("\n");
      Console.WriteLine("The Recieved file contents are\n");
      Console.WriteLine("\n");
      string filecontent = System.IO.File.ReadAllText(ServiceEnvironment.fileStorage + "/" + builderId + "/" + msg.filename);
      Console.WriteLine(filecontent);
      Console.WriteLine("\n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "filelist";
      csndMsg.author = "RamaTejaRepaka";
      csndMsg.to = mockrepoaddress;
      csndMsg.from = myAddress;
      List<string> fileslist = sourcefilelist(filecontent);
      csndMsg.arguments = fileslist;
      csndMsg.filename = msg.filename;
      csndMsg.builderId = builderId;
      Console.WriteLine("\nRequesting for all the source files in the request\n");
      Console.WriteLine("\n");
      this.comm.postMessage(csndMsg);
      csndMsg.show(); Console.WriteLine("\n");
    }

    //<------------ function responsbible for handling BuildRequest ------------------------->
    Dictionary<string, string> handlebuildrequest(CommMessage msg)
    {
      string filecontent = System.IO.File.ReadAllText(ServiceEnvironment.fileStorage + "/" + builderId + "/" + msg.filename);
      buildRequestContent = filecontent;
      Dictionary<string, string> dict = processbuildrequest(filecontent);
      return dict;
    }

    //<---------- function which returns source file list ------------------->
    List<string> sourcefilelist(string xmlstring)
    {
      List<string> list = new List<string>();
      BuildRequest newRequest = xmlstring.FromXml<BuildRequest>();
      foreach (BuildItem item in newRequest.Builds)
      {
        foreach (file f in item.driver)
        {
          list.Add(f.name);
        }
        foreach (file f in item.sourcefiles)
        {
          list.Add(f.name);
        }
      }
      return list;
    }

    //<------------------ Contains core logic to process build request-----------------------------------> 
    public Dictionary<string, string> processbuildrequest(string xmlstring)
    {
      Dictionary<string, string> dict = new Dictionary<string, string>();
      BuildRequest newRequest = xmlstring.FromXml<BuildRequest>();
      foreach (BuildItem item in newRequest.Builds)
      {
        string child = "";
        List<string> parent = new List<string>();
        foreach (file f in item.driver)
        {
          parent.Add(f.name);
          filelist.Add(f.name);

        }
        foreach (file f in item.sourcefiles)
        {
          child = child + f.name + "  ";
          filelist.Add(f.name);
        }
        foreach (string str in parent)
        {
          dict.Add(str, child);
        }
      }
      return dict;
    }


    //<------------ function responsible for sending ready messages to MotherBuilder---------------------->
    void sendReadyMessage(string tosend)
    {
      Console.WriteLine("\n");
      Console.WriteLine("Processing completed.........");
      Console.WriteLine("\n");
      Console.WriteLine("Sending ready message to   " + tosend);
      Console.WriteLine("\n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "I am Ready";
      csndMsg.author = "RamaTejaRepaka";
      csndMsg.to = tosend;
      csndMsg.from = myAddress;
      comm.postMessage(csndMsg);
      csndMsg.show();
    }

    //<--------function responsible for handling builds by sending file requests to the Mock Repository------------>
    Boolean handlebuilds(CommMessage msg)
    {
      msg.show();
      Console.WriteLine("\n");
      Console.WriteLine("Processing the message.......\n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "Please send the specified file in the Message";
      csndMsg.author = "RamaTejaRepaka";
      csndMsg.to = mockrepoaddress;
      csndMsg.from = myAddress;
      csndMsg.filename = msg.filename;
      csndMsg.builderId = builderId;
      Console.WriteLine("Sending the request to Mock Repository");
      Console.WriteLine("\n");
      this.comm.postMessage(csndMsg);
      csndMsg.show(); Console.WriteLine("\n");
      return true;
    }

    //<-------- function that is responsible for creating a thread which keeps on listening for incoming messages --------->
    public void startchildprocess()
    {
      msgHandler = new Thread(HandlerThreadProc);
      msgHandler.Start();
    }
  }

  //<------------------------------------------TEST STUB----------------------------------------------------->
#if (Test_CoreBuilder)

  class TestCoreBuilder
  {
    //driver logic
    static void Main(string[] args)
    {
      Console.WriteLine("-------------------------------------Start of Core Builder" + args[0] + "  at" + " " + args[1] + "------------------------------------");
      Console.WriteLine("\n");
      Console.WriteLine("Listening for Messages ..........");
      Console.WriteLine("\n");
      int port = int.Parse(args[1]);
      int mockport = int.Parse(args[2]);
      int harnessport = int.Parse(args[3]);
      int mockclientport = int.Parse(args[4]);
      CoreBuilder builder = new CoreBuilder(port, mockport, harnessport, mockclientport);
      Console.Title = "Builder";
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.DarkBlue;
      Console.Write("\n Child Process Created");
      Console.Write("\n ====================");
      if (args.Count() == 0)
      {
        Console.Write("\n  please enter integer value on command line");
        return;
      }
      else
      {
        Console.Write("\nFrom child process #{0}\n\n", args[0]);
        builder.startchildprocess();
        Console.WriteLine("\n");
      }
    }
#endif
  }
}
