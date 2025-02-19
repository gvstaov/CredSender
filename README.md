# **CREDSENDER**

Program to send infinite bulk messages to contacts or groups in Whatsapp, without use official API.

## **Tutorial for developers**

### **List of topics:**

1. Requirements
2. How to install
3. How to generate keys
4. How to use

#### Requirements

- Download and install last version of [Google Chrome](https://www.google.com/intl/pt-BR/chrome/)
- Download and install last version of [Visual Studio](https://visualstudio.microsoft.com/pt-br/)
- Download and install last version of [Microsoft .NET Framework](https://dotnet.microsoft.com/en-us/download/dotnet-framework)

### **Open CredSender code files in Visual Studio**

1. After open the Visual Studio, go to File > Open > Project/Solution
   ![Visual Studio tutorial](/Github/VSTUTORIAL.png)
2. It will open file dialog box, navigate to CredSender code files in "Source" folder and select "WaSender.sln" file, then click on "Open" button.
   ![Visual Studio tutorial](/Github/VSTUTORIAL2.png)
3. If you able to find "Solution Explorer" window, it means, CredSender code files are opened successfully, in the "Solution Explorer" you can find all code files and forms.
   ![Visual Studio tutorial](/Github/VSTUTORIAL3.png)

### **How to change software name**

1. In the "Solution Explorer" under the "ProjectCommom" folder, find "Config.cs" file and open it. You can find "AppName" as "WaSender" string here, replace it with your own software name.
   ![Visual Studio tutorial](/Github/VSTUTORIAL4.png)
2. Now you need to change SetUp file name it will show on your software installation process. In the "Solution Explorer", right click on "WaSenderSetUp" project and click on "Properties".
   ![Visual Studio tutorial](/Github/VSTUTORIAL5.png)
3. Give any name for your installer file instead of "WaSenderSetUp".
   ![Visual Studio tutorial](/Github/VSTUTORIAL6.png)
4. Now from the "Solution Explorer", under "WaSenderSetUp" project, find "Primary output from WaSender(Active)" file, double click on it.
   ![Visual Studio tutorial](/Github/VSTUTORIAL7.png)
5. Click on "User's Desktop" folder and find "WaSender" file at right side.
   ![Visual Studio tutorial](/Github/VSTUTORIAL8.png)
6. Right click on "WaSender" file from left side, then click on "Properties Window" option.
   ![Visual Studio tutorial](/Github/VSTUTORIAL9.png)
7. Replace "WaSender" with your software name and save.
   ![Visual Studio tutorial](/Github/VSTUTORIAL10.png)

   ### **How to change software logo**

8. You will need .ICO extension file of your logo. Once you got your icon file, in Visual Studio, from "Solution Explorer", right click on "Wa Sender" project, and click on "Properties".
   ![Visual Studio tutorial](/Github/LOGO1.png)
9. In the opened window, under "Application" section, you can find the current icon of software in "icons and manifest" section. Click on those three dots under "icon" section. Choose your .ICO file and click on "Ok" button. Your selected icon will appear here, select it from this dropdown and save changes, and then you can close this tab.
   ![Visual Studio tutorial](/Github/LOGO2.png) 10. Now from the "Solution Explorer", under "WaSender" project, find "Properties" section. Expand it and find "Resources.resx" file and double click on it.
