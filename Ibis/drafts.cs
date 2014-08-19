//string mySurfaceCheck(Surface s, int u, int v)
//        {
//            String Result = null;
    //    List<Vector3d> myVectorList = new List<Vector3d>();
    //    for (int i = 0; i < u; ++i)
    //    {
    //        for (int j = 0; j < v; ++j)
    //        {
    //            Vector3d V = s.NormalAt(i, j);
    //            myVectorList.Add(V);
    //        }
    //    }
    //    for (int i = 0; i < myVectorList.Count; i++)
    //    {
    //        if (Vector3d.CrossProduct(myVectorList[i], myVectorList[(i + 1) % myVectorList.Count]).Length == 0)
    //        {
    //            Result = "Surface is planar";
    //        }
    //        else
    //        {
    //            Result = "Surface is not planar";
    //            break;
    //        }

    //    }
    //    return Result;
    //}


    //////////
            //XmlDocument XMLFile1 = new XmlDocument();
            //XMLFile1.Load("C:\\Users\\ahosny\\Dropbox\\_F13\\6338\\final project\\Matter\\Matter\\XMLFile1.xml");
            //XmlNodeList myXmlList = XMLFile1.GetElementById(myMaterial); 
            //foreach (XmlNode myNode in myXmlList) {
            //double MinRad = 0.0;
            //string temp = myXmlList[myThickness].InnerText;
            //MinRad = Convert.ToDouble(temp);
            //String rawXPath = "//book[author= '" + larrysName + "']/title/text()";
            // }
            //////////
            //string myXML1 = "C:\\Users\\ahosny\\Dropbox\\_F13\\6338\\final project\\Matter\\Matter\\XMLFile1.xml";
            //XPathDocument myDoc = new XPathDocument(myXML1);
            //XPathNavigator nav = myDoc.CreateNavigator();
            //XPathExpression expr;
            //expr = nav.Compile("/Materials[1]/[mythickness]");
            //XPathNodeIterator iterator = nav.Select(expr);
            //////////







            //List<double> myRadiusList = new List<double>();
            //List<Point3d> myPointList = new List<Point3d>();
            //double myMax = myCurve.Domain.Max;
            //// double myMin = myCurve.Domain.Min;
            //double myStep = myMax / mySampleDensity;
            //for (int i = 0; i < mySampleDensity; i++)
            //{
            //    double t = i * myStep;
            //    Point3d P = new Point3d(myCurve.PointAt(t));
            //    myPointList.Add(P);
            //    Vector3d myCurvatureV = myCurve.CurvatureAt(t);
            //    double myCurvatureD = myCurvatureV.Length;
            //    double myRadius = 1 / myCurvatureD;
            //    myRadiusList.Add(myRadius);
            //}



  //List<Circle> myResultok = new List<Circle>();
  //          List<Line> myResultnotok = new List<Line>();
  //          for (int i = 0; i < myRadiusList.Count; i++)
  //          {
  //              if (myRadiusList[i] >= MinRad)
  //              {
  //                  Circle ok = new Circle(myPointList[i], 1.0);

  //                  myResultok.Add(ok);
  //              }
  //              else
  //              {
  //                  Line notok = new Line(myPointList[i].X, myPointList[i].Y, myPointList[i].Z, myPointList[i].X + 2, myPointList[i].Y + 2, myPointList[i].Z + 2);

  //                  myResultnotok.Add(notok);
  //              }
  //          }


//SawapanStatRhino.RStatSystem sys = new SawapanStatRhino.RStatSystem();

//SawapanStatica.StatMaterial mat = sys.AddMaterial(SawapanStatica.MATERIALTYPES.STEEL, "steel");

//   Mesh ms=Mesh.CreateFromPlane(Plane.WorldXY, new Interval(-1.0, 1.0), new Interval(-1.0, 1.0), 5,5);
//   sys.AddMesh(ms, 0.04, mat);

//   sys.AddSupportAtPoint(new Point3d(ms.Vertices[0]), SawapanStatica.BOUNDARYCONDITIONS.ALL);
//   sys.AddSupportAtPoint(new Point3d(ms.Vertices[1]), SawapanStatica.BOUNDARYCONDITIONS.ALL);

//   SawapanStatRhino.RStatNode n = sys.FindNode(new Point3d(1.0, 1.0, 0.0), 10.0);
//   if (n != null)
//   {
//       n.AddLoad(0.0, 0.0, -1000.0);
//   }

//   sys.DeadLoadFactor = 1.0;

//   sys.SolveSystem();

//   Mesh vismesh = sys.GetColoredMesh(0.0, SawapanStatica.StatSystem.MESHCOLORMODE.VONMISES_STRESS_MAX, 0.0);
//////////


//This is for overriding the dropdown menu from the component itself not the input parameters.
//public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
//{
//    base.AppendAdditionalMenuItems(menu);
//}


//public override void DrawViewportWires(IGH_PreviewArgs args)
//{

//    //args.Display.DrawSprite(
//    base.DrawViewportWires(args);
//}

//public override void DrawViewportMeshes(IGH_PreviewArgs args)
//{


//    base.DrawViewportMeshes(args);
//}


///////// Attempt at dynamically adjustin input parameters: adding one at the end works.

//private void Param_ObjectChangedAdd(Object sender, GH_ObjectChangedEventArgs e)
//{
//    if (e.Type == GH_ObjectEventType.Sources)
//    {
//        int n = Params.Input.Count;
//        if (Params.Input[n - 1] == sender)
//        {
//            if (Params.Input[n - 1].SourceCount > 0)
//            {
//                Grasshopper.Kernel.Parameters.Param_GenericObject param = new Grasshopper.Kernel.Parameters.Param_GenericObject();

//                param.Optional = true;
//                param.MutableNickName = false;
//                param.Access = GH_ParamAccess.tree;

//                param.Name = "Input " + (n + 1).ToString();
//                param.NickName = (n + 1).ToString();
//                param.Description = "Input #" + (n + 1).ToString();

//                Params.RegisterInputParam(param);

//                param.ObjectChanged += Param_ObjectChangedAdd;

//                Params.OnParametersChanged();
//            }
//      }
//    }
//}


/////////