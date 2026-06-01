Imports System
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.Configuration

Public Class Download
    Inherits System.Web.UI.Page

    Private Const CONNECTION_STRING_KEY As String = "KYCConnString"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' Validate session or query parameters securely
        Dim idStr As String = Request.QueryString("id")
        Dim typeStr As String = Request.QueryString("type")

        Dim recordId As Integer = 0
        If String.IsNullOrEmpty(idStr) OrElse Not Integer.TryParse(idStr, recordId) Then
            Response.Redirect("ManageKYC.aspx?missingfile=1", True)
            Return
        End If

        If String.IsNullOrEmpty(typeStr) Then
            Response.Redirect("ManageKYC.aspx?missingfile=1", True)
            Return
        End If

        ' Map the document type to its corresponding database column
        Dim targetColumn As String = ""
        Select Case typeStr.ToLower()
            Case "aadhaar"
                targetColumn = "AadhaarCardPath"
            Case "pan"
                targetColumn = "PANCardPath"
            Case "passportdl"
                targetColumn = "PassportDLPath"
            Case "addressproof"
                targetColumn = "AddressProofPath"
            Case "signature"
                targetColumn = "SignatureScanPath"
            Case Else
                Response.Redirect("ManageKYC.aspx?missingfile=1", True)
                Return
        End Select

        Dim connString As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString
        ' Select target path dynamically but securely (targetColumn is hardcoded from SELECT CASE mapping, so safe from SQL injection)
        Dim selectQuery As String = String.Format("SELECT [{0}] FROM [KYCDetails] WHERE [Id] = @Id", targetColumn)
        Dim relativePath As String = ""

        Try
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand(selectQuery, conn)
                    cmd.Parameters.AddWithValue("@Id", recordId)
                    conn.Open()
                    Dim result As Object = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        relativePath = result.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Log exception internally and redirect back gracefully
            Response.Redirect("ManageKYC.aspx?missingfile=1", True)
            Return
        End Try

        ' Check if path was found in DB
        If String.IsNullOrEmpty(relativePath) Then
            Response.Redirect("ManageKYC.aspx?missingfile=1", True)
            Return
        End If

        ' Resolve physical path and perform strict security validations
        Try
            Dim physicalPath As String = Server.MapPath(relativePath)
            Dim uploadsFolder As String = Server.MapPath("~/Uploads/")

            ' 1. Directory Traversal Protection: Validate path stays strictly inside Uploads folder
            Dim fullPhysicalPath As String = Path.GetFullPath(physicalPath)
            Dim fullUploadsFolder As String = Path.GetFullPath(uploadsFolder)

            If Not fullPhysicalPath.StartsWith(fullUploadsFolder, StringComparison.OrdinalIgnoreCase) Then
                ' Security Violation: Path is outside the uploads directory!
                Response.Redirect("ManageKYC.aspx?missingfile=1", True)
                Return
            End If

            ' 2. Validate physical file existence on disk
            If Not File.Exists(fullPhysicalPath) Then
                Response.Redirect("ManageKYC.aspx?missingfile=1", True)
                Return
            End If

            ' 3. Resolve clean MIME type
            Dim ext As String = Path.GetExtension(fullPhysicalPath).ToLower()
            Dim mimeType As String = "application/octet-stream"
            If ext = ".pdf" Then
                mimeType = "application/pdf"
            ElseIf ext = ".jpg" OrElse ext = ".jpeg" Then
                mimeType = "image/jpeg"
            End If

            ' 4. Clear and configure response headers
            Response.Clear()
            Response.ContentType = mimeType
            
            ' Display inline (preview) in browser for PDF/Images, otherwise trigger download
            Response.AddHeader("Content-Disposition", String.Format("inline; filename={0}", Path.GetFileName(fullPhysicalPath)))
            Response.AddHeader("Content-Length", New FileInfo(fullPhysicalPath).Length.ToString())
            
            ' Stream physical file directly to the client output stream
            Response.TransmitFile(fullPhysicalPath)
            Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Catch ex As System.Threading.ThreadAbortException
            ' Standard redirect or complete request thread abort
        Catch ex As Exception
            ' Fallback gracefully
            Response.Redirect("ManageKYC.aspx?missingfile=1", True)
        End Try
    End Sub
End Class
