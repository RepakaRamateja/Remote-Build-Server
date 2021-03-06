﻿/////////////////////////////////////////////////////////////////////
// MPCommService.cs - service for MessagePassingComm               //
// ver 3.2                                                         //
// Author: Repaka RamaTeja,rrepaka@syr.edu                         //
// Application: CSE681 Project 4-Remote Build Server               //
// Environment: C# console                                         //
/////////////////////////////////////////////////////////////////////
/*
 * Started this project with C# Console Project wizard
 * - Added references to:
 *   - System.ServiceModel
 *   - System.Runtime.Serialization
 *   - System.Threading;
 *   - System.IO;
 *   
 * Package Operations:
 * -------------------
 * Demonstrates service for MessagePassingComm
 * 
 *  Public Interface
 * -------------------
 * This package defines three classes:
 * - Sender which implements the public methods:
 *   -------------------------------------------
 *   - connect          : opens channel and attempts to connect to an endpoint, 
 *                        trying multiple times to send a connect message
 *   - close            : closes channel
 *   - postMessage      : posts to an internal thread-safe blocking queue, which
 *                        a sendThread then dequeues msg, inspects for destination,
 *                        and calls connect(address, port)
 *   - postFile         : attempts to upload a file in blocks
 *   - getLastError     : returns exception messages on method failure
 *   - transferFile     : uploads file to Receiver instance at given destination
 *   - sendfile         : uploads file to the given UrL reciever instance
 * - Receiver which implements the public methods:
 *   ---------------------------------------------
 *   - start            : creates instance of ServiceHost which services incoming messages
 *   - postMessage      : Sender proxies call this message to enqueue for processing
 *   - getMessage       : called by Receiver application to retrieve incoming messages
 *   - close            : closes ServiceHost
 *   - openFileForWrite : opens a file for storing incoming file blocks
 *   - writeFileBlock   : writes an incoming file block to storage
 *   - closeFile        : closes newly uploaded file
 *   - openFileForWritedestination : called by Sender's proxy to open file on Receiver destination
 *   
 * - Comm which implements, using Sender and Receiver instances, the public methods:
 *   -------------------------------------------------------------------------------
 *   - postMessage      : send CommMessage instance to a Receiver instance
 *   - getMessage       : retrieves a CommMessage from a Sender instance
 *   - postFile         : called by a Sender instance to transfer a file
 *   - sendfile         : called by remote Comm to upload file by providing destination URL
 *   - transferfile      : called by remote Comm to upload the given file to the given url at given location 
 *    
 * The Package also implements the class TestPCommService with public methods:
 * ---------------------------------------------------------------------------
 * - testSndrRcvr()     : test instances of Sender and Receiver
 * - testComm()         : test Comm instance
 * - compareMsgs        : compare two CommMessage instances for near equality
 * - compareFileBytes   : compare two files byte by byte
 * - getClientFileList  : collect file names from client's FileStore
 *   
 * Required files: 
 * ---------------
 *  MPCommService.cs,IMPCommService.cs
 *  
 * Build Process
 * -------------  
 * csc MPCommService.cs IMPCommService.cs
 * devenv MessagePassingCommService.csproj 
 * 
 * Maintenance History:
 * --------------------
 * ver 3.2 : 6th December 2017
 * - added function openFileForWritedestination
 * - modified postfile function decalartion by adding one more parameter in both Comm and Sender 
 * - added transfer file and send file functions in sender and Comm 
 * ver 3.1 : 01 Dec 2017
 * - added some text to these maintenance comments
 * ver 3.0 : 26 Oct 2017
 * - Receiver receive thread processing changed to discard connect messages
 * - added close, size, and restart functions
 * - changed Sender.connect to return false instead of break on fail to connect
 * ver 2.1 : 20 Oct 2017
 * - minor changes to these comments
 * ver 2.0 : 19 Oct 2017
 * - renamed namespace and several components
 * - eliminated IPluggable.cs
 * ver 1.0 : 14 Jun 2017
 * - first release
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Threading;
using System.IO;

namespace MessagePassingComm
{
  ///////////////////////////////////////////////////////////////////
  // Receiver class - receives CommMessages and Files from Senders

  public class Receiver : IMessagePassingComm
  {
    public static SWTools.BlockingQueue<CommMessage> rcvQ { get; set; } = null;
    ServiceHost commHost = null;
    FileStream fs = null;
    string lastError = "";

    public string Address { get; private set; }

    /*----< constructor >------------------------------------------*/

    public Receiver()
    {
      if (rcvQ == null)
        rcvQ = new SWTools.BlockingQueue<CommMessage>();
    }
    /*----< create ServiceHost listening on specified endpoint >---*/
    /*
     * baseAddress is of the form: http://IPaddress or http://networkName
     */
    public void start(string baseAddress, int port)
    {

      Address = baseAddress + ":" + port.ToString() + "/IMessagePassingComm";
      TestUtilities.putLine(string.Format("starting Receiver on thread {0}", Thread.CurrentThread.ManagedThreadId));
      createCommHost(Address);
    }


    /*----< create ServiceHost listening on specified endpoint >---*/
    /*
     * address is of the form: http://IPaddress:8080/IMessagePassingComm
     */
    public void createCommHost(string address)
    {
      WSHttpBinding binding = new WSHttpBinding();
      Uri baseAddress = new Uri(address);
      commHost = new ServiceHost(typeof(Receiver), baseAddress);
      commHost.AddServiceEndpoint(typeof(IMessagePassingComm), binding, baseAddress);
      commHost.Open();
    }


    /*----< enqueue a message for transmission to a Receiver >-----*/

    public void postMessage(CommMessage msg)
    {
      msg.threadId = Thread.CurrentThread.ManagedThreadId;
      TestUtilities.putLine(string.Format("sender enqueuing message on thread {0}", Thread.CurrentThread.ManagedThreadId));
      rcvQ.enQ(msg);
    }
    /*----< retrieve a message sent by a Sender instance >---------*/

    public CommMessage getMessage()
    {
      CommMessage msg = rcvQ.deQ();
      if (msg.type == CommMessage.MessageType.closeReceiver)
      {
        close();
      }
      if (msg.type == CommMessage.MessageType.connect)
      {
        msg = rcvQ.deQ();  // discarding the connect message
      }
      return msg;
    }
    /*----< close ServiceHost >------------------------------------*/

    public void close()
    {
      TestUtilities.putLine("closing receiver - please wait");
      //Console.Out.Flush();
      commHost.Close();
      (commHost as IDisposable).Dispose();
    }
    /*---< called by Sender's proxy to open file on Receiver >-----*/
    public bool openFileForWrite(string name, string builderid)
    {
      try
      {
        string writePath = Path.Combine(ServiceEnvironment.fileStorage + "/" + builderid, name);
        fs = File.OpenWrite(writePath);
        return true;
      }
      catch (Exception ex)
      {
        lastError = ex.Message;
        return false;
      }
    }


    /*---< called by Sender's proxy to open file on Receiver destination >-----*/
    public bool openFileForWritedestination(string filename, string destination)
    {
      try
      {
        string writePath = Path.Combine(destination, filename);
        fs = File.OpenWrite(writePath);
        return true;
      }
      catch (Exception ex)
      {
        lastError = ex.Message;
        return false;
      }
    }

    /*----< write a block received from Sender instance >----------*/

    public bool writeFileBlock(byte[] block)
    {
      try
      {
        fs.Write(block, 0, block.Length);
        return true;
      }
      catch (Exception ex)
      {
        lastError = ex.Message;
        return false;
      }
    }

    /*----< close Receiver's uploaded file >-----------------------*/
    public void closeFile()
    {
      fs.Close();
    }
  }
  ///////////////////////////////////////////////////////////////////
  // Sender class - sends messages and files to Receiver

  public class Sender
  {
    private IMessagePassingComm channel;
    private ChannelFactory<IMessagePassingComm> factory = null;
    private SWTools.BlockingQueue<CommMessage> sndQ = null;
    private int port = 0;
    private string fromAddress = "";
    private string toAddress = "";
    Thread sndThread = null;
    int tryCount = 0, maxCount = 10;
    string lastError = "";
    string lastUrl = "";

    bool channelWorking = false;


    /*----< constructor >------------------------------------------*/

    public Sender(string baseAddress, int listenPort)
    {
      port = listenPort;
      fromAddress = baseAddress + listenPort.ToString() + "/IMessagePassingComm";
      sndQ = new SWTools.BlockingQueue<CommMessage>();
      TestUtilities.putLine(string.Format("starting Sender on thread {0}", Thread.CurrentThread.ManagedThreadId));
      sndThread = new Thread(threadProc);
      sndThread.Start();
    }
    /*----< creates proxy with interface of remote instance >------*/

    public void createSendChannel(string address)
    {
      EndpointAddress baseAddress = new EndpointAddress(address);
      WSHttpBinding binding = new WSHttpBinding();
      factory = new ChannelFactory<IMessagePassingComm>(binding, address);
      channel = factory.CreateChannel();
    }
    /*----< attempts to connect to Receiver instance >-------------*/

    public bool connect(string baseAddress, int port)
    {
      toAddress = baseAddress + ":" + port.ToString() + "/IMessagePassingComm";
      return connect(toAddress);
    }
    /*----< attempts to connect to Receiver instance >-------------*/
    /*
     * - attempts a finite number of times to connect to a Receiver
     * - first attempt to send will throw exception of no listener
     *   at the specified endpoint
     * - to test that we attempt to send a connect message
     */
    public bool connect(string toAddress)
    {
      createSendChannel(toAddress);
      CommMessage connectMsg = new CommMessage(CommMessage.MessageType.connect);
      while (true)
      {
        try
        {
          channel.postMessage(connectMsg);
          tryCount = 0;
          channelWorking = true;
          return true;
        }
        catch (Exception ex)
        {
          if (++tryCount < maxCount)
          {
            TestUtilities.putLine("failed to connect - waiting to try again");
            //Thread.Sleep(timeToSleep);
          }
          else
          {
            TestUtilities.putLine("failed to connect - quitting");
            lastError = ex.Message;
            channelWorking = false;
            return false;
          }
        }
      }
    }
    /*----< closes Sender's proxy >--------------------------------*/

    public void close()
    {
      if (factory != null && factory.State == CommunicationState.Opened)
        factory.Close();
    }
    /*----< processing for receive thread >------------------------*/
    /*
     * - send thread dequeues send message and posts to channel proxy
     * - thread inspects message and routes to appropriate specified endpoint
     */
    void threadProc()
    {
      while (true)
      {
        TestUtilities.putLine(string.Format("sender enqueuing message on thread {0}", Thread.CurrentThread.ManagedThreadId));
        CommMessage msg = sndQ.deQ();
        if (msg.type == CommMessage.MessageType.closeSender)
        {
          TestUtilities.putLine("Sender send thread quitting");
          break;
        }
        if (msg.to == lastUrl)
        {
          channel.postMessage(msg);
        }
        else
        {
          if (channelWorking)
            close();
          if (!connect(msg.to))
          {
            //sndQ.enQ(msg); // message that failed to send will be immediately put back to the queue
            // sndQ.end(msg) <-- removed, message will be discarded and there will be no attempt to resend
            continue;
          }
          lastUrl = msg.to;
          channel.postMessage(msg);
        }
      }
    }
    /*----< main thread enqueues message for sending >-------------*/

    public void postMessage(CommMessage msg)
    {
      sndQ.enQ(msg);
    }

    /*----< uploads file to Receiver instance >--------------------*/
    public bool postFile(string fileName, string builderid)
    {
      FileStream fs = null;
      long bytesRemaining;
      try
      {
        string path = Path.Combine(ClientEnvironment.fileStorage, fileName);
        fs = File.OpenRead(path);
        bytesRemaining = fs.Length;
        channel.openFileForWrite(fileName, builderid);
        while (true)
        {
          long bytesToRead = Math.Min(ClientEnvironment.blockSize, bytesRemaining);
          byte[] blk = new byte[bytesToRead];
          long numBytesRead = fs.Read(blk, 0, (int)bytesToRead);
          bytesRemaining -= numBytesRead;

          channel.writeFileBlock(blk);
          if (bytesRemaining <= 0)
            break;
        }
        channel.closeFile();
        fs.Close();
      }
      catch (Exception ex)
      {
        lastError = ex.Message;
        return false;
      }
      return true;
    }

    /*----< uploads file to Receiver instance at given destination >--------------------*/
    public bool transferFile(string fileName, string source, string destination)
    {
      FileStream fs = null;
      long bytesRemaining;
      try
      {
        string path = Path.Combine(source, fileName);
        fs = File.OpenRead(path);
        bytesRemaining = fs.Length;
        channel.openFileForWritedestination(fileName, destination);
        while (true)
        {
          long bytesToRead = Math.Min(ClientEnvironment.blockSize, bytesRemaining);
          byte[] blk = new byte[bytesToRead];
          long numBytesRead = fs.Read(blk, 0, (int)bytesToRead);
          bytesRemaining -= numBytesRead;

          channel.writeFileBlock(blk);
          if (bytesRemaining <= 0)
            break;
        }
        channel.closeFile();
        fs.Close();
      }
      catch (Exception ex)
      {
        lastError = ex.Message;
        return false;
      }
      return true;
    }

    /*------< uploads file to the given UrL reciever instance>------------*/
    public bool sendfile(string fileName, string url, string builderid)
    {
      close();
      if (!connect(url))
        return false;
      bool val = postFile(fileName, builderid);
      if (val == true)
      {
        lastUrl = url;
      }
      return val;
    }

    /*--------< uploads file to the given Url reciever instance>-------------*/
    public bool transferfile(string fileName, string source, string destination, string url)
    {
      close();
      if (!connect(url))
        return false;
      bool val = transferFile(fileName, source, destination);
      if (val == true)
      {
        lastUrl = url;
      }
      return val;
    }

  }
  ///////////////////////////////////////////////////////////////////
  // Comm class combines Receiver and Sender

  public class Comm
  {
    private Receiver rcvr = null;
    private Sender sndr = null;

    public string ReceiverAddress
    {
      get { return rcvr.Address; }
    }

    /*----< constructor >------------------------------------------*/
    /*
     * - starts listener listening on specified endpoint
     */
    public Comm(string baseAddress, int port)
    {
      rcvr = new Receiver();
      rcvr.start(baseAddress, port);
      sndr = new Sender(baseAddress, port);
    }
    /*----< post message to remote Comm >--------------------------*/

    public void postMessage(CommMessage msg)
    {
      sndr.postMessage(msg);
    }
    /*----< retrieve message from remote Comm >--------------------*/

    public CommMessage getMessage()
    {
      return rcvr.getMessage();
    }
    /*----< called by remote Comm to upload file >-----------------*/
    public bool postFile(string filename, string builderid)
    {
      return sndr.postFile(filename, builderid);
    }

    /*-----< called by remote Comm to upload file by providing destination URL-------------->*/
    public bool sendfile(string filename, string url, string builderId)
    {
      return sndr.sendfile(filename, url, builderId);
    }

    /*------< called by remote Comm to upload the given file to the given url at given location---->*/
    public bool transferfile(string filename, string source, string destination, string url)
    {
      return sndr.transferfile(filename, source, destination, url);
    }

  }
  ///////////////////////////////////////////////////////////////////
  // TestPCommService class - tests Receiver, Sender, and Comm

  class TestPCommService
  {
    /*----< collect file names from client's FileStore >-----------*/

    public static List<string> getClientFileList()
    {
      List<string> names = new List<string>();
      string[] files = Directory.GetFiles(ClientEnvironment.fileStorage);
      foreach (string file in files)
      {
        names.Add(Path.GetFileName(file));
      }
      return names;
    }
    /*----< compare CommMessages property by property >------------*/
    /*
     * - skips threadId property
     */
    public static bool compareMsgs(CommMessage msg1, CommMessage msg2)
    {
      bool t1 = (msg1.type == msg2.type);
      bool t2 = (msg1.to == msg2.to);
      bool t3 = (msg1.from == msg2.from);
      bool t4 = (msg1.author == msg2.author);
      bool t5 = (msg1.command == msg2.command);
      //bool t6 = (msg1.threadId == msg2.threadId);
      bool t7 = (msg1.errorMsg == msg2.errorMsg);
      if (msg1.arguments.Count != msg2.arguments.Count)
        return false;
      for (int i = 0; i < msg1.arguments.Count; ++i)
      {
        if (msg1.arguments[i] != msg2.arguments[i])
          return false;
      }
      return t1 && t2 && t3 && t4 && t5 && /*t6 &&*/ t7;
    }
    /*----< compare binary file's bytes >--------------------------*/
    static bool compareFileBytes(string filename, string subfolder)
    {
      TestUtilities.putLine(string.Format("testing byte equality for \"{0}\"", filename));

      string fileSpec1 = Path.Combine(ClientEnvironment.fileStorage, filename);
      string fileSpec2 = Path.Combine(ServiceEnvironment.fileStorage + "/" + subfolder, filename);
      try
      {
        byte[] bytes1 = File.ReadAllBytes(fileSpec1);
        byte[] bytes2 = File.ReadAllBytes(fileSpec2);
        if (bytes1.Length != bytes2.Length)
          return false;
        for (int i = 0; i < bytes1.Length; ++i)
        {
          if (bytes1[i] != bytes2[i])
            return false;
        }
      }
      catch (Exception ex)
      {
        TestUtilities.putLine(string.Format("\n  {0}\n", ex.Message));
        return false;
      }
      return true;
    }
    /*----< test Sender and Receiver classes >---------------------*/

    public static bool testSndrRcvr()
    {
      TestUtilities.vbtitle("testing Sender & Receiver");

      bool test = true;
      Receiver rcvr = new Receiver();
      rcvr.start("http://localhost", 8080);
      Sender sndr = new Sender("http://localhost", 8080);

      CommMessage sndMsg = new CommMessage(CommMessage.MessageType.request);
      sndMsg.command = "show";
      sndMsg.author = "Jim Fawcett";
      sndMsg.to = "http://localhost:8080/IMessagePassingComm";
      sndMsg.from = "http://localhost:8080/IMessagePassingComm";

      sndr.postMessage(sndMsg);
      CommMessage rcvMsg;
      // get connection message
      rcvMsg = rcvr.getMessage();
      if (ClientEnvironment.verbose)
        rcvMsg.show();
      // get first info message
      rcvMsg = rcvr.getMessage();
      if (ClientEnvironment.verbose)
        rcvMsg.show();
      if (!compareMsgs(sndMsg, rcvMsg))
        test = false;
      TestUtilities.checkResult(test, "sndMsg equals rcvMsg");
      TestUtilities.putLine();

      sndMsg.type = CommMessage.MessageType.closeReceiver;
      sndr.postMessage(sndMsg);
      rcvMsg = rcvr.getMessage();
      if (ClientEnvironment.verbose)
        rcvMsg.show();
      if (!compareMsgs(sndMsg, rcvMsg))
        test = false;
      TestUtilities.checkResult(test, "Close Receiver");
      TestUtilities.putLine();

      sndMsg.type = CommMessage.MessageType.closeSender;
      if (ClientEnvironment.verbose)
        sndMsg.show();
      sndr.postMessage(sndMsg);
      // rcvr.getMessage() would fail because server has shut down
      // no rcvMsg so no compare

      TestUtilities.putLine("last message received\n");
      return test;
    }

    /*----< test Comm instance >-----------------------------------*/
    public static bool testComm()
    {
      TestUtilities.vbtitle("testing Comm");
      bool test = true;
      Comm comm = new Comm("http://localhost", 8081);
      CommMessage csndMsg = new CommMessage(CommMessage.MessageType.request);
      csndMsg.command = "show";
      csndMsg.author = "Jim Fawcett";
      csndMsg.to = "http://localhost:8081/IMessagePassingComm";
      csndMsg.from = "http://localhost:8081/IMessagePassingComm"; comm.postMessage(csndMsg);
      CommMessage crcvMsg = comm.getMessage();
      if (ClientEnvironment.verbose)
        crcvMsg.show();

      if (!compareMsgs(csndMsg, crcvMsg))
        test = false;
      TestUtilities.checkResult(test, "csndMsg equals crcvMsg"); TestUtilities.putLine();
      TestUtilities.vbtitle("testing file transfer"); bool testFileTransfer = true;
      List<string> names = getClientFileList();
      foreach (string name in names)
      {
        TestUtilities.putLine(string.Format("transferring file \"{0}\"", name));
        bool transferSuccess = comm.postFile(name, "CommTestfiles");
        TestUtilities.checkResult(transferSuccess, "transfer");
      }
      foreach (string name in names)
      {
        if (!compareFileBytes(name, "CommTestfiles"))
        {
          testFileTransfer = false; break;
        }
      }
      TestUtilities.checkResult(testFileTransfer, "file transfers"); TestUtilities.putLine();
      TestUtilities.vbtitle("test receiver close");
      csndMsg.type = CommMessage.MessageType.closeReceiver;
      comm.postMessage(csndMsg);
      crcvMsg = comm.getMessage();
      if (ClientEnvironment.verbose)
        crcvMsg.show();
      if (!compareMsgs(csndMsg, crcvMsg))
        test = false;
      TestUtilities.checkResult(test, "closeReceiver");
      TestUtilities.putLine();
      csndMsg.type = CommMessage.MessageType.closeSender; comm.postMessage(csndMsg);
      if (ClientEnvironment.verbose)
        csndMsg.show();
      TestUtilities.putLine("last message received\n");
      return test && testFileTransfer;
    }

    /*----< do the tests >-----------------------------------------*/

    //<------------------------------------------TEST STUB----------------------------------------------------->
#if (Test_MPCommService)
    //driver logic
    static void Main(string[] args)
    {
      ClientEnvironment.verbose = true;
      TestUtilities.vbtitle("testing Message-Passing Communication", '=');

      /*----< uncomment to see Sender & Receiver testing >---------*/
      //TestUtilities.checkResult(testSndrRcvr(), "Sender & Receiver");
      //TestUtilities.putLine();

      TestUtilities.checkResult(testComm(), "Comm");
      TestUtilities.putLine();

      TestUtilities.putLine("Press key to quit\n");
      if (ClientEnvironment.verbose)
        Console.ReadKey();
    }
#endif
  }

}
