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

            List<Surface> mySurfaceList = new List<Surface>();
            if (!DA.GetDataList(0, mySurfaceList))
            {
                return;
            }
            double myInnerRadius = 0.0;
            if (!DA.GetData(1, ref myInnerRadius))
            {
                return;
            }
            bool myFlip = false;
            if (!DA.GetData(2, ref myFlip))
            {
                return;
            }
            //////////
            List<double> myAngleList = new List<double>();
            List<Surface> myNewSurfaceList = new List<Surface>();
            
            //Mian loop
            for (int i = 0; i < mySurfaceList.Count-1; i++)
            {
                //Figure out the angles
                double u1 = (mySurfaceList[i].Domain(0).Max - mySurfaceList[i].Domain(0).Min) / 2;
                double v1 = (mySurfaceList[i].Domain(1).Max - mySurfaceList[i].Domain(1).Min) / 2;
                Vector3d myNormal1 = mySurfaceList[i].NormalAt(u1, v1);
                double u2= (mySurfaceList[i+1].Domain(0).Max - mySurfaceList[i+1].Domain(0).Min) / 2;
                double v2 = (mySurfaceList[i+1].Domain(1).Max - mySurfaceList[i+1].Domain(1).Min) / 2;
                Vector3d myNormal2 = mySurfaceList[i+1].NormalAt(u1, v1);
                double myAngle = Vector3d.VectorAngle(myNormal1, myNormal2) * (180/Math.PI);
                myAngleList.Add(myAngle);

                //Create fillet between surfaces , add it to list later on..
                Surface[] myFillet = Rhino.Geometry.Surface.CreateRollingBallFillet(mySurfaceList[i], myFlip, mySurfaceList[i+1], myFlip, myInnerRadius, 0.0);
                //Get P1 and P3 (begining and end of fillet)
                Point3d P1 = new Point3d(myFillet[0].PointAt(myFillet[0].Domain(0).Min, myFillet[0].Domain(1).Min));
                Point3d P3 = new Point3d(myFillet[0].PointAt(myFillet[0].Domain(0).Max, myFillet[0].Domain(1).Max));
                //evaluate surface 0 at P1 to get u,v coordinates
                double uOne;
                double vOne;
                mySurfaceList[i].ClosestPoint(P1, out  uOne, out  vOne);
                //trim surface 0 and add to list
                Surface[] mySurface0 = mySurfaceList[i].Split(1, vOne);
                myNewSurfaceList.Add(mySurface0[1]);
                //add fillet to list
                myNewSurfaceList.Add(myFillet[0]);
                //evaluate surface 1 at P3 to get u,v coordinates
               double uTwo;
               double vTwo;
                mySurfaceList[i+1].ClosestPoint(P3, out  uTwo, out  vTwo);
                //trim surface 1 and add to list
              Surface[] mySurface1 = mySurfaceList[i+1].Split(1, vTwo);
                myNewSurfaceList.Add(mySurface1[0]);
            }
            //Now to solve the intersection problem for all surfaces excluding fillets, the first surface and the last one
            for (int i = myNewSurfaceList.Count - 3; i > 2; i = i - 2)
            {
                //for 2
                Point3d P1 = new Point3d(myNewSurfaceList[i ].PointAt(myNewSurfaceList[i ].Domain(0).Min, myNewSurfaceList[i ].Domain(1).Min));
                Point3d P2 = new Point3d(myNewSurfaceList[i ].PointAt(myNewSurfaceList[i ].Domain(0).Max, myNewSurfaceList[i ].Domain(1).Min));
           //for 3
                Point3d P3 = new Point3d(myNewSurfaceList[i - 1].PointAt(myNewSurfaceList[i - 1].Domain(0).Max, myNewSurfaceList[i - 1].Domain(1).Max));
                Point3d P4 = new Point3d(myNewSurfaceList[i - 1].PointAt(myNewSurfaceList[i - 1].Domain(0).Min, myNewSurfaceList[i - 1].Domain(1).Max));
                //create surface from these points.. add it to end of myNewSurfaceList
                NurbsSurface myNurbs =  NurbsSurface.CreateFromCorners(P1, P2, P3, P4);
               
                //delete these two surfaces and add myNurbs
                myNewSurfaceList.RemoveAt(i);
                myNewSurfaceList.RemoveAt(i-1);
                myNewSurfaceList.Insert(i, myNurbs);


                //Failed attempt to convert the two surfaces to brep and intersect them
               //Brep myBrep1 =  myNewSurfaceList[i].t
               //Brep myBrep2 = myNewSurfaceList[i+1].ToBrep();
               //Brep[] myIntersection = Brep.CreateBooleanIntersection(myBrep1, myBrep2, 0.001);
               //myNewBrepList.Add(myIntersection[0]);
            }

            //////////
            DA.SetDataList(0, myAngleList);
            DA.SetDataList(1, myNewSurfaceList);

     

           

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
