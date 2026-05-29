Imports System
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Web.Configuration

Public Class ManageKYC
    Inherits System.Web.UI.Page

    Private Const CONNECTION_STRING_KEY As String = "KYCConnString"

    ''' <summary>
    ''' Handles Page Load and reads PRG patterns for action toasts.
    ''' </summary>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            EnsureDatabaseSchema()
            
            If Not IsPostBack Then
                ' Bind counts and records
                BindMetricsCounters()
                BindKYCRecords()

                ' Handle Post-Redirect-Get visual alert toasts
                If Request.QueryString("update") = "1" Then
                    InjectServerToast("KYC Profile Updated", "The KYC profile changes have been registered successfully in our database.", "success")
                ElseIf Request.QueryString("deleted") = "1" Then
                    InjectServerToast("KYC Profile Deleted", "The record has been permanently removed from the banking database.", "warning")
                ElseIf Request.QueryString("status") = "1" Then
                    InjectServerToast("Status Approved", "The profile has been successfully approved for active banking access.", "success")
                ElseIf Request.QueryString("status") = "2" Then
                    InjectServerToast("Status Rejected", "The profile has been marked as rejected due to compliance audit failure.", "danger")
                ElseIf Request.QueryString("status") = "3" Then
                    InjectServerToast("Status Pending", "The profile has been marked back to pending audit review.", "warning")
                End If
            End If
        Catch ex As Exception
            InjectServerToast("Initialization Error", "Failed to load dashboard: " & ex.Message.Replace("'", "\'"), "danger")
        End Try
    End Sub

    ''' <summary>
    ''' Updates active statistics and counts from database.
    ''' </summary>
    Private Sub BindMetricsCounters()
        Dim connString As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString
        Dim countQuery As String = "SELECT " & _
            "COUNT(*) As Total, " & _
            "SUM(CASE WHEN VerificationStatus = 'Pending' THEN 1 ELSE 0 END) As Pending, " & _
            "SUM(CASE WHEN VerificationStatus = 'Verified' THEN 1 ELSE 0 END) As Verified, " & _
            "SUM(CASE WHEN VerificationStatus = 'Rejected' THEN 1 ELSE 0 END) As Rejected " & _
            "FROM KYCDetails"

        Dim lblTotalCount As Label = CType(FindControlRecursive(Me, "lblTotalCount"), Label)
        Dim lblPendingCount As Label = CType(FindControlRecursive(Me, "lblPendingCount"), Label)
        Dim lblVerifiedCount As Label = CType(FindControlRecursive(Me, "lblVerifiedCount"), Label)
        Dim lblRejectedCount As Label = CType(FindControlRecursive(Me, "lblRejectedCount"), Label)

        Try
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand(countQuery, conn)
                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            If lblTotalCount IsNot Nothing Then lblTotalCount.Text = reader("Total").ToString()
                            If lblPendingCount IsNot Nothing Then lblPendingCount.Text = If(reader("Pending") Is DBNull.Value, "0", reader("Pending").ToString())
                            If lblVerifiedCount IsNot Nothing Then lblVerifiedCount.Text = If(reader("Verified") Is DBNull.Value, "0", reader("Verified").ToString())
                            If lblRejectedCount IsNot Nothing Then lblRejectedCount.Text = If(reader("Rejected") Is DBNull.Value, "0", reader("Rejected").ToString())
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' If database is empty, leave at 0
        End Try
    End Sub

    ''' <summary>
    ''' Fetches, filters, and binds records to the Repeater.
    ''' </summary>
    Private Sub BindKYCRecords(Optional ByVal name As String = "", Optional ByVal aadhaar As String = "", Optional ByVal pan As String = "", Optional ByVal mobile As String = "")
        Dim connString As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString
        
        ' Parameterized query matching criteria securely
        Dim selectQuery As String = "SELECT Id, FullLegalName, AadhaarNumber, PANNumber, MobileNumber, ApplicationDate, VerificationStatus FROM KYCDetails WHERE 1=1"
        
        If Not String.IsNullOrEmpty(name) Then
            selectQuery &= " AND FullLegalName LIKE @Name"
        End If
        If Not String.IsNullOrEmpty(aadhaar) Then
            selectQuery &= " AND AadhaarNumber = @Aadhaar"
        End If
        If Not String.IsNullOrEmpty(pan) Then
            selectQuery &= " AND PANNumber = @PAN"
        End If
        If Not String.IsNullOrEmpty(mobile) Then
            selectQuery &= " AND MobileNumber = @Mobile"
        End If

        selectQuery &= " ORDER BY CreatedAt DESC"

        Dim rptKYCList As Repeater = CType(FindControlRecursive(Me, "rptKYCList"), Repeater)

        Using conn As New SqlConnection(connString)
            Using cmd As New SqlCommand(selectQuery, conn)
                If Not String.IsNullOrEmpty(name) Then
                    cmd.Parameters.AddWithValue("@Name", "%" & name & "%")
                End If
                If Not String.IsNullOrEmpty(aadhaar) Then
                    cmd.Parameters.AddWithValue("@Aadhaar", aadhaar)
                End If
                If Not String.IsNullOrEmpty(pan) Then
                    cmd.Parameters.AddWithValue("@PAN", pan.ToUpper())
                End If
                If Not String.IsNullOrEmpty(mobile) Then
                    cmd.Parameters.AddWithValue("@Mobile", mobile)
                End If

                Dim dt As New DataTable()
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using

                If rptKYCList IsNot Nothing Then
                    rptKYCList.DataSource = dt
                    rptKYCList.DataBind()
                End If
            End Using
        End Using

        ' Refresh stats as well to stay updated
        BindMetricsCounters()
    End Sub

    ''' <summary>
    ''' Handles individual command operations (View, Delete).
    ''' </summary>
    Protected Sub rptKYCList_ItemCommand(ByVal source As Object, ByVal e As RepeaterCommandEventArgs)
        Dim recordId As Integer = Convert.ToInt32(e.CommandArgument)

        If e.CommandName = "View" Then
            LoadRecordInModal(recordId)
        ElseIf e.CommandName = "Delete" Then
            DeleteRecordSecurely(recordId)
        End If
    End Sub

    ''' <summary>
    ''' Performs parameterized SQL DELETE statement.
    ''' </summary>
    Private Sub DeleteRecordSecurely(ByVal recordId As Integer)
        Dim connString As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString
        Dim deleteQuery As String = "DELETE FROM KYCDetails WHERE Id = @Id"

        Try
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand(deleteQuery, conn)
                    cmd.Parameters.AddWithValue("@Id", recordId)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' Redirect GET back to prevent F5 resubmissions
            Response.Redirect("ManageKYC.aspx?deleted=1", False)
            Context.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            InjectServerToast("Deletion Failure", "Failed to delete: " & ex.Message.Replace("'", "\'"), "danger")
        End Try
    End Sub

    ''' <summary>
    ''' Retrieves full 45+ fields dynamically for detail audit display.
    ''' </summary>
    Private Sub LoadRecordInModal(ByVal recordId As Integer)
        Dim connString As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString
        Dim selectQuery As String = "SELECT * FROM KYCDetails WHERE Id = @Id"

        ' Resolve all modal controls recursively at runtime
        Dim hdnSelectedId As HiddenField = CType(FindControlRecursive(Me, "hdnSelectedId"), HiddenField)
        Dim lblModalId As Label = CType(FindControlRecursive(Me, "lblModalId"), Label)
        Dim lblModalRegDate As Label = CType(FindControlRecursive(Me, "lblModalRegDate"), Label)
        Dim lblModalStatusText As Label = CType(FindControlRecursive(Me, "lblModalStatusText"), Label)
        Dim lblAccountType As Label = CType(FindControlRecursive(Me, "lblAccountType"), Label)
        Dim lblCustomerType As Label = CType(FindControlRecursive(Me, "lblCustomerType"), Label)
        Dim lblBranch As Label = CType(FindControlRecursive(Me, "lblBranch"), Label)
        Dim lblEmail As Label = CType(FindControlRecursive(Me, "lblEmail"), Label)
        Dim lblMobile As Label = CType(FindControlRecursive(Me, "lblMobile"), Label)
        Dim lblAltMobile As Label = CType(FindControlRecursive(Me, "lblAltMobile"), Label)
        Dim lblFullName As Label = CType(FindControlRecursive(Me, "lblFullName"), Label)
        Dim lblFatherName As Label = CType(FindControlRecursive(Me, "lblFatherName"), Label)
        Dim lblMotherName As Label = CType(FindControlRecursive(Me, "lblMotherName"), Label)
        Dim lblSpouseName As Label = CType(FindControlRecursive(Me, "lblSpouseName"), Label)
        Dim lblAadhaarDOB As Label = CType(FindControlRecursive(Me, "lblAadhaarDOB"), Label)
        Dim lblGender As Label = CType(FindControlRecursive(Me, "lblGender"), Label)
        Dim lblMarital As Label = CType(FindControlRecursive(Me, "lblMarital"), Label)
        Dim lblNationality As Label = CType(FindControlRecursive(Me, "lblNationality"), Label)
        Dim lblReligion As Label = CType(FindControlRecursive(Me, "lblReligion"), Label)
        Dim lblResStatus As Label = CType(FindControlRecursive(Me, "lblResStatus"), Label)
        Dim lblBirthPlace As Label = CType(FindControlRecursive(Me, "lblBirthPlace"), Label)
        Dim lblAddressType As Label = CType(FindControlRecursive(Me, "lblAddressType"), Label)
        Dim lblCorrAddress As Label = CType(FindControlRecursive(Me, "lblCorrAddress"), Label)
        Dim lblIsSameAddress As Label = CType(FindControlRecursive(Me, "lblIsSameAddress"), Label)
        Dim lblPermanentAddress As Label = CType(FindControlRecursive(Me, "lblPermanentAddress"), Label)
        Dim lblOccupation As Label = CType(FindControlRecursive(Me, "lblOccupation"), Label)
        Dim lblEmployerName As Label = CType(FindControlRecursive(Me, "lblEmployerName"), Label)
        Dim lblDesignation As Label = CType(FindControlRecursive(Me, "lblDesignation"), Label)
        Dim lblBusinessNature As Label = CType(FindControlRecursive(Me, "lblBusinessNature"), Label)
        Dim lblIncome As Label = CType(FindControlRecursive(Me, "lblIncome"), Label)
        Dim lblFundsSource As Label = CType(FindControlRecursive(Me, "lblFundsSource"), Label)
        Dim lblAadhaarNum As Label = CType(FindControlRecursive(Me, "lblAadhaarNum"), Label)
        Dim lblAadhaarName As Label = CType(FindControlRecursive(Me, "lblAadhaarName"), Label)
        Dim lblPAN As Label = CType(FindControlRecursive(Me, "lblPAN"), Label)
        Dim lblPANName As Label = CType(FindControlRecursive(Me, "lblPANName"), Label)
        Dim lblDLNum As Label = CType(FindControlRecursive(Me, "lblDLNum"), Label)
        Dim lblDLDOB As Label = CType(FindControlRecursive(Me, "lblDLDOB"), Label)
        Dim lblDLName As Label = CType(FindControlRecursive(Me, "lblDLName"), Label)
        
        Dim lnkDocAadhaar As HyperLink = CType(FindControlRecursive(Me, "lnkDocAadhaar"), HyperLink)
        Dim lnkDocPAN As HyperLink = CType(FindControlRecursive(Me, "lnkDocPAN"), HyperLink)
        Dim lnkDocSignature As HyperLink = CType(FindControlRecursive(Me, "lnkDocSignature"), HyperLink)
        
        Dim pnlDocPassportDL As PlaceHolder = CType(FindControlRecursive(Me, "pnlDocPassportDL"), PlaceHolder)
        Dim lnkDocPassportDL As HyperLink = CType(FindControlRecursive(Me, "lnkDocPassportDL"), HyperLink)
        
        Dim pnlDocAddress As PlaceHolder = CType(FindControlRecursive(Me, "pnlDocAddress"), PlaceHolder)
        Dim lnkDocAddress As HyperLink = CType(FindControlRecursive(Me, "lnkDocAddress"), HyperLink)

        Try
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand(selectQuery, conn)
                    cmd.Parameters.AddWithValue("@Id", recordId)
                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            ' Store Id in hidden field for status changes
                            If hdnSelectedId IsNot Nothing Then hdnSelectedId.Value = recordId.ToString()

                            ' Header Metadata
                            If lblModalId IsNot Nothing Then lblModalId.Text = recordId.ToString()
                            If lblModalRegDate IsNot Nothing Then lblModalRegDate.Text = Convert.ToDateTime(reader("CreatedAt")).ToString("yyyy-MM-dd HH:mm")
                            If lblModalStatusText IsNot Nothing Then lblModalStatusText.Text = reader("VerificationStatus").ToString()

                            ' Account Type
                            If lblAccountType IsNot Nothing Then lblAccountType.Text = reader("AccountType").ToString()
                            If lblCustomerType IsNot Nothing Then lblCustomerType.Text = reader("CustomerType").ToString()
                            If lblBranch IsNot Nothing Then lblBranch.Text = reader("PreferredBranch").ToString()

                            ' Contact
                            If lblEmail IsNot Nothing Then lblEmail.Text = reader("Email").ToString()
                            If lblMobile IsNot Nothing Then lblMobile.Text = reader("MobileNumber").ToString()
                            If lblAltMobile IsNot Nothing Then lblAltMobile.Text = If(reader("AlternateMobileNumber") Is DBNull.Value, "N/A", reader("AlternateMobileNumber").ToString())

                            ' Personal
                            If lblFullName IsNot Nothing Then lblFullName.Text = reader("FullLegalName").ToString()
                            If lblFatherName IsNot Nothing Then lblFatherName.Text = reader("FatherName").ToString()
                            If lblMotherName IsNot Nothing Then lblMotherName.Text = reader("MotherName").ToString()
                            If lblSpouseName IsNot Nothing Then lblSpouseName.Text = reader("SpouseGuardianName").ToString()
                            If lblAadhaarDOB IsNot Nothing Then lblAadhaarDOB.Text = Convert.ToDateTime(reader("AadhaarDOB")).ToString("yyyy-MM-dd")
                            If lblGender IsNot Nothing Then lblGender.Text = reader("Gender").ToString()
                            If lblMarital IsNot Nothing Then lblMarital.Text = reader("MaritalStatus").ToString()
                            If lblNationality IsNot Nothing Then lblNationality.Text = reader("Nationality").ToString()
                            If lblReligion IsNot Nothing Then lblReligion.Text = If(reader("Religion") Is DBNull.Value, "N/A", reader("Religion").ToString())
                            If lblResStatus IsNot Nothing Then lblResStatus.Text = reader("ResidentialStatus").ToString()
                            
                            Dim place As String = If(reader("PlaceOfBirth") Is DBNull.Value, "", reader("PlaceOfBirth").ToString())
                            Dim country As String = If(reader("CountryOfBirth") Is DBNull.Value, "", reader("CountryOfBirth").ToString())
                            If lblBirthPlace IsNot Nothing Then
                                lblBirthPlace.Text = String.Format("{0}, {1}", If(place = "", "N/A", place), If(country = "", "N/A", country))
                            End If

                            ' Address
                            If lblAddressType IsNot Nothing Then lblAddressType.Text = reader("TypeOfAddress").ToString()
                            If lblCorrAddress IsNot Nothing Then
                                lblCorrAddress.Text = String.Format("{0}, {1}, {2}, P.O. {3}, {4}, {5}, {6} - {7}", _
                                    reader("StreetHouseLandmark").ToString(), reader("AreaLocality").ToString(), _
                                    reader("LocationVillageTown").ToString(), reader("PostOffice").ToString(), _
                                    reader("CityDistrict").ToString(), reader("State").ToString(), _
                                    reader("Country").ToString(), reader("Pincode").ToString())
                            End If
                            
                            If lblIsSameAddress IsNot Nothing Then lblIsSameAddress.Text = reader("IsPermanentAddressSame").ToString()
                            If lblPermanentAddress IsNot Nothing Then
                                If reader("IsPermanentAddressSame").ToString() = "Yes" Then
                                    lblPermanentAddress.Text = "Same as correspondence address."
                                Else
                                    lblPermanentAddress.Text = reader("PermanentAddress").ToString()
                                End If
                            End If

                            ' Occupation
                            If lblOccupation IsNot Nothing Then lblOccupation.Text = reader("OccupationType").ToString()
                            If lblEmployerName IsNot Nothing Then lblEmployerName.Text = If(reader("EmployerName") Is DBNull.Value, "N/A", reader("EmployerName").ToString())
                            If lblDesignation IsNot Nothing Then lblDesignation.Text = If(reader("Designation") Is DBNull.Value, "N/A", reader("Designation").ToString())
                            If lblBusinessNature IsNot Nothing Then lblBusinessNature.Text = If(reader("NatureOfBusiness") Is DBNull.Value, "N/A", reader("NatureOfBusiness").ToString())
                            If lblIncome IsNot Nothing Then lblIncome.Text = reader("AnnualIncomeRange").ToString()
                            If lblFundsSource IsNot Nothing Then lblFundsSource.Text = reader("SourceOfFunds").ToString()

                            ' ID Proofs
                            If lblAadhaarNum IsNot Nothing Then lblAadhaarNum.Text = MaskAadhaar(reader("AadhaarNumber"))
                            If lblAadhaarName IsNot Nothing Then lblAadhaarName.Text = reader("AadhaarName").ToString()
                            If lblPAN IsNot Nothing Then lblPAN.Text = reader("PANNumber").ToString()
                            If lblPANName IsNot Nothing Then lblPANName.Text = reader("PANHolderName").ToString()
                            
                            If lblDLNum IsNot Nothing Then lblDLNum.Text = If(reader("DrivingLicenceNumber") Is DBNull.Value, "N/A", reader("DrivingLicenceNumber").ToString())
                            If lblDLDOB IsNot Nothing Then lblDLDOB.Text = If(reader("DrivingLicenceDOB") Is DBNull.Value, "N/A", Convert.ToDateTime(reader("DrivingLicenceDOB")).ToString("yyyy-MM-dd"))
                            If lblDLName IsNot Nothing Then lblDLName.Text = If(reader("DrivingLicenceName") Is DBNull.Value, "N/A", reader("DrivingLicenceName").ToString())

                            ' Document links
                            If lnkDocAadhaar IsNot Nothing Then lnkDocAadhaar.NavigateUrl = ResolveUrl(reader("AadhaarCardPath").ToString())
                            If lnkDocPAN IsNot Nothing Then lnkDocPAN.NavigateUrl = ResolveUrl(reader("PANCardPath").ToString())
                            If lnkDocSignature IsNot Nothing Then lnkDocSignature.NavigateUrl = ResolveUrl(reader("SignatureScanPath").ToString())

                            If reader("PassportDLPath") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("PassportDLPath").ToString()) Then
                                If pnlDocPassportDL IsNot Nothing Then pnlDocPassportDL.Visible = True
                                If lnkDocPassportDL IsNot Nothing Then lnkDocPassportDL.NavigateUrl = ResolveUrl(reader("PassportDLPath").ToString())
                            Else
                                If pnlDocPassportDL IsNot Nothing Then pnlDocPassportDL.Visible = False
                            End If

                            If reader("AddressProofPath") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(reader("AddressProofPath").ToString()) Then
                                If pnlDocAddress IsNot Nothing Then pnlDocAddress.Visible = True
                                If lnkDocAddress IsNot Nothing Then lnkDocAddress.NavigateUrl = ResolveUrl(reader("AddressProofPath").ToString())
                            Else
                                If pnlDocAddress IsNot Nothing Then pnlDocAddress.Visible = False
                            End If

                            ' Trigger script execution to show modal popup
                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "OpenModal", "showDetailsModal();", True)
                        Else
                            InjectServerToast("Record Alert", "Failed to retrieve profile: record not found.", "danger")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            InjectServerToast("Retrieval Error", "Error loading profile details: " & ex.Message.Replace("'", "\'"), "danger")
        End Try
    End Sub

    ''' <summary>
    ''' Search Filtering Execution.
    ''' </summary>
    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtSearchName As TextBox = CType(FindControlRecursive(Me, "txtSearchName"), TextBox)
        Dim txtSearchAadhaar As TextBox = CType(FindControlRecursive(Me, "txtSearchAadhaar"), TextBox)
        Dim txtSearchPAN As TextBox = CType(FindControlRecursive(Me, "txtSearchPAN"), TextBox)
        Dim txtSearchMobile As TextBox = CType(FindControlRecursive(Me, "txtSearchMobile"), TextBox)

        Dim searchName As String = If(txtSearchName IsNot Nothing, txtSearchName.Text.Trim(), "")
        Dim searchAadhaar As String = If(txtSearchAadhaar IsNot Nothing, txtSearchAadhaar.Text.Trim(), "")
        Dim searchPAN As String = If(txtSearchPAN IsNot Nothing, txtSearchPAN.Text.Trim(), "")
        Dim searchMobile As String = If(txtSearchMobile IsNot Nothing, txtSearchMobile.Text.Trim(), "")

        ' Verify inputs
        If Not String.IsNullOrEmpty(searchAadhaar) AndAlso searchAadhaar.Length <> 12 Then
            InjectServerToast("Filter Warning", "Aadhaar search query must be exactly 12 digits.", "warning")
            Return
        End If
        If Not String.IsNullOrEmpty(searchMobile) AndAlso searchMobile.Length <> 10 Then
            InjectServerToast("Filter Warning", "Mobile search query must be exactly 10 digits.", "warning")
            Return
        End If

        BindKYCRecords(searchName, searchAadhaar, searchPAN, searchMobile)
    End Sub

    ''' <summary>
    ''' Clears search filters and binds pristine data.
    ''' </summary>
    Protected Sub btnResetSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtSearchName As TextBox = CType(FindControlRecursive(Me, "txtSearchName"), TextBox)
        Dim txtSearchAadhaar As TextBox = CType(FindControlRecursive(Me, "txtSearchAadhaar"), TextBox)
        Dim txtSearchPAN As TextBox = CType(FindControlRecursive(Me, "txtSearchPAN"), TextBox)
        Dim txtSearchMobile As TextBox = CType(FindControlRecursive(Me, "txtSearchMobile"), TextBox)

        If txtSearchName IsNot Nothing Then txtSearchName.Text = ""
        If txtSearchAadhaar IsNot Nothing Then txtSearchAadhaar.Text = ""
        If txtSearchPAN IsNot Nothing Then txtSearchPAN.Text = ""
        If txtSearchMobile IsNot Nothing Then txtSearchMobile.Text = ""
        
        BindKYCRecords()
    End Sub

    ''' <summary>
    ''' Verification status workflow triggers.
    ''' </summary>
    Protected Sub btnStatusPending_Click(ByVal sender As Object, ByVal e As EventArgs)
        UpdateVerificationStatus("Pending", 3)
    End Sub

    Protected Sub btnStatusVerify_Click(ByVal sender As Object, ByVal e As EventArgs)
        UpdateVerificationStatus("Verified", 1)
    End Sub

    Protected Sub btnStatusReject_Click(ByVal sender As Object, ByVal e As EventArgs)
        UpdateVerificationStatus("Rejected", 2)
    End Sub

    ''' <summary>
    ''' Performs parameterized status update query.
    ''' </summary>
    Private Sub UpdateVerificationStatus(ByVal newStatus As String, ByVal statusRedirectCode As Integer)
        Dim hdnSelectedId As HiddenField = CType(FindControlRecursive(Me, "hdnSelectedId"), HiddenField)
        Dim idStr As String = If(hdnSelectedId IsNot Nothing, hdnSelectedId.Value, "")
        Dim recordId As Integer = 0

        If Not String.IsNullOrEmpty(idStr) AndAlso Integer.TryParse(idStr, recordId) Then
            Dim connString As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString
            Dim updateQuery As String = "UPDATE KYCDetails SET VerificationStatus = @Status WHERE Id = @Id"

            Try
                Using conn As New SqlConnection(connString)
                    Using cmd As New SqlCommand(updateQuery, conn)
                        cmd.Parameters.AddWithValue("@Status", newStatus)
                        cmd.Parameters.AddWithValue("@Id", recordId)
                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

                ' Redirect GET back to dashboard (PRG pattern)
                Response.Redirect("ManageKYC.aspx?status=" & statusRedirectCode, False)
                Context.ApplicationInstance.CompleteRequest()
            Catch ex As Exception
                InjectServerToast("Audit Update Error", "Failed to update workflow state: " & ex.Message.Replace("'", "\'"), "danger")
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Mask Aadhaar for compliance and core-banking privacy.
    ''' </summary>
    Public Function MaskAadhaar(ByVal val As Object) As String
        If val Is Nothing OrElse IsDBNull(val) Then Return ""
        Dim str As String = val.ToString()
        If str.Length = 12 Then
            Return String.Format("XXXX-XXXX-{0}", str.Substring(8))
        End If
        Return str
    End Function

    ''' <summary>
    ''' Dynamically sets premium status badge icons.
    ''' </summary>
    Public Function GetStatusIcon(ByVal val As Object) As String
        If val Is Nothing OrElse IsDBNull(val) Then Return "bi-hourglass-split"
        Dim status As String = val.ToString()
        If status = "Verified" Then Return "bi-patch-check-fill"
        If status = "Rejected" Then Return "bi-x-circle-fill"
        Return "bi-hourglass-split"
    End Function

    ''' <summary>
    ''' Bootstraps the local LocalDB configuration and creates DB/Table structures automatically if missing.
    ''' </summary>
    Private Sub EnsureDatabaseSchema()
        Dim masterConnStr As String = "Server=(localdb)\MSSQLLocalDB;Database=master;Integrated Security=True;"
        Dim mainConnStr As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString

        Try
            Using conn As New SqlConnection(masterConnStr)
                Dim dbQuery As String = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'KYCDB') CREATE DATABASE KYCDB;"
                Using cmd As New SqlCommand(dbQuery, conn)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception("Failed to ensure database creation on LocalDB: " & ex.Message, ex)
        End Try

        Try
            Using conn As New SqlConnection(mainConnStr)
                Dim tblQuery As String = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KYCDetails]') AND type in (N'U')) " & _
                    "CREATE TABLE [dbo].[KYCDetails] (" & _
                    "[Id] INT IDENTITY(1,1) PRIMARY KEY, " & _
                    "[AccountType] NVARCHAR(50) NOT NULL, " & _
                    "[CustomerType] NVARCHAR(50) NOT NULL, " & _
                    "[PreferredBranch] NVARCHAR(100) NOT NULL, " & _
                    "[ApplicationDate] DATE NOT NULL, " & _
                    "[Email] NVARCHAR(150) NOT NULL, " & _
                    "[EmailOTP] NVARCHAR(10) NOT NULL, " & _
                    "[MobileNumber] NVARCHAR(15) NOT NULL, " & _
                    "[AlternateMobileNumber] NVARCHAR(15) NULL, " & _
                    "[AadhaarMobileOTP] NVARCHAR(10) NOT NULL, " & _
                    "[AadhaarNumber] NVARCHAR(20) NOT NULL CONSTRAINT UQ_Aadhaar UNIQUE, " & _
                    "[AadhaarOTP] NVARCHAR(10) NOT NULL, " & _
                    "[AadhaarName] NVARCHAR(100) NOT NULL, " & _
                    "[AadhaarDOB] DATE NOT NULL, " & _
                    "[Gender] NVARCHAR(20) NOT NULL, " & _
                    "[FullLegalName] NVARCHAR(100) NOT NULL, " & _
                    "[FatherName] NVARCHAR(100) NOT NULL, " & _
                    "[MotherName] NVARCHAR(100) NOT NULL, " & _
                    "[SpouseGuardianName] NVARCHAR(100) NOT NULL, " & _
                    "[MaritalStatus] NVARCHAR(50) NOT NULL, " & _
                    "[Nationality] NVARCHAR(50) NOT NULL, " & _
                    "[Religion] NVARCHAR(50) NULL, " & _
                    "[ResidentialStatus] NVARCHAR(50) NOT NULL, " & _
                    "[PlaceOfBirth] NVARCHAR(100) NULL, " & _
                    "[CountryOfBirth] NVARCHAR(100) NULL, " & _
                    "[StreetHouseLandmark] NVARCHAR(250) NOT NULL, " & _
                    "[AreaLocality] NVARCHAR(150) NOT NULL, " & _
                    "[LocationVillageTown] NVARCHAR(150) NOT NULL, " & _
                    "[PostOffice] NVARCHAR(100) NOT NULL, " & _
                    "[CityDistrict] NVARCHAR(100) NOT NULL, " & _
                    "[State] NVARCHAR(100) NOT NULL, " & _
                    "[Country] NVARCHAR(100) NOT NULL, " & _
                    "[Pincode] NVARCHAR(10) NOT NULL, " & _
                    "[TypeOfAddress] NVARCHAR(50) NOT NULL, " & _
                    "[IsPermanentAddressSame] NVARCHAR(5) NOT NULL, " & _
                    "[PermanentAddress] NVARCHAR(500) NULL, " & _
                    "[OccupationType] NVARCHAR(50) NOT NULL, " & _
                    "[EmployerName] NVARCHAR(150) NULL, " & _
                    "[NatureOfBusiness] NVARCHAR(150) NULL, " & _
                    "[Designation] NVARCHAR(100) NULL, " & _
                    "[AnnualIncomeRange] NVARCHAR(100) NOT NULL, " & _
                    "[SourceOfFunds] NVARCHAR(100) NOT NULL, " & _
                    "[PANNumber] NVARCHAR(20) NOT NULL CONSTRAINT UQ_PAN UNIQUE, " & _
                    "[PANHolderName] NVARCHAR(100) NOT NULL, " & _
                    "[DrivingLicenceNumber] NVARCHAR(30) NULL, " & _
                    "[DrivingLicenceDOB] DATE NULL, " & _
                    "[DrivingLicenceName] NVARCHAR(100) NULL, " & _
                    "[AadhaarCardPath] NVARCHAR(500) NOT NULL, " & _
                    "[PANCardPath] NVARCHAR(500) NOT NULL, " & _
                    "[PassportDLPath] NVARCHAR(500) NULL, " & _
                    "[AddressProofPath] NVARCHAR(500) NULL, " & _
                    "[SignatureScanPath] NVARCHAR(500) NOT NULL, " & _
                    "[CreatedAt] DATETIME DEFAULT GETDATE(), " & _
                    "[VerificationStatus] NVARCHAR(20) DEFAULT 'Pending' NOT NULL" & _
                    ");"
                Using cmd As New SqlCommand(tblQuery, conn)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using

                Dim alterQuery As String = "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[KYCDetails]') AND name = 'VerificationStatus') " & _
                    "ALTER TABLE [dbo].[KYCDetails] ADD [VerificationStatus] NVARCHAR(20) DEFAULT 'Pending' NOT NULL;"
                Using cmdAlter As New SqlCommand(alterQuery, conn)
                    cmdAlter.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception("Failed to bootstrap table structures on KYCDB: " & ex.Message, ex)
        End Try
    End Sub

    ''' <summary>
    ''' Helper to dynamically search for a server control recursively.
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

    ''' <summary>
    ''' Dynamically injects elegant sliding toast notifications into the response stream.
    ''' </summary>
    Private Sub InjectServerToast(ByVal title As String, ByVal message As String, ByVal type As String)
        Dim litServerToasts As Literal = CType(FindControlRecursive(Me, "litServerToasts"), Literal)
        If litServerToasts IsNot Nothing Then
            Dim script As String = String.Format(
                "<script type='text/javascript'>" & vbCrLf &
                "    window.addEventListener('DOMContentLoaded', () => {{" & vbCrLf &
                "        showToast('{0}', '{1}', '{2}');" & vbCrLf &
                "    }});" & vbCrLf &
                "</script>",
                title.Replace("'", "\'"),
                message.Replace("'", "\'"),
                type
            )
            litServerToasts.Text = script
        End If
    End Sub
End Class
