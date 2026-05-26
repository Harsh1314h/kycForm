Public Class _Default
    Inherits System.Web.UI.Page

    ' =========================================================================
    ' SECTION 1: Basic Account Information Controls
    ' =========================================================================
    Protected WithEvents ddlAccountType As System.Web.UI.WebControls.DropDownList
    Protected WithEvents rdoIndividual As System.Web.UI.HtmlControls.HtmlInputRadioButton
    Protected WithEvents rdoNonIndividual As System.Web.UI.HtmlControls.HtmlInputRadioButton
    Protected WithEvents ddlBranch As System.Web.UI.WebControls.DropDownList
    Protected WithEvents txtApplicationDate As System.Web.UI.WebControls.TextBox

    ' =========================================================================
    ' SECTION 2: Contact & Verification Controls
    ' =========================================================================
    Protected WithEvents txtEmail As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtEmailOTP As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtMobileNumber As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtAlternateMobile As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtAadhaarMobileOTP As System.Web.UI.WebControls.TextBox

    ' =========================================================================
    ' SECTION 3: Aadhaar (UIDAI) Details Controls
    ' =========================================================================
    Protected WithEvents txtAadhaarNumber As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtAadhaarOTP As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtAadhaarName As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtAadhaarDOB As System.Web.UI.WebControls.TextBox
    Protected WithEvents rdoMale As System.Web.UI.HtmlControls.HtmlInputRadioButton
    Protected WithEvents rdoFemale As System.Web.UI.HtmlControls.HtmlInputRadioButton
    Protected WithEvents rdoOther As System.Web.UI.HtmlControls.HtmlInputRadioButton

    ' =========================================================================
    ' SECTION 5: Address Details Controls
    ' =========================================================================
    Protected WithEvents txtStreet As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtLocality As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtTown As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtPostOffice As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtCity As System.Web.UI.WebControls.TextBox
    Protected WithEvents ddlState As System.Web.UI.WebControls.DropDownList
    Protected WithEvents txtCountry As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtPincode As System.Web.UI.WebControls.TextBox
    Protected WithEvents ddlAddressType As System.Web.UI.WebControls.DropDownList
    Protected WithEvents rdoSameYes As System.Web.UI.HtmlControls.HtmlInputRadioButton
    Protected WithEvents rdoSameNo As System.Web.UI.HtmlControls.HtmlInputRadioButton
    Protected WithEvents txtPermanentAddress As System.Web.UI.WebControls.TextBox

    ''' <summary>
    ''' Handles the Page Load event. Pre-populates default values on initial load.
    ''' </summary>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Set the Date of Application to today's date in YYYY-MM-DD format for the HTML5 date picker
            txtApplicationDate.Text = DateTime.Today.ToString("yyyy-MM-dd")
        End If
    End Sub
End Class
