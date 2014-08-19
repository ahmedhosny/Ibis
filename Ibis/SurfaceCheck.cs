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
    public class SurfaceCheck : GH_Component
    {

        //////////public Declarations(Available to whole class)

        public System.Drawing.Bitmap SurfaceCheckBitmap = new Bitmap("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\Icons\\surfacecheck.png");

   public SurfaceCheck()
            : base("SurfaceCheck", "SrfChk", "Get planar, single-curved or double-curved elements of your geometry", "Ibis", "Generic Geometry Analysis")
        {
        }


   //////////here are my functions:

   String mySurfaceCheckFunction(Surface s, int sam, double tol)
   {
       String Result = null;
       List<double> myKappa0List = new List<double>();
       List<double> myKappa1List = new List<double>();
       double MIN0 = s.Domain(0).Min;
       double MAX0 = s.Domain(0).Max;
       double MIN1 = s.Domain(1).Min;
       double MAX1 = s.Domain(1).Max;

       double uStep = (MAX0 - MIN0) / (sam - 1);
       double vStep = (MAX1 - MIN1) / (sam - 1);
       for (int i = 0; i < sam; ++i)
       {
           for (int j = 0; j < sam; ++j)
           {
               double utemp = s.Domain(0).Min + i * uStep;
               double vtemp = s.Domain(1).Min + j * vStep;
               SurfaceCurvature mySurfaceCurvature = s.CurvatureAt(utemp, vtemp);
               double temp0 = mySurfaceCurvature.Kappa(0);
               myKappa0List.Add(temp0);
               double temp1 = mySurfaceCurvature.Kappa(1);
               myKappa1List.Add(temp1);
           }
       }
       for (int i = 0; i < myKappa0List.Count; i++)
       {
           if (Math.Abs(myKappa0List[i]) < tol && Math.Abs(myKappa1List[i]) < tol)
           {
               Result = "zero";
           }
           else if (Math.Abs(myKappa0List[i]) < tol || Math.Abs(myKappa1List[i]) < tol)
           {
               Result = "one";
           }
           else
           {
               Result = "two";
           }
       }
       return Result;

   }

   //////////
   //Set the inputs: //name, nickname, Description, Access
   protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
   {
       pManager.AddSurfaceParameter("Surface", "Surface", "Insert Surface Here", GH_ParamAccess.list);
       pManager[0].Optional = true;
       pManager.AddIntegerParameter("SampleDensity", "SampleDensity", "Insert Sample Density Here", GH_ParamAccess.item);
       pManager[1].Optional = true;
       pManager.AddNumberParameter("FlatnessTolerance", "FlatnessTolerance", "Insert your Flatness Tolerance here", GH_ParamAccess.item);
       pManager[2].Optional = true;

   }

   //Set the outputs:
   protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
   {
       pManager.AddSurfaceParameter("Planar", "Planar-Green", "Planar Surfaces", GH_ParamAccess.list);
       pManager.AddSurfaceParameter("SingleCurved", "SingleCurved-Yellow", "Single Curved Surfaces", GH_ParamAccess.list);
       pManager.AddSurfaceParameter("DoubleCurved", "DoubleCurved-Red", "Double Curved Surfaces", GH_ParamAccess.list);

       //pManager.HideParameter(index)


   }

   //Global variables inside the class not the method (SolveInstance)
   List<Surface> myPlanarSurface = new List<Surface>();
   List<Surface> mySingleCurved = new List<Surface>();
   List<Surface> myDoubleCurved = new List<Surface>();
   //List<Mesh> meshes = new List<Mesh>();

   //here is where the thinking happens...
   //DA.GetData vs DA.SetData:
   protected override void SolveInstance(IGH_DataAccess DA)
   {

       //meshes.Clear();
       myPlanarSurface.Clear();
       mySingleCurved.Clear();
       myDoubleCurved.Clear();

       //////////
       List<Surface> mySurfaceList = new List<Surface>();
       //int mySampleCount = 0;
       if (!DA.GetDataList<Surface>(0, mySurfaceList))
       {
           return;
       }
       int mySampleDensity = 0;
       if (!DA.GetData(1, ref mySampleDensity))
       {
           return;
       }
       double myFlatTol = 0.0;
       if (!DA.GetData(2, ref myFlatTol))
       {
           return;
       }
       //////////



       for (int i = 0; i < mySurfaceList.Count; i++)
       {
           //////////:-
           //Surface s=mySurfaceList[i];
           //Mesh m=Mesh.CreateFromPlane(Plane.WorldXY, s.Domain(0), s.Domain(1), 10, 10);

           //for(int j=0; j<m.Vertices.Count; ++j) {
           //    double u=m.Vertices[j].X;
           //    double v=m.Vertices[j].Y;

           //    m.Vertices.SetVertex(j, s.PointAt(u,v));
           //    ColorHSL c=new ColorHSL(j/(double)m.Vertices.Count, 1.0, 0.5);
           //    m.VertexColors.SetColor(j, c.ToArgbColor());
           //}

           //m.FaceNormals.ComputeFaceNormals();
           //m.Normals.ComputeNormals();

           //meshes.Add(m);
           //////////:-

           string A = mySurfaceCheckFunction(mySurfaceList[i], mySampleDensity, myFlatTol);
           if (A == "zero")
           {
               myPlanarSurface.Add(mySurfaceList[i]);
           }
           else if (A == "one")
           {
               mySingleCurved.Add(mySurfaceList[i]);
           }
           else
           {
               myDoubleCurved.Add(mySurfaceList[i]);
           }
       }
       //////////
       DA.SetDataList(0, myPlanarSurface);
       DA.SetDataList(1, mySingleCurved);
       DA.SetDataList(2, myDoubleCurved);
       //////////
   }


   //////////OVERRIDING DISPLAY IN VIEWPORT
   public override void DrawViewportMeshes(IGH_PreviewArgs args)
   {
       //Params.Output[0].VolatileData.AllData(true)
       //int myWireDensity = 1;
       System.Drawing.Color myGreen = System.Drawing.Color.FromArgb(0, 255, 0);
       System.Drawing.Color myYellow = System.Drawing.Color.FromArgb(255, 255, 0);
       System.Drawing.Color myRed = System.Drawing.Color.FromArgb(255, 0, 0);
       //  base.DrawViewportMeshes(args);
       //////////:-
       //foreach (Mesh mm in meshes)
       //{
       //    args.Display.DrawMeshFalseColors(mm);
       //}
       //////////:-
       if (myPlanarSurface != null)
       {
           foreach (Surface temp in myPlanarSurface)
           {
               Brep temp2 = temp.ToBrep();
               DisplayMaterial myMaterial = new DisplayMaterial(myGreen);
               //args.Display.DrawSurface(temp, myGreen, myWireDensity);
               args.Display.DrawBrepShaded(temp2, myMaterial);
           }
       }
       if (mySingleCurved != null)
       {
           foreach (Surface temp in mySingleCurved)
           {
               Brep temp2 = temp.ToBrep();
               DisplayMaterial myMaterial = new DisplayMaterial(myYellow);
               //args.Display.DrawSurface(temp, myYellow, myWireDensity);
               args.Display.DrawBrepShaded(temp2, myMaterial);
           }
       }
       if (myDoubleCurved != null)
       {
           foreach (Surface temp in myDoubleCurved)
           {
               Brep temp2 = temp.ToBrep();
               DisplayMaterial myMaterial = new DisplayMaterial(myRed);
               //args.Display.DrawSurface(temp, myRed, myWireDensity);
               args.Display.DrawBrepShaded(temp2, myMaterial);
           }

       }
   }

        ////////// Icon of component (needs more work) :-
        protected override Bitmap Icon
        {
            get
            {
                return new Bitmap(SurfaceCheckBitmap);
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("{1c312999-f3b9-4a88-9d49-9b09c8f46d2f}"); }
                                    
        }
    }
}
