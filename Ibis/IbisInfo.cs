using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Ibis
{
    public class IbisInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Ibis";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("4d0ff62f-eca5-4bdd-a097-f82e0a74c2f1");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Ahmed Hosny";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "ahosny@gsd.harvard.edu";
            }
        }
    }
}
