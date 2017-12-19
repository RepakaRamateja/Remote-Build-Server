///////////////////////////////////////////////////////////////////////////////
// CodePopUp.xaml.cs - Displays text file source in response to double-click //
// ver 1.0                                                                   //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2017           //
// Application: CSE681 CSE681 Project 4-Remote Build Server                  //
// Environment: C# console                                                   //                                                                     
///////////////////////////////////////////////////////////////////////////////

/*
 * Package Operations:
 * -------------------
 *  Demonstrates the functionalities of Code Pop up
 * 
 * Public Interface
 * ----------------
 * This does not contain any public methods
 *  But only one public constructor MainWindow --- which initializes the main component through method InitializeComponent
 *  
 * Required Files:
 * ---------------
 * CodePopUp.xaml.cs
 * 
 * Build Process
 * -------------
 * csc CodePopUp.xaml.cs
 * devenv MockClient.csproj
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Navigator
{
  public partial class CodePopUp : Window
  {
    //constructor which initializes the mainwindow through method InitializeComponent 
    public CodePopUp()
    {
      InitializeComponent();
    }
  }
}
