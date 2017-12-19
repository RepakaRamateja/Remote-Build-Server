cd TestExecutive/bin/Debug
start TestExecutive.exe 
cd ../../../
cd MockClient/bin/Debug
start TestRequestBuilder.exe
cd ../../../
cd MockRepository/bin/Debug
start MockRepository.exe 8080 8090
cd ../../../
cd MotherBuilder/bin/Debug
start MotherBuilder.exe 8091 8080 8040 2 8010
cd ../../../
cd MockTestHarness/bin/Debug 
start MockTestHarness.exe 8040 8080
