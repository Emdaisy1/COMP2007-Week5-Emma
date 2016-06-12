using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// required for EF DB access
using COMP2007_Week5.Models;
using System.Web.ModelBinding;


namespace COMP2007_Week5
{
    public partial class Students : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // if loading the page for the first time, populate the grid from EF DB
            if (!IsPostBack)
            {
                // Get data
                this.GetStudents();
            }

        }

        protected void GetStudents()
        {
            // connect to EF DB
            using (DefaultConnection db = new DefaultConnection())
            {
                // query the Students table using EF and LINQ
                var Students = (from allStudents in db.Students
                                select allStudents);

                //bind the result to the GridView
                StudentsGridView.DataSource = Students.ToList();
                StudentsGridView.DataBind();
            }

        }
    }
}