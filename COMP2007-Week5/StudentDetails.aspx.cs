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
            if((!IsPostBack) && (Request.QueryString.Count > 0))
            {
                this.GetStudent();
            }
        }

        protected void GetStudent()
        {
            //Populate form with existing data from database
            int StudentID = Convert.ToInt32(Request.QueryString["StudentID"]);

            //Connect to EF framework
            using(DefaultConnection db = new DefaultConnection())
            {
                //Populate student object instance with Student ID from URL parameter
                Student updatedStudent = (from student in db.Students
                                          where student.StudentID == StudentID
                                          select student).FirstOrDefault();

                //Map student properties to form controls
                if(updatedStudent != null)
                {
                    LastNameTextBox.Text = updatedStudent.LastName;
                    FirstNameTextBox.Text = updatedStudent.FirstMidName;
                    EnrollmentDateTextBox.Text = updatedStudent.EnrollmentDate.ToString("yyyy-MM-dd");
                }
            }
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

                int StudentID = 0;

                //IF adding a new student, run this, else skip it
                if(Request.QueryString.Count > 0 ) //Our URL HAS a student id
                {
                    StudentID = Convert.ToInt32(Request.QueryString["StudentID"]);

                    newStudent = (from student in db.Students
                                  where student.StudentID == StudentID
                                  select student).FirstOrDefault();
                }

                newStudent.LastName = LastNameTextBox.Text;
                newStudent.FirstMidName = FirstNameTextBox.Text;
                newStudent.EnrollmentDate = Convert.ToDateTime(EnrollmentDateTextBox.Text);

                //Only add if new student
                if(StudentID == 0)
                {
                    db.Students.Add(newStudent);
                }

                // run insert in DB
                db.SaveChanges();

                // redirect to the updated students page
                Response.Redirect("~/Students.aspx");
            }
        }
    }
}