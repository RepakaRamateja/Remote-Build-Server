/////////////////////////////////////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs - Demonstrate all MainWindow.xaml.cs operations                              //
// Ver :1.0                                                                                        //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                         //
// Application: CSE681 CSE681 Project 4-Remote Build Server                                        //
// Environment: C# console                                                                         //
/////////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ---------------------
 * Demonstrates operations for generating build items and adding them to the build request,
 * sending the generated build requests for validation to mock repository and also sending the generated
 * build request for processing to mock repository,it also contains the automated functions to demonstrate the requirements 
 * 
 * Public Interface
 * ------------------
 * It does not contain any public methods
 * But only one public constructor MainWindow --- which initializes the main component through method InitializeComponent
 * 
 * Required Files:
 * ---------------
 * BuildRequest.cs, App.xaml, MainWinodw.xaml, MainWinodw.xaml.cs, CodePopUp.xaml
 * 
 * Build Process
 * ---------------
 * csc BuildRequest App.xaml MainWinodw.xaml MainWinodw.xaml.cs CodePopUp.xaml
 * devenv MockClient.csproj
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 6th December 2017
 * - first release
 * 
 */
using Build_Request;
using MessagePassingComm;
using Navigator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Utilities;
using WinForms = System.Windows.Forms;


namespace MockClient
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary> // class responsible for all the creating all the GUI
  public partial class MainWindow : Window
  {
    BuildRequest buildrequest = new BuildRequest();
    private Comm comm;
    private string MockRepoaddress { get; set; } = "http://localhost:8080/IMessagePassingComm";
    Thread rcvThread = null;
    Dictionary<string, Action<CommMessage>> messageDispatcher = new Dictionary<string, Action<CommMessage>>();

    //constructor which initializes the mainwindow through method InitializeComponent 
    public MainWindow()
    {
      InitializeComponent();
      deleteallfiles("../../../MockClient/ClientStorage/Downloadedlogs");
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

    //<----------contains logic to delete some files in a directory with the given extension--------------------------------->
    public void deletesomefiles(string path, string ext)
    {
      string[] picList = Directory.GetFiles(path, ext);
      foreach (string f in picList)
      {
        File.Delete(f);
      }
    }

    //<------function i.e window Loaded Event handler which automates the Requirements demonstration with out manual input---------------> 
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      comm = new Comm("http://localhost", 8010);
      Console.WriteLine("------------------------- Mock client started at address " + comm.ReceiverAddress + "  -------------------");
      initializeMessageDispatcher();
      rcvThread = new Thread(rcvThreadProc);
      rcvThread.Start();
      Createbuildrequest(sender, e);// used to create the build request
      sendbuildRequests(sender, e);// used to send created build requests to Mock Repository
      sendRequestsforprocessing(sender, e); // for requesting the repository to send a build request in its storage to the Build Server for build processing
    }

    //----< define how to process each message command >-------------
    void initializeMessageDispatcher()
    {
      // load remoteFiles listbox with files from root
      messageDispatcher["getFiles"] = (CommMessage msg) =>
      {
        driverlist.Items.Clear();
        foreach (string dir in msg.arguments)
        {
          driverlist.Items.Add(dir);
        }
        filelist.Items.Clear();
        foreach (string dir in msg.arguments)
        {
          filelist.Items.Add(dir);
        }
      };
      // load RemoteBuildrequests listbox with xmlfiles from root
      messageDispatcher["getXmlFiles"] = (CommMessage msg) =>
      {
        RemoteBuildrequests.Items.Clear();
        foreach (string dir in msg.arguments)
        {
          RemoteBuildrequests.Items.Add(dir);
        }
      };

      // used for handling validation success msg for further improvements
      messageDispatcher["Validation passed"] = (CommMessage msg) =>
      {
        //In future enhancements  it will be used to notify the client gui 

      };

      // used for handling validation success msg for further improvements
      messageDispatcher["loglist"] = (CommMessage msg) =>
      {
        logFileList.Items.Clear();
        foreach (string dir in msg.arguments)
        {
          logFileList.Items.Add(dir);
        }

      };
      // used for handling log sent status in future enhancement
      messageDispatcher["logfilelistsent"] = (CommMessage msg) =>
      { };
    }

    //----< define processing for GUI's receive thread >-------------
    void rcvThreadProc()
    {
      Console.Write("\n  starting client's receive background thread \n");
      while (true)
      {
        CommMessage csndMsg = comm.getMessage();
        Console.WriteLine("\n The Comm message recieved at the Mock client \n");
        csndMsg.show();
        if (csndMsg.command == null)
          continue;
        // pass the Dispatcher's action value to the main thread for execution
        Dispatcher.Invoke(messageDispatcher[csndMsg.command], new object[] { csndMsg });
      }
    }


    //<--- automated function used for demonstarting build request generation with out manual input--------->
    private void Createbuildrequest(object sender, RoutedEventArgs e)
    {
      Console.WriteLine("\n --------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 11");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\n");
      Console.WriteLine("\n creating the Build Item \n");
      Console.WriteLine("\n Selecting the test driver \n");
      driverlist.Items.Add("TestDriver1.cs");
      Console.WriteLine("\n Selecting the source files\n");
      filelist.Items.Add("TestedOne.cs");
      filelist.Items.Add("TestedTwo.cs");
      generaterequest(sender, e);
      driverlist.Items.Clear();
      filelist.Items.Clear();
      Console.WriteLine("\n Adding the build item to the build Request \n");
      Console.WriteLine("\n creating the second Build Item \n");
      Console.WriteLine("\n Selecting another test driver \n");
      driverlist.Items.Add("SecondTestDriver.cs");
      Console.WriteLine("\n Selecting the source files for the test driver\n");
      filelist.Items.Add("Interfaces.cs");
      filelist.Items.Add("TestedLib.cs");
      filelist.Items.Add("TestedLibDependency.cs");
      Console.WriteLine("\n Adding the new build item to the existing build Request \n ");
      generaterequest(sender, e);
      filename.Text = "TestBuildRequest1.xml";
      savecontents(sender, e);
      Console.WriteLine("\n Thus it clearly demonstrates the Requirement 11 \n");
      Console.WriteLine("\n Note: you can do it manually again by following the steps in test executive \n");
    }

    //<-- automated function used for sending build requests generated to mock repository with out manual input (Req 12) ------>
    private void sendbuildRequests(object sender, RoutedEventArgs e)
    {
      Console.WriteLine("\n --------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 12");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\n");
      Console.WriteLine("\n Selecting Build Requests \n");
      Console.WriteLine("\n BuildRequest1.xml selected \n");
      localbuildrequests.Items.Add("BuildRequest1.xml");
      Console.WriteLine("\n BuildRequest2.xml selected \n");
      localbuildrequests.Items.Add("BuildRequest2.xml");
      Console.WriteLine("\n BuildRequest3.xml selected \n");
      localbuildrequests.Items.Add("BuildRequest3.xml");
      sendbuildrequests(sender, e);
      Console.WriteLine("\n Now check the validation messages from the mock repository in this console\n");
      Console.WriteLine("\n Also check in the Mock Repository console\n");
      Console.WriteLine("\n Thus it clearly demonstrates requirement 12");
      Console.WriteLine("\n Note: you can also try manually again by following steps in test executive \n");
    }

    //<--- automated function used to send request the repository to send a build request in its storage to the Build Server for build processing 
    private void sendRequestsforprocessing(object sender, RoutedEventArgs e)
    {
      Console.WriteLine("\n --------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 13");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\n");
      Console.WriteLine("\n Selecting Build Requests \n");
      Console.WriteLine("\n BuildRequest1.xml selected \n");
      RemoteBuildrequests.Items.Add("BuildRequest1.xml");
      Console.WriteLine("\n BuildRequest2.xml selected \n");
      RemoteBuildrequests.Items.Add("BuildRequest2.xml");
      Console.WriteLine("\n BuildRequest3.xml selected \n");
      RemoteBuildrequests.Items.Add("BuildRequest3.xml");
      sendbuildCommMessage(sender, e);
      Console.WriteLine("\n Now check the  messages send to the mock repository in this console\n");
      Console.WriteLine("\n Also check in the Mock Repository console\n");
      Console.WriteLine("\n Thus it clearly demonstrates requirement 13");
      Console.WriteLine("\n Note: you can also try manually again by following steps in test executive \n");
    }


    //<------ A event handler for the show selected button which shows only selected items in the listbox
    private void getselectedsource(object sender, RoutedEventArgs e)
    {
      List<object> filenames = new List<object>();
      foreach (var item in filelist.SelectedItems)
      {
        filenames.Add(item);
      }
      filelist.Items.Clear();
      foreach (object obj in filenames)
      {
        filelist.Items.Add(obj);
      }
    }

    //<----A event handler for the generate button which generates build request from the values in the listbox
    private void generaterequest(object sender, RoutedEventArgs e)
    {
      BuildItem element = new BuildItem();
      List<object> filenames = new List<object>();
      foreach (var item in filelist.Items)
      {
        filenames.Add(item);
      }
      List<object> drivers = new List<object>();
      foreach (var item in driverlist.Items)
      {
        drivers.Add(item);
      }

      foreach (string item in drivers)
      {
        file one = new file();
        one.name = item;
        element.addDriver(one);
      }

      foreach (string item in filenames)
      {
        file one = new file();
        one.name = item;
        element.addCode(one);
      }
      buildrequest.Builds.Add(element);
      string xml = buildrequest.ToXml();
      Console.WriteLine("\n The build Request generated after adding the build Item is \n");
      Console.WriteLine("\n");
      Console.WriteLine(xml);
      Console.WriteLine("\n");
      XmlTextBlock.Text = xml;
    }

    //<------ A event handler for select driver button which sends a request to mock repo to get the contents
    private void getrepocontents(object sender, RoutedEventArgs e)
    {
      Console.WriteLine("\n Sending the Request to the Mock Repository to get the Repo contents");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "Repocontents";
      csndMsg.author = "RamaTeja";
      csndMsg.to = MockRepoaddress;
      csndMsg.from = comm.ReceiverAddress;
      this.comm.postMessage(csndMsg);
      csndMsg.show();
    }

    //<------ A event handler for the show selected button which shows only selected items in the listbox
    private void showselecteddrivers(object sender, RoutedEventArgs e)
    {
      List<object> filenames = new List<object>();
      foreach (var item in driverlist.SelectedItems)
      {
        filenames.Add(item);
      }
      driverlist.Items.Clear();
      foreach (object obj in filenames)
      {
        driverlist.Items.Add(obj);
      }
    }

    //<----- A event handler for the clear button which clears all the values in the GUI elements
    private void cleartextblock(object sender, RoutedEventArgs e)
    {
      XmlTextBlock.Text = "Xml Contents";
      filelist.Items.Clear();
      driverlist.Items.Clear();
      filename.Text = "Enter file name";
      buildrequest = null;
      buildrequest = new BuildRequest();
    }

    //<--------- A event handler function for save contents button which saves files MockClient Location
    private void savecontents(object sender, RoutedEventArgs e)
    {
      Console.WriteLine("\nSaving the contents\n");
      string name = filename.Text;
      string content = XmlTextBlock.Text;
      string path = "../../../MockClient/ClientStorage/" + name;
      File.WriteAllText(path, content);
      Console.WriteLine("File could be found at the path as " + path);
    }


    //<------- A event handler to get the Mock client Local generated Build Requests-----------------> 
    private void getMockclientBuildRequests(object sender, RoutedEventArgs e)
    {
      string[] BuildrequeestList = Directory.GetFiles("../../../MockClient/ClientStorage", "*.xml");
      List<string> xmlfilenames = new List<string>();
      foreach (string file in BuildrequeestList)
      {
        xmlfilenames.Add(Path.GetFileName(file));
      }
      localbuildrequests.Items.Clear();
      foreach (string xmlfile in xmlfilenames)
      {
        localbuildrequests.Items.Add(xmlfile);
      }
    }

    //<------- A event handler to get the Remote Mock Repo Xml build Requests------------------------->
    private void getremoteBuildRequests(object sender, RoutedEventArgs e)
    {
      Console.WriteLine("\n Sending the message to Mock repo for the list of xml files in Mock Repo storage \n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "sendxmlfilelist";
      csndMsg.author = "RamaTeja";
      csndMsg.to = MockRepoaddress;
      csndMsg.from = comm.ReceiverAddress;
      this.comm.postMessage(csndMsg);
      csndMsg.show();
    }

    //<-------- A event handler for show selected Requests button which shows only selected items in the listbox
    private void showselectedlocalxmlfiles(object sender, RoutedEventArgs e)
    {
      List<object> filenames = new List<object>();
      foreach (var item in localbuildrequests.SelectedItems)
      {
        filenames.Add(item);
      }
      localbuildrequests.Items.Clear();
      foreach (object obj in filenames)
      {
        localbuildrequests.Items.Add(obj);
      }
    }

    //<-------- A event handler for show selected Remote Build Requests button which shows only selected items in the listbox
    private void showselectedRemotexmlfiles(object sender, RoutedEventArgs e)
    {
      List<object> filenames = new List<object>();
      foreach (var item in RemoteBuildrequests.SelectedItems)
      {
        filenames.Add(item);
      }
      RemoteBuildrequests.Items.Clear();
      foreach (object obj in filenames)
      {
        RemoteBuildrequests.Items.Add(obj);
      }

    }

    //<---------- A event handler for sending the build request files to Mock Repository -------------------->
    private void sendbuildrequests(object sender, RoutedEventArgs e)
    {
      string dir = "../../../MockClient/ClientStorage";
      List<object> filenames = new List<object>();
      foreach (var item in localbuildrequests.Items)
      {
        filenames.Add(item);
      }
      //sending file in the form of comm message because mock repo easily validates the filecontent if valid then it saves in to its storage --------->
      foreach (var file in filenames)
      {
        string filecontent = System.IO.File.ReadAllText(dir + "/" + file);
        Console.WriteLine("\n Sending the file " + file + "  to Mock Repo for validation\n");
        Console.WriteLine("\nThe content of file " + file + " is displayed below \n");
        Console.WriteLine(filecontent);
        Console.WriteLine("\n");
        CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
        csndMsg.command = "filexmlrequest";
        csndMsg.author = "RamaTeja";
        csndMsg.to = MockRepoaddress;
        csndMsg.from = comm.ReceiverAddress;
        csndMsg.filename = file.ToString();
        csndMsg.filecontent = filecontent;
        this.comm.postMessage(csndMsg);
      }

    }

    //<-------------- A event handler for sending build comm message to mock repo which inturn sends to the build server for further processing ------->
    private void sendbuildCommMessage(object sender, RoutedEventArgs e)
    {
      List<object> filenames = new List<object>();
      foreach (var item in RemoteBuildrequests.Items)
      {
        filenames.Add(item);
      }
      foreach (var file in filenames)
      {
        Console.WriteLine("\n\nsending BuildRequest Comm Message to Mock Repository\n");
        CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
        csndMsg.command = "BuildRequestFileName";
        csndMsg.author = "RamaTeja";
        csndMsg.filename = file.ToString();
        csndMsg.to = MockRepoaddress;
        csndMsg.from = comm.ReceiverAddress;
        this.comm.postMessage(csndMsg);
        csndMsg.show(); Console.WriteLine("\n");
      }
    }

    //<------------------------- A event handler for retriving the remote logs------------------------------>
    private void getRemoteLogs(object sender, RoutedEventArgs e)
    {
      logFileList.Items.Clear();
      Console.WriteLine("\n Sending the message to Mock repo for the Logs in Mock Repo storage \n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "logs";
      csndMsg.author = "RamaTeja";
      csndMsg.to = MockRepoaddress;
      csndMsg.from = comm.ReceiverAddress;
      this.comm.postMessage(csndMsg);
      csndMsg.show(); Console.WriteLine("\n");
    }
    //<---------------------- A event handler for downloading the selected log files ---------------------->
    private void downloadlogs(object sender, RoutedEventArgs e)
    {
      List<string> filenames = new List<string>();
      foreach (string item in logFileList.SelectedItems)
      {
        filenames.Add(item);
      }
      Console.WriteLine("\n Sending the message to Mock repo asking list of log files  \n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "logfilelist";
      csndMsg.author = "RamaTeja";
      csndMsg.to = MockRepoaddress;
      csndMsg.from = comm.ReceiverAddress;
      csndMsg.arguments = filenames;
      this.comm.postMessage(csndMsg);
      csndMsg.show(); Console.WriteLine("\n");
    }

    //<-------------   A event handler for retriving the downloaded files from local repo  ---------------------->
    private void Retrivedownloadedlogfiles(object sender, RoutedEventArgs e)
    {
      string[] BuildrequeestList = Directory.GetFiles("../../../MockClient/ClientStorage/Downloadedlogs", "*.log");
      List<string> xmlfilenames = new List<string>();
      foreach (string file in BuildrequeestList)
      {
        xmlfilenames.Add(Path.GetFileName(file));
      }
      downloadedlogfiles.Items.Clear();
      foreach (string xmlfile in xmlfilenames)
      {
        downloadedlogfiles.Items.Add(xmlfile);
      }

    }

    //----< show selected file in code popup window >----------------
    private void localFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      string fileName = downloadedlogfiles.SelectedValue as string;
      try
      {
        string path = System.IO.Path.Combine("../../../MockClient/ClientStorage/Downloadedlogs", fileName);
        string contents = File.ReadAllText(path);
        CodePopUp popup = new CodePopUp();
        popup.codeView.Text = contents;
        popup.Show();
      }
      catch (Exception ex)
      {
        string msg = ex.Message;
      }
    }

  }
}
