///////////////////////////////////////////////////////////////////////////////
// THMessages.cs - Demonstrate XML Serializer on TestHarnes Data Structures  //
// ver 1.0                                                                   //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2017           //
// Application: CSE681 CSE681  Project 4-Remote Build Server                 //
// Environment: C# console                                                   //
///////////////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* ===================
* Demonstrates serializing and deserializing complex data structures used
* in TestHarnes.
* 
* This demo serializes and deserializes TestRequest and TestResults instances.
* It then Creates and parses a TestRequest Message and a TestResults Message,
* retrieving copies of the original data structures.
* 
* The purpose of this demo is to show that using a single message class with
* an XML body is a reasonable alternative for message passing in Project #4. 
*
* Public Interface
* ----------------
*  TestElement te1 = new TestElement(); // part of test request represent as one seperate entity in build request
*  te1.addDriver(driverfile); // add driver to the TestElement
*  te1.addCode(sourcefile);// adds source files to the TestElement 
*  TestRequest tr = new TestRequest();  //used to create the Test request
*  tr.Builds.Add(te1); //add TestElement to the test request
*  
* Required Files:
* ---------------
* THMessages.cs 
* 
* Build Process
* -------------
* Compiler command: csc  THMessages.cs 
* devenv TestHarnessMessages.csproj
* 
* Maintenance History:
* --------------------
* ver 1.0 : 07 Sep 2017
* first release
* 
* 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Utilities;

namespace TestHarnessMessages
{
  ///////////////////////////////////////////////////////////////////
  // TestElement and TestRequest classes
  //
  public class TestElement  /* information about a single test */
  {
    public string testName { get; set; }
    public string testDriver { get; set; }
    public List<string> testCodes { get; set; } = new List<string>();

    //default constructor
    public TestElement() { }
    // parameterized constructor which takes name 
    public TestElement(string name)
    {
      testName = name;
    }
    //<---- function that adds driver to the test request ------------->
    public void addDriver(string name)
    {
      testDriver = name;
    }
    //<--------- function that adds source code dlls to the test request ------------>
    public void addCode(string name)
    {
      testCodes.Add(name);
    }

    //<------ function that provides string representation of the object ------------> 
    public override string ToString()
    {
      string temp = "\n    test: " + testName;
      temp += "\n      testDriver: " + testDriver;
      foreach (string testCode in testCodes)
        temp += "\n      testCode:   " + testCode;
      return temp;
    }
  }

  public class TestRequest  /* a container for one or more TestElements */
  {
    public string author { get; set; }
    public List<TestElement> tests { get; set; } = new List<TestElement>();
    //default constructor
    public TestRequest() { }
    // parameterized constructor which takes auth 
    public TestRequest(string auth)
    {
      author = auth;
    }
   //<--------object into string----------------->
    public override string ToString()
    {
      string temp = "\n  author: " + author;
      foreach (TestElement te in tests)
        temp += te.ToString();
      return temp;
    }
  }
  ///////////////////////////////////////////////////////////////////
  // TestResult and TestResults classes
  //
  public class TestResult  /* information about processing of a single test */
  {
    public string testName { get; set; }
    public bool passed { get; set; }
    public string log { get; set; }
    
    //default constructor
    public TestResult() { }

    // parameterized constructor which takes name and status as arguments
    public TestResult(string name, bool status)
    {
      testName = name;
      passed = status;
    }
        //<--------used for logging logitem----------->
    public void addLog(string logItem)
    {
      log += logItem;
    }
    //<--------object into string----------------->
        public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("\n    Test: " + testName + " " + passed);
      sb.Append("\n    log:  " + log);
      return sb.ToString();
    }
  }

  public class TestResults  /* a container for one or more TestResult instances */
  {
    public string author { get; set; }
    public DateTime timeStamp { get; set; }
    public List<TestResult> results { get; set; } = new List<TestResult>();

    //default constructor
    public TestResults() { }
    // parameterized constructor which takes auth and datetime as arguments
    public TestResults(string auth, DateTime ts)
    {
      author = auth;
      timeStamp = ts;
    }
        //<------addtestresults----------->
    public TestResults add(TestResult rslt)
    {
      results.Add(rslt);
      return this;
    }
        //<--------object into string----------------->
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("\n  Author: " + author + " " + timeStamp.ToString());
      foreach (TestResult rslt in results)
      {
        sb.Append(rslt.ToString());
      }
      return sb.ToString();
    }
  }

  //---------------------------------------< test stub >------------------------------------------------
#if (TEST_THMESSAGES)
  class TestTHMessages
  {
    //driver logic
    static void Main(string[] args)
    {
      "Testing THMessage Class".title('=');
      Console.WriteLine();

      ///////////////////////////////////////////////////////////////
      // Serialize and Deserialize TestRequest data structure

      "Testing Serialization of TestRequest data structure".title();

      TestElement te1 = new TestElement();
      te1.testName = "test1";
      te1.addDriver("td1.dll");
      te1.addCode("tc1.dll");
      te1.addCode("tc2.dll");

      TestElement te2 = new TestElement();
      te2.testName = "test2";
      te2.addDriver("td2.dll");
      te2.addCode("tc3.dll");
      te2.addCode("tc4.dll");

      TestRequest tr = new TestRequest();
      tr.author = "Jim Fawcett";
      tr.tests.Add(te1);
      tr.tests.Add(te2);
      string trXml = tr.ToXml();
      Console.Write("\n  Serialized TestRequest data structure:\n\n  {0}\n", trXml);

      "Testing Deserialization of TestRequest from XML".title();

      TestRequest newRequest = trXml.FromXml<TestRequest>();
      string typeName = newRequest.GetType().Name;
           
      Console.Write("\n  deserializing xml string results in type: {0}\n", typeName);
      Console.Write(newRequest);
      Console.WriteLine();

    }
  }
#endif
}
