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
    public class Gauge : GH_Component
    {

        //////////public Declarations(Available to whole class)

         public XmlDocument IBIS_XML = new XmlDocument();
        public System.Drawing.Bitmap GuageBitmap = new Bitmap("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\Icons\\guage.png");


        public Gauge()
            : base("Gauge", "Gauge", "Calculate the Guage of your sheet", "Ibis", "Generic Geometry Analysis")
        {
        }

        //////////
        //Set the inputs: //name, nickname, Description, Access
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Material", "Material", "Choose your Material" , GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddNumberParameter("Gauge", "Gauge", "Enter your Gauge" , GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        //Set the outputs:
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Thickness(in)", "Thickness(in)", "Thickness(in)", GH_ParamAccess.item);

        }


        protected override void BeforeSolveInstance()
        {

            Grasshopper.Kernel.Parameters.Param_Integer myMaterialParam = Params.Input[0] as Grasshopper.Kernel.Parameters.Param_Integer;
            myMaterialParam.ClearNamedValues();

            //////////  Code for automatically loading Materials from XML when intance of class is initiated.
            IBIS_XML.Load("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\IBIS_XML.xml");
            List<String> myMaterialNameList = new List<string>();
            XmlNodeList myList = IBIS_XML.GetElementsByTagName("Material");
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





            Grasshopper.Kernel.Parameters.Param_Integer myGrainParam = Params.Input[2] as Grasshopper.Kernel.Parameters.Param_Integer;
            myGrainParam.ClearNamedValues();


            Grasshopper.Kernel.Parameters.Param_Integer myThicknessParam = Params.Input[3] as Grasshopper.Kernel.Parameters.Param_Integer;
            myThicknessParam.ClearNamedValues();


            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Integer> myMaterialValue = myMaterialParam.VolatileData as Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Integer>;
            //GH_Structure<GH_Integer> data = param0.VolatileData as GH_Structure<GH_Integer>;

        }





        //DA.GetData vs DA.SetData:
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //////////

            double myGauge = 0.0;
            if (!DA.GetData(0, ref myGauge))
            {
                return;
            }
            int myMaterial =0;
            if (!DA.GetData(1, ref myMaterial))
            {
                return;
            }




            ////////// Code to loop through XML nodes and get MinRad value:-
            //double myDensity = 0.0;
            //string temp = IBIS_XML.SelectSingleNode("IBIS/Mass/Material2[@id= '" + myMaterial + "']").InnerText;
            //myDensity = Convert.ToDouble(temp);
            //////////
            double myGauge = 0.0;
            DA.SetData(0, myGauge);

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
