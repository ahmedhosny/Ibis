using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.GUI; // for UI signs
using Rhino.Collections;//For RhinoList.. 
using System.Xml; //for XML
using System.Xml.XPath; //for xpath 
using System.Collections.Generic; //for List
using Rhino.Display; //for displaying text and colors in viewport
using System.Windows.Forms; //for dropdown menu
using System.Drawing;//for Bitmapgra
namespace Ibis
{
    public class Thickness : GH_Component
    {

        //////////public Declarations(Available to whole class)

         public XmlDocument IBIS_XML = new XmlDocument();
        public System.Drawing.Bitmap GuageBitmap = new Bitmap("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\Icons\\guage.png");


        public Thickness()
            : base("Thickness", "Thickness", "Calculate the Thickness of your sheet metal", "Ibis", "Generic Geometry Analysis")
        {
        }

        //Set the inputs: //name, nickname, Description, Access
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Material", "Material", "Choose your Material" , GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Gauge", "Gauge", "Enter your Gauge" , GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        //Set the outputs:
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Thickness(in)", "Thickness(in)", "Thickness(in)", GH_ParamAccess.item);

        }
        //Get list of materials and their corresponding gauge values
        protected override void BeforeSolveInstance()
        {

            Grasshopper.Kernel.Parameters.Param_Integer myMaterialParam = Params.Input[0] as Grasshopper.Kernel.Parameters.Param_Integer;
            myMaterialParam.ClearNamedValues();
            Grasshopper.Kernel.Parameters.Param_Integer myGaugeParam = Params.Input[1] as Grasshopper.Kernel.Parameters.Param_Integer;
            myGaugeParam.ClearNamedValues();

            //////////  Code for automatically loading Materials from XML when intance of class is initiated.
            IBIS_XML.Load("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\IBIS_XML.xml");
            List<String> myMaterialNameList = new List<string>();
            XmlNodeList myList = IBIS_XML.GetElementsByTagName("ThicknessMaterial");
            foreach (XmlNode mx in myList)
            {
                XmlElement x = mx as XmlElement;
                string myXMLMaterial = x.GetAttribute("type");
                myMaterialNameList.Add(myXMLMaterial);
            }

            for (int i = 0; i < myMaterialNameList.Count; i++)
            {
                myMaterialParam.AddNamedValue(myMaterialNameList[i], i + 1);


            }


            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Integer> myMaterialValue = myMaterialParam.VolatileData as Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Integer>;

            if (myMaterialValue.IsEmpty) return;

            //Code to load the Gauge list depending on user Material input:-
            List<String> myGaugeList = new List<string>();
            IBIS_XML.Load("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\IBIS_XML.xml");
            foreach (Grasshopper.Kernel.Types.GH_Integer myThis in myMaterialValue.AllData(true))
            {
                switch (myThis.Value)
                {

                    case 1: //for standard steel
                        //////////Gauge numbers
                        XmlNodeList myNodeList1 = IBIS_XML.SelectNodes("IBIS/ThicknessGauge/ThicknessMaterial[@id=1]/Gauge");
                        foreach (XmlNode myNode in myNodeList1)
                        {
                            XmlElement myElement = myNode as XmlElement;
                            String myXMLGauge = myElement.GetAttribute("type");
                            myGaugeList.Add(myXMLGauge);
                        }

                        for (int i = 0; i < myGaugeList.Count; i++)
                        {
                            myGaugeParam.AddNamedValue(myGaugeList[i], i + 1001);
                        }
                        break;

                }
                return;
            }
        }



        //DA.GetData vs DA.SetData:
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //////////

         
            int myMaterial =0;
            if (!DA.GetData(0, ref myMaterial))
            {
                return;
            }
            int myGauge = 0;
            if (!DA.GetData(1, ref myGauge))
            {
                return;
            }

            ////////// Code to loop through XML nodes and get Thickness value:-
           double myThicknessInches= 0.0;
           string temp = IBIS_XML.SelectSingleNode("IBIS/ThicknessGauge/ThicknessMaterial[@id= '" + myMaterial + "']/Gauge[@id= '" + myGauge + "']").InnerText;
           myThicknessInches = Convert.ToDouble(temp);
            //////////

           DA.SetData(0, myThicknessInches);

            //////////
        }
       

        ////////// Icon of component (needs more work) :-
        protected override Bitmap Icon
        {
            get
            {
                return new Bitmap(GuageBitmap);
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("{224de7e2-4fa4-4fdd-b323-5b8f4d24fa8a}"); }
                                    
        }
    }
}
