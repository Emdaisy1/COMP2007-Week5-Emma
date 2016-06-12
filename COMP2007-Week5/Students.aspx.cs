using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// required for EF DB access
using COMP2007_Week5.Models;
using System.Web.ModelBinding;
using System.Linq.Dynamic;


namespace COMP2007_Week5
{
    public partial class Students : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // if loading the page for the first time, populate the grid from EF DB
            if (!IsPostBack)
            {
                Session["SortColumn"] = "StudentID"; //Default column for sorting
                Session["SortDirection"] = "ASC"; //Default direction for sorting
                // Get data
                this.GetStudents();
            }

        }

        protected void GetStudents()
        {
            // connect to EF DB
            using (DefaultConnection db = new DefaultConnection())
            {
                //Sort string
                string SortString = Session["SortColumn"].ToString() + " " + Session["SortDirection"].ToString();
                // query the Students table using EF and LINQ
                var Students = (from allStudents in db.Students
                                select allStudents);

                //bind the result to the GridView
                StudentsGridView.DataSource = Students.AsQueryable().OrderBy(SortString).ToList();
                StudentsGridView.DataBind();
            }

        }

        /**
         * <summary>
         * This event handler deletes a student from the DB using the EF
         * </summary>
         * 
         * @method StudentsGridView_RowDeleting
         * @param {object} sender
         * @param {GridViewDeleteEventArgs} e
         * @returns {void}
         */
        protected void StudentsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //Store what row was clicked
            int selectedRow = e.RowIndex;

            //Get the selected Student ID using the grid's data key
            int StudentID = Convert.ToInt32(StudentsGridView.DataKeys[selectedRow].Values["StudentID"]);

            //Use ef to find student in DB and remove them
            using(DefaultConnection db = new DefaultConnection())
            {
                //Create student class object to store query for the student to delete
                Student deletedStudent = (from StudentRecords in db.Students
                                          where StudentRecords.StudentID == StudentID
                                          select StudentRecords).FirstOrDefault();
                //Remove student
                db.Students.Remove(deletedStudent);

                //Save DB changes
                db.SaveChanges();

                //Refresh DB
                this.GetStudents();
            }

        }

        /**
         * <summary>
         * This event handler allows pagination to occur on students page
         * </summary>
         * 
         * @method StudentsGridView_PageIndexChanging
         * @param {object} sender
         * @param {GridViewPageEventArgs} e
         * @returns {void}
         */
        protected void StudentsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //Set page #
            StudentsGridView.PageIndex = e.NewPageIndex;

            //Refresh grid
            this.GetStudents();
        }

        protected void PageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Set new page size
            StudentsGridView.PageSize = Convert.ToInt32(PageSizeDropDownList.SelectedValue);

            //Refresh grid
            this.GetStudents();
        }

        protected void StudentsGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            //Get column to sort by
            Session["SortColumn"] = e.SortExpression;

            //Refresh grid
            this.GetStudents();

            //Direction toggle
            Session["SortDirection"] = Session["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
        }

        protected void StudentsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (IsPostBack)
            {
                if(e.Row.RowType == DataControlRowType.Header)//Only fire if header clicked
                {
                    LinkButton linkButton = new LinkButton();

                    for(int index = 0; index < StudentsGridView.Columns.Count - 1; index++)
                    {
                        if(StudentsGridView.Columns[index].SortExpression == Session["SortColumn"].ToString())
                        {
                            if(Session["SortDirection"].ToString() == "ASC")
                            {
                                linkButton.Text = " <i class = 'fa fa-caret-up fa-lg' ></i> ";
                            }
                            else
                            {
                                linkButton.Text = " <i class = 'fa fa-caret-down fa-lg' ></i> ";
                            }

                            e.Row.Cells[index].Controls.Add(linkButton);
                        }
                    }
                }
            }
        }
    }
}