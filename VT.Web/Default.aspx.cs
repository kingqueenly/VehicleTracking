using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VT.Web;
using System.Data.Objects;

namespace VT_Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CarInfo ci = new CarInfo();

            VTEntities vte = new VTEntities();
            ObjectSet<Location> x = vte.Locations;
            vte.SaveChanges();
        }
    }
}