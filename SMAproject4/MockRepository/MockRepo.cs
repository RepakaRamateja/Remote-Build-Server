///////////////////////////////////////////////////////////////////////////////////////////////////
// MockRepo.cs - Demonstrate RepoMock  operations                                                //
// ver 1.0                                                                                       //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                       //
// Application: CSE681 Project 4-Remote Build Server                                             //
// Environment: C# console                                                                       //
///////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ====================
 * Demonstrates Mock Repo  operations like  listenforincomingmessages , sending Mother builder BuildRequest messages
 * and handlingincomingmessages,validating xml request,storing the logs send by child builders and mock test harness,handling file request both xml and source files
 * and also handling log requests from mock client
 * 
 *  Public Interface
 * ----------------
 * MockRepo repo = new MockRepo(); //for doing mock repo operations 
 * repo.sendrequest(); //sending the request to the Mother Builder
 * repo.handleincomingmessages();//handles incoming messages to Mock Repository
 * repo.sendbuildermessage();//sends reply messages to the sender i.e is here Mother builder
 * repo.listenforincomingmessages();//used to create a  thread for handling incoming messages
 * repo.deleteallfiles(string path);//contains logic to delete all files in a directory
 * repo.deletefiles(string path, string pattern);//contains logic to delete some files in a directory with given pattern
 * repo.handleincomingmessages();// function that handles incoming messages to Mock Repository
 * repo.listenforincomingmessages();//function used to create a  thread for handling incoming messages
 * repo.sendbuildermessage();//function that sends reply messages to the sender i.e is here Mother builder
 * repo.handlefilelistrequest();//function that handles the request filelist
 * repo.handlerepocontentlist();//function that sends the repo contents List 
 * repo.handlexmlfilelist();//function that sends the repo contents xml file List
 * repo.sendmessagetobuildserver();//function forwards the recieved build request msg to build server
 * repo.parsebuildmessage();//contains logic for parsing buildmessage
 * repo.validatebuildrequest();//function that mainly validates the build request
 * repo.savesbuildlog; //saves the build log sent by child builders 
 * repo.savestestlog; //saves the Test log sent by Mock Test Harness
 * repo.sendlogstoclient; //send the logs to the Mock client
 * repo.sendlogfilelist; //sends list of requested log files to Mock client
 * 
 * Required Files:
 * ---------------
 * MockRepo.cs, MPCommService.cs, IMPCommService.cs
 * 
 *  Build Process
 * --------------
 * Compiler command: csc  MockRepo.cs MPCommService.cs IMPCommService.cs
 * devenv MockRepository.csproj 
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 6 December 2017
 * - first release
 */
using Build_Request;
using MessagePassingComm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace MockRepository
{
  //class which contains operations related to MockRepo like handling incoming messages, sending requests
  class MockRepo
  {
    Comm comm = null;
    private static int startport { get; set; } = 0;
    private static string repoaddress { get; set; } = null;
    private static string serveraddress { get; set; } = null;
    string MockRepoLocation = "../../../MockRepository/RepoStorage";
    Thread msgHandler;

    //<-------------constructor which takes mock repo starting port and serverport------------------>
    MockRepo(int startport, int serverport)
    {
      comm = new Comm("http://localhost", startport);
      repoaddress = "http://localhost:" + startport.ToString() + "/IMessagePassingComm";
      serveraddress = "http://localhost:" + serverport.ToString() + "/IMessagePassingComm";
      deleteallfiles("../../../MockRepository/RepoStorage/ChildBuildersLogs");
      deleteallfiles("../../../MockRepository/RepoStorage/TestHarnessLogs");
      deleteallfiles("../../../MockRepository/RepoStorage/BuildRequestLogs");
      deletefiles("../../../MockRepository/RepoStorage", "*.xml");
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

    //<----------contains logic to delete some files in a directory with given pattern--------------------------------->
    public void deletefiles(string path, string pattern)
    {
      string[] picList = Directory.GetFiles(path, pattern);
      foreach (string f in picList)
      {
        File.Delete(f);
      }
    }

    //<--------------function that handles incoming messages to Mock Repository--------------------->
    public void handleincomingmessages()
    {
      while (true)
      {
        CommMessage msg = comm.getMessage();
        if (msg.command == "Please send the specified file in the Message")
        {
          Console.WriteLine("\n");
          Console.WriteLine("Mock Repository recieved message from  " + msg.from);
          msg.show(); Console.WriteLine("\n");
          Console.WriteLine("Mock repository processing the Request"); Console.WriteLine("\n");
          bool val = this.comm.sendfile(msg.filename, msg.from, msg.builderId);
          Console.WriteLine("File Transfer status  " + val);
          Console.WriteLine("\n");
          sendbuildermessage(msg);
        }

        if (msg.command == "BuildRequestFileName")
          sendmessagetobuildserver(msg);

        if (msg.command == "filelist")
        {
          Console.WriteLine("\n");
          Console.WriteLine("Mock Repository recieved message from  " + msg.from);
          msg.show(); Console.WriteLine("\n");
          handlefilelistrequest(msg);
        }

        if (msg.command == "Repocontents")
          handlerepocontentlist(msg);
        if (msg.command == "sendxmlfilelist")
          handlexmlfilelist(msg);
        if (msg.command == "filexmlrequest")
          validatebuildrequest(msg);
        if (msg.command == "buildlog")
          savesbuildlog(msg);
        if (msg.command == "Testlog")
          savestestlog(msg);
        if (msg.command == "logs")
          sendlogstoclient(msg);
        if (msg.command == "logfilelist")
          sendlogfilelist(msg);
      }
    }

    //<--------------- sends list of requested log files to Mock client ------------------------------>
    private void sendlogfilelist(CommMessage msg)
    {
      List<string> filelist = msg.arguments;
      Console.WriteLine("\n Mock repository processing the log filelist Request "); Console.WriteLine("\n");
      foreach (string str in filelist)
      {
        Console.WriteLine("\n Mock repository sending  the file  " + str + "to " + msg.from);
        bool val = this.comm.transferfile(str, "../../../MockRepository/RepoStorage/BuildRequestLogs", "../../../MockClient/ClientStorage/Downloadedlogs", msg.from);
        if (val == false)
          val = this.comm.transferfile(str, "../../../MockRepository/RepoStorage/TestHarnessLogs", "../../../MockClient/ClientStorage/Downloadedlogs", msg.from);
        Console.WriteLine("\nFile Transfer status  " + val);
        Console.WriteLine("\n");
      }
      Console.WriteLine("Sending  sucess status of file list request " + msg.from);
      Console.WriteLine("\n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "logfilelistsent";
      csndMsg.author = "RamaTeja";
      csndMsg.filename = msg.filename;
      csndMsg.to = msg.from;
      csndMsg.from = msg.to;
      this.comm.postMessage(csndMsg);
      csndMsg.show();
    }

    //<---------------------------- send the logs to the Mock client--------------------------------->
    private void sendlogstoclient(CommMessage msg)
    {
      Console.WriteLine("\n");
      Console.WriteLine("Mock Repository recieved message from  " + msg.from);
      msg.show(); Console.WriteLine("\n");
      string path = "../../../MockRepository/RepoStorage/BuildRequestLogs";
      // string[] fileList = Directory.GetFiles(path, "*.log",SearchOption.AllDirectories);
      string[] buildlogList = Directory.GetFiles(path, "*.log");
      List<string> list = new List<string>();
      for (int i = 0; i < buildlogList.Length; i++)
      {
        string filename = Path.GetFileName(buildlogList[i]);
        list.Add(filename);
      }
      string[] TestlogList = Directory.GetFiles("../../../MockRepository/RepoStorage/TestHarnessLogs", "*.log");
      for (int i = 0; i < TestlogList.Length; i++)
      {
        string filename = Path.GetFileName(TestlogList[i]);
        list.Add(filename);
      }
      Console.WriteLine("\n Sending  the log files list to Mock client " + msg.from);
      Console.WriteLine("\n\n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "loglist";
      csndMsg.author = "RamaTeja";
      csndMsg.to = msg.from;
      csndMsg.from = repoaddress;
      csndMsg.arguments = list;
      this.comm.postMessage(csndMsg);
      csndMsg.show();
    }

    //<-------------- saves the Test log sent by Mock Test Harness ------------------------------------------>
    private void savestestlog(CommMessage msg)
    {
      Console.WriteLine("\n");
      Console.WriteLine("Mock Repository recieved message from  " + msg.from);
      msg.show(); Console.WriteLine("\n");
      Console.WriteLine("\n The recieved Test logs for test request name --> " + msg.filename);
      Console.WriteLine(msg.filecontent);
      string timestamp = "\n####################  Starting of a test ############################\n\n";
      timestamp = timestamp + "The time when Test harness executed these logs is" + DateTime.Now.ToFileTime();
      timestamp = timestamp + "\n\n\n";
      msg.filecontent = timestamp + msg.filecontent;
      msg.filecontent = msg.filecontent + "\n####################   Ending of a test ##################################\n\n";
      using (StreamWriter sw = File.AppendText(MockRepoLocation + "/" + "TestHarnessLogs" + "/" + msg.filename + ".log"))
      {
        sw.Write(msg.filecontent);
      }
      Console.WriteLine("\n");
      Console.WriteLine("Test log could be found at the location ---->" + MockRepoLocation + "/TestHarnessLogs"); Console.WriteLine("\n");
    }

    //<-------------- saves the build log sent by child builders ------------------------------------------>
    private void savesbuildlog(CommMessage msg)
    {
      Console.WriteLine("\n");
      Console.WriteLine("Mock Repository recieved message from  " + msg.from);
      msg.show(); Console.WriteLine("\n");
      Console.WriteLine("\n The recieved build logs for build request name --> " + msg.filename);
      Console.WriteLine(msg.filecontent);
      string temppfile = msg.filename;
      temppfile = temppfile.Replace(".xml", "");
      File.WriteAllText(MockRepoLocation + "/" + "BuildRequestLogs" + "/" + temppfile + ".log", msg.filecontent);
      using (StreamWriter sw = File.AppendText(MockRepoLocation + "/" + "ChildBuildersLogs" + "/" + msg.builderId + ".log"))
      {
        sw.Write(msg.filecontent);
      }
      Console.WriteLine("\n");
      Console.WriteLine("Build log could be found at the location ---->" + MockRepoLocation + "/BuildLogs"); Console.WriteLine("\n");
    }


    // <------------- function that mainly validates the build request------------------->
    private void validatebuildrequest(CommMessage msg)
    {
      Console.WriteLine("\n");
      Console.WriteLine("Mock Repository recieved message from  " + msg.from);
      msg.show(); Console.WriteLine("\n");
      Console.WriteLine("\n The recieved build request is displayed below \n");
      Console.WriteLine(msg.filecontent);
      Console.WriteLine("\n\n");
      string filecontent = msg.filecontent;
      BuildRequest buildRequest = filecontent.FromXml<BuildRequest>();
      string path = "../../../MockRepository/RepoStorage";
      string[] fileList = Directory.GetFiles(path, "*.cs");
      List<string> list = new List<string>();
      for (int i = 0; i < fileList.Length; i++)
      {
        string filename = Path.GetFileName(fileList[i]);
        list.Add(filename);
      }
      Boolean validation = parsebuildmessage(buildRequest, list);
      if (validation == true)
      {
        File.WriteAllText(path + "/" + msg.filename, msg.filecontent);
        Console.WriteLine("Build Request saved in the mockrepo  repositorystorage whose path is ---->  " + "../../../MockRepository/RepoStorage");
        Console.WriteLine("\n");
        Console.WriteLine("The validation status is" + validation + "\n"); Console.WriteLine("\n");
        Console.WriteLine("Sending  validation status msg to mock client " + msg.from);
        Console.WriteLine("\n");
        CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
        csndMsg.command = "Validation passed";
        csndMsg.author = "RamaTeja";
        csndMsg.to = msg.from;
        csndMsg.from = repoaddress;
        csndMsg.filename = msg.filename;
        this.comm.postMessage(csndMsg);
        csndMsg.show();
      }
    }

    //<--------------contains logic for parsing buildmessage----------------------------------->
    public Boolean parsebuildmessage(BuildRequest request, List<string> dircontents)
    {
      Boolean found = true;
      Console.WriteLine("\n");
      Console.WriteLine("-------------------------------------------Mock Repo parsing build message----------------------------------------"); Console.WriteLine("\n");
      Console.WriteLine("\n");
      foreach (BuildItem item in request.Builds)
      {
        foreach (file f in item.driver)
        {
          if (dircontents.Contains(f.name))
          {
            Console.WriteLine("test driver: " + f.name + "  found"); Console.WriteLine("\n");
          }
          else
          {
            found = false;
          }
        }
        foreach (file f in item.sourcefiles)
        {
          if (dircontents.Contains(f.name))
          {
            Console.WriteLine("source file: " + f.name + "  found"); Console.WriteLine("\n");
          }

          else
          {
            found = false;
          }
        }
      }

      if (found == false)
      {
        Console.WriteLine("Build request validation failed"); Console.WriteLine("\n");
        return false;
      }

      else
      {
        Console.WriteLine("Build request validation passed"); Console.WriteLine("\n");
        return true;
      }
    }


    //<------------------ function forwards the recieved build request msg to build server------------------>
    private void sendmessagetobuildserver(CommMessage msg)
    {
      Console.WriteLine("\n");
      Console.WriteLine("Mock repo recived request from Mock client");
      Console.WriteLine("\n");
      msg.show();
      Console.WriteLine("forwarding the request to Build server");
      CommMessage csndMsg = msg;
      csndMsg.to = serveraddress;
      csndMsg.from = repoaddress;
      this.comm.postMessage(csndMsg);
      csndMsg.show();
    }

    // <-------------------  function that sends the repo contents xml file List ------------------------------------->
    private void handlexmlfilelist(CommMessage msg)
    {
      Console.WriteLine("\n");
      Console.WriteLine("Mock Repository recieved message from  " + msg.from);
      msg.show(); Console.WriteLine("\n");
      string path = "../../../MockRepository/RepoStorage";
      string[] fileList = Directory.GetFiles(path, "*.xml");
      List<string> list = new List<string>();
      for (int i = 0; i < fileList.Length; i++)
      {
        string filename = Path.GetFileName(fileList[i]);
        list.Add(filename);
      }
      Console.WriteLine("\n Sending  the xml repo list to Mock client " + msg.from);
      Console.WriteLine("\n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "getXmlFiles";
      csndMsg.author = "RamaTeja";
      csndMsg.to = msg.from;
      csndMsg.from = repoaddress;
      csndMsg.arguments = list;
      this.comm.postMessage(csndMsg);
      csndMsg.show();
    }

    //<-------------------  function that sends the repo contents List ------------------------------------->
    private void handlerepocontentlist(CommMessage msg)
    {
      Console.WriteLine("\n");
      Console.WriteLine("Mock Repository recieved message from  " + msg.from);
      msg.show(); Console.WriteLine("\n");
      string path = "../../../MockRepository/RepoStorage";
      string[] fileList = Directory.GetFiles(path, "*.cs");
      List<string> list = new List<string>();
      for (int i = 0; i < fileList.Length; i++)
      {
        string filename = Path.GetFileName(fileList[i]);
        list.Add(filename);
      }
      Console.WriteLine("\n Sending  the repo list to Mock client " + msg.from);
      Console.WriteLine("\n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "getFiles";
      csndMsg.author = "RamaTeja";
      csndMsg.to = msg.from;
      csndMsg.from = repoaddress;
      csndMsg.arguments = list;
      this.comm.postMessage(csndMsg);
      csndMsg.show();
    }


    //<----------- function that handles the request filelist-------------------------->
    private void handlefilelistrequest(CommMessage msg)
    {
      List<string> filelist = msg.arguments;
      Console.WriteLine("\n Mock repository processing the filelist Request "); Console.WriteLine("\n");
      foreach (string str in filelist)
      {
        Console.WriteLine("\n Mock repository sending  the file  " + str + "to " + msg.from);
        bool val = this.comm.sendfile(str, msg.from, msg.builderId);
        Console.WriteLine("\nFile Transfer status  " + val);
        Console.WriteLine("\n");
      }
      Console.WriteLine("Sending  sucess status of file list request " + msg.from);
      Console.WriteLine("\n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "filelistsent";
      csndMsg.author = "RamaTeja";
      csndMsg.filename = msg.filename;
      csndMsg.to = msg.from;
      csndMsg.from = msg.to;
      this.comm.postMessage(csndMsg);
      csndMsg.show();
    }


    //<--------------function that sends reply messages to the sender i.e is here Mother builder------>
    public void sendbuildermessage(CommMessage msg)
    {
      Console.WriteLine("Sending Reply Message to " + msg.from);
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "filesent";
      csndMsg.author = "RamaTeja";
      csndMsg.filename = msg.filename;
      csndMsg.to = msg.from;
      csndMsg.from = msg.to;
      this.comm.postMessage(csndMsg);
      csndMsg.show();
    }

    //<-----function used to create a  thread for handling incoming messages----------------->
    public void listenforincomingmessages()
    {
      msgHandler = new Thread(handleincomingmessages);
      msgHandler.Start();
    }




    //<------------------------------------------TEST STUB----------------------------------------------------->
#if (TEST_MockRepo)

    //Driver Logic
    static void Main(string[] args)
    {
      int startport = int.Parse(args[0]);
      int serverport = int.Parse(args[1]);
      Console.Title = "Mock Repository";
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.DarkBlue;
      MockRepo mock = new MockRepo(startport, serverport);
      Console.WriteLine("-----------------------------Start of Mock Repository at " + args[0] + " -------------------------------------");
      Console.WriteLine("\n");
      Console.WriteLine("\n Mockrepo repositorystorage relative path is  " + " ../../../MockRepository/RepoStorage \n"); Console.WriteLine("\n");
      Console.WriteLine("Listens for incoming Comm messages\n");
      mock.listenforincomingmessages();
    }
#endif

  }
}




