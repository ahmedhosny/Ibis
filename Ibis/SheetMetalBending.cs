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
    public class SheetMetalBending : GH_Component
    {

        //////////public Declarations(Available to whole class)

         public XmlDocument IBIS_XML = new XmlDocument();
         public System.Drawing.Bitmap SheetMetalBendingBitmap = new Bitmap("D:\\Dropbox\\_F13\\6338\\GH Plugin\\Ibis\\Ibis\\Icons\\SheetMetalBending.png");


        public SheetMetalBending()
            : base("SheetMetalBending", "SheetMetalBending", "Calculate the Bend Allowance and Deduction of your sheet metal", "Ibis", "Fabrication Techniques")
        {
        }

        //Set the inputs: //name, nickname, Description, Access
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Thickness(in)", "Thickness(in)", "Input your Thickness", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddNumberParameter("BendAngle", "BendAngle", "Input your BendAngle", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("InnerRadius", "InnerRadius", "Input your InnerRadius", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("K-Factor", "K-Factor", "Input your K-Factor", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("YieldStrength(psi)", "YieldStrength(psi)", "Input your YieldStrength", GH_ParamAccess.item);
            pManager[4].Optional = true; //36000 to 60000
            pManager.AddNumberParameter("ElasticityModulus(ksi)", "ElasticityModulus(ksi)", "Input your ElasticityModulus", GH_ParamAccess.item);
            pManager[5].Optional = true; //29000 or 29x10^6 psi
            //Rule of thumb for K-Factor, between 0.0 and 0.5
            //Radius < Thickness, K=.25 
            //Radius < 2 * Thickness, K=.33 
            //Radius > 2 * Thickness, K=.5
        }

        //Set the outputs:
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("BendAllowance1(in)", "BendAllowance1(in)", "BendAllowance1(in)", GH_ParamAccess.item);
            pManager.AddNumberParameter("BendAllowance2(in)", "BendAllowance2(in)", "BendAllowance2(in)", GH_ParamAccess.item);
            pManager.AddNumberParameter("BendDeduction(in)", "BendDeduction(in)", "BendDeduction(in)", GH_ParamAccess.item);
            pManager.AddNumberParameter("ResultantInnerRadius", "ResultantInnerRadius", "ResultantInnerRadius", GH_ParamAccess.item);
            pManager.AddNumberParameter("ResultantBendAngle", "ResultantBendAngle", "ResultantBendAngle", GH_ParamAccess.item);
        }
       


        //DA.GetData vs DA.SetData:
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //////////

         
            double myThickness =0.0;
            if (!DA.GetData(0, ref myThickness))
            {
                return;
            }
            double myBendAngle = 0.0;
            if (!DA.GetData(1, ref myBendAngle))
            {
                return;
            }
              double myInnerRadius = 0.0;
            if (!DA.GetData(2, ref myInnerRadius ))
            {
                return;
            }
            double myKFactor = 0.0;
            if (!DA.GetData(3, ref myKFactor))
            {
                return;
            }
            double myYS = 0.0;
            if (!DA.GetData(4, ref myYS))
            {
                return;
            }
            double myEM = 0.0;
            if (!DA.GetData(5, ref myEM))
            {
                return;
            }


            double myBendAllowance1 = (Math.PI / 180) * myBendAngle * (myInnerRadius + myKFactor * myThickness);
            ////Test
            double myBendAllowance2 = (0.0078 * myThickness + 0.0174 * myInnerRadius) * myBendAngle;
            double myBendDeduction = 2 * (Math.Tan(myBendAngle / 2) * (myInnerRadius + myThickness)) - myBendAllowance1;
            //Outside setback is (Math.Tan(myBendAngle / 2) * (myInnerRadius + myThickness))
            double myRiToRfRatio = 4 * Math.Pow(((myInnerRadius * myYS) / (myEM * myThickness)), 3) - 3 * ((myInnerRadius * myYS) / (myEM * myThickness)) + 1;
            double myFinalInnerRadius = myInnerRadius / myRiToRfRatio;
            double myFinalBendAngle = myBendAngle * myRiToRfRatio;
            ////Test
            //double myRiToRfRatio2 = (((2 * myInnerRadius) / myThickness) + 1) / (((2 * myFinalInnerRadius) / myThickness) + 1);
            DA.SetData(0, myBendAllowance1);
            DA.SetData(1, myBendAllowance2);
            DA.SetData(2, myBendDeduction);
            DA.SetData(3, myFinalInnerRadius);
            DA.SetData(4, myFinalBendAngle);

            //////////
        }
       

        ////////// Icon of component (needs more work) :-
        protected override Bitmap Icon
        {
            get
            {
                return new Bitmap(SheetMetalBendingBitmap);
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("{f568cbb2-0dd8-43ed-9637-3e3aaed3360f}"); }
                                    
        }
    }
}
