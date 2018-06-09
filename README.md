					Remote Build Server  

Background Information:

In order to successfully implement big systems we need to partition code into relatively small parts and thoroughly test each of the parts before inserting them into the software baseline2. As new parts are added to the baseline and as we make changes to fix latent errors or performance problems we will re-run test sequences for those parts and, perhaps, for the entire baseline. Because there are so many packages the only way to make this intensive testing practical is to automate the process.


The process, described above, supports continuous integration. That is, when new code is created for a system, we build and test it in the context of other code which it calls, and which call it. As soon as all the tests pass, we check in the code and it becomes part of the current baseline. There are several services necessary to efficiently support continuous integration.


a Federation of servers, each providing a dedicated service for continuous integration.

![alt text](https://github.com/RepakaRamateja/Remote-Build-Server/blob/master/images/federation.png)

</br>

The Federation consists of:


Repository:
Holds all code and documents for the current baseline, along with their dependency relationships. It also holds test results and may cache build images.


Build Server:
Based on build requests and code sent from the Repository, the Build Server builds test libraries for submission to the Test Harness. On completion, if successful, the build server submits test libraries and test requests to the Test Harness, and sends build logs to the Repository.



Test Harness:
Runs tests, concurrently for multiple users, based on test requests and libraries sent from the Build Server. Clients will checkin, to the Repository, code for testing, along with one or more test requests. The repository sends code and requests to the Build Server, where the code is built into libraries and the test requests and libraries are then sent to the Test Harness. The Test Harness executes tests, logs results, and submits results to the Repository. It also notifies the author of the tests of the results.


Client:
The user's primary interface into the Federation, serves to submit code and test requests to the Repository. Later, it will be used view test results, stored in the repository.


</br>

This project focuses on  developing  a Remote Build Server and document its design with a document.

</br>

Build Server functionality:

</br>

Provide a Build Server that uses Message-Passing Communication based on your Comm prototype.

Use mock Repository and Test Harness servers that are functioning processes, with message-passing communication. However, they provide just enough functionality to demonstrate that your Remote Build Server functions as expected.

Provide a mock Client process, using WPF, based on your GUI prototype that has just enough functionality to demonstrate that your Build Server functions as expected.

Build Server Design Document should:
elaborating with design details.

Show activity diagrams, package diagrams, and class diagrams that illustrate the way you've implemented your server and its environment.

Comments on possible inadequacies and errors of commission.

Draws conclusions about what you like and don't like about your final implementation.

Note that, in these project, we will not be integrating our Build Server with a Federation's Repository and Test Harness Servers. Instead, we will build mock Repository and Test Harness servers that supply just enough functionality to demonstrate operations of the Remote Build Server. The Build Server will use a "Federation ready" communication channel to communicate with the mock servers, and we will build a mock client that has just enough functionality to demonstrate Build Server working in this environment.


So the mock Repository and mock Test Harness are simple servers, running in their own processes, using our Message-Passing Communication, to send and receive requests and replys. However, the Mock operations are simple not nearly as complex as full up Federated servers.

</br>

--------------------------------------------------------    Requirements:  -----------------------------------------------

Build Server

</br>

Shall be prepared using C#, the .Net Framework, and Visual Studio 2017.

Shall include a Message-Passing Communication Service built with WCF. It is expected that you will build on your Project #3 Comm Prototype.


The Communication Service shall support accessing build requests by Pool Processes from the mother Builder process, sending and receiving build requests, and sending and receiving files.


Shall provide a Repository server that supports client browsing to find files to build, builds an XML build request string and sends that and the cited files to the Build Server.


Shall provide a Process Pool component that creates a specified number of processes on command.


Pool Processes shall use message-passing communication to access messages from the mother Builder process.


Each Pool Process shall attempt to build each library, cited in a retrieved build request, logging warnings and errors.


If the build succeeds, shall send a test request and libraries to the Test Harness for execution, and shall send the build log to the repository.


The Test Harness shall attempt to load each test library it receives and execute it. It shall submit the results of testing to the Repository.


Shall include a Graphical User Interface, built using WPF.


The GUI client shall be a separate process, implemented with WPF and using message-passing communication. It shall provide mechanisms to get file lists from the Repository, and select files for packaging into a test library1, e.g., a test element specifying driver and tested files, added to a build request structure. It shall provide the capability of repeating that process to add other test libraries to the build request structure.


The client shall send build request structures to the repository for storage and transmission to the Build Server.


The client shall be able to request the repository to send a build request in its storage to the Build Server for build processing.


Your As-Built Design Document


Shall used activity diagrams, package diagrams, and class diagrams to describe the essential parts of your design and implementation.


Shall comment on changes to the core concept as your design evolved, and on deficiencies you feel your project incorporates3.

</br>

Package diagram

</br>

![alt text](https://github.com/RepakaRamateja/Remote-Build-Server/blob/master/images/package.png)

</br>

Process pool diagram (please refer FinalSmaOcd for more better clear understanding)

</br>

![alt text](https://github.com/RepakaRamateja/Remote-Build-Server/blob/master/images/processpool.png)

</br>


						Execution Instructions:

Prerequisites:

 Microsoft Visual Studio 2015 0r 2017


  Now follow the below steps


               open developer command prompt for vs2017 by administrator mode

               Navigate to the downloaded project location   

               Run compile.bat

               Run run.bat

</br>

UserInterface 

</br>

Generate Xml Requests

</br>

![alt text](https://github.com/RepakaRamateja/Remote-Build-Server/blob/master/images/1.png)

</br>

Handle Build Requests

</br>

![alt text](https://github.com/RepakaRamateja/Remote-Build-Server/blob/master/images/2.png)

</br>

View Logs

</br>

![alt text](https://github.com/RepakaRamateja/Remote-Build-Server/blob/master/images/3.png)

</br>

Downloading Remote Logs by selecting and clicking on Download Button

</br>

![alt text](https://github.com/RepakaRamateja/Remote-Build-Server/blob/master/images/4.png)

</br>

Viewing the downloaded logs 

</br>

![alt text](https://github.com/RepakaRamateja/Remote-Build-Server/blob/master/images/5.png)


</br>

![alt text](https://github.com/RepakaRamateja/Remote-Build-Server/blob/master/images/6.png)


</br>

Please go through the design document (FinalSmaOcd) for more better clear understanding.
