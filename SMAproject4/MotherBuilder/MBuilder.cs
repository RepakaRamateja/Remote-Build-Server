///////////////////////////////////////////////////////////////////////////////////////////////////
// MBuilder.cs - Demonstrate MotherBuilder operations                                            //
// ver 1.0                                                                                       //
// Author: Repaka RamaTeja,rrepaka@syr.edu                                                       //
// Application: CSE681 Project 4-Remote Build Server                                             //
// Environment: C# console                                                                       //
///////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ====================
 * Demonstrates MBuilder  operations like  creatingmultipleProcess ,handling multiple blockingqueues, Handling communication using WCf with created process
 * and also handlingincomingmessages
 * 
 *  Public Interface
 * ----------------
 * MBuilder mbuilder = new MBuilder(port) //for doing motherbuilder operations takes port as argument
 * mbuilder.createProcess(int i, int port, string mockport)//process class to create a Core builder on the given port number Mock port and number as arguments
 * mbuilder.sendchildinitialmessage(int number, int stport)//sends initial messages to child during startup to know the ready status of childs
 * mbuilder.handleblockingqueues()//function that keeps on listening which handles all the incoming messages
 * mbuilder.listentoqueues()//function used for creating a thread for handling incoming messages to Mother builder
 * mbuilder.processqueuemessages()//function responsible for processing  Blocking ready queue, Blocking Build request queue messages
 * mbuilder.startprocessing(int count, string mockport)//function responsible for creating process on the given port number also sends mock repo port number as arguments
 * mbuilder.sendshutdownmessage(CommMessage msg)//function responsible for sending shutdown message to the Core Builders
 * mbuilder.startprocesspool(int noofprocess)//function mainly responsible for creating process pool by taking input no of processes
 * 
 * Required Files:
 * ---------------
 * MBuilder.cs, MPCommService.cs, IMPCommService.cs
 * 
 *  Build Process
 * --------------
 * Compiler command: csc  MBuilder.cs MPCommService.cs IMPCommService.cs
 * devenv MotherBuilder.csproj 
 * 
 * Maintenance History:
 * --------------------
 *ver 1.0 : 6th December 2017
 * - first release
 */
using MessagePassingComm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MotherBuilder
{
    //class contains all the operations related to motherbuilder like creating blocking queue , creating process pool and handling communication etc 
    public class MBuilder
    {
        string serverip { get; set; } = null;
        string harnessport { get; set; } = null;
        string Mockclientport { get; set; } = null;
        private static int startport { get; set; } = 0;
        Comm comm = null;
        Thread msgHandler;
        private static SWTools.BlockingQueue<CommMessage> readyqueue { get; set; } = null;
        private static SWTools.BlockingQueue<CommMessage> Buildrequestqueue { get; set; } = null;
        //constructor which initializes startport
        public MBuilder(int startport,string testharnessport,string mockclientport)
        {
            if (readyqueue == null)
            {
                readyqueue = new SWTools.BlockingQueue<CommMessage>();
                Buildrequestqueue = new SWTools.BlockingQueue<CommMessage>();
            }
            serverip = "http://localhost:" + startport.ToString() + "/IMessagePassingComm";
            harnessport = testharnessport;
            Mockclientport = mockclientport;
            comm = new Comm("http://localhost", startport);
        }

        //<------------ function that uses process class to create a Core builder on the given port number as argument--------------->
        bool createProcess(int i, int port, string mockport)
        {
            Process proc = new Process();
            string fileName = "..\\..\\..\\CoreBuilder\\bin\\debug\\CoreBuilder.exe";
            string absFileSpec = Path.GetFullPath(fileName);
            Console.Write("\n  attempting to start {0}", absFileSpec);
            string commandline = i.ToString();
            try
            {
                proc.StartInfo.FileName = fileName;
                string processnumber = i.ToString();
                string portn = port.ToString();
                proc.StartInfo.Arguments = processnumber + " " + portn + "  " + mockport + "  " + harnessport + "  " + Mockclientport;
                proc.Start();
            }
            catch (Exception ex)
            {
                Console.Write("\n  {0}", ex.Message);
                return false;
            }
            return true;
        }

        //<----- sends initial messages to child during startup to know the ready status of childs----------->
        void sendchildinitialmessage(int number, int stport)
        {
            for (int i = 0; i < number; i++)
            {
                string port = stport.ToString();
                string childip = "http://localhost:" + port + "/IMessagePassingComm";
                CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
                csndMsg.command = "Are u ready?????";
                csndMsg.author = "Rama Teja Repaka";
                csndMsg.to = childip;
                csndMsg.from = serverip;
                this.comm.postMessage(csndMsg);
                Console.WriteLine("\n\n");
                Console.WriteLine(" Initially asking the ready status of Builder " + (i + 1).ToString() + "  " + csndMsg.to);
                Console.WriteLine("\n");
                csndMsg.show(); Console.WriteLine("\n");
                Console.WriteLine("\n");
                stport++;
            }
        }

        //<--------- function that keeps on listening which handles all the incoming messages ------->
        void handleblockingqueues()
        {
            while (true)
            {
                CommMessage msg = comm.getMessage();
                if (msg.command == "BuildRequestFileName")
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Mother Builder processing Build request recieved ");
                    Console.WriteLine("\n");
                    msg.show();
                    Console.WriteLine("\n");
                    Buildrequestqueue.enQ(msg);
                    Console.WriteLine("\n");
                    Console.WriteLine("\n Inserted into the Build Request queue\n");
                }

                if (msg.command == "I am Ready")
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Mother Builder processing  message recieved from the Builder : " + msg.from);
                    Console.WriteLine("\n");
                    msg.show();
                    readyqueue.enQ(msg);
                    Console.WriteLine("\n");
                    Console.WriteLine("\n Inserted into the Ready queue \n");
                }

                if (msg.command == "quit")
                {
                    Console.WriteLine(" Handling shutdown message recieved ");
                    sendshutdownmessage(msg);
                }



            }
        }

        //<---- function used for creating a thread for handling incoming messages to Mother builder----->
        void listentoqueues()
        {
            msgHandler = new Thread(handleblockingqueues);
            msgHandler.Start();
        }

        //<------------function responsible for processing  Blocking ready queue, Blocking Build request queue messages -------------------------->
        void processqueuemessages()
        {
            while (true)
            {

                if (readyqueue != null && Buildrequestqueue != null)
                {
                    Console.WriteLine("Dequeing message from ready queue and Build Request queue If any");
                    Console.WriteLine("\n");
                    CommMessage readymsg = readyqueue.deQ();
                    CommMessage buildmsg = Buildrequestqueue.deQ();
                    buildmsg.to = readymsg.from;
                    buildmsg.from = readymsg.to;
                    Console.WriteLine("Forwarding to the builder " + buildmsg.to); Console.WriteLine("\n");
                    buildmsg.show();
                    Console.WriteLine("\n");
                    this.comm.postMessage(buildmsg);
                }
            }
        }

        //<-------function responsible for creating process on the given port number also sends mock repo port number as arguments--------------> 
        void startprocessing(int count, string mockport)
        {
            int stport = startport;

            Console.WriteLine(stport);
            Console.WriteLine(mockport);

            for (int i = 1; i <= count; ++i)
            {
                if (createProcess(i, stport, mockport))
                {
                    Console.Write(" - succeeded");
                }
                else
                {
                    Console.Write(" - failed");
                }
                stport++;
            }
            sendchildinitialmessage(count, startport);
        }

        //<-------- function responsible for sending shutdown message to the Core Builders --------------->
        void sendshutdownmessage(CommMessage msg)
        {
            int noofbuilders = msg.Noofprocess;
            int stport = startport;
            for (int i = 0; i < noofbuilders; i++)
            {
                string port = stport.ToString();
                string childip = "http://localhost:" + port + "/IMessagePassingComm";
                CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
                csndMsg.command = "quit";
                csndMsg.author = "RamaTejaRepaka";
                csndMsg.to = childip;
                csndMsg.from = serverip;
                this.comm.postMessage(csndMsg);
                Console.WriteLine("\n\n");
                Console.WriteLine(" Sending the shutdown message to Builder " + (i + 1).ToString() + "  " + csndMsg.to);
                Console.WriteLine("\n");
                csndMsg.show(); Console.WriteLine("\n");
                Console.WriteLine("\n");
                stport++;
            }
        }

        //<--------- function mainly responsible for creating process pool----------------->
        void startprocesspool(int noofprocess)
        {
            int count = noofprocess;
            startprocessing(count, "8080");
        }

        //---------------------------------<TEST STUB>-------------------------------------------
#if (TEST_MotherBuilder)
        class TestBuilder
        {
            //driver logic
            static void Main(string[] args)
            {
                Console.Title = "MotherBuilder";
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("-----------------------Start of Mother Builder at port " + args[0] + "-----------------------------");
                Console.WriteLine("\n");
                startport = int.Parse(args[0]);
                MBuilder mbuilder = new MBuilder(startport - 1,args[2],args[4]);
                int noofprocess = int.Parse(args[3]);
                mbuilder.startprocesspool(noofprocess);
                mbuilder.listentoqueues();
                mbuilder.processqueuemessages();
                
            }
        }

        }
#endif

    }
   


