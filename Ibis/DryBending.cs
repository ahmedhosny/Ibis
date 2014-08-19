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
    public class DryBending : GH_Component
    {

        //////////public Declarations(Available to whole class)

        public XmlDocument IBIS_XML = new XmlDocument();
        public System.Drawing.Bitmap DryBendingBitmap = new Bitmap("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\Icons\\drybend.png");


        public DryBending()
            : base("DryBending", "DryBend", "Evaluate your geometry with the dry bending technique", "Ibis", "Fabrication Techniques")
        {
            
        }

        //Set the inputs: //name, nickname, Description, Access
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //as here acts like a cast operation,if conversion is not possible, it will return null
            pManager.AddIntegerParameter("Material", "Material", "Choose Material", GH_ParamAccess.item);
            pManager[0].Optional = true;

            pManager.AddSurfaceParameter("Surface", "Surface", "Insert Surface Here", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("BendDirection", "BendDirection", " Choose Bend Direction", GH_ParamAccess.item);
            pManager[2].Optional = true;

            //////////
            pManager.AddIntegerParameter("Thickness", "Thickness", "Choose Thickness", GH_ParamAccess.item);
            pManager[3].Optional = true;
            //myThicknessParam.AddNamedValue("1/4 inch", 1001);
            //myThicknessParam.AddNamedValue("5/16 inch", 1002);
            //myThicknessParam.AddNamedValue("3/8 inch", 1003);
            //myThicknessParam.AddNamedValue("1/2 inch", 1004);
            //myThicknessParam.AddNamedValue("5/8 inch", 1005);
            //myThicknessParam.AddNamedValue("3/4 inch", 1006);
            pManager.AddIntegerParameter("Sample density", "Samples", "Insert Sample density here", GH_ParamAccess.item);
            pManager[4].Optional = true;
            //pManager[4].ObjectChanged += Param_ObjectChangedAdd; Only addition worked, only after last parameter and only on changes done to last parameter...

        }


        //Set the outputs:
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // pManager.AddSurfaceParameter("SuccesfulSurfaces", "SuccesfulSurfaces", "Surfaces that have succesfully been dry bent ", GH_ParamAccess.list);
            // pManager.AddSurfaceParameter("FailedSurfaces", "FailedSurfaces", "Surfaces that could not be dry bent", GH_ParamAccess.list);
            pManager.AddPointParameter("FailedAreas", "FailedAreas", "Areas that could not be dry bent", GH_ParamAccess.list);
            pManager.AddNumberParameter("MinimumRadius", "MinimumRadius", "Minimum Bending Radius at user-defined input parameters", GH_ParamAccess.item);


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

            if (myMaterialValue.IsEmpty) return;

            //Code to load the thickness list depending on user Material input:-
            List<String> myThicknessList = new List<string>();
            List<String> myBendList = new List<string>();
            IBIS_XML.Load("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\IBIS_XML.xml");
            foreach (Grasshopper.Kernel.Types.GH_Integer myThis in myMaterialValue.AllData(true))
            {
                switch (myThis.Value)
                {

                    case 1:
                        //////////for thickness:
                        XmlNodeList myNodeList1 = IBIS_XML.SelectNodes("IBIS/DryBending/Material[@id=1]/BendDirection/Thickness");
                        foreach (XmlNode myNode in myNodeList1)
                        {
                            XmlElement myElement = myNode as XmlElement;
                            String myXMLThickness = myElement.GetAttribute("type");
                            myThicknessList.Add(myXMLThickness);
                        }


                        for (int i = 0; i < 6; i++) //This needs to be changed!!
                        {
                            myThicknessParam.AddNamedValue(myThicknessList[i], i + 1001);
                        }
                        //////////for benddirection
                        XmlNodeList myNodeList1b = IBIS_XML.SelectNodes("IBIS/DryBending/Material[@id=1]/BendDirection");
                        foreach (XmlNode myNode in myNodeList1b)
                        {
                            XmlElement myElement = myNode as XmlElement;
                            String myXMLBend = myElement.GetAttribute("type");
                            myBendList.Add(myXMLBend);
                        }


                        for (int i = 0; i < myBendList.Count; i++)
                        {
                            myGrainParam.AddNamedValue(myBendList[i], i + 101);
                        }
                        break;

                    case 2:
                        //////////for thickness:
                        XmlNodeList myNodeList2 = IBIS_XML.SelectNodes("IBIS/DryBending/Material[@id=2]/BendDirection/Thickness");
                        foreach (XmlNode myNode in myNodeList2)
                        {
                            XmlElement myElement = myNode as XmlElement;
                            String myXMLThickness = myElement.GetAttribute("type");
                            myThicknessList.Add(myXMLThickness);
                        }


                        for (int i = 0; i < myThicknessList.Count; i++)
                        {
                            myThicknessParam.AddNamedValue(myThicknessList[i], i + 1001);
                        }
                        //////////for benddirection
                        XmlNodeList myNodeList2b = IBIS_XML.SelectNodes("IBIS/DryBending/Material[@id=2]/BendDirection");
                        foreach (XmlNode myNode in myNodeList2b)
                        {
                            XmlElement myElement = myNode as XmlElement;
                            String myXMLBend = myElement.GetAttribute("type");
                            myBendList.Add(myXMLBend);
                        }


                        for (int i = 0; i < myBendList.Count; i++)
                        {
                            myGrainParam.AddNamedValue(myBendList[i], i + 101);
                        }
                        break;

                    case 3:
                        XmlNodeList myNodeList3 = IBIS_XML.SelectNodes("IBIS/DryBending/Material[@id=3]/BendDirection/Thickness");
                        foreach (XmlNode myNode in myNodeList3)
                        {
                            XmlElement myElement = myNode as XmlElement;
                            String myXMLThickness = myElement.GetAttribute("type");
                            myThicknessList.Add(myXMLThickness);
                        }


                        for (int i = 0; i < myThicknessList.Count; i++)
                        {
                            myThicknessParam.AddNamedValue(myThicknessList[i], i + 1001);
                        }
                        break;
                }
                return;
            }
        }




















        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //myFailedPointList.Clear();
            //myThicknessList.Clear();

            int myMaterial = 0;
            if (!DA.GetData(0, ref myMaterial))
            {
                return;
            }

            //myThicknessList.Clear();




            Surface mySurface = null;
            if (!DA.GetData(1, ref mySurface))
            {
                return;
            }



            int myGrain = 0;


            if (!DA.GetData(2, ref myGrain))
            {
                return;
            }




            int myThickness = 0;
            if (!DA.GetData(3, ref myThickness))
            {
                return;
            }


            ///////////GetData cont. :-

            int mySampleDensity = 0;
            if (!DA.GetData(4, ref mySampleDensity))
            {
                return;
            }

            //////////Pre-Processing:-
            List<double> myRadii = new List<double>();
            List<Point3d> myPointList = new List<Point3d>();

            double MIN0 = mySurface.Domain(0).Min;
            double MAX0 = mySurface.Domain(0).Max;
            double MIN1 = mySurface.Domain(1).Min;
            double MAX1 = mySurface.Domain(1).Max;
            double uStep = (MAX0 - MIN0) / mySampleDensity;
            double vStep = (MAX1 - MIN1) / mySampleDensity;
            for (int i = 0; i < mySampleDensity; i++)
            {
                for (int j = 0; j < mySampleDensity; j++)
                {
                    double u = i * uStep;
                    double v = j * vStep;
                    Point3d P = mySurface.PointAt(u, v);
                    myPointList.Add(P);
                    SurfaceCurvature mySurfaceCurvature = mySurface.CurvatureAt(u, v);
                    double myKappa0 = Math.Abs(mySurfaceCurvature.Kappa(0));
                    double myKappa1 = Math.Abs(mySurfaceCurvature.Kappa(1));

                    if (myKappa0 > myKappa1)
                    {
                        double myRadius = 1 / myKappa0;
                        myRadii.Add(myRadius);

                    }
                    else
                    {
                        double myRadius = 1 / myKappa1;
                        myRadii.Add(myRadius);

                    }

                }


            }


            ////////// Code to loop through XML nodes and get MinRad value:-
            double MinRad = 0.0;
            string temp = IBIS_XML.SelectSingleNode("IBIS/DryBending/Material[@id= '" + myMaterial + "']/BendDirection[@id= '" + myGrain + "']/Thickness[@id= '" + myThickness + "']").InnerText;
            MinRad = Convert.ToDouble(temp);

            //////////PostProcessing:-

            //List<Surface> myFailedSurfaceList = new List<Surface>();
            List<Point3d> myFailedPointList = new List<Point3d>();
            for (int i = 0; i < myRadii.Count; i++)
            {
                if (myRadii[i] < MinRad)
                {
                    myFailedPointList.Add(myPointList[i]);
                    // myFailedSurfaceList.Add(mySurface);
                }

            }




            //////////
            //List<NurbsSurface> myNurbsList = new List<NurbsSurface>();

            //List<int> myNotOkIndex = new List<int>();
            //List<int> myOkIndex = new List<int>();

            //foreach (NurbsSurface myNurbsTemp in myNurbsList)
            //{
            //    double subMIN0 = myNurbsTemp.Domain(0).Min;
            //    double subMAX0 = myNurbsTemp.Domain(0).Max;
            //    double subMIN1 = myNurbsTemp.Domain(1).Min;
            //    double subMAX1 = myNurbsTemp.Domain(1).Max;

            //    double v = (MAX0 - MIN0) / 2;
            //    double u = (MAX1 - MIN1) / 2;


            //    SurfaceCurvature mySurfaceCurvature = myNurbsTemp.CurvatureAt(u, v);



            //}

            //for (int j = 0; j < myRadii.Count; j++)
            //{

            //    if (myRadii[j] < MinRad)
            //    {
            //        myNotOkIndex.Add(j);

            //    }
            //    else
            //    {
            //        myOkIndex.Add(j);
            //    }

            //}


            //for (int i = 0; i < myOkIndex.Count; i++)
            //{
            //    myOkNurbsList.Add(myNurbsList[myOkIndex[i]]);

            //}
            //for (int i = 0; i < myNotOkIndex.Count; i++)
            //{
            //    myNotOkNurbsList.Add(myNurbsList[myNotOkIndex[i]]);

            //}



            ////////// Outputs:-
            //DA.SetDataList(0, myOkNurbsList);
            // DA.SetDataList(1, myNotOkNurbsList);
            // DA.SetDataList(0, myFailedSurfaceList);
            DA.SetDataList(0, myFailedPointList);
            DA.SetData(1, MinRad);



        }

        ////////// Icon of component (needs more work) :-
        protected override Bitmap Icon
        {
            get
            {
                return new Bitmap(DryBendingBitmap);
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("{28706862-69c8-43cc-8dd5-8e09c5e59fde}"); }
        }
    }
}
