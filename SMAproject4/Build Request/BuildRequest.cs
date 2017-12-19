///////////////////////////////////////////////////////////////////////////////////////////////////
// BuildRequest.cs - Demonstrate Serialization and DeSerialization on BuildServer Data structures//
// ver 1.0                                                                                       //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                       //
// Application: CSE681 Project 4-Remote Build Server                                             //
// Environment: C# console                                                                       //
///////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * Demonstrates serializing and deserializing complex data structures used in Build Server.
 * This demo serializes and deserializes BuildRequest  instances.
 * It then Creates and parses a BuildRequest Message,
 * retrieving copies of the original data structures.
 * 
 * The purpose of this demo is to show that using a single message class with
 * an XML body is a reasonable alternative for message passing in Project #4. 
 * 
 * Public Interface
 * ----------------
 *   BuildItem te1 = new BuildItem(); // part of build request represent as one seperate entity in build request
 *   te1.addDriver(driverfile); // add driver to the build item
 *   te1.addCode(sourcefile);// adds source files to the build request 
 *   te1.ToString();//used for getting the string representation of an object
 *   BuildRequest tr = new BuildRequest();  //used to create the build request
 *   tr.Builds.Add(te1); //add build items to the build request
 *   tr.ToString();// used for getting the string representation of an object
 * Required Files:
 * ---------------
 * BuildRequest.cs
 * 
 * Build Process
 * -------------
 * csc BuildRequest.cs 
 * devenv Build Request.csproj 
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 06 December 2017
 * - first release
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utilities;

namespace Build_Request
{
    // File class which can used to store attributes like name etc.
   public class file
   {
        public string name { get; set; }
       
   }

    ///////////////////////////////////////////////////////////////////
    //Build Item class

    public class BuildItem  /* information about a single Build Item */
    {

        public string builddesc { get; set; }
        
        public List<file> driver { get; set; } = new List<file>();

        public List<file> sourcefiles { get; set; } = new List<file>();
        
        public BuildItem() { }

        //<--------------------Build Item Constructor --------------------------------->
        public BuildItem(string name)
        {
            builddesc = name;
        }

        //<-------------------used for adding the driver name to the list----------------------------->
        public void addDriver(file name)
        {
            driver.Add(name);
        }
        //<--------------------used for adding the file names to the source code list------------------->
        public void addCode(file name)
        {
            sourcefiles.Add(name);
        }

        //<-------------used for getting the string representation of an object------------------------->
        public override string ToString()
        {
            string temp = "\n    description: " + builddesc;
            foreach (file files in driver)
                temp += "\n      driver: " + files;
            foreach (file file in sourcefiles)
                temp += "\n      file:   " + file;
            return temp;
        }

    }

    ///////////////////////////////////////////////////////////////////
    //Build Request class
   public class BuildRequest
    {
        public string author { get; set; }
        public List<BuildItem> Builds { get; set; } = new List<BuildItem>();
        //<---------- Default BuildRequest Constructor--------------------------------->
        public BuildRequest() { }
        //<---------- Build Request parameterized constructor initializes author-------------------------->
        public BuildRequest(string auth)
        {
            author = auth;
        }
        //<------------used for getting the string representation of the object--------------------------->
        public override string ToString()
        {
            string temp = "\n  author: " + author;
            foreach (BuildItem te in Builds)
                temp += te.ToString();
            return temp;
        }
    }

 //<------------------------------------------TEST STUB----------------------------------------------------->
#if (TEST_BuildRequest)

    class testbuildRequest
    {
        //<-----------------------------------------Driver Logic------------------------------------------->
        static void Main(string[] args)
        {
            BuildItem te1 = new BuildItem();
            te1.builddesc = "firstproject";
            file driver = new file();
            driver.name="TestDriver.cs";
            te1.addDriver(driver);
            file one =new file();
            one.name="TestedOne.cs";
            file two= new file();
            two.name="TestedTwo.cs";
            te1.addCode(one);
            te1.addCode(two);
            BuildRequest tr = new BuildRequest();
            tr.author = "Rama Teja Repaka";
            tr.Builds.Add(te1);
            string trXml = tr.ToXml();
            Console.Write("\n  BuildRequest data structure:\n\n  {0}\n", trXml);
            "Testing Deserialization of TestRequest from XML".title();
            BuildRequest newRequest = trXml.FromXml<BuildRequest>();
            string typeName = newRequest.GetType().Name;
            Console.Write("\n  deserializing xml string results in type: {0}\n", typeName);
            foreach (BuildItem te in newRequest.Builds)
            {
                foreach (file files in te.driver)
                {
                    string name = files.name;
                    Console.WriteLine("driver--->" + name);
                }
                foreach (file file in te.sourcefiles)
                {
                    string name = file.name;
                    Console.WriteLine("sourcefile-->" + name);
                }
            }
            Console.WriteLine();
        }
    }

#endif

}
