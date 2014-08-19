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
    public class Weight : GH_Component
    {

        //////////public Declarations(Available to whole class)

        public XmlDocument IBIS_XML = new XmlDocument();
        public System.Drawing.Bitmap WeightBitmap = new Bitmap("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\Icons\\weight.png");


        public Weight()
            : base("Weight", "Weight", "Calculate the Weight of your geometry", "Ibis", "Generic Geometry Analysis")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Material", "Material", "Choose your Material", GH_ParamAccess.item);
            pManager[0].Optional = true;
            Grasshopper.Kernel.Parameters.Param_Integer myMaterialParam = Params.Input[0] as Grasshopper.Kernel.Parameters.Param_Integer;
            myMaterialParam.ClearNamedValues();

            //////////  Code for automatically loading Materials from XML when intance of class is initiated.
            IBIS_XML.Load("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\IBIS_XML.xml");
            List<String> myMaterialNameList = new List<string>();
            XmlNodeList myList = IBIS_XML.GetElementsByTagName("Material2");
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

            pManager.AddSurfaceParameter("Surface", "Surface", "Insert Surface Here", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Thickness", "Thickness", "Insert your Thickness here", GH_ParamAccess.item);
            pManager[2].Optional = true;

        }

        //Set the outputs:
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Weight", "Weight", "Weight of geometry", GH_ParamAccess.item);

        }


        //here is where the thinking happens...
        //DA.GetData vs DA.SetData:
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //////////

            int myMaterial = 0;
            if (!DA.GetData(0, ref myMaterial))
            {
                return;
            }
            List<Surface> mySurfaceList = new List<Surface>();
            if (!DA.GetDataList(1, mySurfaceList))
            {
                return;
            }
            double myThickness = 0.0;
            if (!DA.GetData(2, ref myThickness))
            {
                return;
            }
            //////////


            double myVolumeTotal = 0.0;


            for (int i = 0; i < mySurfaceList.Count; i++)
            {

                AreaMassProperties myArea = AreaMassProperties.Compute(mySurfaceList[i]);
                double myVolume = myArea.Area * myThickness;
                myVolumeTotal = +myVolume;
            }




            ////////// Code to loop through XML nodes and get MinRad value:-
            double myDensity = 0.0;
            string temp = IBIS_XML.SelectSingleNode("IBIS/Mass/Material2[@id= '" + myMaterial + "']").InnerText;
            myDensity = Convert.ToDouble(temp);
            //////////
            double myMass = 0.0;
            myMass = myDensity * myVolumeTotal;
            DA.SetData(0, myMass);

            //////////
        }
       
       

        ////////// Icon of component (needs more work) :-
        protected override Bitmap Icon
        {
            get
            {
                return new Bitmap(WeightBitmap);
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("{bd06f659-0372-41e2-a90a-1082c4b4100a}"); }
                                    
        }
    }
}
