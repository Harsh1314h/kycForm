Public Class _Default
    Inherits System.Web.UI.Page

    ''' <summary>
    ''' Handles the Page Load event. Pre-populates default values on initial load.
    ''' </summary>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Find the control dynamically in the tree to eliminate all static and dynamic compiler errors completely
            Dim txtAppDate As TextBox = CType(FindControlRecursive(Me, "txtApplicationDate"), TextBox)
            If txtAppDate IsNot Nothing Then
                ' Set the Date of Application to today's date in YYYY-MM-DD format for the HTML5 date picker
                txtAppDate.Text = DateTime.Today.ToString("yyyy-MM-dd")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Helper method to search for a server control recursively through the controls tree.
    ''' </summary>
    Private Function FindControlRecursive(ByVal root As Control, ByVal id As String) As Control
        If root Is Nothing Then Return Nothing
        If root.ID = id Then Return root
        For Each child As Control In root.Controls
            Dim found As Control = FindControlRecursive(child, id)
            If found IsNot Nothing Then Return found
        Next
        Return Nothing
    End Function
End Class
