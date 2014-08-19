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
using System.IO;//for exporting txt file
namespace Ibis
{
    public class SurfaceAngle : GH_Component
    {

        //////////public Declarations(Available to whole class)

        public XmlDocument IBIS_XML = new XmlDocument();
        public System.Drawing.Bitmap SurfaceAngleBitmap = new Bitmap("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\Icons\\SurfaceAngle.png");


        public SurfaceAngle()
            : base("SurfaceAngle ", "SurfaceAngle ", "Calculate angles between surfaces", "Ibis", "Generic Geometry Analysis")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surfaces", "Surfaces", "Input your Surfaces", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager.AddNumberParameter("InnerBendRadius", "InnerBendRadius", "Input your InnerBendRadius", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddBooleanParameter("Flip", "Flip", "Flipped or not?", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        //Set the outputs:
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Angles", "Angles", "Angles between Surfaces", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("AdjustedSurfaces", "AdjustedSurfaces", "AdjustedSurfaces", GH_ParamAccess.list);
           

        }


        //here is where the thinking happens...
        //DA.GetData vs DA.SetData:
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //////////






            List<Point3d> tempList = new List<Point3d>();
            
   
            Brep myBrep = new Brep();
            myBrep = Brep.CreateFromBox(tempList);





            List<NurbsCurve> myTempList = new List<NurbsCurve>();
            myTempList.Add(L1.ToNurbsCurve());
            myTemp










            //////////
        }
       
       

        ////////// Icon of component (needs more work) :-
        protected override Bitmap Icon
        {
            get
            {
                return new Bitmap(SurfaceAngleBitmap);
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("{c3e5723e-d887-487e-b0b7-e8cf57858223}"); }
                                    
        }
    }
}
