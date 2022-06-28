#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.EthernetIP;
using FTOptix.NativeUI;
using FTOptix.CommunicationDriver;
using FTOptix.UI;
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
#endregion

#region 

using System.Xml;
using System.Xml.Schema;
using System.IO;



using System.ComponentModel;
using System.Diagnostics;
//using System.Drawing;

using System.Security;
#endregion






public class DesignTimeNetLogic1 : BaseNetLogic
{
    [ExportMethod]
    public void Method1()
    {
        // Insert code to be executed by the method

        //string XMLpath;




        // Insert code to be executed by the method
        // QPlatform.Alarm.DigitalAlarm
        //string XMLpath = Directory.GetCurrentDirectory() + @"\MachineBuilder_ControlLogix.L5X";
        string XMLpath = Directory.GetCurrentDirectory() + @"\SFC_Routine_SFC.L5X";

        Log.Info("Current Path: " +  XMLpath );

        // Create and load the XML document.
        XmlDocument l5x = new XmlDocument();
        l5x.Load(XMLpath);

        // Make changes to the document.
        XmlElement controller = (XmlElement)l5x.DocumentElement.FirstChild;
        Log.Info("Found Project: "  + controller.GetAttribute("Name") + " Version: " + controller.GetAttribute("MajorRev") + "." + controller.GetAttribute("MinorRev") + " Controller: " + controller.GetAttribute("ProcessorType") );



        searchMBLObj(controller);        
        ReadSfc(controller);
    }


    
    public static void searchMBLObj (XmlElement controller)
    {
        //XmlNodeList tagList = l5x.GetElementsByTagName("Tag");
        XmlNodeList tagList = controller.GetElementsByTagName("Tag");

        Console.WriteLine("Found StateMaschine: ");
        bool foundMblObj = false;

        for (int i = 0; i < tagList.Count; i++)
        {
            XmlElement testNode = (XmlElement)tagList[i];

            if (testNode.GetAttribute("DataType") == "raM_Opr_ISA880001AnnexD_2010" || testNode.GetAttribute("DataType") == "raM_Opr_ISATR880002_2015")
            {
                //XmlElement parentNode = testNode.ParentNode();
                XmlElement GrandParentNode = (XmlElement)tagList[i].ParentNode.ParentNode;
                Log.Info(testNode.GetAttribute("Name") + " von " + GrandParentNode.GetAttribute("Name"));
                var myPanel = Project.Current.FindObject("Panel1");
                var newLabel = InformationModel.MakeObject<Label>(GrandParentNode.GetAttribute("Name"));
                newLabel.Text = GrandParentNode.GetAttribute("Name");
                myPanel.Add(newLabel);
                foundMblObj = true;

            }
        }
        if (foundMblObj == true)
        {
            Log.Info("found MBL Library Objects");
        }
        else
        {
            Log.Info("no MBL Library Objects found");
        }
        return;
    }



    public static void ReadSfc(XmlElement controller)
    {
        Log.Info("Read SFC");
        XmlNodeList SFCContent_Step = controller.GetElementsByTagName("Step");
        XmlNodeList SFCContent_Transition = controller.GetElementsByTagName("Transition");
        XmlNodeList SFCContent_Branch = controller.GetElementsByTagName("Branch");

        var myfirstP = Project.Current.FindObject("ScrollView2");
        var myPanel = InformationModel.MakeObject<Panel>("SFC");
        //myPanel.HorizontalAlignment = "Stretch";


        myPanel.Width = -1;
        myPanel.Height = -1;
        


        myfirstP.Add(myPanel);

        CreateSfStep(SFCContent_Step);
        Log.Info("Step done");
        CreateSfStep(SFCContent_Transition);
        Log.Info("Transition done");
        CreateSfStep(SFCContent_Branch);
        Log.Info("Branch done");

        Log.Info("done");
        return;
    }



    public static void CreateSfStep(XmlNodeList SFCContent)
    {
        var mySFCPanel = Project.Current.FindObject("SFC");

        Log.Info("start loop: " + SFCContent.Count);
        for (int i = 0; i < SFCContent.Count; i++)
        {
            XmlElement testNode = (XmlElement)SFCContent[i];

            if (testNode.Name == "Step" || testNode.Name == "Transition")
            {
                var myStep = InformationModel.MakeObject<Rectangle>(testNode.GetAttribute("Operand"));
                myStep.LeftMargin   = (float)Convert.ToDouble(testNode.GetAttribute("X"));
                myStep.TopMargin    = (float)Convert.ToDouble(testNode.GetAttribute("Y"));


                myStep.Height       = 80;
                myStep.FillColor    = Colors.Transparent;
                myStep.BorderThickness = 3;


                var newLabel = InformationModel.MakeObject<Label>(testNode.GetAttribute("Operand"));
                newLabel.Text = testNode.GetAttribute("Operand");




                if (testNode.Name == "Step")
                {
                    myStep.Width            = 80;
                    myStep.BorderColor      = Colors.DarkGray;
                    myStep.BorderThickness  = 3;

                    newLabel.LeftMargin     = 10;
                    newLabel.TopMargin      = 60;
                }



                if (testNode.Name == "Transition")
                {
                    myStep.Width            = 120;
                    myStep.BorderColor      = Colors.Transparent;
                    myStep.BorderThickness  = 0;

                    newLabel.LeftMargin     = 45;
                    newLabel.TopMargin      = 20;

                    var myhorizontal = InformationModel.MakeObject<Rectangle>("Horizontal");
                    var myvertical = InformationModel.MakeObject<Rectangle>("Vertical");

                    myhorizontal.Height     = 3;
                    myhorizontal.Width      = 80;
                    myhorizontal.FillColor  = Colors.DarkGray;
                    myhorizontal.TopMargin  = 40;
                    myhorizontal.LeftMargin = 0;


                    myvertical.Height       = 60;
                    myvertical.Width        = 3;
                    myvertical.FillColor    = Colors.DarkGray;
                    myvertical.TopMargin    = 0;
                    myvertical.LeftMargin   = 40;

                    myStep.Add(myhorizontal);
                    myStep.Add(myvertical);
                }







                myStep.Add(newLabel);
                mySFCPanel.Add(myStep);
                //mySFCPanel.Add(newLabel);
            }
            else
            {
                var myBranch = InformationModel.MakeObject<Rectangle>("Branch" + testNode.GetAttribute("ID"));
                myBranch.TopMargin = (float)Convert.ToDouble(testNode.GetAttribute("Y"));
                myBranch.Height = 3;
                myBranch.Width = 80;
                myBranch.FillColor = Colors.Black;
                myBranch.LeftMargin = 100;

                myBranch.FillColor = Colors.Black;
                mySFCPanel.Add(myBranch);
            }                     
        }
        return;
    }
}
