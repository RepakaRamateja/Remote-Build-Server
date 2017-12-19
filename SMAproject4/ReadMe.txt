Please run run.bat as adminstrator just a reminder

Mock client is started at port number 8010

Mock repo is started at port number 8080

Mother builder is started at port number 8090

Child builder is started at port number 8091

Child builder 2 started at port number 8092

Mock test harness started at port number 8040


Mock repo  contains two command line arguments  

1) Mock repo port number
2) BuildServer(Mother builder) port number


Mother builder contains two arguments

1)Child process start port number
2)Mock Repo port number
3)TestHarness port number
4)no of pool process to be started
5)Mock Client port number


Mock Test harness takes two arguments

1)Mock test harness start port number

2)Mock repo port number


Locations of mock client local repository is  SMAproject4\MockClient\ClientStorage

Location of Mock repo is  SMAproject4\MockRepository\RepoStorage

Location of child builders is SMAproject4\CoreBuilder\Builderstorage

Location of Mock test harness is SMAproject4\MockTestHarness\DLLRepository

---------------------------------------------------------------------------------------------------------------
If you want to send any request for processing please use BuildRequest4.xml which i created for instructors to test
--------------------------------------------------------------------------------------------------------------------
follow the instructions in handle requests tab which are written in textblock

i.e get the local requests and select buildrequest4.xml and click on send

then view the remote build requests and select buildrequest4.xml and click on build button........... 
-----------------------------------------------------------------------------------------------------------------------  

In order to generate Build Requests

navigate to Generate Request tab

click on getRepoContents

now select driver and select source files 

click on show selected files

now you will see only selected files

now click generate build request item is   generated and added to build request

now repeat to add many number of build request items to build request

if you want to start from the strach click on clear button

click on save to save the request.

In the handle request tab instructions is clearly written in the wpf tab itself if you want to send any requests for processing 

for viewing the logs also instructions are clearly written please follow the steps in the logs tab itself. 

-------------------------------------------------------------------------------------------------------------------------------
In order to view the logs at client side 

click on getremote logs then select what log u want to see click on download

then click on load downloaded logs all the logs will shown there

double click on to open through pop up

also see the instructions in the logs view to understand clearly

