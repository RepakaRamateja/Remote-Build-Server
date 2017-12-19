///////////////////////////////////////////////////////////////////////////////////////////////////
//Executive.cs - Demonstration of Requirements for Project 4 Remote Build Server                 //
// ver 1.0                                                                                       //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                       //
// Application: CSE681 Project 4-Remote Build Server                                             //
// Environment: C# console                                                                       //
///////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 *  Demonstrates all the project4: Remote Build Server  Requirements in a clear and  understandable way
 * 
 * Public Interface
 * ----------------
 * This does not contain any public methods
 *   
 * Required Files:
 * ---------------
 * Executive.cs
 * 
 * Build Process
 * -------------
 * csc Executive.cs
 * devenv TestExecutive.csproj
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 6th December 2017
 * - first release
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestExecutive
{

  class Executive
  {
    //<---------------- function used for demonstrating requirement one----------------------->
    private void demonstrateRequirement1()
    {
     
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 1");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\n");
      Console.WriteLine("Implemented using C# langauge as you can see all .cs files in the SMAproject4 folder");
      Console.WriteLine("\n");
      Console.WriteLine("Using C# Framework Version" + Environment.Version.ToString());
      Console.WriteLine("\n");
      Console.WriteLine("Implemented code using visual studio as you can see the solution file SMAproject4.sln in the SMAproject4 folder");
      Console.WriteLine("\n");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
    }

    //<----------------- function used for demonstrating requirements from 2 to 3----------------------------->
    private void demonstrateRequirementupto3()
    {
      Console.WriteLine("Demonstration of Requirement 2");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Included a Message Passing service built using WCF which can be found at location \n  " + "  SMAproject4/MessagePassingCommService    SMAproject4/IService");
      Console.WriteLine("\n");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 3");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\nMother Builder and Core Builders(childs) are communicating each other using the MessagePassingCommService\n");
      Console.WriteLine("Observation  from Mother Builder Console, CoreBuilders(child) Console and Mock Repository Console are:\n");
      Console.WriteLine("Mother builder sends Two types of Requests to Core Builders(child)\n");
      Console.WriteLine("1.BuildRequest using MessagePassingCommService\n");
      Console.WriteLine("2.ReadyRequest to know the status of Builder using MessagePassingCommService\n");
      Console.WriteLine("Core Builders processes the Requests\n");
      Console.WriteLine("If it is a Ready request then it sends ready response using MessagePassingCommService\n");
      Console.WriteLine("If it is a Build request then it asks for xml file specfied in the build request from the Mock repository\n");
      Console.WriteLine("Mock Repository sends the BuildRequest xml file using MessagePassingCommService to Corresponding Core Builder by \n using its recieved Comm message address\n");
      Console.WriteLine("Mock Repository also sends Acknowledgement message file sent to Core Builder\n");
      Console.WriteLine("So pool process and Mother builder will be using MessagePassingCommService to transfer BuildRequests and Files  \n which clearly shows Requirement 3\n");
      Console.WriteLine("Mock repo Location is SMAproject4/MockRepository/RepoStorage\n");
      Console.WriteLine("Child Builder Location is SMAproject4/CoreBuilder/Builderstorage\n");
      Console.WriteLine("when the child builder starts  folder will be created in SMAproject4/CoreBuilder/Builderstorage\n");
      Console.WriteLine("folder name will be childbuilder portnumber + files");
      Console.WriteLine("\n");
    }

    //<--------------- function used for demonstrating Requirement 4------------------------------------->
    private void Requirement4()
    {
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 4");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\nNavigate to the  Generate request tab in the wpf application\n");
      Console.WriteLine("To get the Repository server contents click on getRepocontents button\n");
      Console.WriteLine("Then Repository server contents will be populated in the list boxes of drivers and source files\n");
      Console.WriteLine("Now client can select the driver and source files and click on generate then build request is generated\n");
      Console.WriteLine("Generated request is displayed on the wpf application for verification\n");
      Console.WriteLine("user can save the generated request by clicking save button where request would be saved at location \n SMAproject4/MockClient/ClientStorage \n");
      Console.WriteLine("Now navigate to Handle Build Request tab click on local build request \n");
      Console.WriteLine("Then user can see all the build Requests that are generated \n");
      Console.WriteLine("Select on some build Requests and click on show selected build requests\n");
      Console.WriteLine("Now click on Send Requests now all Requests will be sent to Mock Repository\n");
      Console.WriteLine("You can find the send Requests at SMAproject4/MockRepository/RepoStorage\n");
      Console.WriteLine("The Gui has provided with a button Remote Build requests which gives all the build Requests in the Mock repositiory\n");
      Console.WriteLine("You can validate by both the ways\n");
      Console.WriteLine("After getting the remote build requests select the requests and \n click on show selected requests to display selected requests\n");
      Console.WriteLine("Now click on send Requests then Requests would be sent from mock repository to build server for further processsing\n");
      Console.WriteLine("Thus requirement 4 is clearly demonstrated \n");
    }
    //<------------- function used for demonstrating Requirement 5 and 6--------------------------------------->
    private void Requirement5to6()
    {
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 5");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\nIncluded a Process Pool component Mother Builder which creates No of child processes on Command\n");
      Console.WriteLine("Open the project properties or see the run.bat where you can see the command line arguments\n");
      Console.WriteLine("\nMother Builder can be found at the location SMAproject4/MotherBuilder\n");
      Console.WriteLine("Mother builder creates no of child builders based on command line arguments\n");
      Console.WriteLine("You can observer the run.bat command while starting mother builder\n");
      Console.WriteLine("This clearly demonstrates Requirement 5\n");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 6");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\nObservation from the Mother Builder console and Child Builder Console \n");
      Console.WriteLine("you can easily find the usage of message passing communication service by the messages\n");
      Console.WriteLine("You can observe flow of Build Requests and Ready messages\n");
      Console.WriteLine("if you see the project references both the projects use MessagePassingComm Service\n");
      Console.WriteLine("Observing the code clearly shows the usage of MessagePassingComm Service\n");
      Console.WriteLine("This clearly demonstrates requirement 6\n");
    }

    //<------------- function used for demonstrating Requirement 6 to 9--------------------------------------->
    private void requirementupto9()
    {
      Console.WriteLine("\n--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 7");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\nNavigate to the child builders console\n");
      Console.WriteLine("\nobserve the messages recieved from mother builder\n");
      Console.WriteLine("you can see child builder sends request for source files to mock repository\n");
      Console.WriteLine("You can see child builder builds libraries after recieving the source files from mock repo\n");
      Console.WriteLine("you can see the build logs that are displayed in the console \n");
      Console.WriteLine("you can find the command used to generate builds \n");
      Console.WriteLine("If there is any error you can also see the errors\n");
      Console.WriteLine("If Build succeds then you can see success msg in the console\n");
      Console.WriteLine("This clearly demonstrates Requirement 7\n ");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 8");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\nyou see the dll files and test request generated if build succeds  at location SMAproject4/CoreBuilder/Builderstorage/Builder + childportnumber\n");
      Console.WriteLine("After the build succeds Child builders send dll files and test request to Test harness \n");
      Console.WriteLine("you can find the test request and libraries at the location \nSMAproject4/MockTestHarness/DLLRepository/childbuilderportnumber +files");
      Console.WriteLine("\nYou can see the status of files  sent from child builder console to Mock test harness console\n");
      Console.WriteLine("you can see the file sent messages from child builders console \n");
      Console.WriteLine("you can also see the test request recieved contents in the mock test harness console\n");
      Console.WriteLine("You can also see the logs that are saved locally at \n SMAproject4/CoreBuilder/Builderstorage/Builder + portnumber \n");
      Console.WriteLine("There you can see the log file with .log extension which contains all the builds performed by that builder\n");
      Console.WriteLine("It contains all information about that build whether it is success or failure \n");
      Console.WriteLine("Observe the consoles you can see child builder sends logs to mock repository \n");
      Console.WriteLine("In the mock repository console you can see the recieved logs\n");
      Console.WriteLine("Find the build logs in the mock repo location SMAproject4/MockRepository/RepoStorage/BuildRequestLogs \n"); 
      Console.WriteLine("this clearly shows the demonstration of requirement 8 \n");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 9");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
    }

    //<------------- function used for demonstrating Requirement 9 to 11--------------------------------------->
    private void requirementupto11()
    {
      Console.WriteLine("\nnavigate to the test harness console you can clearly see recieved test requests\n");
      Console.WriteLine("Observer that test harness parses the test request and starts loading dll files\n");
      Console.WriteLine("You can see the execution results of loaded libraries in the test harness console\n");
      Console.WriteLine("you can find the test results sent msg from mock test harness to mock repository\n");
      Console.WriteLine("In the mock repository console you can see the recieved test result msg in the console\n");
      Console.WriteLine("In the mock repository you can find the test results at location SMAproject4/MockRepository/RepoStorage/TestHarnessLogs");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 10");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\nIncluded a MockClient Package which is built using WPF \n which could be found at location :  SMAproject4/MockClient");
      Console.WriteLine("\n This shows the demonstration of requirement 10 clearly \n");
      Console.WriteLine("\n --------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 11");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\n");
      Console.WriteLine("Navigate to the wpf application \n");
      Console.WriteLine("Navigate to Generate Request tab\n");
      Console.WriteLine("Click on getRepoContents to get the list of files from the mock repository\n");
      Console.WriteLine("List of files will be populated in the driver and source files list box \n");
      Console.WriteLine("select driver and click on show selected driver \n");
      Console.WriteLine("similarly select source files and click on show selected source files \n");
      Console.WriteLine("Now click on generate button to see the build Item added to the  new build request generated \n");
      Console.WriteLine("Now again click ongetRepoContents to get the list of files from the mock repository\n");
      Console.WriteLine("List of files will be populated in the driver and source files list box \n");
      Console.WriteLine("select driver and click on show selected driver \n");
      Console.WriteLine("similarly select source files and click on show selected source files \n");
      Console.WriteLine("Now click on generate button to see the new build item added to existing build request \n");
      Console.WriteLine("To start again from start click on clear button\n");
      Console.WriteLine("This clearly shows the demonstartion of requirement 11 \n");
      Console.WriteLine("Navigate to the mock client console you see the automated demonstartion results of requirement 11");
    }
    //<------------- function used for demonstrating Requirement 12 to 13--------------------------------------->
    private void requirementupto13()
    {
      Console.WriteLine("\n --------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 12");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\n");
      Console.WriteLine("Now navigate to Handle Build Request tab click on local build request \n");
      Console.WriteLine("Then user can see all the build Requests that are generated \n");
      Console.WriteLine("Select on some build Requests and click on show selected build requests\n");
      Console.WriteLine("Now click on Send Requests now all Requests will be sent to Mock Repository\n");
      Console.WriteLine("You can find the send Requests at SMAproject4/MockRepository/RepoStorage\n");
      Console.WriteLine("The Gui has provided with a  button Remote Build requests which gives all the build Requests in the Mock repositiory\n");
      Console.WriteLine("You can validate by both the ways\n");
      Console.WriteLine("Thus requirement 12 is clearly demonstrated\n");
      Console.WriteLine("\n --------------------------------------------------------------------------------------------------------");
      Console.WriteLine("Demonstration of Requirement 13");
      Console.WriteLine("--------------------------------------------------------------------------------------------------------");
      Console.WriteLine("\n");
      Console.WriteLine("After getting the remote build requests select the requests and \n\nclick on show selected requests to display selected requests\n");
      Console.WriteLine("Now click on send Requests then Requests would be sent from mock repository to build server for further processsing\n");
      Console.WriteLine("Observe the Mother builder console where it recieves build requests from the mock repo\n");
      Console.WriteLine("Observe the child builders console where all the builds happens libraries generated \n");
      Console.WriteLine("Test request forwarded to Mock test harness\n");
      Console.WriteLine("Logs sent to mock repo\n");
      Console.WriteLine("Test harness loading executing dlls and sending logs to mock repository\n");
      Console.WriteLine("All the processing in done by one button click (build) \n");
      Console.WriteLine("This clearly demonstartes requirement 13 \n");
      Console.WriteLine("now navigate to the logs view  and follow the instructions written in logs view to see the logs through mock client\n");
    }
    //<------- To explain clearly how things are happening-------------------------------->
    private void note()
    {
      Console.WriteLine("-------------------------------------------Test Executive started------------------------------------------------------");
      Console.WriteLine("\n");
      Console.WriteLine("!!!!!!!!!!!!!!!  Note: Over view of things happening behind automation before demonstration !!!!!!!!!!!!!!!!!!!!!!!!!!!\n\n");
      Console.WriteLine("Mock client started at 8010 port number\n");
      Console.WriteLine("Mock Repo started started at 8080 port number\n");
      Console.WriteLine("Build Server started at port number 8090 \n");
      Console.WriteLine("Child builder1 started at port number 8091 \n");
      Console.WriteLine("Child builder2 started at port number 8092 \n");
      Console.WriteLine("Mock test harness started at port number 8040\n");
      Console.WriteLine("Mock client created TestBuildRequest1.xml which contains two build items for the requirement 11\n");
      Console.WriteLine("Mock client sends \n\nBuildRequest1.xml\n\nBuildRequest2.xml\n\nBuildRequest3.xml to the mock repository for storage \n");
      Console.WriteLine("BuildRequest1.xml contains two build items\n");
      Console.WriteLine("BuildRequest2.xml contains 1 build items\n");
      Console.WriteLine("BuildRequest3.xml contains 1 build items\n");
      Console.WriteLine("Now mock client sends requests(BuildRequest1.xml,BUildRequest2.xml,BuildRequest3.xml) \n\nto the repository to send a build request in its storage to the Build Server for build processing\n");
      Console.WriteLine("Now back ground process starts like building files and loading dlls \n");
      Console.WriteLine("If you want to send a request manually use BuildRequest4.xml\n");
      Console.WriteLine("In the handle request view click on getlocal requests and click on send\n");
      Console.WriteLine("Now click on getremoterequests now you will see BuildRequest4.xml\n");
      Console.WriteLine("Now click on build that's it\n");
      Console.WriteLine("Now all set to see the requirements demonstration \n\n");
    }
    //<------------------------------------------TEST STUB----------------------------------------------------->
#if (Test_Executive)
    //driver logic
    static void Main(string[] args)
    {
      Console.Title = "Test Executive";
      Executive exec = new Executive();
      exec.note();
      exec.demonstrateRequirement1();
      exec.demonstrateRequirementupto3();
      exec.Requirement4();
      exec.Requirement5to6();
      exec.requirementupto9();
      exec.requirementupto11();
      exec.requirementupto13();
      Console.ReadLine();
    }
#endif
  }
}
