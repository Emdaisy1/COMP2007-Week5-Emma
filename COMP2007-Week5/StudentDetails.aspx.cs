using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// required using statements to access EF DB
using COMP2007_Week5.Models;
using System.Web.ModelBinding;

namespace COMP2007_Week5
{
    public partial class StudentDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Students.aspx");
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // connect to EF DB
            using (DefaultConnection db = new DefaultConnection())
            {
                // use the student model to save a new record
                Student newStudent = new Student();

                newStudent.LastName = LastNameTextBox.Text;
                newStudent.FirstMidName = FirstNameTextBox.Text;
                newStudent.EnrollmentDate = Convert.ToDateTime(EnrollmentDateTextBox.Text);

                // adds a new studdent to the Student Table collection
                db.Students.Add(newStudent);

                // run insert in DB
                db.SaveChanges();

                // redirect to the updated students page
                Response.Redirect("~/Students.aspx");
            }
        }
    }
}