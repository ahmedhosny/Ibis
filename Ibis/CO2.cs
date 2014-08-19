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
    public class CO2 : GH_Component
    {

        //////////public Declarations(Available to whole class)

        public XmlDocument IBIS_XML = new XmlDocument();
        public System.Drawing.Bitmap CO2Bitmap = new Bitmap("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\Icons\\CO2.png");


        public CO2()
            : base("CO2 footprint", "CO2", "Calculate the CO2 footprint of your geometry", "Ibis", "Generic Geometry Analysis")
        {
        }


        //////////
        //Set the inputs: //name, nickname, Description, Access
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Material", "Material", "Choose your Material", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddSurfaceParameter("Surface", "Surface", "Insert Surface Here", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Process", "Process", "Choose your Process ", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Thickness", "Thickness", "Insert Thickness here ", GH_ParamAccess.item);
            pManager[3].Optional = true;

        }

        //Set the outputs:
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("CO2 footprint", "CO2 footprint", "CO2 footprint", GH_ParamAccess.item);

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
            int myProcess = 0;
            if (!DA.GetData(2, ref myProcess))
            {
                return;
            }
            double myThickness = 0;
            if (!DA.GetData(3, ref myThickness))
            {
                return;
            }

            //////////

            //double myVolumeTotal = 0.0;


            //for (int i = 0; i < mySurfaceList.Count; i++)
            //{

            //    AreaMassProperties myArea = AreaMassProperties.Compute(mySurfaceList[i]);
            //    double myVolume = myArea.Area * myThickness;
            //    myVolumeTotal = +myVolume;
            //}






            double myCO2 = 0.0;
            ////////// Code to loop through XML nodes and get MinRad value:-
            //double myDensity = 0.0;
            //string temp = IBIS_XML.SelectSingleNode("IBIS/Mass/Material2[@id= '" + myMaterial + "']").InnerText;
            //myDensity = Convert.ToDouble(temp);
            //////////

            DA.SetData(0, myCO2);

            //////////
        }
       

        ////////// Icon of component (needs more work) :-
        protected override Bitmap Icon
        {
            get
            {
                return new Bitmap(CO2Bitmap);
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("{456e8cd8-626d-41ce-8a41-4a77dd072d10}"); }
                                    
        }
    }
}
