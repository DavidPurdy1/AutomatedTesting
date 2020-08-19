# Console Tests

A library built for Automated testing of Intact using Winium Driver.

This includes a WebApp test result UI to see the results of all tests ran
https://github.com/DavidPurdy1/Testing-DashBoard

-----
# Getting Started
Download from the zip from the release for the test runner

Clone this repo and https://github.com/DavidPurdy1/Testing-DashBoard 

Set any configurations

Run the Testing Dashboard and choose the test you want to run

-----
# Features to Add 
- Add documentation to the jira wiki on stuff like test cases, update running a test, etc....

- Add a scheduler to have the tests run at a certain time or interval

- Add annotations (Unimplemented Exeception) 

- Port to winApp driver

- Add documentation and implement definition definition to make things safer. 

- THE WEB APP IS NOT ADDING RUNS WITH MULTIPLE TEST CASES

# Features Added 

- Creation of Types and definitions and documents

- Logging in(Set the path for the application and the user) 

- Adding Documents to InZone (set up the InZone definitions and documents in the right path for it to work)

- Adding Documents through batch review(Set up the definition and type for batch review)

- If failed can take screenshots then add them to a specified directory

- Default values for everything, but option to specify exactly what you want for most of the tests. Trying to balance ease and customization of the tests 

- Inzone recognizes correct definitions when coming in. 

- Optional Fail file that writes each test run, which contains test cases, where each test case can contain documents, and each document can have metadata values

- Logout and log back into intact testing

- Search Intact

- Support for recognition

- Check for both interruptions and for errors thrown by the application

- Get and Set Windows and Dispose Error Messages after each test. 
