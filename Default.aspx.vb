Public Class _Default
    Inherits System.Web.UI.Page

    ' Explicitly declare the single control used in code-behind to clear Visual Studio static compiler warnings
    Protected WithEvents txtApplicationDate As System.Web.UI.WebControls.TextBox

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
