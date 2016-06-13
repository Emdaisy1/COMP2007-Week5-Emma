using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using COMP2007_Week5.Models;
using System.Web.ModelBinding;

namespace COMP2007_Week5
{
    public partial class DepartmentDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((!IsPostBack) && (Request.QueryString.Count > 0))
            {
                this.GetDepartment();
            }
        }

        protected void GetDepartment()
        {
            //Populate form with existing data from database
            int DepartmentID = Convert.ToInt32(Request.QueryString["DepartmentID"]);

            //Connect to EF framework
            using (DefaultConnection db = new DefaultConnection())
            {
                //Populate student object instance with Student ID from URL parameter
                Department updatedDepartment = (from department in db.Departments
                                          where department.DepartmentID == DepartmentID
                                          select department).FirstOrDefault();

                //Map student properties to form controls
                if (updatedDepartment != null)
                {
                    NameTextBox.Text = updatedDepartment.Name;
                    BudgetTextBox.Text = Convert.ToString(updatedDepartment.Budget);
                }
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Departments.aspx");
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // connect to EF DB
            using (DefaultConnection db = new DefaultConnection())
            {
                // use the student model to save a new record
                Department newDepartment = new Department();

                int DepartmentID = 0;

                //IF adding a new student, run this, else skip it
                if (Request.QueryString.Count > 0) //Our URL HAS a student id
                {
                    DepartmentID = Convert.ToInt32(Request.QueryString["DepartmentID"]);

                    newDepartment = (from department in db.Departments
                                     where department.DepartmentID == DepartmentID
                                  select department).FirstOrDefault();
                }

                newDepartment.Name = NameTextBox.Text;
                newDepartment.Budget = Convert.ToInt32(BudgetTextBox.Text);

                //Only add if new student
                if (DepartmentID == 0)
                {
                    db.Departments.Add(newDepartment);
                }

                // run insert in DB
                db.SaveChanges();

                // redirect to the updated students page
                Response.Redirect("~/Departments.aspx");
            }
        }
    }
}