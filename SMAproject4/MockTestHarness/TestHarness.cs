///////////////////////////////////////////////////////////////////////////////////////////////////
// TestHarness.cs - Demonstrate MocktestHarness  operations                                      //
// ver 1.0                                                                                       //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                       //
// Application: CSE681 Project 4-Remote Build Server                                             //
// Environment: C# console                                                                       //
///////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ===================
 * Demonstrates TestHarness  operations like processingtestrequest,LoadingFromComponentLibFolder
 * loadingAndExercisingTesters,runningSimulatedTest
 * 
 * Public Interface
 * ----------------
 * TestHarness harness = new TestHarness();//used for TestHarness operations
 * harness.processtestrequest(string path, Buildserver serv) //contains logic to process the test request
 * harness.loadAndExerciseTesters();//load assemblies from testersLocation and run their tests
 * harness.handleincomingmessages //function that handles incoming messages to Mock Repository
 * harness.deleteallfiles  //contains logic to delete all files in a directory
 * harness.listenforincomingmessages //function used to create a  thread for handling incoming messages
 * 
 * Required Files:
 * ---------------
 * BuildRequest.cs, Serialization.cs,THMessages.cs, TestMockHarness.cs,MPCommService.cs, IMPCommService.cs
 * 
 * Build Process
 * -------------
 * Compiler command: csc TestMockHarness.cs BuildRequest.cs  Serialization.cs THMessages.cs  MPCommService.cs  IMPCommService.cs
 * devenv MockTestHarness.csproj 
 *  
 * Maintenance History:
 * --------------------
 * ver 1.0 : 06 December 2017
 * - first release
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MessagePassingComm;
using System.Threading;
using System.Security.Policy;

namespace MockTestHarness
{
  //class that has operations related to Mock test harness like parsing test request,handling incoming messages,loadAndExerciseTesters etc
  class TestHarness
  {
    static Comm comm = null;
    private static int startport { get; set; } = 0;
    private static string repoaddress { get; set; } = null;
    private static string myAddress { get; set; } = null;
    Thread msgHandler;
    public static string testersLocation { get; set; } = ".";
    static string filecontent = null;

    // Default constructor that sets testers location
    public TestHarness()
    {
      testersLocation = "../../../MockTestHarness/DLLRepository";
    }

    //constructor that takes startport and mock repository port as arguments
    public TestHarness(int startport, int mockport)
    {
      comm = new Comm("http://localhost", startport);
      repoaddress = "http://localhost:" + mockport.ToString() + "/IMessagePassingComm";
      myAddress = "http://localhost:" + startport.ToString() + "/IMessagePassingComm";
      testersLocation = Path.GetFullPath(testersLocation);
      Console.WriteLine("\n Deleting the old Unused dll files during start up \n");
      deleteallfiles("../../../MockTestHarness/DLLRepository/8091Files");
      deleteallfiles("../../../MockTestHarness/DLLRepository/8092Files");
      deleteallfiles("../../../MockTestHarness/TestLogs");

    }




    //<--------------function that handles incoming messages to Mock Repository--------------------->
    public void handleincomingmessages()
    {
      while (true)
      {
        CommMessage msg = comm.getMessage();
        if (msg.command == "executedll")
        {
          string builderid = msg.builderId;
          List<string> dllfilelist = msg.arguments;
          string xmlstring = File.ReadAllText("../../../MockTestHarness/DLLRepository" + "/" + builderid + "/" + msg.filename);
          Console.WriteLine("\n");
          Console.WriteLine("Test Request:");
          Console.WriteLine(xmlstring); Console.WriteLine("\n\n");
          Console.WriteLine("\n Parsing the Test request and Loading the dll files ......\n");
          loadAndExerciseTesters(builderid, dllfilelist, msg.filename);
        }
      }
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

    //<-----function used to create a  thread for handling incoming messages----------------->
    public void listenforincomingmessages()
    {
      msgHandler = new Thread(handleincomingmessages);
      msgHandler.Start();
    }

    /*----< library binding error event handler >------------------*/
    /*
     *  This function is an event handler for binding errors when
     *  loading libraries.  These occur when a loaded library has
     *  dependent libraries that are not located in the directory
     *  where the Executable is running.
     */
    static Assembly LoadFromComponentLibFolder(object sender, ResolveEventArgs args)
    {
      Console.Write("\n  called binding error event handler");
      string folderPath = testersLocation;
      string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
      if (!File.Exists(assemblyPath)) return null;
      Assembly assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
      return assembly;
    }

    //----< load assemblies from testersLocation and run their tests >-----
    public string loadAndExerciseTesters(string builderid, List<string> dlllist, string filename)
    {
      filecontent = "";
      filecontent = filecontent + " start  of  log  for  " + filename; filecontent = filecontent + "\n\n";
      //AppDomain currentDomain = AppDomain.CurrentDomain;
      Evidence adevidence = AppDomain.CurrentDomain.Evidence;
      AppDomain currentDomain = AppDomain.CreateDomain("MyDomain", adevidence);
      // currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromComponentLibFolder);
      try
      {
        TestHarness loader = new TestHarness();
        testersLocation = Path.GetFullPath(testersLocation + "/" + builderid);
        string[] files = Directory.GetFiles(testersLocation, "*.dll");
        Random rnd = new Random();
        foreach (string file in files)
        {
          int rval = rnd.Next(1, 100000);
          string fileName = Path.GetFileName(file);
          string oldfilename = file;
          string newfilename = testersLocation + "\\" + rval.ToString() + fileName;
          System.IO.File.Move(oldfilename, newfilename);
          Assembly asm = Assembly.LoadFile(newfilename);
          if (dlllist.Contains(fileName))
          {
            Console.Write("\n  loaded {0}", fileName); filecontent = filecontent + "\n loaded {0} " + fileName; filecontent = filecontent + "\n";
            ;                        // exercise each tester found in assembly
            Type[] types = asm.GetTypes();
            foreach (Type t in types)
            {
              // if type supports ITest interface then run test
              if (t.GetInterface("TestBuild.ITest", true) != null)
                if (!loader.runSimulatedTest(t, asm, filename, builderid))
                  Console.Write("\n  test {0} failed to run", t.ToString()); filecontent = filecontent + "\n  test {0} failed to run \n";
            }
          }
        }
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
      finally
      {
        AppDomain.Unload(currentDomain);
      }

      return "Simulated Testing completed";
    }


    //----< run tester t from assembly asm >-------------------------------
    bool runSimulatedTest(Type t, Assembly asm, string filename, string builderid)
    {
      try
      {
        Console.WriteLine("\n");
        Console.Write(
          "\n  attempting to create instance of {0}", t.ToString()
          ); filecontent = filecontent + "\n\n"; filecontent = filecontent + "\n attempting to create instance of {0} ," + t.ToString(); filecontent = filecontent + "\n\n";

        object obj = asm.CreateInstance(t.ToString());
        MethodInfo method1 = t.GetMethod("say");
        if (method1 != null)
          method1.Invoke(obj, new object[0]);
        bool status = false;
        MethodInfo method = t.GetMethod("test");
        if (method != null)
          status = (bool)method.Invoke(obj, new object[0]);

        Func<bool, string> act = (bool pass) =>
        {
          if (pass)
            return "passed";
          return "failed";
        };
        Console.WriteLine("\n #########################  Test status ###########################\n");
        filecontent = filecontent + "\n\n";
        filecontent = filecontent + "###################  Test status #############################\n";
        Console.Write("\n  test {0}", act(status)); Console.WriteLine("\n"); filecontent = filecontent + "\n";
        filecontent = filecontent + "\n  test {0} " + act(status);
        filecontent = filecontent + "\n\n";
      }
      catch (Exception ex)
      {
        Console.Write("\n  test failed with message \"{0}\"", ex.Message); filecontent = filecontent + "\n";
        filecontent = filecontent + "\n  test failed with message \"{0}\"" + ex.Message;
        return false;
      }
      filename = filename.Replace(".xml", "");
      using (StreamWriter sw = File.AppendText("../../../MockTestHarness/TestLogs" + "/" + filename + ".log"))
      {
        sw.Write(filecontent);
      }
      sendlogtorepository(filecontent, filename, builderid);
      filecontent = null;
      return true;
    }

    //<---------  method used to send the test request logs to the Mock Repository  ---------------------->
    private void sendlogtorepository(string filecontent, string filename, string builderid)
    {
      Console.WriteLine("\n sending the logs for the Test request file " + filename);
      Console.WriteLine(filecontent);
      Console.WriteLine("\n\n");
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "Testlog";
      csndMsg.author = "RamaTejaRepaka";
      csndMsg.to = repoaddress;
      csndMsg.from = myAddress;
      csndMsg.filename = filename;
      csndMsg.filecontent = filecontent;
      Console.WriteLine("\n the log message sent to the Mock Repo is shown below\n");
      csndMsg.show();
      Console.WriteLine("\n");
      comm.postMessage(csndMsg);
      Console.WriteLine("\n\n");
    }

  }


#if (TEST_Harness)
  //<---------------------------------------TEST STUB------------------------------------------------>
  class test_TestHarness
  {
    //<-----------------------------------------Driver Logic------------------------------------------->
    static void Main(string[] args)
    {
      int startport = int.Parse(args[0]);
      int mockrepoport = int.Parse(args[1]);
      Console.Title = "MockTestHarness";
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.DarkBlue;
      TestHarness harness = new TestHarness(startport, mockrepoport);
      Console.WriteLine("-----------------------------Start of Mock Test harness at " + args[0] + " -------------------------------------");
      Console.WriteLine("\n\n ----------- The dll delivered by child builders will be found at MockTestHarness/DLLRepository n\n");
      Console.WriteLine("\n\n The child builder 8091 delivers its files to --> MockTestHarness/DLLRepository/8091Files \n\n");
      Console.WriteLine("\n\n The child builder 8092 delivers its files to --> MockTestHarness/DLLRepository/8092Files \n\n");
      Console.WriteLine("Listens for incoming Comm messages\n");
      harness.listenforincomingmessages();
    }
  }
#endif



}
