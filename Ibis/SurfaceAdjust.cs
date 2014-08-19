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
    public class SurfaceAdjust : GH_Component
    {

        //////////public Declarations(Available to whole class)

        public System.Drawing.Bitmap SurfaceAdjustBitmap = new Bitmap("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\Icons\\surfaceadjust.png");


        public SurfaceAdjust()
            : base("Surface Adjust", "SrfAdjust", "Adjusts Surface for Fabrication", "Ibis", "Geometry Rectification")
        {

        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "Surface", "Insert Surface Here", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("MinimumRadius", "MinimumRadius", "Insert Minimum Radius Here", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("SampleDensity", "SampleDensity", "Insert Sample Density Here", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("ChangeFactor", "ChangeFactor", "Insert Change factor Here.", GH_ParamAccess.item);
            pManager[3].Optional = true;

        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("AdjustedSurface", "AdjustedSurface", "Your adjusted surface", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Surface mySurface = null;
            if (!DA.GetData(0, ref mySurface))
            {
                return;
            }
            int myMinRad = 0;
            if (!DA.GetData(1, ref myMinRad))
            {
                return;
            }
            int mySampleDensity = 0;
            if (!DA.GetData(2, ref mySampleDensity))
            {
                return;
            }
            double myFactor = 0.0;
            if (!DA.GetData(3, ref myFactor))
            {
                return;
            }


            List<Curve> myCurveList = new List<Curve>();
            List<double> myOffsetList = new List<double>();
            List<Point3d> myOriginalList = new List<Point3d>();

            List<List<Point3d>> myNewListList = new List<List<Point3d>>();
            List<Vector3d> myNormalList = new List<Vector3d>();
            //Get Curve from Surface
            double MIN1 = mySurface.Domain(1).Min;
            double MAX1 = mySurface.Domain(1).Max;
            double myStep = (MAX1 - MIN1) / mySampleDensity;
            for (int i = 0; i < (mySampleDensity - 1); i++)
            {
                Curve myCurve = mySurface.IsoCurve(0, i * myStep); //Problem here? 1 and 0?
                myCurveList.Add(myCurve);
            }
            //Loop through Curve;
            foreach (Curve myCurve in myCurveList)
            {
                /*Curve c = myCurve.DuplicateCurve();
                
                
                for(int iter=0; iter<10; ++iter) {
                    double[] t = c.DivideByCount(100, true);
                    List<Point3d> cp = new List<Point3d>();
                    for (int k = 0; k <t.Length; ++k)
                    {
                        Point3d p = c.PointAt(t[k]);
                        Vector3d k1=c.CurvatureAt(t[k]);
                        p += k1 * 0.01;
                        cp.Add(p);
                    }
                    c = NurbsCurve.CreateInterpolatedCurve(cp, 3);
                }*/

                List<Point3d> myNewList = new List<Point3d>();
                double Max = myCurve.Domain.Max;
                double Min = myCurve.Domain.Min;
                double step = (Max - Min) / mySampleDensity;
                for (int i = 0; i < mySampleDensity - 1; i++)
                {
                    Point3d P = new Point3d(myCurve.PointAt(step * i));
                    myOriginalList.Add(P); //Make List of Points
                    myNewList.Add(P);
                    Vector3d myCurvature = myCurve.CurvatureAt(step * i);
                    double myRadius = 1 / myCurvature.Length;
                    double myOffset = myMinRad - myRadius;
                    myOffsetList.Add(myOffset); //Make List of offset
                    double u;
                    double v;
                    mySurface.ClosestPoint(P, out u, out v);
                    Vector3d myNormal = mySurface.NormalAt(u, v);
                    myNormal.Unitize();
                    myNormalList.Add(myNormal);
                    SurfaceCurvature mySurfaceCurvature = mySurface.CurvatureAt(u, v);
                    //double myGaussian = mySurfaceCurvature.Gaussian;//Gaussian from Surface

                    double Kappa0 = mySurfaceCurvature.Kappa(0);//Problem here?
                    if (myOffset > 0)
                    {
                        if (Math.Abs(Kappa0) > 0)
                        {
                            Point3d PNew = new Point3d();
                            PNew = P + myNormal * myOffset * myFactor;
                            myNewList.RemoveAt(i);
                            myNewList.Insert(i, PNew);

                        }
                        else
                        {
                            Point3d PNew = new Point3d();
                            PNew = P - myNormal * myOffset * myFactor;
                            myNewList.RemoveAt(i);
                            myNewList.Insert(i, PNew);

                        }
                    }
                }
                myNewListList.Add(myNewList);
            }
            List<Curve> myNewCurveList = new List<Curve>();
            Brep FINAL = new Brep();

            for (int i = 0; i < myNewListList.Count; i++)
            {
                Curve myNewCurve = NurbsCurve.CreateControlPointCurve(myNewListList[i], 3);
                myNewCurveList.Add(myNewCurve);
            }
            FINAL = Brep.CreateFromLoft(myNewCurveList, Point3d.Unset, Point3d.Unset, LoftType.Normal, false)[0];
            DA.SetData(0, FINAL);
        }
       
       

        ////////// Icon of component (needs more work) :-
        protected override Bitmap Icon
        {
            get
            {
                return new Bitmap(SurfaceAdjustBitmap);
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("{c1f31004-6c7f-4913-9ecb-8bbb069c08be}"); }
                                    
        }
    }
}
