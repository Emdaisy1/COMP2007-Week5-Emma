using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using COMP2007_Week5.Models;
using System.Web.ModelBinding;
using System.Linq.Dynamic;

namespace COMP2007_Week5
{
    public partial class Departments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["SortColumn"] = "DepartmentID";
                Session["SortDirection"] = "ASC";
                // Get data
                this.GetDepartments();
            }
        }

        protected void GetDepartments()
        {
            //Connect to DB
            using (DefaultConnection db = new DefaultConnection())
            {
                string SortString = Session["SortColumn"].ToString() + " " + Session["SortDirection"].ToString();
                var Departments = (from allDepartments in db.Departments
                                select allDepartments);

                DepartmentsGridView.DataSource = Departments.AsQueryable().OrderBy(SortString).ToList();
                DepartmentsGridView.DataBind();
            }

        }

        /**
         * <summary>
         * This event handler deletes a department from the DB using the EF
         * </summary>
         * 
         * @method DepartmentsGridView_RowDeleting
         * @param {object} sender
         * @param {GridViewDeleteEventArgs} e
         * @returns {void}
         */
        protected void DepartmentsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int selectedRow = e.RowIndex;

            int DepartmentID = Convert.ToInt32(DepartmentsGridView.DataKeys[selectedRow].Values["DepartmentID"]);

            using (DefaultConnection db = new DefaultConnection())
            {
                Department deletedDepartment = (from DepartmentRecords in db.Departments
                                          where DepartmentRecords.DepartmentID == DepartmentID
                                          select DepartmentRecords).FirstOrDefault();
                db.Departments.Remove(deletedDepartment);

                db.SaveChanges();

                this.GetDepartments();
            }
        }

        /**
         * <summary>
         * This event handler allows pagination to occur on departments page
         * </summary>
         * 
         * @method DepartmentsGridView_PageIndexChanging
         * @param {object} sender
         * @param {GridViewPageEventArgs} e
         * @returns {void}
         */
        protected void DepartmentsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //Set page #
            DepartmentsGridView.PageIndex = e.NewPageIndex;

            //Refresh grid
            this.GetDepartments();
        }

        protected void PageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Set new page size
            DepartmentsGridView.PageSize = Convert.ToInt32(PageSizeDropDownList.SelectedValue);

            //Refresh grid
            this.GetDepartments();
        }

        protected void DepartmentsGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            //Get column to sort by
            Session["SortColumn"] = e.SortExpression;

            //Refresh grid
            this.GetDepartments();

            //Direction toggle
            Session["SortDirection"] = Session["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
        }

        protected void DepartmentsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (IsPostBack)
            {
                if (e.Row.RowType == DataControlRowType.Header)//Only fire if header clicked
                {
                    LinkButton linkButton = new LinkButton();

                    for (int index = 0; index < DepartmentsGridView.Columns.Count - 1; index++)
                    {
                        if (DepartmentsGridView.Columns[index].SortExpression == Session["SortColumn"].ToString())
                        {
                            if (Session["SortDirection"].ToString() == "ASC")
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